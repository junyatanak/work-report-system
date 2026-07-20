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

        return View(menuItems);
    }
}