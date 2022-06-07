using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using App.Core.Clients;
using App.Core.Middleware.IPAddressTools;
using App.Core.Settings;
using EasyCronJob.Abstractions;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sophie.Repository.Interface;
using Sophie.Resource.Entities;
using Sophie.Units;

namespace Sophie.Services.NotificationService
{
    public class ConsoleCronJob : CronJobService
    {
        private readonly ILog _log4net = log4net.LogManager.GetLogger(typeof(ConsoleCronJob));
        private readonly ILogger<ConsoleCronJob> _logger;

        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public string rootPathUpload = "";

        public ConsoleCronJob(ICronConfiguration<ConsoleCronJob> cronConfiguration, ILogger<ConsoleCronJob> logger,
            [FromServices] IServiceScopeFactory serviceScopeFactory, IHttpContextAccessor httpContextAccessor)
            : base(cronConfiguration.CronExpression, cronConfiguration.TimeZoneInfo)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _httpContextAccessor = httpContextAccessor;

            #if DEBUG
            //rootPathUpload = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"/wwwroot"; // "Sophie/bin/Debug/net5.0/wwwroot"
            rootPathUpload = System.IO.Directory.GetCurrentDirectory() + @"/wwwroot"; // "Sophie/wwwroot"
            #else
            //rootPathUpload = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"/wwwroot"; // "Sophie/bin/Debug/net5.0/wwwroot"
            rootPathUpload = System.IO.Directory.GetCurrentDirectory() + @"/wwwroot"; // "Sophie/wwwroot"
            #endif
        }

