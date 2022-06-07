using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sophie.Services.NotificationService
{
    public interface INotificationService
    {
        public Task<bool> SendSingleNotification(string deviceToken, string title, string body);
        public Task<bool> SendMultipleNotification(List<string> listDeviceToken, string title, string body);
    }
}
