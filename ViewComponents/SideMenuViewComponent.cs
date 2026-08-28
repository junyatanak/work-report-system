using Microsoft.AspNetCore.Mvc;
using DailyWorkReport.ViewModels.Navigation;

namespace DailyWorkReport.ViewComponents;

public class SideMenuViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var menuItems = new List<MenuItemViewModel>
        {
            new () {Text = "Work Report", Controller = "WorkReports", Action = "Index", IconClass = "bi-file-text"},
            new () {Text = "Production Order", Controller = "ProductionOrders", Action = "Index", IconClass = "bi-box-seam"},
        };
        if (User.IsInRole("Admin"))
        {
            menuItems.Add(new()
            {
                Text = "Master Data",
                IconClass = "bi-database",
                SubItems = new List<MenuItemViewModel>
                {
                    new() { Text = "Product", Controller = "Products", Action = "Index", IconClass = "bi-box-seam" },
                    new() { Text = "Process", Controller = "Processes", Action = "Index", IconClass = "bi-wrench" },
                    new() { Text = "Work Class", Controller = "WorkClasses", Action = "Index", IconClass = "bi-tags" },
                    new() { Text = "Work Pattern", Controller = "WorkPatterns", Action = "Index", IconClass = "bi-diagram-3" },
                    new() { Text = "Standard Work Time", Controller = "StandardWorkTimes", Action = "Index", IconClass = "bi-clock" },
                    new() { Text = "Worker", Controller = "Workers", Action = "Index", IconClass = "bi-person-badge" },
                }
            });
            menuItems.Add(new(){Text = "User", Controller = "Users", Action = "Index", IconClass = "bi-people"});
        }

        return View(menuItems);
    }
}