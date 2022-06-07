using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using System.Net;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.IO;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Responses;
using Microsoft.Extensions.Localization;
using log4net;
using App.Core.Constants;
using App.Core.Policy;
using App.Core.Entities;
using App.Core.Settings;
using Sophie.Units;
using App.Core.Clients;
using App.Core.Middleware.IPAddressTools;
using Sophie.Model;
using App.Core.Units;
using App.Core.Units.Log4Net;
using App.SharedLib.Repository.Interface;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Sophie.Controllers.API
{
    [ApiController]
    [Produces("application/json")]
    [Route(RoutePrefix.API)]//api/[controller]
    [ApiExplorerSettings(GroupName = "v1")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = PolicyPrefix.RequiresUser)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [MultiPolicysAuthorizeAttribute(Policys = RolePrefix.User + "," + RolePrefix.Account + "," + RolePrefix.Guest, IsAnd = false)]
    public class SysController : BaseController
    {
        private readonly ILog _log4net = log4net.LogManager.GetLogger(typeof(SysController));
        private readonly ILogger<SysController> _logger;
        
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITimezoneRepository _timezoneRepository;

        public SysController(ILogManager logManager, IMapper mapper, IConfiguration configuration, IStringLocalizer localizer, IAuthRepository authRepo, IOptions<ApplicationSettings> applicationSettingAccessor,
            ILogger<SysController> logger, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager, ITimezoneRepository timezoneRepository)
            : base(logManager, mapper, configuration, localizer, authRepo, applicationSettingAccessor)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _timezoneRepository = timezoneRepository;
        }

        public class TestTimezoneModel
        {
            [DefaultValue("2021-11-20T06:00:00")]
            public string Timezone1 { get; set; }

            [DefaultValue("2021-11-20T12:00:00")]
            public string Timezone2 { get; set; }

            [DefaultValue("2021-11-20T21:00:00")]
            public DateTime Timezone3 { get; set; }
        }

        /// <summary>
        /// Test timezone
        /// </summary>
        /// <returns>The result test timezone</returns>
        // POST: api/Sys/TestTimezone
        [HttpPost("TestTimezone")]
        [AllowAnonymous]
        public IActionResult TestTimezone(TestTimezoneModel model)
        {
            try
            {
                TimeSpan offsetLocal = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
                TimeSpan offsetUtc   = TimeZoneInfo.Utc.GetUtcOffset(DateTime.UtcNow);
                string now = DateTime.Now.ToString();
                string utcNow = DateTime.UtcNow.ToString();
                string vietNam = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")).ToString();


                DateTime _timezone1 = DateTimes.Parse(model.Timezone1);
                DateTime _timezone2 = DateTimes.ParseExact(model.Timezone2);
                DateTime _timezone3 = model.Timezone3;

                ApplicationTimezone applicationTimezone = _timezoneRepository.UpdateTimezone(_timezone1, _timezone2, _timezone3);

                return Ok(new {
                    Message = "Test timezone successfully",
                    Timezone1Parse = _timezone1.ToString(DateTimes.format2()),
                    Timezone2ParseExact = _timezone2.ToString(DateTimes.format2()),
                    Timezone3DateTime = _timezone3.ToString(DateTimes.format2()),
                    ApplicationTimezone = applicationTimezone,

                    OffsetLocal = offsetLocal,
                    OffsetUtc = offsetUtc,
                    Now = now,
                    UtcNow = utcNow,
                    VietNam = vietNam,
                });
            }
            catch (Exception ex)
            {
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/Auth/TestTimezone", ex);
            }
        }

        /// <summary>
        /// Test reset random key
        /// </summary>
        /// <returns>The result test reset random key</returns>
        // GET: api/Sys/RamdomKey
        [HttpGet("RandomKey")]
        [AllowAnonymous]
        public IActionResult RandomKey()
        {
            try
            {
                var clientCode = Guid.NewGuid().ToString("n").Substring(0, 8);
                var secretKey = new Encrypt().RandomKey256Bit();
                var ivKey = new Encrypt().RandomIVBit();
                Logs.debug($"RandomKey: secretKey={secretKey}, ivKey={ivKey}");

                SystemSettings systemSettings = SystemManage.Get();
                systemSettings.ClientCode = clientCode;
                systemSettings.SecretKey = secretKey;
                systemSettings.IVKey = ivKey;

                SystemManage.Update(systemSettings);

                return Ok(new { Message = "Random key successfully" });
            }
            catch (Exception ex)
            {
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/Auth/RandomKey", ex);
            }
        }

        /// <summary>
        /// Get operating system
        /// </summary>
        /// <returns>The result operating of system</returns>
        // POST: api/Sys/OperatingSystem
        [HttpPost("OperatingSystem")]
        [AllowAnonymous]
        public IActionResult OperatingSystem()
        {
            try
            {
                // Check system enable
                SystemSettings _systemSettings = SystemManage.Get();

                return Ok(new
                {
                    IsEnable = (_systemSettings != null) ? _systemSettings.IsEnable : false,
                    Languages = localizer("BASE_LANGUAGE"),
                    RandomId = StringEx.RandomId(),
                });
            }
            catch (Exception ex)
            {
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/Auth/OperatingSystem", ex);
            }
        }

        /// <summary>
        /// Get ipAddress of client
        /// </summary>
        /// <returns>The result ip address for client</returns>
        // GET: api/Sys/IpClient
        [HttpGet("IpClient")]
        [AllowAnonymous]
        public IActionResult IpClient()
        {
            try
            {
                Dictionary<string, Object> listIpClient = IPAddressFinder.ListIpClient(this.HttpContext);

                IPAddress ipAddress = IPAddressFinder.Find(this.HttpContext, _applicationSettings, false);
                Logs.debug($"RemoteIpAddress: {ipAddress.ToString()}");
                List<string> listIngore = new List<string>() { "::1", "127.0.0.1" };
                listIngore.AddRange(ClientManage.GetAllIpClient());

                object resultCity = new { };
                if (Array.Exists(listIngore.ToArray(), item => ipAddress.ToString().ToLower().IndexOf(item.ToLower()) > -1) ||
                    ipAddress.ToString().ToLower().IndexOf("192.168.") > -1)
                {
                    resultCity = new
                    {
                        City = "Quy Nhon",
                        Location = new {
                            Latitude = 13.782979733739062,
                            Longitude = 109.21966424309834,
                            AccuracyRadius = 200,
                            TimeZone = "SE Asia Standard Time"
                        },
                        Continent = "Southeast Asia",
                        Country = "Vietnam",
                        RegisteredCountry = "Vietnam"
                    };
                }
                else
                {
                    string rootPathDB = $"{Directory.GetCurrentDirectory()}/GeoLite2";
                    DatabaseReader readerCity = new DatabaseReader(rootPathDB + "/GeoLite2-City.mmdb");
                    CityResponse city = readerCity.City(ipAddress);
                    resultCity = new
                    {
                        City = city.City.Name ?? "Quy Nhon",
                        Location = new
                        {
                            Latitude = city.Location.Latitude ?? 13.782979733739062,
                            Longitude = city.Location.Longitude ?? 109.21966424309834,
                            AccuracyRadius = city.Location.AccuracyRadius ?? 200,
                            TimeZone = city.Location.TimeZone ?? "SE Asia Standard Time"
                        },
                        Continent = city.Continent.Name ?? "Southeast Asia",
                        Country = city.Country.Name ?? "Vietnam",
                        RegisteredCountry = city.RegisteredCountry.Name ?? "Vietnam"
                    };
                }

                return Ok(new { ListIpClient = listIpClient, City = resultCity });
            }
            catch (Exception ex)
            {
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/Auth/IpClient", ex);
            }
        }

        /// <summary>
        /// Handshake for client, clientId="2020202020212021", clientSecret="key_secret_mobile", xForwardedFor="myIpClient", clientId,clientSecret encode md5
        /// </summary>
        /// <returns>The result handshake for client</returns>
        // PATCH: api/Sys/Handshake
        [HttpPatch("Handshake")]
        [AllowAnonymous]
        public async Task<IActionResult> Handshake(HandshakeModel model)
        {
            try
            {
                await Task.Delay(1000);

                // Validate ModelState
                CustomBadRequest? resultValidate = IsEmptyModel(model);
                if (resultValidate != null)
                {
                    _logManager.WarnLog4net(_log4net, $"[Handshake Error] Data is null");
                    return new StatusCodeResult((int)HttpStatusCode.Forbidden);//403-Forbidden, 401-Unauthorized,
                }

                // Get IpClient from xForwardedFor --> 3.1.154.100 || 127.0.0.1
                var xForwardedFor = model.xForwardedFor.Replace("http://", "").Replace("https://", "");
                List<DomainOptions> listDomain = _applicationSettings.ListDomain;
                foreach (DomainOptions domain in listDomain)
                {
                    if (xForwardedFor.IndexOf(domain.Name) == 0)
                    {
                        xForwardedFor = xForwardedFor.Replace(domain.Name, domain.Ip);
                        break;
                    }
                }

                //=== Check xForwardedFor in WhitelListIp
                bool isInWhiteListIp = true;
                bool isValid = true;
                if (isValid)
                {
                    //[WEB]
                    if (model.operatingSystem == TypeOperatingSystem.WEB)
                    {
                        //Notes: Check client in WhiteListIp
                        List<string> whiteListIp = _applicationSettings.WhitelListIp;
                        List<string> listClientCheck = new List<string>();
                        listClientCheck.AddRange(whiteListIp);
                        listClientCheck.AddRange(ClientManage.GetAllIpClient());
                        //Logs.debug($"WhiteListIp = {Newtonsoft.Json.JsonConvert.SerializeObject(listClientCheck, JsonSettings.SettingForNewtonsoft)}");
                        isInWhiteListIp = listClientCheck.Where(ip => ip.Equals(xForwardedFor, StringComparison.CurrentCultureIgnoreCase)).Any();
                    }
                    //[ANDROID, IOS]
                    else
                    {
                        //Notes: Check client in Viet Nam
                        #if DEBUG
                        IpInfo ipInfo = IPAddressFinder.GetInfoFromIp(xForwardedFor);
                        if (ipInfo == null)
                        {
                            _logManager.WarnLog4net(_log4net, $"[Handshake Mobile Error] Client {xForwardedFor} get ip info is null");
                            return new StatusCodeResult((int)HttpStatusCode.Forbidden);//403-Forbidden, 401-Unauthorized,
                        }
                        if (!ipInfo.Country?.Equals("VN", StringComparison.OrdinalIgnoreCase) ?? false)
                        {
                            _logManager.WarnLog4net(_log4net, $"[Handshake Mobile Error] Client {xForwardedFor} not support. Only support in Viet Nam \n{Newtonsoft.Json.JsonConvert.SerializeObject(ipInfo, JsonSettings.SettingForNewtonsoftPretty)}");
                            return new StatusCodeResult((int)HttpStatusCode.Forbidden);//403-Forbidden, 401-Unauthorized,
                        }
                        #endif
                    }
                }
                if (!isInWhiteListIp)
                {
                    _logManager.WarnLog4net(_log4net, $"[Handshake Error] Client {xForwardedFor} not accept in WhiteListIp. \nxForwardedFor: {xForwardedFor}, isInWhiteListIp: {isInWhiteListIp}. \n{Newtonsoft.Json.JsonConvert.SerializeObject(model, JsonSettings.SettingForNewtonsoftPretty)}");
                    return new StatusCodeResult((int)HttpStatusCode.Forbidden);//403-Forbidden, 401-Unauthorized,
                }

                //=== Check xForwardedFor & ipClient [OriginIP || RealIP]
                string ipClient = "";
                IPAddress remoteIpAddress = IPAddressFinder.Find(this.HttpContext, _applicationSettings, true);
                if (remoteIpAddress != null)
                {
                    // If we got an IPV6 address, then we need to ask the network for the IPV4 address 
                    // This usually only happens when the browser is on the same machine as the server.
                    if (remoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        remoteIpAddress = System.Net.Dns.GetHostEntry(remoteIpAddress).AddressList.First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                    }
                    ipClient = remoteIpAddress.ToString();
                }
                Logs.debug($"Find IpClient = {ipClient}");
                if (!ipClient.Equals("127.0.0.1") && !xForwardedFor.Equals(ipClient))
                {
                    _logManager.WarnLog4net(_log4net, $"[Handshake Error] Client {xForwardedFor} with xForwardedFor: {xForwardedFor} not same find ipClient: {ipClient}");
                    return new StatusCodeResult((int)HttpStatusCode.Forbidden);//403-Forbidden, 401-Unauthorized,
                }

                //=== Check clientId & clientSecret & clientCode
                //SystemSettings _systemSettings = await _redisService.GetAsync<SystemSettings>(RedisKeyManager.SystemSettings);
                SystemSettings _systemSettings = SystemManage.Get();
                var clientId = new Encrypt(_applicationSettings.ClientId).DecryptString();//2020202020212021
                var clientSecret = new Encrypt(_applicationSettings.ClientSecret).DecryptString();//key_secret_mobile
                //var clientCode = new Encrypt(_applicationOptions.ClientCode).DecryptString();//12345678
                // Get key _clientCode by redis cache if != null
                var clientCode = _systemSettings.ClientCode;
                //"2020202020212021" = "c96b1299d7af6611405443a4e5c3e1d8"
                //"key_secret_mobile" = "0c1c0cf67a95b973f2ff18b56e925517"
                PasswordVerificationResult isVerifyClientId = new Encrypt(model.clientId).VerifyTextHashMD5(clientId);
                PasswordVerificationResult isVerifyClientSecret = new Encrypt(model.clientSecret).VerifyTextHashMD5(clientSecret);

                if (isVerifyClientId != PasswordVerificationResult.Success || isVerifyClientSecret != PasswordVerificationResult.Success)
                {
                    _logManager.WarnLog4net(_log4net, $"[Handshake Error] Client {xForwardedFor} with ClientId & ClientSecret is invalid. \n{Newtonsoft.Json.JsonConvert.SerializeObject(model, JsonSettings.SettingForNewtonsoftPretty)}");
                    return new StatusCodeResult((int)HttpStatusCode.Forbidden);//403-Forbidden, 401-Unauthorized,
                }
                _logManager.InforLog4net(_log4net, $"[Handshake Success] Client {xForwardedFor} \n{Newtonsoft.Json.JsonConvert.SerializeObject(model, JsonSettings.SettingForNewtonsoftPretty)}");

                //[WEB]
                if (model.operatingSystem == TypeOperatingSystem.WEB)
                {
                    DateTime date = DateTimes.Now();//12345678@@@2020-07-01T10:51:43.563Z
                    DateTime dateVN = date.ToVietNamEx();
                    LogsEx.debug($"[SysController] date: {date} - Kind: {date.Kind}, dateVN: {dateVN} - Kind: {dateVN.Kind}");
                    string webHandshake = clientCode + "@@@" + dateVN.ToString();
                    webHandshake = webHandshake + "###" + model.deviceName;
                    webHandshake = webHandshake + "&&&" + model.deviceId;
                    webHandshake = webHandshake + "***" + model.deviceLocation ?? "VietNam";
                    webHandshake = webHandshake + "$$$" + model.deviceLanguage;

                    var responseData = new
                    {
                        xHandshake = new Encrypt(webHandshake).EncryptString(),
                        xForwardedFor = xForwardedFor,
                        xSecretKey = _systemSettings.SecretKey ?? "encrypt123456789encrypt123456789",
                        xIVKey = _systemSettings.IVKey ?? "123456789encrypt"
                    };

                    return Ok(responseData);
                }
                //[ANDROID, IOS]
                else
                {
                    DateTime date = DateTimes.Now();//12345678@@@2020-07-01T10:51:43.563Z
                    DateTime dateVN = date.ToVietNamEx();
                    LogsEx.debug($"[SysController] date: {date} - Kind: {date.Kind}, dateVN: {dateVN} - Kind: {dateVN.Kind}");
                    string webHandshake = clientCode + "@@@" + dateVN.ToString();//deviceId@@@2020-07-01T10:51:43.563Z
                    webHandshake = webHandshake + "###" + model.deviceName;
                    webHandshake = webHandshake + "&&&" + model.deviceId;
                    webHandshake = webHandshake + "***" + model.deviceLocation ?? "VietNam";
                    webHandshake = webHandshake + "$$$" + model.deviceLanguage;

                    string mobileHandshake = clientCode + "@@@" + dateVN.ToString();//deviceId@@@2020-07-01T10:51:43.563Z
                    mobileHandshake = mobileHandshake + "###" + model.deviceName;
                    mobileHandshake = mobileHandshake + "&&&" + model.deviceId;
                    mobileHandshake = mobileHandshake + "***" + model.deviceLocation ?? "VietNam";
                    mobileHandshake = mobileHandshake + "$$$" + model.deviceLanguage;

                    var responseData = new
                    {
                        xHandshake = new Encrypt(webHandshake).EncryptString(),
                        xMHandshake = new Encrypt(mobileHandshake).EncryptString(),
                        xForwardedFor = xForwardedFor,
                        xSecretKey = _systemSettings.SecretKey ?? "encrypt123456789encrypt123456789",
                        xIVKey = _systemSettings.IVKey ?? "123456789encrypt"
                    };

                    return Ok(responseData);
                }
            }
            catch (Exception ex)
            {
                return LogExceptionEvent(_log4net, $"{RoutePrefix.ACCOUNT}/Auth/Handshake", ex, model);
            }
        }



#if DEBUG
        public class EncryptDataModel
        {
            public string Value { get; set; }
        }

        /// <summary>
        /// Encrypt data
        /// </summary>
        /// <returns>The data encrypt.</returns>
        // POST: api/adminuser/Auth/Encrypt
        [HttpPost("Encrypt")]
        [AllowAnonymous]
        public IActionResult Encrypt(EncryptDataModel model)
        {
            string encrypt = new Encrypt(model.Value).EncryptString();
            return ResponseData(new { Value = encrypt });
        }

        /// <summary>
        /// Decrypt data
        /// </summary>
        /// <returns>The data decrypt.</returns>
        // POST: api/adminuser/Auth/Decrypt
        [HttpPost("Decrypt")]
        [AllowAnonymous]
        public IActionResult Decrypt(EncryptDataModel model)
        {
            string decrypt = new Encrypt(model.Value).DecryptString();
            return ResponseData(new { Value = decrypt });
        }

        /// <summary>
        /// Hash password MD5 hashTable
        /// </summary>
        /// <returns>The result data encrypt.</returns>
        // POST: api/adminuser/Auth/GetHashMD5
        [HttpPost("GetHashMD5")]
        [AllowAnonymous]
        public IActionResult GetHashMD5(string password)
        {
            string hashedPassword = new Encrypt(password).GetHashMD5();
            return ResponseData(new { hashedPassword });
        }

        /// <summary>
        /// Hash verify MD5 hashTable
        /// </summary>
        /// <returns>The result data verify.</returns>
        // POST: api/adminuser/Auth/VerifyHashMD5
        [HttpPost("VerifyHashMD5")]
        [AllowAnonymous]
        public IActionResult VerifyHashMD5(string oldHashedPassword, string newPassword)
        {
            string newHashedPassword = new Encrypt(newPassword).GetHashMD5();
            PasswordVerificationResult passwordVerificationResult = new Encrypt(oldHashedPassword).VerifyHashMD5(newHashedPassword);
            return ResponseData(new { result = passwordVerificationResult });
        }

        /// <summary>
        /// Encrypt data with AES-256, key="encrypt123456789", iv"123456789encrypt"
        /// </summary>
        /// <returns>The data encrypt.</returns>
        // POST: api/adminuser/Auth/EncryptAES256
        [HttpPost("EncryptAES256")]
        [AllowAnonymous]
        public IActionResult EncryptAES256(EncryptDataModel model)
        {
            try
            {
                string encrypt = new Encrypt("").EncryptStringToBytes_Aes(model.Value);
                Logs.debug("EncryptAES256: " + encrypt);
                return ResponseData(new { Value = encrypt });
            }
            catch (Exception)
            {
                return ResponseBadRequest(new CustomBadRequest("Can not Encrypt, value invalid format AES-256", this.ControllerContext));
            }
        }

        /// <summary>
        /// Decrypt data with AES-256, key="encrypt123456789", iv"123456789encrypt"
        /// </summary>
        /// <returns>The data decrypt.</returns>
        // POST: api/adminuser/Auth/DecryptAES256
        [HttpPost("DecryptAES256")]
        [AllowAnonymous]
        public IActionResult DecryptAES256(EncryptDataModel model)
        {
            try
            {
                string decrypt = new Encrypt("").DecryptBytesToString_Aes(model.Value);
                Logs.debug("DecryptAES256: " + decrypt);
                return ResponseData(new { Value = decrypt });
            }
            catch (Exception)
            {
                return ResponseBadRequest(new CustomBadRequest("Can not Decrypt, value invalid format AES-256", this.ControllerContext));
            }
        }
#endif
    }
}
