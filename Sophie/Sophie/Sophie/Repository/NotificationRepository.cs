using System;
using System.Collections.Generic;
using App.Core.Entities;
using App.Core.Services;
using App.SharedLib.Repository;
using log4net;
using MongoDB.Bson;
using MongoDB.Driver;
using Sophie.Repository.Interface;
using Sophie.Resource.Entities;
using Sophie.Units;

namespace Sophie.Repository
{
    public class NotificationRepository : BaseRepository, INotificationRepository
    {
        private readonly ILog _log4net = log4net.LogManager.GetLogger(typeof(NotificationRepository));
        private readonly LogMongoService _logMongoService;

        private readonly IMongoCollection<Notification> _collectionNotification;
        private readonly IMongoCollection<NotificationConnective> _collectionNotificationConnective;

        public NotificationRepository(LogMongoService logMongoService) : base()
        {
            _logMongoService = logMongoService;
            _collectionNotification = _database.GetCollection<Notification>($"Notification");
            _collectionNotificationConnective = _database.GetCollection<NotificationConnective>($"NotificationConnective");

            var indexOptions = new CreateIndexOptions();

            var indexKeys_1 = Builders<Notification>.IndexKeys.Ascending(item => item.NotificationId);
            var indexModelNotification = new List<CreateIndexModel<Notification>>();
            indexModelNotification.Add(new CreateIndexModel<Notification>(indexKeys_1, indexOptions));
            _collectionNotification.Indexes.CreateMany(indexModelNotification);

            var indexKeys_2 = Builders<NotificationConnective>.IndexKeys.Ascending(item => item.NotificationConnectiveId);
            var indexKeys_3 = Builders<NotificationConnective>.IndexKeys.Ascending(item => item.NotificationId);
            var indexKeys_4 = Builders<NotificationConnective>.IndexKeys.Ascending(item => item.AccountId);
            var indexKeys_5 = Builders<NotificationConnective>.IndexKeys.Ascending(item => item.DoctorId);
            var indexModelNotificationConnective = new List<CreateIndexModel<NotificationConnective>>();
            indexModelNotificationConnective.Add(new CreateIndexModel<NotificationConnective>(indexKeys_2, indexOptions));
            indexModelNotificationConnective.Add(new CreateIndexModel<NotificationConnective>(indexKeys_3, indexOptions));
            indexModelNotificationConnective.Add(new CreateIndexModel<NotificationConnective>(indexKeys_4, indexOptions));
            indexModelNotificationConnective.Add(new CreateIndexModel<NotificationConnective>(indexKeys_5, indexOptions));
            _collectionNotificationConnective.Indexes.CreateMany(indexModelNotificationConnective);
        }

        // Notification
        public Notification CreateNotification(Notification item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.NotificationId = Guid.NewGuid().ToString();
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionNotification.InsertOne(item);
            return item;
        }

        public Notification RestoreNotification(Notification item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.NotificationId = (!string.IsNullOrEmpty(item.NotificationId) ? item.NotificationId : Guid.NewGuid().ToString());
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionNotification.InsertOne(item);
            return item;
        }

        public Notification DeleteNotification(string notificationId)
        {
            return _collectionNotification.FindOneAndDelete(item => item.NotificationId == notificationId);
        }

        public List<Notification> ListNotification(int pageIndex = 0, int pageSize = 99)
        {
            return _collectionNotification.Find(item => true).Skip(pageIndex * pageSize).Limit(pageSize).ToList();
        }

        public Notification UpdateNotification(Notification item)
        {
            Notification _item = _collectionNotification.Find(x => x.NotificationId == item.NotificationId).FirstOrDefault();
            if (_item == null) return null;

            var update = Builders<Notification>.Update
                .Set("NotificationId", item.NotificationId)
                .Set("Type", item.Type)
                .Set("TypeFor", item.TypeFor)
                .Set("TypeSend", item.TypeSend)
                .Set("Images", item.Images)
                .Set("Subject", item.Subject)
                .Set("SubjectType", item.SubjectType)
                .Set("DateTime", item.DateTime)
                .Set("Content", item.Content)
                .Set("Created", _item.Created)
                .Set("Updated", DateTimes.Now());
            
            return _collectionNotification.FindOneAndUpdate(x => x.Id == _item.Id, update);
        }

        [Obsolete]
        public long TotalNotification()
        {
            return _collectionNotification.Count(item => true);
        }

        public Notification FindByIdNotification(string notificationId)
        {
            return _collectionNotification.Find(item => item.NotificationId == notificationId).FirstOrDefault();
        }



        // NotificationConnective
        public NotificationConnective CreateNotificationConnective(NotificationConnective item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.NotificationConnectiveId = Guid.NewGuid().ToString();
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionNotificationConnective.InsertOne(item);
            return item;
        }

        public NotificationConnective RestoreNotificationConnective(NotificationConnective item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.NotificationConnectiveId = (!string.IsNullOrEmpty(item.NotificationConnectiveId) ? item.NotificationConnectiveId : Guid.NewGuid().ToString());
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionNotificationConnective.InsertOne(item);
            return item;
        }

        public NotificationConnective DeleteNotificationConnective(string notificationConnectiveId)
        {
            return _collectionNotificationConnective.FindOneAndDelete(item => item.NotificationConnectiveId == notificationConnectiveId);
        }

        public List<NotificationConnective> ListNotificationConnective(int pageIndex = 0, int pageSize = 99)
        {
            return _collectionNotificationConnective.Find(item => true).Skip(pageIndex * pageSize).Limit(pageSize).ToList();
        }

        public NotificationConnective UpdateNotificationConnective(NotificationConnective item)
        {
            NotificationConnective _item = _collectionNotificationConnective.Find(x => x.NotificationConnectiveId == item.NotificationConnectiveId).FirstOrDefault();
            if (_item == null) return null;

            var update = Builders<NotificationConnective>.Update
                .Set("NotificationConnectiveId", item.NotificationConnectiveId)
                .Set("NotificationId", item.NotificationId)
                .Set("AccountId", item.AccountId)
                .Set("DoctorId", item.DoctorId)
                .Set("IsRead", item.IsRead)
                .Set("Created", _item.Created)
                .Set("Updated", DateTimes.Now());
            
            return _collectionNotificationConnective.FindOneAndUpdate(x => x.Id == _item.Id, update);
        }

        [Obsolete]
        public long TotalNotificationConnective()
        {
            return _collectionNotificationConnective.Count(item => true);
        }

        public NotificationConnective FindByIdNotificationConnective(string notificationConnectiveId)
        {
            return _collectionNotificationConnective.Find(item => item.NotificationConnectiveId == notificationConnectiveId).FirstOrDefault();
        }

        public List<NotificationConnective> FindByIdAccount(string accountId)
        {
            return _collectionNotificationConnective.Find(item => item.AccountId == accountId).ToList();
        }

        public List<NotificationConnective> FindByIdNoti(string notificationId)
        {
            return _collectionNotificationConnective.Find(item => item.NotificationId == notificationId).ToList();
        }

    }
}
