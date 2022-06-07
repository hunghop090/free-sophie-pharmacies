using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using App.Core.Entities;
using App.Core.Units.Log4Net;
using Sophie.Views;

namespace Sophie.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogManager _log4net;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public HomeController(ILogManager log4net, ILogger<HomeController> logger, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _log4net = log4net;
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View("Index");
        }

        public IActionResult Privacy()
        {
            return View("Privacy");
        }

        public IActionResult Login()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return LocalRedirect("/admin/dashboard");
            }

            return Redirect("/admin/user/login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string code)
        {
            return View(new ErrorMVCModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, StatusCode = code });
        }
    }

}
