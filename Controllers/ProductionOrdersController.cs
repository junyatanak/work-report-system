using DailyWorkReport.Data;
using DailyWorkReport.ViewModels.ProductionOrder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DailyWorkReport.Controllers;

public class ProductionOrdersController : Controller
{
    private readonly ApplicationDbContext _context;
    public ProductionOrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var productionOrders = await _context.ProductionOrders
            .Include(po => po.Product)
            .OrderByDescending(po => po.Id)
            .Select(po => new ProductionOrderDisplayViewModel
            {
                Id = po.Id,
                OrderNumber = po.OrderNumber,
                ProductName = po.Product.Name,
                OrderQty = po.OrderQty,
                DueDate = po.DueDate,
                IsReported = po.WorkReports.Any()
            })
            .ToListAsync();

        return View(productionOrders);
    }
    
}