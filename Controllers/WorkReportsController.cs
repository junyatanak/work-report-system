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

        if(!ModelState.IsValid)
        {
            return View(vm);
        }

        // Continue with the rest of the logic...
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

}