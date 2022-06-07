using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using App.Core.Entities;
using App.SharedLib.Repository.Interface;
using Sophie.Services.EmailSenderService;
using Sophie.Units;
using App.Core.Constants;

namespace Sophie.Areas.Admin.Pages.User
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly IAuthRepository _authRepo;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(IAuthRepository authRepo, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<ExternalLoginModel> logger, IEmailSender emailSender)
        {
            _authRepo = authRepo;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string LoginProvider { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public IActionResult OnGetAsync()
        {
            return RedirectToPage("./Login");
        }

        public IActionResult OnPostAsync(string provider, string? returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            Logs.debug("Properties: " + JsonSerializer.Serialize(properties));
            return new ChallengeResult(provider, properties);
        }

        // For test
        //https://localhost:5001/Admin/Account/ExternalLogin?handler=Login&provider=Google&returnUrl=/Admin/Account/Login
        //https://localhost:44350/Admin/Account/ExternalLogin?handler=Login&provider=Google&returnUrl=/Admin/Account/Login
        public IActionResult OnGetLoginAsync(string provider, string? returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            Logs.debug("Properties: " + JsonSerializer.Serialize(properties));
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                Logs.debug($"Error from external provider: {remoteError}");
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                Logs.debug("Error loading external login information.");
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
            Logs.debug("LoginProvider: " + info.LoginProvider + ", ProviderKey: " + info.ProviderKey);

            // Sign in the user with this external login provider if the user already has a login.
            // Notes: setting in CMS
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }

            if (result.IsLockedOut) {
                return RedirectToPage("./Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ReturnUrl = returnUrl;
                LoginProvider = info.LoginProvider;
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    Input = new InputModel
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                    };
                    Logs.debug("Auto login Google if same email: " + Input.Email);
                    var user = await _signInManager.UserManager.FindByEmailAsync(Input.Email);
                    if (user != null)
                    {
                        IList<string> roles = await _authRepo.GetListRoleWithUser(user.UserName);
                        string[] listRoles = roles.ToArray();
                        if (listRoles.Contains(RolePrefix.AdminSys) || listRoles.Contains(RolePrefix.Developer))
                        {
                            //result = await _signInManager.PasswordSignInAsync(user.Email, new Encrypt(user.Password).DecryptString(), true, lockoutOnFailure: false);
                            IdentityResult signInResult = await _userManager.AddLoginAsync(user, info);
                            if (signInResult.Succeeded)
                            {
                                await _signInManager.SignInAsync(user, isPersistent: false);
                                _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                                return LocalRedirect(returnUrl);
                            }
                        }
                        Logs.debug("Error user not grant login permission with google.");
                        ErrorMessage = "Error: User not grant login permission with google.";
                        return RedirectToPage("./Login", new { ReturnUrl = returnUrl, ErrorMessage });
                    }else {
                        Logs.debug("Error user not found.");
                        ErrorMessage = "Error: Invalid login attempt.";
                        return RedirectToPage("./Login", new { ReturnUrl = returnUrl, ErrorMessage });
                    }
                }
                return Page();
            }
        }

        // Confirmation and create user
        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            // Get the information about the user from the external login provider
            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                        // If account confirmation is required, we need to show the link if we don't have a real email sender
                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("./RegisterConfirmation", new { Email = Input.Email });
                        }

                        await _signInManager.SignInAsync(user, isPersistent: false);
                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code },
                            protocol: Request.Scheme);

                        var subject = "Confirm your email address";
                        var htmlContent = $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";
                        await _emailSender.SendEmail(Input.Email, subject, htmlContent);

                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            LoginProvider = info.LoginProvider;
            ReturnUrl = returnUrl;
            return Page();
        }
    }
}
