using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Sophie.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;


        public IndexModel(ILogger<IndexModel> logger, SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        //public void OnGet()
        //{
        //}

        public IActionResult OnGet()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return LocalRedirect("/admin/dashboard");
            }

            return Redirect("/admin/user/login");
        }
    }
}
