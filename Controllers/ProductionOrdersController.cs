using DailyWorkReport.Data;
using DailyWorkReport.Models;
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
    
    public IActionResult Create()
    {
        return View(new ProductionOrderCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductionOrderCreateViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var productionOrder = new ProductionOrder
        {
            OrderNumber = vm.OrderNumber,
            ProductId = vm.ProductId!.Value,
            OrderQty = vm.OrderQty!.Value,
            DueDate = vm.DueDate
        };

        _context.ProductionOrders.Add(productionOrder);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var productionOrder = await _context.ProductionOrders
            .Include(p => p.Product)
            .FirstOrDefaultAsync(p => p.Id == id);
        if(productionOrder == null)
        {
            return NotFound();
        }
        if(await _context.WorkReports.AnyAsync(w => w.ProductionOrderId == id))
        {
            return Forbid();
        }

        var vm = new ProductionOrderEditViewModel
        {
            Id = productionOrder.Id,
            OrderNumber = productionOrder.OrderNumber,
            ProductCode = productionOrder.Product.ProductCode,
            ProductId = productionOrder.ProductId,
            OrderQty = productionOrder.OrderQty,
            DueDate = productionOrder.DueDate
        };
        return View(vm);

    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductionOrderEditViewModel vm)
    {
        if(id != vm.Id)
        {
            return BadRequest();
        }
        if(await _context.WorkReports.AnyAsync(w => w.ProductionOrderId == id))
        {
            return Forbid();
        }
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var productionOrder = await _context.ProductionOrders.FindAsync(id);
        if(productionOrder == null)
        {
            return NotFound();
        }

        productionOrder.OrderNumber = vm.OrderNumber;
        productionOrder.ProductId = vm.ProductId!.Value;
        productionOrder.OrderQty = vm.OrderQty!.Value;
        productionOrder.DueDate = vm.DueDate;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
        
    }

    [HttpGet]
    public async Task<IActionResult> FindProductByCode(string code)
    {
        var product = await _context.Products
            .Where(p => p.ProductCode == code)
            .Select(p => new{p.Id, p.Name})
            .FirstOrDefaultAsync();
        if(product == null)
        {
            return NotFound();
        }

        return Json(product);
    }
}