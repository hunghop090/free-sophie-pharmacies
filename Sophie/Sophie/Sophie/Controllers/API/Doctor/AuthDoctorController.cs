using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using log4net;
using App.Core.Constants;
using App.Core.Policy;
using App.Core.Entities;
using Sophie.Services.EmailSenderService;
using App.Core.Units.Log4Net;
using App.SharedLib.Repository.Interface;
using App.Core.Settings;
using App.Core.Units;
using Sophie.Units;
using Sophie.Resource.Dtos;
using Sophie.Resource.Entities;
using Sophie.Repository.Interface;
using App.Core.Middleware;
using Sophie.Model;
using Sophie.Resource.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;

namespace Sophie.Controllers.API
{
    [ApiController]
    [Produces("application/json")]
    [Route(RoutePrefix.API_DOCTOR)]//api/[controller]
    [ApiExplorerSettings(GroupName = "v6")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = PolicyPrefix.RequiresUser)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [MultiPolicysAuthorizeAttribute(Policys = RolePrefix.Doctor, IsAnd = false)]
    public class AuthDoctorController : BaseController
    {
        private readonly ILog _log4net = log4net.LogManager.GetLogger(typeof(AuthDoctorController));
        private readonly ILogger<AuthDoctorController> _logger;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly IAccountRepository _accountRepository;
        private readonly ILoginProviderRepository _loginProviderRepository;

        private readonly int _dayExpiredLogin = 30;// 30 day - config in appsettings.json

