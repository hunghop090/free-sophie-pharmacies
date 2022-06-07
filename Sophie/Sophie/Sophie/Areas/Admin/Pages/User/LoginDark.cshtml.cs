using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using App.Core.Entities;
using App.SharedLib.Repository.Interface;
using Sophie.Units;

namespace Sophie.Areas.Admin.Pages.User
{
    [AllowAnonymous]
    public class LoginDarkModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly IAuthRepository _authRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginDarkModel(IConfiguration config, IAuthRepository authRepo, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<LoginModel> logger)
        {
            _config = config;
            _authRepo = authRepo;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        [TempData]
        public string ReturnUrl { get; set; }
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }


        public async Task OnGetAsync(string? returnUrl = null, string? errorMessage = null)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                ErrorMessage = errorMessage;
                ModelState.AddModelError(string.Empty, errorMessage.Replace("Error: ", ""));
            }
            returnUrl = returnUrl ?? Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ReturnUrl = returnUrl;
        }


        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl = "/admin/dashboard";//returnUrl ?? Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                Logs.debug("Login : " + result);
                Logs.debug("UserLogin : " + JsonSerializer.Serialize(Input));

                if (result.Succeeded)
                {
                    var user = await _signInManager.UserManager.FindByEmailAsync(Input.Email);
                    _logger.LogInformation("User logged in: " + JsonSerializer.Serialize(user));

                    // Registry cookie
                    //await RegistryCookie(user);
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }
            else
            {
                string messages = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                Logs.debug("Error: " + messages);
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private async Task RegistryCookie(ApplicationUser userDb)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(JwtRegisteredClaimNames.NameId, userDb.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.UniqueName, userDb.UserName));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, userDb.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Amr, userDb.TwoFactorEnabled ? "mfa" : "pwd"));

            var roles = await _authRepo.GetListRoleWithUser(userDb.UserName);
            claims.AddRange(roles.Select(r => new Claim("role", r)));

            ClaimsIdentity identity = new ClaimsIdentity(claims, "Cookies", "user", "role");
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme, principal: principal, properties: new AuthenticationProperties
            {
                IsPersistent = true, // for 'remember me' feature
                ExpiresUtc = DateTimes.Now().AddMinutes(60)
            });
        }
    }
}
