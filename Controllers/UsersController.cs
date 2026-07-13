using System.Data;
using DailyWorkReport.Constants;
using DailyWorkReport.Models;
using DailyWorkReport.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DailyWorkReport.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
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
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewData["RoleName"] = new SelectList(roles, "Name", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel vm)
        {
            if(!ModelState.IsValid)
            {
                var roles = await _roleManager.Roles.ToListAsync();
                ViewData["RoleName"] = new SelectList(roles, "Name", "Name",vm.RoleName);
                return View(vm);
            }

            var user = new ApplicationUser
            {
                UserName = vm.UserName
            };

            var result = await _userManager.CreateAsync(user, vm.Password);
            if(!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                var roles = await _roleManager.Roles.ToListAsync();
                ViewData["RoleName"] = new SelectList(roles, "Name", "Name",vm.RoleName);
                return View(vm);
            }
            await _userManager.AddToRoleAsync(user, vm.RoleName);
            return RedirectToAction(nameof(Index));

        }


    }
}
