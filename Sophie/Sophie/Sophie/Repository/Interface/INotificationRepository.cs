using System;
using System.Collections.Generic;
using Sophie.Resource.Entities;

namespace Sophie.Repository.Interface
{
    public interface INotificationRepository
    {
        // Notification
        Notification CreateNotification(Notification item);
        Notification RestoreNotification(Notification item);
        Notification DeleteNotification(string notificationId);
        List<Notification> ListNotification(int pageIndex = 0, int pageSize = 99);
        Notification UpdateNotification(Notification item);
        long TotalNotification();

        Notification FindByIdNotification(string notificationId);



        // NotificationConnective
        NotificationConnective CreateNotificationConnective(NotificationConnective item);
        NotificationConnective RestoreNotificationConnective(NotificationConnective item);
        NotificationConnective DeleteNotificationConnective(string notificationConnectiveId);
        List<NotificationConnective> ListNotificationConnective(int pageIndex = 0, int pageSize = 99);
        NotificationConnective UpdateNotificationConnective(NotificationConnective item);
        long TotalNotificationConnective();

        NotificationConnective FindByIdNotificationConnective(string notificationConnectiveId);
        List<NotificationConnective> FindByIdAccount(string accountId);
        List<NotificationConnective> FindByIdNoti(string notificationId);
    }
}
