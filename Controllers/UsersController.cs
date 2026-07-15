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
            await PopulateRoleSelectList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel vm)
        {
            if(!ModelState.IsValid)
            {
                await PopulateRoleSelectList(vm.RoleName);
                return View(vm);
            }

            if(!await _roleManager.RoleExistsAsync(vm.RoleName))
            {
                ModelState.AddModelError(string.Empty, "Selected role does not exist.");
                await PopulateRoleSelectList(vm.RoleName);
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
                await PopulateRoleSelectList(vm.RoleName);
                return View(vm);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, vm.RoleName);
            if(!roleResult.Succeeded)
            {
                foreach(var error in roleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                await PopulateRoleSelectList(vm.RoleName);
                return View(vm);
            }

            return RedirectToAction(nameof(Index));

        }


        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if(string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if(user == null) return NotFound();
            var roles = await _userManager.GetRolesAsync(user);
            var vm = new UserDisplayViewModel
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                RoleName = roles.FirstOrDefault() ?? string.Empty
            };
            await PopulateRoleSelectList(vm.RoleName);
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserDisplayViewModel input)
        {
            if(string.IsNullOrEmpty(id) || id != input.Id) return NotFound();
            if(!ModelState.IsValid)
            {
                await PopulateRoleSelectList(input.RoleName);
                return View(input);
            }
            var user = await _userManager.FindByIdAsync(id);
            if(user == null) return NotFound();
            user.UserName = input.UserName;
            var updateResult = await _userManager.UpdateAsync(user);
            if(!updateResult.Succeeded)
            {
                foreach(var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                await PopulateRoleSelectList(input.RoleName);
                return View(input);
            }
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!currentRoles.Contains(input.RoleName))
            {
                if (currentRoles.Any())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!removeResult.Succeeded)
                    {
                        foreach (var error in removeResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        await PopulateRoleSelectList(input.RoleName);
                        return View(input);
                    }
                }
                var addResult = await _userManager.AddToRoleAsync(user, input.RoleName);
                if (!addResult.Succeeded)
                {
                    foreach (var error in addResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    await PopulateRoleSelectList(input.RoleName);
                    return View(input);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateRoleSelectList(string? selected = null)
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewData["RoleName"] = new SelectList(roles, "Name", "Name", selected);            
        }


    }
}
