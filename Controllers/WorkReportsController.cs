using DailyWorkReport.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace DailyWorkReport.Controllers;

public class WorkReportsController : Controller
{
    private readonly ApplicationDbContext _context;
    public WorkReportsController(ApplicationDbContext context)
    {
        _context = context;
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

}