        private Sophie.Resource.Model.ConfigurationsModel LoadConfig()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _configurationsRepository = scope.ServiceProvider.GetRequiredService<IConfigurationsRepository>();
                List<Configurations> listConfig = _configurationsRepository.ListConfigurations();
                if (listConfig.Count > 0)
                {
                    Configurations configurations = listConfig[0];
                    Sophie.Resource.Model.ConfigurationsModel configurationsModel = Newtonsoft.Json.JsonConvert.DeserializeObject<Sophie.Resource.Model.ConfigurationsModel>(configurations.Settings, JsonSettings.SettingForNewtonsoft);
                    return configurationsModel;
                }
            }
            return null;
        }

        private string GetMyIp()
        {
            //using (var scope = _serviceScopeFactory.CreateScope())
            //{
            //    var _httpContextAccessor = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            //    string myIp = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
            //    Logs.debug($"[ConsoleCronJob] MyIp {Newtonsoft.Json.JsonConvert.SerializeObject(_httpContextAccessor, JsonSettings.SettingForNewtonsoft)}");
            //    return myIp;
            //}

            //string myIp = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
            //Logs.debug($"[ConsoleCronJob] MyIp {Newtonsoft.Json.JsonConvert.SerializeObject(_httpContextAccessor.HttpContext, JsonSettings.SettingForNewtonsoft)}");
            //return myIp;

            List<string> localIpAddress = new List<string>();
            IPAddress[] ips = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            
            // 192.168.*.*
            //string rgx_1 = @"192{1}\.168{1}\..*";
            //var result_1 = ips.FirstOrDefault(x => {
            //    Match m1 = Regex.Match(x.ToString(), rgx_1);
            //    return m1.Captures.Count >= 1;
            //});
            //string ipAddress_1 = result_1?.ToString();
            //if(!string.IsNullOrEmpty(ipAddress_1))
            //{
            //    localIpAddress.Add(ipAddress_1);
            //}

            // *.*.*.*
            string rgx_2 = @"..*\..*\..*";
            var result_2 = ips.Where(x => {
                Match m2 = Regex.Match(x.ToString(), rgx_2);
                return m2.Captures.Count >= 1;
            }).ToList();
            foreach(var ipAddress_2 in result_2)
            {
                if (!string.IsNullOrEmpty(ipAddress_2?.ToString()))
                {
                    localIpAddress.Add(ipAddress_2?.ToString());
                }
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(localIpAddress, JsonSettings.SettingForNewtonsoft);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            //_logger.LogInformation("Start");
            Logs.debug($"[ConsoleCronJob] Start {DateTimes.Now()}");
            return base.StartAsync(cancellationToken);
        }

        protected override Task ScheduleJob(CancellationToken cancellationToken)
        {
            //_logger.LogInformation("Scheduled");
            Logs.debug($"[ConsoleCronJob] Scheduled {DateTimes.Now()}");
            return base.ScheduleJob(cancellationToken);
        }

        public override async Task<Task> DoWork(CancellationToken cancellationToken)
        {
            //_logger.LogInformation("DoWork");
            Logs.debug($"[ConsoleCronJob] DoWork {DateTimes.Now()}");

            try
            {
                Sophie.Resource.Model.ConfigurationsModel configurations = LoadConfig();
                int notificationExpriedRead = int.Parse(configurations?.PaymentSetting?.NotificationExpriedRead ?? "7");
                int notificationExpriedUnRead = int.Parse(configurations?.PaymentSetting?.NotificationExpriedUnRead ?? "30");
                Logs.debug($"[ConsoleCronJob] NotificationExpriedRead: {notificationExpriedRead}, NotificationExpriedUnRead : {notificationExpriedUnRead}");

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var _notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
                    List<Notification> listNotification = _notificationRepository.ListNotification(0, int.MaxValue);
                    int sizeDeletedNotification = 0;
                    int sizeDeletedNotificationConnective = 0;
                    foreach (var notification in listNotification)
                    {
                        double totalDays = System.Math.Abs(Math.Round((Convert.ToDateTime(DateTimes.Now()) - Convert.ToDateTime(notification.DateTime ?? DateTimes.Now())).TotalDays, 0));
                        if (totalDays >= notificationExpriedUnRead)
                        {
                            _logger.LogInformation($"[ConsoleCronJob] Remove Notification {totalDays} days ago, {notification.NotificationId}");
                            sizeDeletedNotification += 1;

                            // remove old file
                            if (notification.Images != null)
                            {
                                foreach (var file in notification.Images)
                                {
                                    if (System.IO.File.Exists($"{rootPathUpload}/{file}")) System.IO.File.Delete($"{rootPathUpload}/{file}");
                                }
                            }
                            _notificationRepository.DeleteNotification(notification.NotificationId);

                            // remove NotificationConnective
                            List<NotificationConnective> listNotificationConnective = _notificationRepository.FindByIdNoti(notification.NotificationId);
                            foreach (var notificationConnective in listNotificationConnective)
                            {
                                _notificationRepository.DeleteNotificationConnective(notificationConnective.NotificationConnectiveId);
                            }
                        }
                        else if (totalDays < notificationExpriedUnRead)
                        {
                            // remove NotificationConnective
                            List<NotificationConnective> listNotificationConnective = _notificationRepository.FindByIdNoti(notification.NotificationId);
                            foreach (var notificationConnective in listNotificationConnective)
                            {
                                if (notificationConnective.IsRead && totalDays >= notificationExpriedRead)
                                {
                                    _logger.LogInformation($"[ConsoleCronJob] Remove NotificationConnective {totalDays} days ago, {notificationConnective.NotificationConnectiveId}");
                                    sizeDeletedNotificationConnective += 1;

                                    _notificationRepository.DeleteNotificationConnective(notificationConnective.NotificationConnectiveId);
                                }
                            }
                        }
                    }

                    await SendMessageToSlack(LogLevel.Warning,
                        "Lịch tự động xóa thông báo cũ",
                        Newtonsoft.Json.JsonConvert.SerializeObject(new
                        {
                            SizeNotification = listNotification.Count,
                            SizeDeletedNotification = sizeDeletedNotification,
                            SizeDeletedNotificationConnective = sizeDeletedNotificationConnective,
                            NotificationExpriedRead = notificationExpriedRead,
                            NotificationExpriedUnRead = notificationExpriedUnRead,
                        }, JsonSettings.SettingForNewtonsoftPretty),
                        true);
                }
            }
            catch(Exception ex)
            {
                Logs.debug($"[ConsoleCronJob] Exception: {ex.StackTrace}");
                _log4net.Error($"[ConsoleCronJob] Exception: {ex.StackTrace}");
            }
            
            return base.DoWork(cancellationToken);
        }

        public async Task<string> SendMessageToSlack(LogLevel logLevel, string title, string message, bool isAlwaysSend = false)
        {
            try
            {
                //SystemSettings _systemSettings = await _redisService.GetAsync<SystemSettings>(RedisKeyManager.SystemSettings);
                SystemSettings _systemSettings = SystemManage.Get();
                if ((_systemSettings != null && _systemSettings.IsLogs) || isAlwaysSend)
                {
                    string userId = "_";
                    string userEmail = "_";
                    string deviceName = "Chrome";
                    string deviceId = "Chrome.xx.xx.xxxx.xx";
                    string deviceLocation = "VietNam";
                    string deviceLanguage = "VI";
                    string ipClient = GetMyIp() ?? "xxx.xxx.xxx.xxx";

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
                _log4net.Error($"[SendMessageToSlack] Exception: {ex.StackTrace}");
                return ex.StackTrace;
            }
        }

    }
}
