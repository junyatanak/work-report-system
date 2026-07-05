using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using DailyWorkReport.Models;


namespace DailyWorkReport.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }
        // GET: AccountController
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        

    }
}
