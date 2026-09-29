using System.IO;
using System.Text;
using DailyWorkReport.Data;
using DailyWorkReport.Models;
using DailyWorkReport.ViewModels.ProductionOrder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;

namespace DailyWorkReport.Controllers;

[Authorize]
public class ProductionOrdersController : Controller
{
    private readonly ApplicationDbContext _context;
    private const long MaxImportFileSize = 1 * 1024 * 1024;
    private const int MaxImportRows = 1000;
    private const int MaxErrorsToShow = 15;
    private const int MaxDuplicatesToShow = 10; 
    private static readonly string[] DueDateFormats = { "yyyy-M-d", "yyyy/M/d" };
    private sealed record ParsedImportRow(int Line, string OrderNumber, string ProductCode, int OrderQty, DateOnly DueDate);

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
        var orderNumber = NormalizeOrderNumber(vm.OrderNumber);

        if(await _context.ProductionOrders.AnyAsync(po => po.OrderNumber == orderNumber))
        {
            ModelState.AddModelError(nameof(vm.OrderNumber), "This order number already exists.");
        }

        if (!ModelState.IsValid)
        {
            await RepopulateProductNameAsync(vm.ProductId, name => vm.ProductName = name);
            return View(vm);
        }

        var productionOrder = new ProductionOrder
        {
            OrderNumber = orderNumber,
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
            ProductName = productionOrder.Product.Name,
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
        var orderNumber = NormalizeOrderNumber(vm.OrderNumber);
        if(await _context.ProductionOrders.AnyAsync(po => po.OrderNumber == orderNumber && po.Id != id))
        {
            ModelState.AddModelError(nameof(vm.OrderNumber), "This order number already exists.");
        }
        if (!ModelState.IsValid)
        {
            await RepopulateProductNameAsync(vm.ProductId, name => vm.ProductName = name);  
            return View(vm);
        }

        var productionOrder = await _context.ProductionOrders.FindAsync(id);
        if(productionOrder == null)
        {
            return NotFound();
        }

        productionOrder.OrderNumber = orderNumber;
        productionOrder.ProductId = vm.ProductId!.Value;
        productionOrder.OrderQty = vm.OrderQty!.Value;
        productionOrder.DueDate = vm.DueDate;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
        
    }

    public async Task<IActionResult> Delete(int id)
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
        
        var vm = new ProductionOrderDisplayViewModel
        {
            Id = productionOrder.Id,
            OrderNumber = productionOrder.OrderNumber,
            ProductName = productionOrder.Product.Name,
            OrderQty = productionOrder.OrderQty,
            DueDate = productionOrder.DueDate,
            IsReported = false
        };
        return View(vm);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var productionOrder = await _context.ProductionOrders.FindAsync(id);
        if(productionOrder == null)
        {
            return NotFound();
        }
        if(await _context.WorkReports.AnyAsync(w => w.ProductionOrderId == id))
        {
            return Forbid();
        }

        _context.ProductionOrders.Remove(productionOrder);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Import(IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            return ImportFailed("Please select a CSV file");
        }

        if (!string.Equals(Path.GetExtension(file.FileName), ".csv", System.StringComparison.OrdinalIgnoreCase))
        {
            return ImportFailed("Only .csv files are supported.");
        }

        if(file.Length > MaxImportFileSize)
        {
            return ImportFailed($"The file is too large. Maximum size is {MaxImportFileSize / (1024 * 1024)} MB.");
        }

