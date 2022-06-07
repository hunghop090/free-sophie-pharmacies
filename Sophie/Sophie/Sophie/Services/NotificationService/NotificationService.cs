using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Microsoft.Extensions.Logging;
using Sophie.Units;

namespace Sophie.Services.NotificationService
{
    public class NotificationService : INotificationService
    {
        private string serverKey = "AAAAZnS2Z5E:APA91bEVbUsLuvWBIttsdT3qXaNfUhPw4cp5EqD3U8tgWIPBVS7nkvm4R-qUdkFEojN9sDiey3lGgFCw_ww78MS3FtZc0vaTz63EhleDyrPwkSS-UKSv4JLADIB81LG0o4W5jFR2BOqZ";
        private string senderId = "440044775313";
        private string webAddr = "https://fcm.googleapis.com/fcm/send";

        private readonly ILog _log4net = log4net.LogManager.GetLogger(typeof(NotificationService));
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger;
        }

        public async Task<bool> SendSingleNotification(string deviceToken, string title, string body)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={serverKey}");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Sender", $"id={senderId}");
                var obj = new
                {
                    to = deviceToken,
                    priority = "high",
                    content_available = true,
                    notification = new
                    {
                        name = "Sophie",
                        title = title,
                        body = body,
                        sound = "default",
                        icon = "https://systemsophie.com/favicon.ico",
                        image = "https://systemsophie.com/fcm.png"
                    },
                    data = new
                    {
                        type = "BACKEND_MESSAGE",
                        info = new { },
                    },
                };
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(webAddr, data);
                Logs.debug("[NotificationService] Result Send Notification: " + response.StatusCode);
                return response.StatusCode == System.Net.HttpStatusCode.OK;
            }
        }

        public Task<bool> SendMultipleNotification(List<string> listDeviceToken, string title, string body)
        {
            Task task = new Task(async () =>
            {
                try
                {
                    foreach (var deviceToken in listDeviceToken)
                    {
                        using (var client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={serverKey}");
                            client.DefaultRequestHeaders.TryAddWithoutValidation("Sender", $"id={senderId}");
                            var obj = new
                            {
                                to = deviceToken,
                                priority = "high",
                                show_in_foreground = true,
                                content_available = true,
                                notification = new
                                {
                                    name = "Sophie",
                                    title = title,
                                    body = body,
                                    sound = "default",
                                    icon = "https://systemsophie.com/favicon.ico",
                                    image = "https://systemsophie.com/fcm.png"
                                },
                                data = new
                                {
                                    type = "BACKEND_MESSAGE",
                                    info = new { },
                                },
                            };
                            string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                            var data = new StringContent(json, Encoding.UTF8, "application/json");
                            HttpResponseMessage response = await client.PostAsync(webAddr, data);
                            Logs.debug("[NotificationService] Result Send Notification: " + response.StatusCode);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logs.debug("[NotificationService] Exception Send Notification: " + ex.StackTrace);
                    _logger.LogError("[NotificationService] Exception Send Notification: " + ex.StackTrace);
                    _log4net.Error("[NotificationService] Exception Send Notification: " + ex.StackTrace);
                }
                finally
                {
                }
            });
            task.Start();

            return Task.FromResult(true);
        }
    }
}