        public AuthDoctorController(ILogManager logManager, IMapper mapper, IConfiguration configuration, IStringLocalizer localizer, IAuthRepository authRepo, IOptions<ApplicationSettings> applicationSettingAccessor,
            IHttpContextAccessor httpContextAccessor, IEmailSender emailSender, ILogger<AuthDoctorController> logger, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager,
            IAccountRepository accountRepository, ILoginProviderRepository loginProviderRepository)
            : base(logManager, mapper, configuration, localizer, authRepo, applicationSettingAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _accountRepository = accountRepository;
            _loginProviderRepository = loginProviderRepository;
            if (_applicationSettings.DayExpiredLogin > 0) _dayExpiredLogin = _applicationSettings.DayExpiredLogin;
        }

#if DEBUG
        /// <summary>
        /// GoogleTest https://developers.google.com/oauthplayground
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(object), 200)]
        [HttpPost("GoogleTest")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleTest(string access_token, string id_token)
        {
            try
            {
                var result = await _loginProviderRepository.GoogleTest(access_token, id_token);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        /// <summary>
        /// FacebookTest https://developers.facebook.com/tools/explorer
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(object), 200)]
        [HttpPost("FacebookTest")]
        [AllowAnonymous]
        public async Task<IActionResult> FacebookTest(string access_token)
        {
            try
            {
                var result = await _loginProviderRepository.FacebookTest(access_token);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        /// <summary>
        /// AppleTest 
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(object), 200)]
        [HttpPost("AppleTest")]
        [AllowAnonymous]
        public IActionResult AppleTest(string identityToken = "eyJraWQiOiJZdXlYb1kiLCJhbGciOiJSUzI1NiJ9.eyJpc3MiOiJodHRwczovL2FwcGxlaWQuYXBwbGUuY29tIiwiYXVkIjoiY29tLmhlYWx0aGNhcmUuc29waGllLmRldiIsImV4cCI6MTYzNzk5MDQ3NiwiaWF0IjoxNjM3OTA0MDc2LCJzdWIiOiIwMDE5NzIuNDkxYWYxZGIxNmQ5NGVkZDhjZjU0MDg2ODNkMTVkZGMuMDQxNCIsImNfaGFzaCI6IlJ3b3lUaFM0NENFek5DVUE2TGI2Z0EiLCJlbWFpbCI6ImNudHRpdGszNkBnbWFpbC5jb20iLCJlbWFpbF92ZXJpZmllZCI6InRydWUiLCJhdXRoX3RpbWUiOjE2Mzc5MDQwNzYsIm5vbmNlX3N1cHBvcnRlZCI6dHJ1ZX0.O2eUzdgZi5qiolkfGA8R4mWdgea22NXBKVEOxlkALYim_fo-wtwHf6ncLoUAAVyMlNSg-sI_5z8fbhJSUrDMW1yOo-M2SaL4keQWcvnoGAb6edNF4k4-FmTH4EnIAmC7mU-XzthtYTXbfBRAkc1M_mKUxtMYwsopNoyZ1tEm98aFrxY5RNngBX-BLD_7id_C18I7C9VUCIK6PbD5QGQqEf0RpAZrPaktei69cm_wwCDzrDAQFcBAN-b0TFXUgnlG7GCo_cQgMnMTwGPfBtAj1rftMgJZJx_NsI7SEH7ESH7kYVdMV7TGj0jBg_4EKn9Ly_DCdRWX5P7UJx-8K189iw")
        {
            try
            {
                var result = _loginProviderRepository.AppleTest(identityToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
#endif


        /// <summary>
        /// Google SignIn Register
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // GET: api/doctor/AuthDoctor/GoogleSignIn
        [ProducesResponseType(typeof(object), 200)]
        [HttpPost("GoogleSignIn")]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> GoogleSignIn([FromBody] GoogleSocialCommand model)
        {
            try
            {
                GoogleTokenResponse verifyTokenResponse = await _loginProviderRepository.VerifyGoogleToken(model);
                if (verifyTokenResponse == null) return ResponseBadRequest(new CustomBadRequest("Google token invalid", this.ControllerContext));
                if (verifyTokenResponse.Error.Count > 0) return ResponseBadRequest(new CustomBadRequest("Google bad request", verifyTokenResponse.Error, this.ControllerContext));

                // Validate from google
                string email = model.Profile?.Email;
                string mobile = model.Profile?.Mobile;
                string name = model.Profile?.Name;
                string given_name = model.Profile?.Given_name;
                string family_name = model.Profile?.Family_name;
                if (string.IsNullOrEmpty(email)) return ResponseBadRequest(new CustomBadRequest("We're sorry. We were unable to create your account using Google because there is no email address associated with your Google account. Please add an email address to your Google account and try again", this.ControllerContext));
                if (string.IsNullOrEmpty(email)) return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_INVALID_EMAIL"), this.ControllerContext));
                if (string.IsNullOrEmpty(name)) return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_NOT_GRANT_PERMISSION"), this.ControllerContext));
                if (string.IsNullOrEmpty(given_name)) return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_INVALID_FIRSTNAME"), this.ControllerContext));
                if (string.IsNullOrEmpty(family_name)) return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_INVALID_LASTNAME"), this.ControllerContext));

                // Validate account
                Account account = _accountRepository.FindByEmailAccount(email);
                bool isUpdatedProfile = true; // Đã cập nhật thông tin profile hay chưa?, mặc định đã cập nhật
                if (account == null) {
                    account = new Account()
                    {
                        TypeLogin = TypeLogin.Google,
                        Confirm = true,
                        Active = TypeActive.Actived,
                        PhoneNumber = mobile,
                        Email = email,
                        Username = "",
                        Password = "",
                        Firstname = given_name,
                        Lastname = family_name,
                        Fullname = name,
                        Avatar = model.Profile.Picture
                    };
                    account.TypeLogin = TypeLogin.Google;
                    account.Active = TypeActive.Actived;
                    account = _accountRepository.CreateAccount(account);
                    isUpdatedProfile = false;
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.Profile.Picture))
                    {
                        account.Avatar = model.Profile.Picture;
                        account.Updated = DateTimes.Now();
                        account = _accountRepository.UpdateAccount(account);
                    }
                }

                RefreshToken refreshTokenDb = CreateRefreshTokenDB(account.AccountId, _dayExpiredLogin); // Expiration: AddDays(1)
                JwtSecurityToken accessToken = await CreateAccessToken(account, refreshTokenDb.Token);
                // write token in JwtSecurityTokenHandler
                string writeToken = new JwtSecurityTokenHandler().WriteToken(accessToken);

                if (string.IsNullOrEmpty(account.Email) || string.IsNullOrEmpty(account.Fullname) || string.IsNullOrEmpty(account.Firstname) || string.IsNullOrEmpty(account.Lastname) || account.Birthdate == null || account.Gender == null)
                {
                    isUpdatedProfile = false;
                }

                Logs.debug("GoogleSignIn: " + Newtonsoft.Json.JsonConvert.SerializeObject(model, JsonSettings.SettingForNewtonsoft));
                return ResponseData(new
                {
                    AccessToken = writeToken,
                    Expired = accessToken.ValidTo,
                    RefreshToken = refreshTokenDb.Token,
                    RefreshTokenExpired = refreshTokenDb.Expired,
                    Provider = account.TypeLogin,
                    Confirm = account.Confirm,
                    IsUpdatedProfile = isUpdatedProfile
                });
            }
            catch (Exception ex)
            {
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/Auth/GoogleSignIn", ex, model);
            }
        }

        /// <summary>
        /// Facebook SignIn Register
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // GET: api/doctor/AuthDoctor/FacebookSignIn
        [ProducesResponseType(typeof(object), 200)]
        [HttpPost("FacebookSignIn")]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> FacebookSignIn([FromBody] FacebookLoginSocialCommand model)
        {
            try
            {
                FacebookTokenResponse verifyTokenResponse = await _loginProviderRepository.VerifyFacebookToken(model);
                if (verifyTokenResponse == null) return ResponseBadRequest(new CustomBadRequest("Facebook token invalid", this.ControllerContext));
                if (verifyTokenResponse.Error.Count > 0) return ResponseBadRequest(new CustomBadRequest("Facebook bad request", verifyTokenResponse.Error, this.ControllerContext));

                // Validate from facebook
                string email = model.Profile?.Email;
                string mobile = model.Profile?.Mobile;
                string name = model.Profile?.Name;
                string first_name = model.Profile?.First_name;
                string last_name = model.Profile?.Last_name;
                if (string.IsNullOrEmpty(email)) return ResponseBadRequest(new CustomBadRequest("We're sorry. We were unable to create your account using Facebook because there is no email address associated with your Facebook account. Please add an email address to your Facebook account and try again", this.ControllerContext));
                if (string.IsNullOrEmpty(email)) return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_INVALID_EMAIL"), this.ControllerContext));
                if (string.IsNullOrEmpty(name)) return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_NOT_GRANT_PERMISSION"), this.ControllerContext));
                if (string.IsNullOrEmpty(first_name)) return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_INVALID_FIRSTNAME"), this.ControllerContext));
                if (string.IsNullOrEmpty(last_name)) return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_INVALID_LASTNAME"), this.ControllerContext));

                // Validate account
                Account account = _accountRepository.FindByEmailAccount(email);
                bool isUpdatedProfile = true; // Đã cập nhật thông tin profile hay chưa?, mặc định đã cập nhật
                if (account == null)
                {
                    account = new Account()
                    {
                        TypeLogin = TypeLogin.Facebook,
                        Confirm = true,
                        Active = TypeActive.Actived,
                        PhoneNumber = mobile,
                        Email = email,
                        Username = "",
                        Password = "",
                        Firstname = first_name,
                        Lastname = last_name,
                        Fullname = name,
                    };
                    account.TypeLogin = TypeLogin.Facebook;
                    account.Active = TypeActive.Actived;
                    account = _accountRepository.CreateAccount(account);
                    isUpdatedProfile = false;
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.Profile.Picture))
                    {
                        account.Avatar = model.Profile.Picture;
                        account.Updated = DateTimes.Now();
                        account = _accountRepository.UpdateAccount(account);
                    }
                }

                RefreshToken refreshTokenDb = CreateRefreshTokenDB(account.AccountId, _dayExpiredLogin); // Expiration: AddDays(1)
                JwtSecurityToken accessToken = await CreateAccessToken(account, refreshTokenDb.Token);
                // write token in JwtSecurityTokenHandler
                string writeToken = new JwtSecurityTokenHandler().WriteToken(accessToken);

                if (string.IsNullOrEmpty(account.Email) || string.IsNullOrEmpty(account.Fullname) || string.IsNullOrEmpty(account.Firstname) || string.IsNullOrEmpty(account.Lastname) || account.Birthdate == null || account.Gender == null)
                {
                    isUpdatedProfile = false;
                }

                Logs.debug("FacebookSignIn: " + Newtonsoft.Json.JsonConvert.SerializeObject(model, JsonSettings.SettingForNewtonsoft));
                return ResponseData(new
                {
                    AccessToken = writeToken,
                    Expired = accessToken.ValidTo,
                    RefreshToken = refreshTokenDb.Token,
                    RefreshTokenExpired = refreshTokenDb.Expired,
                    Provider = account.TypeLogin,
                    Confirm = account.Confirm,
                    IsUpdatedProfile = isUpdatedProfile
                });
            }
            catch (Exception ex)
            {
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/Auth/FacebookSignIn", ex, model);
            }
        }

        /// <summary>
        /// Apple SignIn Register
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // GET: api/doctor/AuthDoctor/AppleSignIn
        [ProducesResponseType(typeof(object), 200)]
        [HttpPost("AppleSignIn")]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> AppleSignIn([FromBody] AppleLoginSocialCommand model)
        {
            try
            {
                AppleTokenResponse verifyTokenResponse = await _loginProviderRepository.VerifyAppleToken(model);
                if (verifyTokenResponse == null) return ResponseBadRequest(new CustomBadRequest("Apple token invalid", this.ControllerContext));
                if (verifyTokenResponse.Error.Count > 0) return ResponseBadRequest(new CustomBadRequest("Apple bad request", verifyTokenResponse.Error, this.ControllerContext));

                // Validate from apple
                string email = model.Profile?.Email;
                string mobile = model.Profile?.Mobile;
                string name = model.Profile?.Name;
                string given_name = model.Profile?.Given_name;
                string family_name = model.Profile?.Family_name;
                if (string.IsNullOrEmpty(email)) return ResponseBadRequest(new CustomBadRequest("We're sorry. We were unable to create your account using Apple because there is no email address associated with your Apple account. Please add an email address to your Apple account and try again", this.ControllerContext));
                if (string.IsNullOrEmpty(email)) return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_INVALID_EMAIL"), this.ControllerContext));
                if (string.IsNullOrEmpty(name)) return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_NOT_GRANT_PERMISSION"), this.ControllerContext));
                if (string.IsNullOrEmpty(given_name)) return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_INVALID_FIRSTNAME"), this.ControllerContext));
                if (string.IsNullOrEmpty(family_name)) return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_INVALID_LASTNAME"), this.ControllerContext));

                // Validate account
                Account account = _accountRepository.FindByEmailAccount(email);
                bool isUpdatedProfile = true; // Đã cập nhật thông tin profile hay chưa?, mặc định đã cập nhật
                if (account == null)
                {
                    account = new Account()
                    {
                        TypeLogin = TypeLogin.Apple,
                        Confirm = true,
                        Active = TypeActive.Actived,
                        PhoneNumber = mobile,
                        Email = email,
                        Username = "",
                        Password = "",
                        Firstname = given_name,
                        Lastname = family_name,
                        Fullname = name,
                    };
                    account.TypeLogin = TypeLogin.Apple;
                    account.Active = TypeActive.Actived;
                    account = _accountRepository.CreateAccount(account);
                    isUpdatedProfile = false;
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.Profile.Picture))
                    {
                        account.Avatar = model.Profile.Picture;
                        account.Updated = DateTimes.Now();
                        account = _accountRepository.UpdateAccount(account);
                    }
                }

                RefreshToken refreshTokenDb = CreateRefreshTokenDB(account.AccountId, _dayExpiredLogin); // Expiration: AddDays(1)
                JwtSecurityToken accessToken = await CreateAccessToken(account, refreshTokenDb.Token);
                // write token in JwtSecurityTokenHandler
                string writeToken = new JwtSecurityTokenHandler().WriteToken(accessToken);

                if (string.IsNullOrEmpty(account.Email) || string.IsNullOrEmpty(account.Fullname) || string.IsNullOrEmpty(account.Firstname) || string.IsNullOrEmpty(account.Lastname) || account.Birthdate == null || account.Gender == null)
                {
                    isUpdatedProfile = false;
                }

                Logs.debug("AppleSignIn: " + Newtonsoft.Json.JsonConvert.SerializeObject(model, JsonSettings.SettingForNewtonsoft));
                return ResponseData(new
                {
                    AccessToken = writeToken,
                    Expired = accessToken.ValidTo,
                    RefreshToken = refreshTokenDb.Token,
                    RefreshTokenExpired = refreshTokenDb.Expired,
                    Provider = account.TypeLogin,
                    Confirm = account.Confirm,
                    IsUpdatedProfile = isUpdatedProfile
                });
            }
            catch (Exception ex)
            {
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/Auth/AppleSignIn", ex, model);
            }
        }


        /// <summary>
        /// Registry user
        /// </summary>
        /// <param name="model"></param>    
        /// <returns>Result registry for user</returns>
        // GET: api/doctor/AuthDoctor/Register
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(AccountRegistryDto model)
        {
            try
            {
                // Validate ModelState
                CustomBadRequest? resultValidate = IsEmptyModel(model);
                if (resultValidate != null) return ResponseNotFound(resultValidate);

                // Validate account
                Account account = null;
                if (model.TypeLogin == TypeLogin.Phone)
                {
                    if (!StringEx.IsValidPhone(model.PhoneNumber)) return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_INVALID_PHONE_FORMAT"), this.ControllerContext));
                    account = _accountRepository.FindByPhoneAccount(model.PhoneNumber);
                }
                else if (model.TypeLogin == TypeLogin.Email)
                {
                    if (!StringEx.IsValidEmail(model.Email)) return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_INVALID_EMAIL_FORMAT"), this.ControllerContext));
                    account = _accountRepository.FindByEmailAccount(model.Email);
                }
                else if (model.TypeLogin == TypeLogin.Other)
                {
                    if (!StringEx.IsValidUsername(model.Username)) return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_INVALID_USERNAME_FORMAT"), this.ControllerContext));
                    account = _accountRepository.FindByUsernameAccount(model.Username);
                }
                else
                {
                    return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_EMAIL_NOT_SUPPORT_LOGIN"), this.ControllerContext));
                }
                if (account != null) return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_EMAIL_REGISTERED_USER"), this.ControllerContext));

                account = _mapper.Map<Account>(model);
                account.Active = TypeActive.Pending;
                account.Password = new Encrypt(model.Password).EncryptString();
                account = _accountRepository.CreateAccount(account);

                RefreshToken refreshTokenDb = CreateRefreshTokenDB(account.AccountId, _dayExpiredLogin); // Expiration AddDays(1)
                JwtSecurityToken accessToken = await CreateAccessToken(account, refreshTokenDb.Token);
                // write token in JwtSecurityTokenHandler
                string writeToken = new JwtSecurityTokenHandler().WriteToken(accessToken);

                LogUserEvent(_logger, TypeAction.Update, TypeStatus.Success, $"{RoutePrefix.ACCOUNT}/Auth/login", $"User register {account.PhoneNumber}", null, null);
                return ResponseData(new
                {
                    AccessToken = writeToken,
                    Expired = accessToken.ValidTo,
                    RefreshToken = refreshTokenDb.Token,
                    RefreshTokenExpired = refreshTokenDb.Expired,
                    Provider = account.TypeLogin,
                    Confirm = account.Confirm,
                    IsUpdatedProfile = false
                });
            }
            catch (Exception ex)
            {
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/Auth/Register", ex, model);
            }
        }

        /// <summary>
        /// Login for user, require password encode md5
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Result login for user</returns>
        // POST: api/doctor/AuthDoctor/Login
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Login model)
        {
            try
            {
                // Validate ModelState
                CustomBadRequest resultValidate = IsEmptyModel(model);
                if (resultValidate != null) return ResponseNotFound(resultValidate);

                // Validate account
                Account account = null;
                if (model.TypeLogin == TypeLogin.Phone)
                {
                    if (!StringEx.IsValidPhone(model.PhoneNumber)) return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_INVALID_PHONE_FORMAT"), this.ControllerContext));
                    account = _accountRepository.FindByPhoneAccount(model.PhoneNumber);
                }
                else if (model.TypeLogin == TypeLogin.Email)
                {
                    if (!StringEx.IsValidEmail(model.Email)) return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_INVALID_EMAIL_FORMAT"), this.ControllerContext));
                    account = _accountRepository.FindByEmailAccount(model.Email);
                }
                else if (model.TypeLogin == TypeLogin.Other)
                {
                    if (!StringEx.IsValidUsername(model.Username)) return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_INVALID_USERNAME_FORMAT"), this.ControllerContext));
                    account = _accountRepository.FindByUsernameAccount(model.Username);
                }else
                {
                    return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_EMAIL_NOT_SUPPORT_LOGIN"), this.ControllerContext));
                }
                if (account == null) return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_LOGIN_ATTEMPT"), this.ControllerContext));

                // Validate password
                string hashedPassword = model.Password;                                  //Password MD5/BCrypt
                string providedPassword = new Encrypt(account.Password).DecryptString(); //Password DB
                if (string.IsNullOrEmpty(hashedPassword)) return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_LOGIN_ATTEMPT"), this.ControllerContext));

                // Verify password by md5
                PasswordVerificationResult passwordVerificationResult = new Encrypt(hashedPassword).VerifyTextHashMD5(providedPassword);
                if (passwordVerificationResult != PasswordVerificationResult.Success) return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_PASS_FAILD_MD5"), this.ControllerContext));

                // Login
                if (account.TwoFactorEnabled)
                {
                    // Verify 2-factor authentication is here
                    string code2FA = model.Code2FA;
                }

                RefreshToken refreshTokenDb = CreateRefreshTokenDB(account.AccountId, _dayExpiredLogin); // Expiration: AddDays(1)
                JwtSecurityToken accessToken = await CreateAccessToken(account, refreshTokenDb.Token);
                // write token in JwtSecurityTokenHandler
                string writeToken = new JwtSecurityTokenHandler().WriteToken(accessToken);

                bool isUpdatedProfile = true; // Đã cập nhật thông tin profile hay chưa?, mặc định đã cập nhật
                if (string.IsNullOrEmpty(account.Email) || string.IsNullOrEmpty(account.Fullname) || string.IsNullOrEmpty(account.Firstname) || string.IsNullOrEmpty(account.Lastname) || account.Birthdate == null || account.Gender == null)
                {
                    isUpdatedProfile = false;
                }

                SetCookie("Access_Token", writeToken);
                SetCookie<Account>("Access_User", account);

                LogUserEvent(_logger, TypeAction.Load, TypeStatus.Success, $"{RoutePrefix.ACCOUNT}/Auth/login", $"User login {account.PhoneNumber}", null, null);
                return ResponseData(new
                {
                    AccessToken = writeToken,
                    Expired = accessToken.ValidTo,
                    RefreshToken = refreshTokenDb.Token,
                    RefreshTokenExpired = refreshTokenDb.Expired,
                    Provider = account.TypeLogin,
                    Confirm = account.Confirm,
                    IsUpdatedProfile = isUpdatedProfile
                });
            }
            catch (Exception ex)
            {
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/Auth/login", ex, model);
            }
        }

        /// <summary>
        /// Refresh token for user logged
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns>New token for user logged</returns>
        // GET: api/doctor/AuthDoctor/RefreshToken
        [HttpPut("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromForm] string refreshToken)
        {
            try
            {
                RefreshToken refreshTokenDb = _accountRepository.FindByRefreshToken(refreshToken);
                if (refreshTokenDb is null) return ResponseNotFound(new CustomBadRequest(localizer("AUTH_TOKEN_NOT_FOUND"), this.ControllerContext));
                if (refreshTokenDb.Expired <= DateTimes.Now()) return ResponseUnauthorized(new CustomBadRequest(localizer("AUTH_TOKEN_EXPIRED"), this.ControllerContext));

                Account account = _accountRepository.FindByIdAccount(refreshTokenDb.UserId);
                if (account is null) return ResponseNotFound(new CustomBadRequest(localizer("BASE_USER_NOT_FOUND"), this.ControllerContext));

                string deviceName = (HttpContext.Items["deviceName"] as string) ?? "Chrome";
                string deviceId = (HttpContext.Items["deviceId"] as string) ?? "Chrome.xx.xx.xxxx.xx";
                refreshTokenDb = _accountRepository.RefreshToken(refreshTokenDb.Token, deviceName, deviceId, _dayExpiredLogin); // Expiration Date AddDays(1)

                JwtSecurityToken accessToken = await CreateAccessToken(account, refreshTokenDb.Token);
                // write token in JwtSecurityTokenHandler
                string writeToken = new JwtSecurityTokenHandler().WriteToken(accessToken);

                LogUserEvent(_logger, TypeAction.Update, TypeStatus.Success, $"{RoutePrefix.ACCOUNT}/Auth/RefreshToken", $"User refresh token", null, null);
                return ResponseData(new
                {
                    AccessToken = writeToken,
                    RefreshToken = refreshTokenDb.Token
                });
            }
            catch (Exception ex)
            {
                LogUserEvent(_logger, TypeAction.Update, TypeStatus.Failure, $"{RoutePrefix.ACCOUNT}/Auth/RefreshToken", $"Error user refresh token", ex, null);
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/Auth/RefreshToken", ex);
            }
        }

        /// <summary>
        /// Logout for user logged
        /// </summary>
        /// <param name="refreshToken"></param>    
        /// <returns>Result logout for user logged</returns>
        // GET: api/doctor/AuthDoctor/Logout
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromForm] string? refreshToken)
        {
            try
            {
                if (string.IsNullOrEmpty(refreshToken))
                {
                    RefreshToken refreshTokenDb = _accountRepository.FindByRefreshToken(refreshToken);
                    if (refreshTokenDb != null)
                    {
                        _accountRepository.RevokeToken(refreshTokenDb.Token);
                    }
                    _accountRepository.RemoveAllToken(refreshTokenDb.UserId);
                }
                
                await SignOutCookie();

                LogUserEvent(_logger, TypeAction.Delete, TypeStatus.Success, $"{RoutePrefix.ACCOUNT}/Auth/Logout", $"User logout", null, null);
                return ResponseData(new { Message = "Logout Success" });
            }
            catch (Exception ex)
            {
                LogUserEvent(_logger, TypeAction.Delete, TypeStatus.Failure, $"{RoutePrefix.ACCOUNT}/Auth/Logout", $"Error user logout", ex, null);
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/Auth/Logout", ex);
            }
        }

        /// <summary>
        /// Check user exits
        /// </summary>
        /// <param name="phoneNumber"></param>    
        /// <returns>Result check user exits</returns>
        // GET: api/doctor/AuthDoctor/Check
        [HttpPost("Check")]
        [AllowAnonymous]
        public IActionResult Check([FromForm] string phoneNumber = "+84389955141")
        {
            try
            {
                // Validate account
                Account account = account = _accountRepository.FindByPhoneAccount(phoneNumber);
                if (account == null) return ResponseData(new { IsCheck = false, Message = localizer("BASE_USER_NOT_FOUND") });

                LogUserEvent(_logger, TypeAction.Delete, TypeStatus.Success, $"{RoutePrefix.ACCOUNT}/Auth/Check", $"User check", null, null);
                return ResponseData(new { Timestamp = DateTimes.Now(), IsCheck = true, Message = localizer("AUTH_EMAIL_REGISTERED_USER") });
            }
            catch (Exception ex)
            {
                LogUserEvent(_logger, TypeAction.Delete, TypeStatus.Failure, $"{RoutePrefix.ACCOUNT}/Auth/Check", $"Error user check", ex, null);
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/Auth/Check", ex);
            }
        }

        #region Render Token

        private RefreshToken CreateRefreshTokenDB(string accountId, int dayExpiration)
        {
            string deviceName = (HttpContext.Items["deviceName"] as string) ?? "Chrome";
            string deviceId = (HttpContext.Items["deviceId"] as string) ?? "Chrome.xx.xx.xxxx.xx";

            RefreshToken item = new RefreshToken
            {
                Id = Guid.NewGuid().ToString(),
                UserId = accountId,
                TypeUser = TypeUser.ACCOUNT,
                TypeDevice = TypeDevice.MOBILE,
                DeviceName = deviceName,
                DeviceId = deviceId,
                Token = Guid.NewGuid().ToString(),
                Expired = DateTimes.Now().AddDays(dayExpiration),
                TotalRefresh = 0,
                TokenFCM = "",
                Location = "Viet Nam"
            };

            RefreshToken refreshTokenDb = _accountRepository.CreateToken(item);
            return refreshTokenDb;
        }

        private async Task<JwtSecurityToken> CreateAccessToken(Account account, string refreshTokenDb)
        {
            List<Claim> claims = await RenderClaimsAsync(account);
            claims.Add(new Claim(JwtSecurityTokenValidator.Prefix_RefreshToken, refreshTokenDb));
            SigningCredentials signingCredentials = RenderCredentials();

            var accessToken = new JwtSecurityToken(
                audience: new Encrypt(_configuration["Authentication:JwtBearer:Audience"]).DecryptString(),
                issuer: new Encrypt(_configuration["Authentication:JwtBearer:Issuer"]).DecryptString(),
                claims: claims,
                expires: DateTimes.Now().AddDays(_dayExpiredLogin), // Expiration: AddDays(1)
                signingCredentials: signingCredentials
            );
            return accessToken;
        }

        private SigningCredentials RenderCredentials()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(new Encrypt(_configuration["Authentication:JwtBearer:Symmetric:Key"]).DecryptString()));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            return signingCredentials;
        }

        private async Task<List<Claim>> RenderClaimsAsync(Account account)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(JwtRegisteredClaimNames.NameId, account.AccountId ?? ""));
            claims.Add(new Claim(JwtRegisteredClaimNames.UniqueName, account.Username ?? ""));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, account.Email ?? ""));
            claims.Add(new Claim(JwtRegisteredClaimNames.Amr, account.TwoFactorEnabled ? "mfa" : "pwd"));
            claims.Add(new Claim(JwtSecurityTokenValidator.Prefix_Role, RolePrefix.Doctor));

            IList<string> roles = await _authRepo.GetListRoleWithUser(account.Email);
            claims.AddRange(roles.Select(role => new Claim(JwtSecurityTokenValidator.Prefix_Role, role)));

            return claims;
        }

        private async Task<string> GenerateJWToken(Account user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(new Encrypt(_configuration["Authentication:JwtBearer:Symmetric:Key"]).DecryptString()));
            var secretKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(new Encrypt(_configuration["Authentication:JwtBearer:Symmetric:Secret"]).DecryptString()));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);
            var ep = new EncryptingCredentials(secretKey, SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

            var handler = new JwtSecurityTokenHandler();

            var jwtSecurityToken = handler.CreateJwtSecurityToken(
                new Encrypt(_configuration["Authentication:JwtBearer:Issuer"]).DecryptString(),
                new Encrypt(_configuration["Authentication:JwtBearer:Audience"]).DecryptString(),
                new ClaimsIdentity(await RenderClaimsAsync(user)),
                DateTime.Now,
                DateTime.Now.AddMinutes(Double.Parse(_configuration["Authentication:JwtBearer:DurationInMinutes"])),
                DateTime.Now,
                signingCredentials,
                ep);

            string tokenString = handler.WriteToken(jwtSecurityToken);
            return tokenString;
        }

        private void SetCookie(string name, string token)
        {
            var cookieOptions = new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Domain = _applicationSettings.DomainWildcard,
                Expires = DateTimes.Now().AddMinutes(60)
            };
            _httpContextAccessor.HttpContext.Response.Cookies.Append(name, token, cookieOptions);
        }

        private void SetCookie<T>(string name, T data)
        {
            var cookieOptions = new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Domain = _applicationSettings.DomainWildcard,
                Expires = DateTimes.Now().AddMinutes(60)
            };
            var value = Newtonsoft.Json.JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.None);
            _httpContextAccessor.HttpContext.Response.Cookies.Append(name, value, cookieOptions);
        }

        private async Task SignOutCookie()
        {
            var cookieOptions = new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Domain = _applicationSettings.DomainWildcard,
                Expires = DateTimes.Now().AddMinutes(60)
            };
            _httpContextAccessor.HttpContext.Response.Cookies.Delete("Access_Token", cookieOptions);
            _httpContextAccessor.HttpContext.Response.Cookies.Delete("Access_User", cookieOptions);

            await _signInManager.SignOutAsync();

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            Logs.debug("SignOut successfully");
        }
        #endregion Render Token






        //#region Send Email Comfirm Registry User
        ///// <summary>
        ///// Send email confirm for registry admin user 
        ///// </summary>
        ///// <param name="callbackUrl"></param>
        ///// <returns>Result status send email</returns>
        //// GET: api/doctor/AuthDoctor/SendEmailRegister
        //[HttpPost("SendEmailRegister")]
        //public async Task<IActionResult> SendEmailRegister(string callbackUrl)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(callbackUrl)) return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_CALLBACK_URL_NOT_FOUND"), this.ControllerContext));
        //
        //        // Validate userLogin
        //        User userLogin = await GetUserAuthenAsync();
        //        if (userLogin == null) return ResponseNotFound(new CustomBadRequest(localizer("BASE_USER_NOT_FOUND"), this.ControllerContext));
        //        if (userLogin.EmailConfirmed) return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_USER_CONFIRMED_EMAIL"), this.ControllerContext));
        //
        //        //====== Send email 1
        //        string code = await _userManager.GenerateEmailConfirmationTokenAsync(userLogin);
        //        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        //        string confirmLink = callbackUrl + "?userId=" + userLogin.Id + "&code=" + code;
        //
        //        string subject = localizer("EMAIL_SUBJECT_CONFIRM_EMAIL") ?? "Confirm your email address";
        //        string htmlTitle = localizer("EMAIL_SUBJECT_CONFIRM_EMAIL") ?? "Confirm your email address";
        //        string htmlContent = EmailStyles.ConfirmRegistryAccount(_localizer, subject, confirmLink);
        //        _ = _emailSender.SendEmail(userLogin.Email, htmlTitle, htmlContent);
        //
        //        LogUserEvent(_logger, TypeAction.Load, TypeStatus.Success, $"{RoutePrefix.ADMINUSER}/Auth/SendEmailRegister", $"User send email for new user registry", null, null);
        //        return ResponseData(new
        //        {
        //            linkConfirm = confirmLink,
        //            email = userLogin.Email
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUserEvent(_logger, TypeAction.Load, TypeStatus.Failure, $"{RoutePrefix.ADMINUSER}/Auth/SendEmailRegister", $"Error user send email for new user registry", ex, null);
        //        return LogExceptionEvent(_log4net, $"{RoutePrefix.ADMINUSER}/Auth/SendEmailRegister", ex);
        //    }
        //}

        ///// <summary>
        ///// Check token register for admin user
        ///// </summary>
        ///// <returns>Result check token register for admin user.</returns>
        //// POST: api/doctor/AuthDoctor/CheckTokenRegister
        //[HttpPost("CheckTokenRegister")]
        //[AllowAnonymous]
        //public async Task<IActionResult> CheckTokenRegister(string userId, string token)
        //{
        //    try
        //    {
        //        User user = await _userManager.FindByIdAsync(userId);
        //        if (user == null) return ResponseNotFound(new CustomBadRequest(localizer("BASE_USER_NOT_FOUND"), this.ControllerContext));
        //
        //        bool isEmailConfirmed = true;
        //        if (user.EmailConfirmed) isEmailConfirmed = false;
        //
        //        LogUserEvent(_logger, TypeAction.Load, TypeStatus.Success, $"{RoutePrefix.ADMINUSER}/Auth/CheckTokenRegister", $"User check token register send to email", null, null);
        //        return ResponseData(new { result = isEmailConfirmed });
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUserEvent(_logger, TypeAction.Load, TypeStatus.Failure, $"{RoutePrefix.ADMINUSER}/Auth/CheckTokenRegister", $"Error user check token register send to email", ex, null);
        //        return LogExceptionEvent(_log4net, $"{RoutePrefix.ADMINUSER}/Auth/CheckTokenRegister", ex);
        //    }
        //}

        ///// <summary>
        ///// Confirm email for registry for admin user
        ///// </summary>    
        ///// <returns>Result status comfirm email for admin user</returns>
        //// POST: api/doctor/AuthDoctor/ConfirmEmailRegister
        //[HttpPost("ConfirmEmailRegister")]
        //[AllowAnonymous]
        //public async Task<IActionResult> ConfirmEmailRegister(string userId, string token)
        //{
        //    try
        //    {
        //        User user = await _userManager.FindByIdAsync(userId);
        //        if (user == null) return ResponseNotFound(new CustomBadRequest(localizer("BASE_USER_NOT_FOUND"), this.ControllerContext));
        //        if (user.EmailConfirmed) return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_USER_HAS_CONFIRMED_EMAIL"), this.ControllerContext));
        //
        //        //====== Confirm email 1
        //        string code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        //        IdentityResult result = await _userManager.ConfirmEmailAsync(user, code);
        //        if (!result.Succeeded) return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_USER_CONFIRMED_INVALID_CODE"), this.ControllerContext));
        //
        //        LogUserEvent(_logger, TypeAction.Update, TypeStatus.Success, $"{RoutePrefix.ADMINUSER}/Auth/ConfirmEmailRegister", $"User confirm token subscribed send to email", null, null);
        //        return ResponseData(new { result = localizer("AUTH_SUCCESS_CONFIRM") });
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUserEvent(_logger, TypeAction.Update, TypeStatus.Failure, $"{RoutePrefix.ADMINUSER}/Auth/ConfirmEmailRegister", $"Error user confirm token subscribed send to email", ex, null);
        //        return LogExceptionEvent(_log4net, $"{RoutePrefix.ADMINUSER}/Auth/ConfirmEmailRegister", ex);
        //    }
        //}
        //#endregion Send Email Comfirm Registry User 






        //#region Send Email Comfirm Forgot Password
        ///// <summary>
        ///// Send email confirm forgot password for admin user
        ///// </summary>
        ///// <param name="callbackUrl"></param>
        ///// <param name="email"></param>
        ///// <returns>Result status send email for admin user</returns>
        //// GET: api/doctor/AuthDoctor/SendEmailForgotPassword
        //[HttpPost("SendEmailForgotPassword")]
        //[AllowAnonymous]
        //public async Task<IActionResult> SendEmailForgotPassword(string callbackUrl, string email)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(callbackUrl)) return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_CALLBACK_URL_NOT_FOUND"), this.ControllerContext));
        //
        //        if (!RegexUtilities.IsValidEmail(email)) return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_INVALID_EMAIL_FORMAT"), this.ControllerContext));
        //
        //        User user = await _userManager.FindByEmailAsync(email);
        //        if (user == null) return ResponseNotFound(new CustomBadRequest(localizer("AUTH_EMAIL_NOT_REGISTERED"), this.ControllerContext));
        //        if (user.Provider != SharedLib.Models.TypeProvider.Website) return ResponseNotFound(new CustomBadRequest($"{localizer("AUTH_LOGIN_PROVIDER_1")} {user.Provider}. {localizer("AUTH_LOGIN_PROVIDER_2")} {user.Provider}", this.ControllerContext));
        //
        //        //====== Send email 1
        //        //string code = await _userManager.GeneratePasswordResetTokenAsync(user);
        //        //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        //
        //        //====== Send email 2
        //        byte[] time = BitConverter.GetBytes(DateTimes.Now().ToBinary());
        //        byte[] key = Guid.NewGuid().ToByteArray();
        //        string code = Convert.ToBase64String(time.Concat(key).ToArray());
        //        string codeEncode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        //
        //        AspNetEmployeeToken token = new AspNetEmployeeToken()
        //        {
        //            Id = 0,
        //            ProviderTypeTokens = Persistence.Resource.TypeTokensProvider.FORGOT_PASSWORD,
        //            Email = email,
        //            Value = code,
        //            TotalRefresh = 0,
        //            ExpirationDate = DateTimes.Now().AddDays(7),
        //            CreationTime = DateTimes.Now(),
        //            LastModified = DateTimes.Now()
        //        };
        //        await _employeeRepository.AddTokens(token);
        //        string confirmLink = callbackUrl + "?userId=" + user.Id + "&code=" + codeEncode;
        //
        //        string subject = localizer("EMAIL_SUBJECT_PASS_RECOVERY") ?? "Confirm password recovery";
        //        string htmlTitle = localizer("EMAIL_SUBJECT_PASS_RECOVERY") ?? "Confirm password recovery";
        //        string htmlContent = EmailStyles.ConfirmPasswordRecovery(_localizer, htmlTitle, confirmLink);
        //        _ = _emailSender.SendEmail(user.Email, subject, htmlContent);
        //
        //        return ResponseData(new { linkConfirm = confirmLink, email = user.Email });
        //    }
        //    catch (Exception ex)
        //    {
        //        return LogExceptionEvent(_log4net, $"{RoutePrefix.ADMINUSER}/Auth/SendEmailForgotPassword", ex);
        //    }
        //}

        ///// <summary>
        ///// Check token forgot password for admin user
        ///// </summary>
        ///// <returns>The result check token forgot password for admin user.</returns>
        //// POST: api/doctor/AuthDoctor/CheckTokenForgotPassword
        //[HttpPost("CheckTokenForgotPassword")]
        //[AllowAnonymous]
        //public async Task<IActionResult> CheckCodeEncodeForgotPassword(string userId, string token)
        //{
        //    try
        //    {
        //        User user = await _userManager.FindByIdAsync(userId);
        //        if (user == null) return ResponseNotFound(new CustomBadRequest(localizer("BASE_USER_NOT_FOUND"), this.ControllerContext));
        //
        //        //====== Confirm email 2
        //        string code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        //        //byte[] data = Convert.FromBase64String(code);
        //        //DateTime time = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
        //        bool check = await _employeeRepository.ComfirmEmployeeTokens(code);
        //
        //        return ResponseData(new { result = check });
        //    }
        //    catch (Exception ex)
        //    {
        //        return LogExceptionEvent(_log4net, $"{RoutePrefix.ADMINUSER}/Auth/CheckTokenForgotPassword", ex);
        //    }
        //}

        ///// <summary>
        ///// Confirm email for forgot password for admin user
        ///// </summary>
        ///// <returns>Result status comfirm email for admin user</returns>
        //// GET: api/doctor/AuthDoctor/ConfirmEmailForgotPassword
        //[HttpPost("ConfirmEmailForgotPassword")]
        //[AllowAnonymous]
        //public async Task<IActionResult> ConfirmEmailForgotPassword(string userId, string token, string newPassword)
        //{
        //    try
        //    {
        //        if (newPassword == null || newPassword.Length < 8) return ResponseBadRequest(new CustomBadRequest("New password at least 8 characters", this.ControllerContext));
        //
        //        // Verify password by connect KMS service decrypt password
        //        //KeysOptions keysOptions = await _redisService.GetAsync<KeysOptions>(RedisKeyManager.KeysOptions);
        //        //newPassword = await _kMSService.Decrypt(_awsOptions.AccessKey, _awsOptions.SecretKey, keysOptions.KeyArnLogin, newPassword);
        //        //if (string.IsNullOrEmpty(newPassword))
        //        //    return ResponseBadRequest(new CustomBadRequest("New password is invalid format KMS", this.ControllerContext));
        //
        //        // Verify password by md5
        //        string hashedPassword = newPassword;
        //        string md5Password = new Encrypt(hashedPassword).GetHashMD5();
        //        if (string.IsNullOrEmpty(md5Password)) return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_PASS_FAILD_MD5"), this.ControllerContext));
        //
        //        var user = await _userManager.FindByIdAsync(userId);
        //        if (user == null) return ResponseNotFound(new CustomBadRequest(localizer("BASE_USER_NOT_FOUND"), this.ControllerContext));
        //        if (user.Provider != SharedLib.Models.TypeProvider.Website)
        //        {
        //            Logs.debug("Can not recovery password for user registry by Google");
        //            return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_NOT_RECOVERY_PASS"), this.ControllerContext));
        //        }
        //
        //        //====== Confirm email 1
        //        //string code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        //        //IdentityResult result = await _userManager.ResetPasswordAsync(user, code, newPassword);
        //        //if (!result.Succeeded)
        //        //    return ResponseNotFound(new CustomBadRequest("A code must be supplied for password reset", this.ControllerContext));
        //
        //        //====== Confirm email 2
        //        string code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        //        //byte[] data = Convert.FromBase64String(code);
        //        //DateTime time = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
        //        bool check = await _employeeRepository.ComfirmEmployeeTokens(code);
        //        if (!check) return ResponseBadRequest(new CustomBadRequest(localizer("AUTH_CONFIRM_TOKEN_NOT_FOUND"), this.ControllerContext));
        //
        //        user.Password = new Encrypt(newPassword).EncryptString();
        //        IdentityResult result = await _userManager.UpdateAsync(user);
        //        if (!result.Succeeded) return ResponseNotFound(new CustomBadRequest(localizer("AUTH_ERROR_RESET_PASS"), this.ControllerContext));
        //
        //        // Remove token
        //        await _employeeRepository.DeleteTokensWithCode(code);
        //        await _employeeRepository.DeleteTokensExpried();
        //
        //        return ResponseData(new { Detail = localizer("AUTH_SUCCESS_RESET_PASS") });
        //    }
        //    catch (Exception ex)
        //    {
        //        return LogExceptionEvent(_log4net, $"{RoutePrefix.ADMINUSER}/Auth/ConfirmEmailForgotPassword", ex);
        //    }
        //}
        //#endregion Send Email Comfirm Forgot Password

    }
}