        var rows = new List<(int Line, ProductionOrderImportRowViewModel Row)>();
        try
        {
            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));

            if (!csv.Read() || !csv.ReadHeader())
            {
                return ImportFailed("The file is empty.");
            }

            csv.ValidateHeader<ProductionOrderImportRowViewModel>();

            while (csv.Read())
            {
                rows.Add((csv.Parser.Row, csv.GetRecord<ProductionOrderImportRowViewModel>()!));
            }

        }
        catch (HeaderValidationException)
        {
            return ImportFailed("Invalid header. Required columns: OrderNumber, ProductCode, OrderQty, DueDate.");
        }
        catch (CsvHelperException ex)
        {
            return ImportFailed($"Failed to read the CSV file (line {ex.Context?.Parser?.Row}).");
        }

        if (rows.Count == 0)
        {
            return ImportFailed("The file contains no data rows.");
        }

        if (rows.Count > MaxImportRows)
        {
            return ImportFailed($"Too many rows. Maximun is {MaxImportRows} rows per import.");
        }

        var errors = new List<string>();
        var parsed = new List<ParsedImportRow>();

        var productCodes = rows
            .Select(r => r.Row.ProductCode?.Trim())
            .Where(c => !string.IsNullOrEmpty(c))
            .Distinct()
            .ToList();

        var productMap = await _context.Products
            .Where(p => productCodes.Contains(p.ProductCode))
            .ToDictionaryAsync(p => p.ProductCode, p => p.Id);

        foreach (var (line, row) in rows)
        {
            var orderNumber = NormalizeOrderNumber(row.OrderNumber?.Trim() ?? string.Empty);
            var productCode = row.ProductCode?.Trim() ?? string.Empty;
            var rowHasError = false;

            if (string.IsNullOrEmpty(orderNumber))
            {
                errors.Add($"Line {line}: Order Number is required.");
                rowHasError = true;
            }

            if (string.IsNullOrEmpty(productCode))
            {
                errors.Add($"Line {line}: Product Code is required.");
                rowHasError = true;
            }
            else if (!productMap.ContainsKey(productCode))
            {
                errors.Add($"Line {line}: Product Code '{productCode}' does not exist.");
                rowHasError = true;
            }

            if (!int.TryParse(row.OrderQty?.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var qty) || qty < 1)
            {
                errors.Add($"Line {line}: Order Quantity must be a whole number of at least 1.");
                rowHasError = true;
            }

            if (!DateOnly.TryParseExact(row.DueDate?.Trim(), DueDateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dueDate))
            {
                errors.Add($"Line {line}: Due Date must be in the format YYYY-MM-DD or YYYY/M/D.");
                rowHasError = true;
            }

            if (!rowHasError)
            {
                parsed.Add(new ParsedImportRow(line, orderNumber, productCode, qty, dueDate));
            }
        }

        var duplicatesInFile = parsed
            .GroupBy(x => x.OrderNumber, StringComparer.Ordinal)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        
        if (duplicatesInFile.Count > 0)
        {
            errors.Add("Duplicate Order Numbers in the file: " + JoinLimited(duplicatesInFile, MaxDuplicatesToShow));
        }

        var orderNumbers = parsed
            .Select(x => x.OrderNumber)
            .Distinct()
            .ToList();

        var existingNumbers = await _context.ProductionOrders
            .Where(o => orderNumbers.Contains(o.OrderNumber))
            .Select(o => o.OrderNumber)
            .ToListAsync();

        if (existingNumbers.Count > 0)
        {
            errors.Add("These order numbers are already registered: " + JoinLimited(existingNumbers, MaxDuplicatesToShow));
        }


        

    }

    private async Task RepopulateProductNameAsync(int? productId, Action<string> setName)
    {
        if(productId is null)
        {
            return;
        }

        var name = await _context.Products
            .Where(p => p.Id == productId)
            .Select(p => p.Name)
            .FirstOrDefaultAsync();

        if(name is not null)
        {
            setName(name);
        }
    }
    private IActionResult ImportFailed(params string[] errors)
    {
        TempData["ImportErrors"] = errors.Length > MaxErrorsToShow
            ? errors.Take(MaxErrorsToShow).Append($"...and {errors.Length - MaxErrorsToShow} more errors.").ToArray()
            : errors;
        return RedirectToAction(nameof(Index));
    }

    private static string JoinLimited(List<string> values, int limit)
    {
        var text = string.Join(", ", values.Take(limit));
        return values.Count > limit ? $"{text}, ...and {values.Count - limit} more" : text;
    }

    private static string NormalizeOrderNumber(string value) => value.Trim().ToUpperInvariant();


}