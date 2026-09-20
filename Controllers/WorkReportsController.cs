using DailyWorkReport.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using DailyWorkReport.ViewModels.WorkReport;
using DailyWorkReport.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;
using DailyWorkReport.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ClosedXML.Excel;
using Microsoft.VisualBasic;


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

    public async Task<IActionResult> Index(WorkReportIndexFilterViewModel filter)
    {
        var query = ApplyFilter(_context.WorkReports, filter);     
        query = ApplySort(query, filter.SortBy, filter.SortDescending);

        var currentUserId = _userManager.GetUserId(User);
        var isAdmin = User.IsInRole("Admin");

        var items = await query
            .Select(w => new WorkReportIndexItemViewModel
            {
                Id = w.Id,
                WorkDate = w.WorkDate,
                ProductionOrderNumber = w.ProductionOrder.OrderNumber,
                ProductName = w.ProductionOrder.Product.Name,
                ProcessName = w.Process.Name,
                TotalProducedQty = w.WorkReportWorkers.Sum(wr => wr.ProducedQty),
                ReporterName = w.User.UserName ?? string.Empty,
                CanEdit = isAdmin || w.UserId == currentUserId
            })
            .ToListAsync();

        var vm = new WorkReportIndexViewModel
        {
            Filter = filter,
            Items = items
        };

        return View(vm);        

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
                ModelState.AddModelError($"WorkReportWorkers[{i}].WorkerNumber", $"Row {i + 1}: Worker number is required.");
                continue;
            }

            if(worker.WorkerId is null)
            {
                ModelState.AddModelError($"WorkReportWorkers[{i}].WorkerNumber", $"Row {i + 1}: Please enter a valid worker number.");
                continue;
            }

            if(worker.ProducedQty is null)
            {
                ModelState.AddModelError($"WorkReportWorkers[{i}].ProducedQty", $"Row {i + 1}: Produced quantity is required.");
                continue;
            }

            if(worker.StartAt == worker.EndAt)
            {
                ModelState.AddModelError($"WorkReportWorkers[{i}].EndAt", $"Row {i + 1}: Start time and end time cannot be the same.");
                continue;
            }

            var (startAt, endAt) = ResolveShiftDateTime(vm.WorkDate, worker.StartAt, worker.EndAt);
            if((endAt - startAt).TotalHours > 12)
            {
                ModelState.AddModelError($"WorkReportWorkers[{i}].EndAt", $"Row {i + 1}: The work duration exceed 12 hours. Please check the start and end times.");
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
            (vm.ProcessOptions, vm.WorkPatternOptions) = await RepopulateProcessWorkPatternOptionsAsync(vm.WorkClassId, vm.ProcessId);
            await RepopulateWorkerNamesAsync(vm.WorkReportWorkers);
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



    public async Task<IActionResult> Details(int id)
    {
        var workReport = await _context.WorkReports
            .Include(w => w.ProductionOrder)
                .ThenInclude(po => po.Product)
                    .ThenInclude(p => p.WorkClass)
            .Include(w => w.Process)
            .Include(w => w.WorkPattern)
            .Include(w => w.User)
            .Include(w => w.WorkReportWorkers)
                .ThenInclude(wr => wr.Worker)
            .FirstOrDefaultAsync(w => w.Id == id);
        
        if (workReport == null)
        {
            return NotFound();
        }

        var vm = new WorkReportDetailsViewModel
        {
            Id = workReport.Id,
            ReporterName = workReport.User.UserName ?? string.Empty,
            WorkDate = workReport.WorkDate,
            ProductionOrderNumber = workReport.ProductionOrder.OrderNumber,
            ProductCode = workReport.ProductionOrder.Product.ProductCode,
            ProductName = workReport.ProductionOrder.Product.Name,
            OrderQty = workReport.ProductionOrder.OrderQty,
            DueDate = workReport.ProductionOrder.DueDate,
            WorkClassName = workReport.ProductionOrder.Product.WorkClass.Name,
            ProcessName = workReport.Process.Name,
            WorkPatternName = workReport.WorkPattern.Name,
            Workers = workReport.WorkReportWorkers.Select(wr => new WorkReportWorkerDetailsViewModel
            {
                WorkerNumber = wr.Worker.WorkerNumber.ToString(),
                WorkerName = wr.Worker.Name ?? string.Empty,
                StartAt = wr.StartAt,
                EndAt = wr.EndAt,
                BreakMinutes = wr.BreakMinutes,
                ProducedQty = wr.ProducedQty
            }).ToList()
        };

        return View(vm);

    }

    public async Task<IActionResult> Edit(int id)
    {
        var workReport = await _context.WorkReports
            .Include(w => w.ProductionOrder)
                .ThenInclude(po => po.Product)
                    .ThenInclude(p => p.WorkClass)
            .Include(w => w.WorkReportWorkers)
                .ThenInclude(wr => wr.Worker)
            .Include(w => w.User)
            .FirstOrDefaultAsync(w => w.Id == id);
        
        if (workReport == null)
        {
            return NotFound();
        }

        if (!CanEditWorkReport(workReport))
        {
            return Forbid();
        }

        var vm = new WorkReportEditViewModel
        {
            Id = workReport.Id,
            ReporterName = workReport.User.UserName ?? string.Empty,
            WorkDate = workReport.WorkDate,
            ProductionOrderNumber = workReport.ProductionOrder.OrderNumber,
            ProductCode = workReport.ProductionOrder.Product.ProductCode,
            ProductName = workReport.ProductionOrder.Product.Name,
            OrderQty = workReport.ProductionOrder.OrderQty,
            DueDate = workReport.ProductionOrder.DueDate,
            WorkClassName = workReport.ProductionOrder.Product.WorkClass.Name,
            WorkClassId = workReport.ProductionOrder.Product.WorkClassId,
            ProcessId = workReport.ProcessId,
            WorkPatternId = workReport.WorkPatternId,
            WorkReportWorkers = workReport.WorkReportWorkers.Select(wr => new WorkReportWorkerInputViewModel
            {
                WorkerNumber = wr.Worker.WorkerNumber,
                WorkerId = wr.WorkerId,
                WorkerName = wr.Worker.Name ?? string.Empty,
                StartAt = TimeOnly.FromDateTime(wr.StartAt),
                EndAt = TimeOnly.FromDateTime(wr.EndAt),
                BreakMinutes = wr.BreakMinutes,
                ProducedQty = wr.ProducedQty
            }).ToList()
        };

        (vm.ProcessOptions, vm.WorkPatternOptions) = await RepopulateProcessWorkPatternOptionsAsync(vm.WorkClassId, vm.ProcessId);

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, WorkReportEditViewModel vm)
    {
        if (id != vm.Id)
        {
            return BadRequest();
        }
        var workReport = await _context.WorkReports
            .Include(w => w.WorkReportWorkers)
            .FirstOrDefaultAsync(w => w.Id == id);

        if (workReport == null)
        {
            return NotFound();
        }

        if (!CanEditWorkReport(workReport))
        {
            return Forbid();
        }

        if (!vm.WorkReportWorkers.Any())
        {
            ModelState.AddModelError(string.Empty, "At least one worker is required.");
        }

        for (int i = 0; i < vm.WorkReportWorkers.Count; i++)
        {
            var worker = vm.WorkReportWorkers[i];

            if (worker.WorkerNumber is null)
            {
                ModelState.AddModelError($"WorkReportWorkers[{i}].WorkerNumber", $"Row {i + 1}: Worker number is required.");
                continue;
            }

            if (worker.WorkerId is null)
            {
                ModelState.AddModelError($"WorkReportWorkers[{i}].WorkerNumber", $"Row {i + 1}: Please enter a valid worker number.");
                continue;
            }

            if (worker.ProducedQty is null)
            {
                ModelState.AddModelError($"WorkReportWorkers[{i}].ProducedQty", $"Row {i + 1}: Produced quantity is required.");
                continue;
            }

            if (worker.StartAt == worker.EndAt)
            {
                ModelState.AddModelError($"WorkReportWorkers[{i}].EndAt", $"Row {i + 1}: Start time and end time cannot be the same.");
                continue;
            }

            var (startAt, endAt) = ResolveShiftDateTime(vm.WorkDate, worker.StartAt, worker.EndAt);
            if ((endAt - startAt).TotalHours > 12)
            {
                ModelState.AddModelError($"WorkReportWorkers[{i}].EndAt", $"Row {i + 1}: Shift duration cannot exceed 12 hours. Please check the start and end times.");
            }

        }

        var totalProducedQty = vm.WorkReportWorkers.Sum(w => w.ProducedQty ?? 0);
        if (totalProducedQty > vm.OrderQty)
        {
            ModelState.AddModelError(string.Empty, "Total produced quantity exceeds the order quantity.");
        }

        if (!ModelState.IsValid)
        {
            (vm.ProcessOptions, vm.WorkPatternOptions) = await RepopulateProcessWorkPatternOptionsAsync(vm.WorkClassId, vm.ProcessId);
            await RepopulateWorkerNamesAsync(vm.WorkReportWorkers);
            return View(vm);
        }

        workReport.WorkDate = vm.WorkDate;
        workReport.ProcessId = vm.ProcessId!.Value;
        workReport.WorkPatternId = vm.WorkPatternId!.Value;

        _context.WorkReportWorkers.RemoveRange(workReport.WorkReportWorkers);

        foreach (var workerInput in vm.WorkReportWorkers)
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

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
        
    }

    public async Task<IActionResult> Delete(int id)
    {
        var workReport = await _context.WorkReports
            .Include(w => w.ProductionOrder)
                .ThenInclude(po => po.Product)
                    .ThenInclude(p => p.WorkClass)
            .Include(w => w.Process)
            .Include(w => w.WorkPattern)
            .Include(w => w.User)
            .Include(w => w.WorkReportWorkers)
                .ThenInclude(wr => wr.Worker)
            .FirstOrDefaultAsync(w => w.Id == id);

        if (workReport == null)
        {
            return NotFound();
        }

        if (!CanEditWorkReport(workReport))
        {
            return Forbid();
        }

        var vm = new WorkReportDetailsViewModel
        {
            Id = workReport.Id,
            ReporterName = workReport.User.UserName ?? string.Empty,
            WorkDate = workReport.WorkDate,
            ProductionOrderNumber = workReport.ProductionOrder.OrderNumber,
            ProductCode = workReport.ProductionOrder.Product.ProductCode,
            ProductName = workReport.ProductionOrder.Product.Name,
            OrderQty = workReport.ProductionOrder.OrderQty,
            DueDate = workReport.ProductionOrder.DueDate,
            WorkClassName = workReport.ProductionOrder.Product.WorkClass.Name,
            ProcessName = workReport.Process.Name,
            WorkPatternName = workReport.WorkPattern.Name,
            Workers = workReport.WorkReportWorkers.Select(wr => new WorkReportWorkerDetailsViewModel
            {
                WorkerNumber = wr.Worker.WorkerNumber.ToString(),
                WorkerName = wr.Worker.Name ?? string.Empty,
                StartAt = wr.StartAt,
                EndAt = wr.EndAt,
                BreakMinutes = wr.BreakMinutes,
                ProducedQty = wr.ProducedQty
            }).ToList()
        };

        return View(vm);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var workReport = await _context.WorkReports
            .Include(w => w.WorkReportWorkers)
            .FirstOrDefaultAsync(w => w.Id == id);

        if (workReport == null)
        {
            return NotFound();
        }

        if (!CanEditWorkReport(workReport))
        {
            return Forbid();
        }

        _context.WorkReportWorkers.RemoveRange(workReport.WorkReportWorkers);
        _context.WorkReports.Remove(workReport);
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

    [HttpGet]
    public async Task<IActionResult> Export(WorkReportIndexFilterViewModel filter)
    {
        var rows = await ApplyFilter(_context.WorkReports, filter)
            .SelectMany(r => r.WorkReportWorkers.Select(w => new
            {
                r.WorkDate,
                Reporter = r.User.UserName ?? string.Empty,
                OrderNumber = r.ProductionOrder.OrderNumber,
                ProductCode = r.ProductionOrder.Product.ProductCode,
                ProductName = r.ProductionOrder.Product.Name,
                OrderQty = r.ProductionOrder.OrderQty,
                DueDate = r.ProductionOrder.DueDate,
                WorkClassName = r.ProductionOrder.Product.WorkClass.Name,
                ProcessName = r.Process.Name,
                WorkPatternName = r.WorkPattern.Name,
                WorkerNumber = w.Worker.WorkerNumber,
                w.StartAt,
                w.EndAt,
                w.BreakMinutes,
                w.ProducedQty
            }))
            .OrderBy(x => x.WorkDate)
            .ThenBy(x => x.OrderNumber)
            .ThenBy(x => x.WorkClassName)
            .ThenBy(x => x.ProcessName)
            .ThenBy(x => x.WorkPatternName)
            .ThenBy(x => x.WorkerNumber)
            .ToListAsync();

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("WorkReports");

        var headers = new[]
        {
            "Work Date", "Reporter", "Order No.", "Product Code", "Product Name", "Order Qty", "Due Date",
            "Work Class", "Process", "Work Pattern", "Worker No.", "Work Hours", "Produced Qty"
        };

        for (var i = 0; i < headers.Length; i++)
        {
            ws.Cell(1, i + 1).Value = headers[i];
        }

        var rowIndex = 2;
        foreach (var row in rows)
        {
            var workHours = Math.Round(((row.EndAt - row.StartAt).TotalMinutes - row.BreakMinutes) / 60.0, 2);

            ws.Cell(rowIndex, 1).Value = row.WorkDate.ToDateTime(TimeOnly.MinValue);
            ws.Cell(rowIndex, 2).Value = row.Reporter;
            ws.Cell(rowIndex, 3).Value = row.OrderNumber;
            ws.Cell(rowIndex, 4).Value = row.ProductCode;
            ws.Cell(rowIndex, 5).Value = row.ProductName;
            ws.Cell(rowIndex, 6).Value = row.OrderQty;
            ws.Cell(rowIndex, 7).Value = row.DueDate.ToDateTime(TimeOnly.MinValue);
            ws.Cell(rowIndex, 8).Value = row.WorkClassName;
            ws.Cell(rowIndex, 9).Value = row.ProcessName;
            ws.Cell(rowIndex, 10).Value = row.WorkPatternName;
            ws.Cell(rowIndex, 11).Value = row.WorkerNumber;
            ws.Cell(rowIndex, 12).Value = workHours;
            ws.Cell(rowIndex, 13).Value = row.ProducedQty;

            rowIndex++;
        }

        ws.Column(1).Style.DateFormat.Format = "yyyy-mm-dd";
        ws.Column(7).Style.DateFormat.Format = "yyyy-mm-dd";
        ws.Column(12).Style.NumberFormat.Format = "0.00";
        ws.Row(1).Style.Font.Bold = true;
        ws.SheetView.FreezeRows(1);
        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        var fileName = $"WorkReports_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);

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

    private async Task<(List<SelectListItem> ProcessOptions, List<SelectListItem> WorkPatternOptions)> RepopulateProcessWorkPatternOptionsAsync(int? workClassId, int? processId)
    {
        if (workClassId is null)
        {
            return (new List<SelectListItem>(), new List<SelectListItem>());
        }

        var processOptions = await _context.StandardWorkTimes
            .Where(s => s.WorkClassId == workClassId)
            .Select(s => new {s.ProcessId, s.Process.Name})
            .Distinct()
            .OrderBy(p => p.Name)
            .Select(p => new SelectListItem
            {
                Value = p.ProcessId.ToString(),
                Text = p.Name
            })
            .ToListAsync();

        if(processId is null)
        {
            return (processOptions, new List<SelectListItem>());
        }

        var workPatternOptions = await _context.StandardWorkTimes
            .Where(s => s.WorkClassId == workClassId && s.ProcessId == processId)
            .Select(s => new { s.WorkPatternId, s.WorkPattern.Name})
            .Distinct()
            .OrderBy(w => w.Name)
            .Select(w => new SelectListItem
            {
                Value = w.WorkPatternId.ToString(),
                Text = w.Name
            })
            .ToListAsync();

        return (processOptions, workPatternOptions);
    }

    private async Task RepopulateWorkerNamesAsync(List<WorkReportWorkerInputViewModel> workers)
    {
        var workerIds = workers
            .Where(w => w.WorkerId is not null)
            .Select(w => w.WorkerId!.Value)
            .ToList();

        var workerNames = await _context.Workers
            .Where(w => workerIds.Contains(w.Id))
            .ToDictionaryAsync(w => w.Id, w => w.Name ?? string.Empty);

        foreach(var workerInput in workers)
        {
            if(workerInput.WorkerId is not null && workerNames.TryGetValue(workerInput.WorkerId.Value, out var name))
            {
                workerInput.WorkerName = name;
            }
        }
    }

    private static IQueryable<WorkReport> ApplySort(IQueryable<WorkReport> query, string? sortBy, bool descending)
    {
        Expression<Func<WorkReport, object>> keySelector = sortBy switch
        {
            "ProductionOrderNumber" => w => w.ProductionOrder.OrderNumber,
            "ProductName" => w => w.ProductionOrder.Product.Name,
            "ProcessName" => w => w.Process.Name,
            "TotalProducedQty" => w => w.WorkReportWorkers.Sum(wr => wr.ProducedQty),
            "ReporterName" => w => w.User.UserName ?? string.Empty,
            _ => w => w.WorkDate
        };

        return descending ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);
    }

    private IQueryable<WorkReport> ApplyFilter(IQueryable<WorkReport> query, WorkReportIndexFilterViewModel filter)
    {
        if (filter.WorkDateFrom is not null)
        {
            query = query.Where(w => w.WorkDate >= filter.WorkDateFrom);
        }
        if (filter.WorkDateTo is not null)
        {
            query = query.Where(w => w.WorkDate <= filter.WorkDateTo);
        }
        if (!string.IsNullOrWhiteSpace(filter.ProductionOrderNumber))
        {
            query = query.Where(w => w.ProductionOrder.OrderNumber.Contains(filter.ProductionOrderNumber));
        }
        if (!string.IsNullOrWhiteSpace(filter.ProductName))
        {
            query = query.Where(w => w.ProductionOrder.Product.Name.Contains(filter.ProductName));
        }
        if (!string.IsNullOrWhiteSpace(filter.ProcessName))
        {
            query = query.Where(w => w.Process.Name.Contains(filter.ProcessName));
        }
        if (!string.IsNullOrWhiteSpace(filter.ReporterName))
        {
            query = query.Where(w => (w.User.UserName ?? string.Empty).Contains(filter.ReporterName));
        }
        return query;
    }

    private bool CanEditWorkReport(WorkReport workReport)
    {
        var currentUserId = _userManager.GetUserId(User);
        return User.IsInRole(Roles.Admin) || workReport.UserId == currentUserId;
    }






}