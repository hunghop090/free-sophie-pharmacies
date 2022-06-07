using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Localization;
using System.Net.Http;
using System.Collections.Generic;
using System.Net;
using log4net;
using App.Core.Units.Log4Net;
using App.SharedLib.Repository.Interface;
using App.Core.Settings;
using App.Core.Entities;
using App.Core.Units;
using Sophie.Units;
using App.Core.Clients;
using Sophie.Model;
using App.Core.Middleware.IPAddressTools;
using Microsoft.Extensions.DependencyInjection;

namespace Sophie.Controllers.API
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public abstract class BaseAPIController : ControllerBase
    {
        private readonly ILog _log4net = log4net.LogManager.GetLogger(typeof(BaseController));
        public ILogManager logManager;
        public IMapper mapper;
        public IConfiguration configuration;
        public IStringLocalizer Localizer;
        public IAuthRepository authRepo;
        public ApplicationSettings applicationSettings;// Inject Data

        protected ILogManager _logManager => logManager ?? (logManager = HttpContext.RequestServices.GetService<ILogManager>());
        protected IMapper _mapper => mapper ?? (mapper = HttpContext.RequestServices.GetService<IMapper>());
        protected IConfiguration _configuration => configuration ?? (configuration = HttpContext.RequestServices.GetService<IConfiguration>());
        protected IStringLocalizer _localizer => Localizer ?? (Localizer = HttpContext.RequestServices.GetService<IStringLocalizer>());
        protected IAuthRepository _authRepo => authRepo ?? (authRepo = HttpContext.RequestServices.GetService<IAuthRepository>());
        protected ApplicationSettings _applicationSettings => applicationSettings ?? (applicationSettings = HttpContext.RequestServices.GetService<IOptions<ApplicationSettings>>().Value);

        public readonly bool isEncrypt = true;

        [NonAction]
        [ApiExplorerSettings(IgnoreApi = true)]
        public string localizer(string name)
        {
            return _localizer[name, this.HttpContext].Value;
        }

        [NonAction]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ApplicationUser> GetUserAuthen()
        {
            try
            {
                ClaimsPrincipal currentUser = this.User;
                if (currentUser == null) return null;
                string userId = currentUser.FindFirst(JwtRegisteredClaimNames.NameId)?.Value;
                if (string.IsNullOrEmpty(userId)) return null;
                //var userName = currentUser.FindFirst(JwtRegisteredClaimNames.UniqueName).Value;
                //var userEmail = currentUser.FindFirst(JwtRegisteredClaimNames.Email).Value;
                //var amr = currentUser.FindFirst(JwtRegisteredClaimNames.Amr).Value;
                //var role = currentUser.FindFirst(MyJwtSecurityTokenValidator.Prefix_Role).Value;
                //var refreshToken = currentUser.FindFirst(MyJwtSecurityTokenValidator.Prefix_RefreshToken).Value;

                ApplicationUser user = await _authRepo.FindById(userId);
                return user;
            }
            catch
            {
                return null;
            }
        }

        [NonAction]
        [ApiExplorerSettings(IgnoreApi = true)]
        public string GetUserIdAuth()
        {
            try
            {
                ClaimsPrincipal? currentUser = this.User;
                if (currentUser == null) return "";
                string userId = currentUser.FindFirst(JwtRegisteredClaimNames.NameId)?.Value;
                if (string.IsNullOrEmpty(userId)) return "";
                return userId;
            }
            catch
            {
                return "";
            }
        }

        [NonAction]
        [ApiExplorerSettings(IgnoreApi = true)]
        public string GetUserEmailAuth()
        {
            try
            {
                ClaimsPrincipal currentUser = this.User;
                if (currentUser == null) return "";
                var userEmail = currentUser.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
                if (string.IsNullOrEmpty(userEmail)) return "";
                return userEmail;
            }
            catch
            {
                return "";
            }
        }



        #region Response Data
        [NonAction]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult ResponseNotFound(CustomBadRequest data)
        {
            try
            {
                bool isSwagger = (HttpContext.Items["isSwagger"] as bool?) ?? false;
                if (isSwagger || !isEncrypt || true)
                {
                    return NotFound(data);
                }
                else
                {
                    //SystemSettings _systemSettings = await _redisService.GetAsync<SystemSettings>(RedisKeyManager.SystemSettings);
                    SystemSettings _systemSettings = SystemManage.Get();
                    string stringData = Newtonsoft.Json.JsonConvert.SerializeObject(data, JsonSettings.SettingForNewtonsoft);
                    string encrypt = new Encrypt().EncryptStringToBytes_Aes(stringData, _systemSettings.SecretKey, _systemSettings.IVKey);
                    return NotFound(encrypt);
                }
            }
            catch (Exception ex)
            {
                Logs.debug("[ResponseNotFound] Exception: " + ex.StackTrace);
                _logManager.ErrorLog4net(_log4net, ex, $"[ResponseNotFound] Exception: " + ex.ToString());
                return NoContent();
            }
        }

        [NonAction]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult ResponseBadRequest(CustomBadRequest data)
        {
            try
            {
                bool isSwagger = (HttpContext.Items["isSwagger"] as bool?) ?? false;
                if (isSwagger || !isEncrypt || true)
                {
                    return BadRequest(data);
                }
                else
                {
                    //SystemSettings _systemSettings = await _redisService.GetAsync<SystemSettings>(RedisKeyManager.SystemSettings);
                    SystemSettings _systemSettings = SystemManage.Get();
                    string stringData = Newtonsoft.Json.JsonConvert.SerializeObject(data, JsonSettings.SettingForNewtonsoft);
                    string encrypt = new Encrypt().EncryptStringToBytes_Aes(stringData, _systemSettings.SecretKey, _systemSettings.IVKey);
                    return BadRequest(encrypt);
                }
            }
            catch (Exception ex)
            {
                Logs.debug("[ResponseBadRequest] Exception: " + ex.StackTrace);
                _logManager.ErrorLog4net(_log4net, ex, $"[ResponseBadRequest] Exception: " + ex.ToString());
                return NoContent();
            }
        }

        [NonAction]
        [ApiExplorerSettings(IgnoreApi = true)]
        public UnauthorizedObjectResult ResponseUnauthorized(object data)
        {
            try
            {
                bool isSwagger = (HttpContext.Items["isSwagger"] as bool?) ?? false;
                if (isSwagger || !isEncrypt || true)
                {
                    return Unauthorized(data);
                }
                else
                {
                    //SystemSettings _systemSettings = await _redisService.GetAsync<SystemSettings>(RedisKeyManager.SystemSettings);
                    SystemSettings _systemSettings = SystemManage.Get();
                    string stringData = Newtonsoft.Json.JsonConvert.SerializeObject(data, JsonSettings.SettingForNewtonsoft);
                    string encrypt = new Encrypt().EncryptStringToBytes_Aes(stringData, _systemSettings.SecretKey, _systemSettings.IVKey);
                    return Unauthorized(encrypt);
                }
            }
            catch (Exception ex)
            {
                Logs.debug("[ResponseUnauthorized] Exception: " + ex.StackTrace);
                _logManager.ErrorLog4net(_log4net, ex, $"[ResponseUnauthorized] Exception: " + ex.ToString());
                return Unauthorized(new { });
            }
        }

        [NonAction]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult ResponseDataFixKey(object data)
        {
            try
            {
                bool isSwagger = (HttpContext.Items["isSwagger"] as bool?) ?? false;
                if (isSwagger || !isEncrypt)
                {
                    return Ok(data);
                }
                else
                {
                    //SystemSettings _systemSettings = await _redisService.GetAsync<SystemSettings>(RedisKeyManager.SystemSettings);
                    SystemSettings _systemSettings = SystemManage.Get();
                    string stringData = Newtonsoft.Json.JsonConvert.SerializeObject(data, JsonSettings.SettingForNewtonsoft);
                    string encrypt = new Encrypt().EncryptStringToBytes_Aes(stringData, null, null);
                    return Ok(encrypt);
                }
            }
            catch (Exception ex)
            {
                Logs.debug("[ResponseData] Exception: " + ex.StackTrace);
                _logManager.ErrorLog4net(_log4net, ex, $"[ResponseData] Exception: " + ex.ToString());
                return NoContent();
            }
        }

        [NonAction]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult ResponseData(object data)
        {
            try
            {
                bool isSwagger = (HttpContext.Items["isSwagger"] as bool?) ?? false;
                if (isSwagger || !isEncrypt)
                {
                    return Ok(data);
                }
                else
                {
                    //SystemSettings _systemSettings = await _redisService.GetAsync<SystemSettings>(RedisKeyManager.SystemSettings);
                    SystemSettings _systemSettings = SystemManage.Get();
                    string stringData = Newtonsoft.Json.JsonConvert.SerializeObject(data, JsonSettings.SettingForNewtonsoft);
                    string encrypt = new Encrypt().EncryptStringToBytes_Aes(stringData, _systemSettings.SecretKey, _systemSettings.IVKey);
                    return Ok(encrypt);
                }
            }
            catch (Exception ex)
            {
                Logs.debug("[ResponseData] Exception: " + ex.StackTrace);
                _logManager.ErrorLog4net(_log4net, ex, $"[ResponseData] Exception: " + ex.ToString());
                return NoContent();
            }
        }

        [NonAction]
        [ApiExplorerSettings(IgnoreApi = true)]
        public CreatedResult ResponseCreated(object data)
        {
            try
            {
                bool isSwagger = (HttpContext.Items["isSwagger"] as bool?) ?? false;
                if (isSwagger || !isEncrypt)
                {
                    return Created("2FA", data);
                }
                else
                {
                    //SystemSettings _systemSettings = await _redisService.GetAsync<SystemSettings>(RedisKeyManager.SystemSettings);
                    SystemSettings _systemSettings = SystemManage.Get();
                    string stringData = Newtonsoft.Json.JsonConvert.SerializeObject(data, JsonSettings.SettingForNewtonsoft);
                    string encrypt = new Encrypt().EncryptStringToBytes_Aes(stringData, _systemSettings.SecretKey, _systemSettings.IVKey);
                    return Created("2FA", encrypt);
                }
            }
            catch (Exception ex)
            {
                Logs.debug("[ResponseCreated] Exception: " + ex.StackTrace);
                _logManager.ErrorLog4net(_log4net, ex, $"[ResponseCreated] Exception: " + ex.ToString());
                return Created("2FA", new { });
            }
        }

        #endregion Response Data ./



        #region LogEventForUser
        [NonAction]
        [ApiExplorerSettings(IgnoreApi = true)]
        public bool LogUserEvent(ILogger logger, TypeAction action, TypeStatus status, string actionAPI, string message, Exception? exception = null, object? dataObject = null)
        {
            try
            {
                #if DEBUG
                Logs.debug($"[LogUserEvent] '{actionAPI}'");
                #endif

                //SystemSettings _systemSettings = await _redisService.GetAsync<SystemSettings>(RedisKeyManager.SystemSettings);
                SystemSettings _systemSettings = SystemManage.Get();
                if (_systemSettings != null && _systemSettings.IsLogs)
                {
                    // Identifying the requested user
                    // Date and time of the event (Ngày giờ của sự kiện)
                    // Relevant user or process (Quy trình của người dùng có liên quan)
                    // Event description (Mô tả sự kiện)
                    // Success or failure of the event (Sự kiện thành công hay thất bại)
                    // Event source (Nguồn sự kiện)
                    // ICT equipment location and identification (Vị trí và thiết bị yêu cầu)
                    // Data identifiers (product ID, Tax File Number (TFN)) (Định dạng dữ liệu Id sản phẩm và mã số thuế)
                    string userId = GetUserIdAuth();
                    string userEmail = GetUserEmailAuth();
                    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userEmail))
                    {
                        Logs.debug("[LogUserEvent] Warning: user not login, can not save log event for user");
                        return false;
                    };
                    string dateTime = DateTimes.Now().ToString(DateTimes.format3());
                    string description = "Something went wrong! We will fix it in the shortest time";
                    if (!string.IsNullOrEmpty(message)) description = message;
                    if (exception != null) description = $"{description} : {exception.ToString()}";
                    string deviceName = (HttpContext.Items["deviceName"] as string) ?? "Chrome";
                    string deviceId = (HttpContext.Items["deviceId"] as string) ?? "Chrome.xx.xx.xxxx.xx";
                    string deviceLocation = (HttpContext.Items["deviceLocation"] as string) ?? "VietNam";
                    string deviceLanguage = (HttpContext.Items["deviceLanguage"] as string) ?? "VI";
                    string dataJson = "{\"objectData\": \"null\"}";
                    if (dataObject != null) dataJson = Newtonsoft.Json.JsonConvert.SerializeObject(dataObject, JsonSettings.SettingForNewtonsoftPretty);
                    LogUserEvent logEvent = new LogUserEvent()
                    {
                        UserId = userId,
                        UserEmail = userEmail,
                        Datetime = dateTime,
                        Action = action,
                        Description = description,
                        Status = status,
                        DeviceName = deviceName,
                        DeviceId = deviceId,
                        DeviceLocation = deviceLocation,
                        DeviceLanguage = deviceLanguage,
                        DataJson = dataJson
                    };
                    Logs.debug($"[LogUserEvent] '{actionAPI}' :\n{Newtonsoft.Json.JsonConvert.SerializeObject(logEvent, JsonSettings.SettingForNewtonsoftPretty)}");
                    if (status == TypeStatus.Success)
                    {
                        _logManager.InforLogger(logger, new EventId(77, $"{userId}"), exception, $"\n{Newtonsoft.Json.JsonConvert.SerializeObject(logEvent, JsonSettings.SettingForNewtonsoftPretty)}");
                        //_logManager.InforLogger(logger, new EventId(77, $"{userId}"), exception, $"@Event\n{Newtonsoft.Json.JsonConvert.SerializeObject(logEvent, JsonSettings.SettingForNewtonsoftPretty)}");
                    }
                    else if (status == TypeStatus.Failure)
                    {
                        //_logManager.ErrorLogger(logger, new EventId(55, $"{userId}"), exception, $"\n{Newtonsoft.Json.JsonConvert.SerializeObject(logEvent, JsonSettings.SettingForNewtonsoftPretty)}");
                        _logManager.ErrorLogger(logger, new EventId(55, $"{userId}"), exception, $"@Event\n{Newtonsoft.Json.JsonConvert.SerializeObject(logEvent, JsonSettings.SettingForNewtonsoftPretty)}");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logs.debug($"[LogUserEvent] Exception: {ex.StackTrace}");
                _logManager.ErrorLog4net(_log4net, $"[LogUserEvent] Exception: {ex.StackTrace}");
                return false;
            }
        }

        [NonAction]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult LogExceptionEvent(ILog log4net, string actionAPI, Exception exception, object objRequest = null)
        {
            try
            {
                if (objRequest != null)
                {
                    Logs.debug($"[LogExceptionEvent] Exception: {exception.StackTrace}\nObjRequest: {Newtonsoft.Json.JsonConvert.SerializeObject(objRequest, JsonSettings.SettingForNewtonsoftPretty)}");
                    _logManager.ErrorLog4net(log4net, exception, $"Exception from api '{actionAPI}' : \n{exception} \nObjRequest: {Newtonsoft.Json.JsonConvert.SerializeObject(objRequest, JsonSettings.SettingForNewtonsoftPretty)}");
                }
                else
                {
                    Logs.debug($"[LogExceptionEvent] Exception: {exception.StackTrace}");
                    _logManager.ErrorLog4net(log4net, exception, $"Exception from api '{actionAPI}' : \n{exception}");
                }
                Task task = new Task(async () =>
                {
                    await this.SendMessageToSlack(LogLevel.Error,
                        $"Lỗi API '{actionAPI}'",
                        Newtonsoft.Json.JsonConvert.SerializeObject(new { Request = objRequest, Exception = exception }, JsonSettings.SettingForNewtonsoftPretty),
                        true);
                });
                task.Start();
                return ResponseBadRequest(new CustomBadRequest(localizer("BASE_EXCEPTION"), this.ControllerContext));
            }
            catch (Exception ex)
            {
                Logs.debug($"[LogExceptionEvent] Exception: {ex.StackTrace}");
                _logManager.ErrorLog4net(_log4net, $"[LogExceptionEvent] Exception: {ex.StackTrace}");
                return ResponseBadRequest(new CustomBadRequest(localizer("BASE_EXCEPTION"), this.ControllerContext));
            }
        }

        [NonAction]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<string> SendMessageToSlack(LogLevel logLevel, string title, string message, bool isAlwaysSend = false)
        {
            try
            {
                SystemSettings _systemSettings = SystemManage.Get();
                if ((_systemSettings != null && _systemSettings.IsLogs) || isAlwaysSend)
                {
                    string userId = !string.IsNullOrEmpty(GetUserIdAuth()) ? GetUserIdAuth() : "_";
                    string userEmail = !string.IsNullOrEmpty(GetUserEmailAuth()) ? GetUserEmailAuth() : "_";
                    string deviceName = FindValueByKey(HttpContext.Items, "deviceName") ?? "Chrome";
                    string deviceId = FindValueByKey(HttpContext.Items, "deviceId") ?? "Chrome.xx.xx.xxxx.xx";
                    string deviceLocation = FindValueByKey(HttpContext.Items, "deviceLocation") ?? "VietNam";
                    string deviceLanguage = FindValueByKey(HttpContext.Items, "deviceLanguage") ?? "VI";
                    IPAddress ipAddress = IPAddressFinder.Find(this.HttpContext, _applicationSettings, false);
                    string ipClient = (ipAddress != null) ? ipAddress.ToString() : "xxx.xxx.xxx.xxx";

                    string AppID = $"A02MMG6NNEP";
                    string ClientID = $"418702088676.2735550770499";
                    string ClientSecret = $"3dbad5734cca7208890a07e5ba3630e7";
                    string SigningSecret = $"970a6d9d805f2e847b950c0c5a27c947";
                    string VerificationToken = $"SxUzyYpiLoqhaOd0kSBr68WF";
                    string slackChannel = $"#sophie";
                    // Guide: https://api.slack.com/apps/A02MMG6NNEP/incoming-webhooks
                    string webhookUrl = $"https://hooks.slack.com/services/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";
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 11_2_0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.182 Safari/537.36");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                    client.Timeout = TimeSpan.FromSeconds(30);
                    Logs.debug($"UrlRequest: {webhookUrl}");
                    //https://app.slack.com/block-kit-builder/TCALN2LKW#%7B%22blocks%22:%5B%7B%22type%22:%22header%22,%22text%22:%7B%22type%22:%22plain_text%22,%22text%22:%22New%20request%22,%22emoji%22:true%7D%7D,%7B%22type%22:%22section%22,%22fields%22:%5B%7B%22type%22:%22mrkdwn%22,%22text%22:%22*Type:*%5CnPaid%20Time%20Off%22%7D,%7B%22type%22:%22mrkdwn%22,%22text%22:%22*Created%20by:*%5Cn%3Cexample.com%7CFred%20Enriquez%3E%22%7D%5D%7D,%7B%22type%22:%22section%22,%22fields%22:%5B%7B%22type%22:%22mrkdwn%22,%22text%22:%22*When:*%5CnAug%2010%20-%20Aug%2013%22%7D%5D%7D,%7B%22type%22:%22actions%22,%22elements%22:%5B%7B%22type%22:%22button%22,%22text%22:%7B%22type%22:%22plain_text%22,%22emoji%22:true,%22text%22:%22Approve%22%7D,%22style%22:%22primary%22,%22value%22:%22click_me_123%22%7D,%7B%22type%22:%22button%22,%22text%22:%7B%22type%22:%22plain_text%22,%22emoji%22:true,%22text%22:%22Reject%22%7D,%22style%22:%22danger%22,%22value%22:%22click_me_123%22%7D%5D%7D%5D%7D
                    object bodyObject = new
                    {
                        channel = slackChannel,
                        text = "Phản hồi tự động từ hệ thống Sophie",
                        blocks = new List<object>() {
                            new {
                                type = "header",
                                text = new {
                                    type = "plain_text",
                                    text = "Phản hồi tự động từ hệ thống Sophie",
                                    emoji = true
                                }
                            },
                            new {
                                type = "section",
                                fields = new List<object>() {
                                    new { type = "mrkdwn", text = $"*Mức cảnh báo:*\n" + ((logLevel == LogLevel.Error)?"Cấp 3 - Ngoại lệ":(logLevel == LogLevel.Warning)?"Cấp 2 - Cảnh báo":"Cấp 1 - Thông báo" )},
                                    new { type = "mrkdwn", text = $"*Vào lúc:*\n{DateTimes.Now().ToVietNam()}" }
                                },
                                accessory = new {
                                    type = "image",
                                    image_url = $"https://calculator.com.vn/img/" + ((logLevel == LogLevel.Error)?"bug_red.png":(logLevel == LogLevel.Warning)?"bug_yellow.png":"bug_green.png"),
                                    alt_text = "thumbnail"
                                }
                            },
                            new {
                                type = "section",
                                fields = new List<object>() {
                                    new { type = "mrkdwn", text = $"*Tài khoản:*\n{userEmail}" },
                                    new { type = "mrkdwn", text = $"*User ID:*\n{userId}" }
                                }
                            },
                            new {
                                type = "section",
                                fields = new List<object>() {
                                    new { type = "mrkdwn", text = $"*Trình duyệt:*\n{deviceName}" },
                                    new { type = "mrkdwn", text = $"*Public IP:*\n{ipClient}" }
                                }
                            },
                            new {
                                type = "section",
                                fields = new List<object>() {
                                    new { type = "mrkdwn", text = $"*Định danh:*\n{deviceId}" },
                                }
                            },
                            new {
                                type = "section",
                                fields = new List<object>() {
                                    new { type = "mrkdwn", text = $"*Vị trí:*\n{deviceLocation}" },
                                    new { type = "mrkdwn", text = $"*Ngôn ngữ:*\n{deviceLanguage}" }
                                }
                            },
                            new {
                                type = "section",
                                text = new { type = "mrkdwn", text = $"Bạn có thể đăng nhập và xem chi tiết xem tại:" },
                                accessory = new {
                                    type = "button",
                                    text = new { type = "plain_text", text = "Chi tiết", emoji = true },
                                    style = "primary",//["primary", "danger"]
                                    value = "click_view_detail",
                                    url = "http://systemsophie.com",
                                    action_id = "button-action"
                                },
                            }
                        },
                        attachments = new List<object>() {
                            new {
                                fallback = "",
                                pretext = "",
                                title = title,
                                title_link = "http://systemsophie.com",
                                text = message,
                                color = (logLevel == LogLevel.Error)?"danger":(logLevel == LogLevel.Warning)?"warning":"good",//["danger", "warning", "good"]
                                fields = new List<object>() {},
                                ts = Math.Floor((decimal)DateTimes.Now().Subtract(new DateTime(1970, 1, 1)).Ticks / TimeSpan.TicksPerSecond)
                            },
                        }
                    };
                    string bodyString = Newtonsoft.Json.JsonConvert.SerializeObject(bodyObject, JsonSettings.SettingForNewtonsoft);
                    Logs.debug($"BodyString: {bodyString}");
                    HttpResponseMessage response = await client.PostAsync(webhookUrl, new StringContent(bodyString, System.Text.Encoding.UTF8, "application/json"));
                    Logs.debug($"StatusCode: {response.StatusCode.GetHashCode()}");
                    string pageContents = await response.Content.ReadAsStringAsync();
                    Logs.debug($"Content: {pageContents}");
                    return pageContents;
                }
                return "System disable logger";
            }
            catch (Exception ex)
            {
                Logs.debug($"[SendMessageToSlack] Exception: {ex.StackTrace}");
                _logManager.ErrorLog4net(_log4net, $"[SendMessageToSlack] Exception: {ex.StackTrace}");
                return ex.StackTrace;
            }
        }

        [NonAction]
        [ApiExplorerSettings(IgnoreApi = true)]
        private string? FindValueByKey(IDictionary<object, object?> data_Array, String key)
        {
            try
            {
                object value;
                if (data_Array.TryGetValue(key, out value))
                {
                    return value.ToString();
                }
            }
            catch { }
            return null;
        }
        #endregion LogEventForUser ./


        #region Validation
        [NonAction]
        [ApiExplorerSettings(IgnoreApi = true)]
        public CustomBadRequest? IsEmptyModel<T>(T model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (!ModelState.IsValid) return new CustomBadRequest(localizer("BASE_INPUT_NOT_VALUE"), this.ControllerContext);
            return null;
        }
        #endregion Validation ./


        #region Redis Cache
        #endregion Redis Cache ./
    }
}
