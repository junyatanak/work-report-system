using Microsoft.AspNetCore.Mvc;
using DailyWorkReport.ViewModels.Navigation;

namespace DailyWorkReport.ViewComponents;

public class SideMenuViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var menuItems = new List<MenuItemViewModel>
        {
            new () {Text = "Work Report", Controller = "WorkReports", Action = "Index", IconClass = "bi-file-text"}
        };
        if (User.IsInRole("Admin"))
        {
            menuItems.Add(new()
            {
                Text = "Master Data",
                IconClass = "bi-database",
                SubItems = new List<MenuItemViewModel>
                {
                    new() { Text = "Product", Controller = "Products", Action = "Index" },
                    new() { Text = "Work Class", Controller = "WorkClasses", Action = "Index" },
                    new() { Text = "Process", Controller = "Processes", Action = "Index" },
                    new() { Text = "Work Pattern", Controller = "WorkPatterns", Action = "Index" },
                    new() { Text = "Standard Work Time", Controller = "StandardWorkTimes", Action = "Index" },
                    new() { Text = "Worker", Controller = "Workers", Action = "Index" },
                }
            });
            menuItems.Add(new(){Text = "User", Controller = "Users", Action = "Index", IconClass = "bi-people"});
        }

        return View(menuItems);
    }
}