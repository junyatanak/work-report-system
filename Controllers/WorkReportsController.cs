using DailyWorkReport.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using DailyWorkReport.ViewModels.WorkReport;
using DailyWorkReport.Models;
using Microsoft.AspNetCore.Identity;


namespace DailyWorkReport.Controllers;
[Authorize]
public class WorkReportsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    public WorkReportsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Create()
    {
        var vm = new WorkReportCreateViewModel();
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(WorkReportCreateViewModel vm)
    {
        await RepopulateProductionOrderDataAsync(vm);

        if(!vm.WorkReportWorkers.Any())
        {
            ModelState.AddModelError(string.Empty, "At least one worker is required.");
        }

        for(int i = 0; i < vm.WorkReportWorkers.Count; i++)
        {
            var worker = vm.WorkReportWorkers[i];

            if(worker.WorkerNumber is null)
            {
                ModelState.AddModelError($"WorkReportWorkers[{i}].WorkerNumber", "Worker number is required.");
                continue;
            }

            if(worker.WorkerId is null)
            {
                ModelState.AddModelError($"WorkReportWorkers[{i}].WorkerNumber", "Please enter a valid worker number.");
                continue;
            }

            if(worker.ProducedQty is null)
            {
                ModelState.AddModelError($"WorkReportWorkers[{i}].ProducedQty", "Produced quantity is required.");
                continue;
            }

            if(worker.StartAt == worker.EndAt)
            {
                ModelState.AddModelError($"WorkReportWorkers[{i}].EndAt", "Start time and end time cannot be the same.");
                continue;
            }

            var (startAt, endAt) = ResolveShiftDateTime(vm.WorkDate, worker.StartAt, worker.EndAt);
            if((endAt - startAt).TotalHours > 12)
            {
                ModelState.AddModelError($"WorkReportWorkers[{i}].EndAt", "The work duration exceed 12 hours. Please check the start and end times.");
            }
        }

        if(vm.ProductionOrderId is not null && vm.OrderQty is not null)
        {
            var totalProducedQty = vm.WorkReportWorkers.Sum(w => w.ProducedQty ?? 0);
            if(totalProducedQty > vm.OrderQty)
            {
                ModelState.AddModelError(string.Empty, "Total produced quantity exceedes the order quantity.");
            } 
        }

        if(!ModelState.IsValid)
        {
            await RepopulateProcessWorkPatternOptionsAsync(vm);
            await RepopulateWorkerNamesAsync(vm);
            return View(vm);
        }

        var userId = _userManager.GetUserId(User)!;

        var workReport = new WorkReport
        {
            WorkDate = vm.WorkDate,
            ProductionOrderId = vm.ProductionOrderId!.Value,
            ProcessId = vm.ProcessId!.Value,
            WorkPatternId = vm.WorkPatternId!.Value,
            UserId = userId
        };

        foreach(var workerInput in vm.WorkReportWorkers)
        {
            var (startAt, endAt) = ResolveShiftDateTime(vm.WorkDate, workerInput.StartAt, workerInput.EndAt);

            workReport.WorkReportWorkers.Add(new WorkReportWorker
            {
                WorkerId = workerInput.WorkerId!.Value,
                StartAt = startAt,
                EndAt = endAt,
                BreakMinutes = workerInput.BreakMinutes,
                ProducedQty = workerInput.ProducedQty!.Value
            });
        }

        _context.WorkReports.Add(workReport);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
        
    }

    [HttpGet]
    public async Task<IActionResult> FindProductionOrderByNumber(string orderNumber)
    {
        var result = await _context.ProductionOrders
            .Where(po => po.OrderNumber == orderNumber)
            .Select(po => new
            {
                ProductionOrderId = po.Id,
                ProductCode = po.Product.ProductCode,
                ProductName = po.Product.Name,
                OrderQty = po.OrderQty,
                DueDate = po.DueDate,
                WorkClassId = po.Product.WorkClassId,
                WorkClassName = po.Product.WorkClass.Name
            })
            .FirstOrDefaultAsync();

        if(result == null)
        {
            return NotFound();
        }

        return Json(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetProcessOptions(int workClassId)
    {
        var processes = await _context.StandardWorkTimes
            .Where(s => s.WorkClassId == workClassId)
            .Select(s => new{ s.ProcessId, s.Process.Name })
            .Distinct()
            .OrderBy(p => p.Name)
            .ToListAsync();

        return Json(processes);
    }
    [HttpGet]
    public async Task<IActionResult> GetWorkPatternOptions(int workClassId, int processId)
    {
        var workPatterns = await _context.StandardWorkTimes
            .Where(s => s.WorkClassId == workClassId && s.ProcessId == processId)
            .Select(s => new { s.WorkPatternId, s.WorkPattern.Name })
            .Distinct()
            .OrderBy(w => w.Name)
            .ToListAsync();

        return Json(workPatterns);
    }
    [HttpGet]
    public async Task<IActionResult> FindWorkerByNumber(int workerNumber)
    {
        var worker = await _context.Workers
            .Where(w => w.WorkerNumber == workerNumber)
            .Select(w => new{ w.Id, w.Name})
            .FirstOrDefaultAsync();

        if(worker == null)
        {
            return NotFound();
        }

        return Json(worker);
    }

    private static (DateTime StartAt, DateTime EndAt) ResolveShiftDateTime(DateOnly workDate, TimeOnly startTime, TimeOnly endTime)
    {
        var startAt = workDate.ToDateTime(startTime);
        var endAt = workDate.ToDateTime(endTime);

        if(endAt <= startAt)
        {
            endAt = endAt.AddDays(1);
        }

        return (startAt, endAt);
    }

    private async Task RepopulateProductionOrderDataAsync(WorkReportCreateViewModel vm)
    {
        if(vm.ProductionOrderId is null)
        {
            return;
        }

        var order = await _context.ProductionOrders
            .Where(po => po.Id == vm.ProductionOrderId)
            .Select(po => new
            {
                po.OrderQty,
                po.DueDate,
                ProductCode = po.Product.ProductCode,
                ProductName = po.Product.Name,
                WorkClassName = po.Product.WorkClass.Name
            })
            .FirstOrDefaultAsync();
        
        if(order is null)
        {
            return;
        }

        vm.OrderQty = order.OrderQty;
        vm.DueDate = order.DueDate;
        vm.ProductCode = order.ProductCode;
        vm.ProductName = order.ProductName;
        vm.WorkClassName = order.WorkClassName;
    }

    private async Task RepopulateProcessWorkPatternOptionsAsync(WorkReportCreateViewModel vm)
    {
        if(vm.WorkClassId is null)
        {
            return;
        }

        vm.ProcessOptions = await _context.StandardWorkTimes
            .Where(s => s.WorkClassId == vm.WorkClassId)
            .Select(s => new SelectListItem
            {
                Value = s.ProcessId.ToString(),
                Text = s.Process.Name
            })
            .Distinct()
            .ToListAsync();

        if(vm.ProcessId is null)
        {
            return;
        }

        vm.WorkPatternOptions = await _context.StandardWorkTimes
            .Where(s => s.WorkClassId == vm.WorkClassId && s.ProcessId == vm.ProcessId)
            .Select(s => new SelectListItem
            {
                Value = s.WorkPatternId.ToString(),
                Text = s.WorkPattern.Name
            })
            .Distinct()
            .ToListAsync();
    }

    private async Task RepopulateWorkerNamesAsync(WorkReportCreateViewModel vm)
    {
        var workerIds = vm.WorkReportWorkers
            .Where(w => w.WorkerId is not null)
            .Select(w => w.WorkerId!.Value)
            .ToList();

        var workerNames = await _context.Workers
            .Where(w => workerIds.Contains(w.Id))
            .ToDictionaryAsync(w => w.Id, w => w.Name ?? string.Empty);

        foreach(var workerInput in vm.WorkReportWorkers)
        {
            if(workerInput.WorkerId is not null && workerNames.TryGetValue(workerInput.WorkerId.Value, out var name))
            {
                workerInput.WorkerName = name;
            }
        }
    }



}