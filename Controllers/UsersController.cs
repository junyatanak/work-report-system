using System.Data;
using DailyWorkReport.Constants;
using DailyWorkReport.Models;
using DailyWorkReport.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DailyWorkReport.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        // GET: UsersController
        public async Task<IActionResult> Index()
        {
            var users = new List<UserDisplayViewModel>();

            foreach(var user in _userManager.Users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var roleName = roles.FirstOrDefault() ?? string.Empty;
                users.Add(new UserDisplayViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    RoleName = roleName
                });
            }

            return View(users);
        }

    }
}
