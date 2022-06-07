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
    public class PharmacistRepository : BaseRepository, IPharmacistRepository
    {
        private readonly ILog _log4net = log4net.LogManager.GetLogger(typeof(PharmacistRepository));
        private readonly LogMongoService _logMongoService;

        private readonly IMongoCollection<Pharmacist> _collectionPharmacist;

        public PharmacistRepository(LogMongoService logMongoService) : base()
        {
            _logMongoService = logMongoService;
            _collectionPharmacist = _database.GetCollection<Pharmacist>($"Pharmacist");

            var indexOptions = new CreateIndexOptions();
            var indexKeys_1 = Builders<Pharmacist>.IndexKeys.Ascending(item => item.PharmacistId);
            var indexKeys_2 = Builders<Pharmacist>.IndexKeys.Ascending(item => item.Specialist);
            var indexKeys_3 = Builders<Pharmacist>.IndexKeys.Ascending(item => item.NamePharmacist);
            var indexModelPharmacist = new List<CreateIndexModel<Pharmacist>>();
            indexModelPharmacist.Add(new CreateIndexModel<Pharmacist>(indexKeys_1, indexOptions));
            indexModelPharmacist.Add(new CreateIndexModel<Pharmacist>(indexKeys_2, indexOptions));
            indexModelPharmacist.Add(new CreateIndexModel<Pharmacist>(indexKeys_3, indexOptions));
            _collectionPharmacist.Indexes.CreateMany(indexModelPharmacist);
        }

        public Pharmacist CreatePharmacist(Pharmacist item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.PharmacistId = Guid.NewGuid().ToString();
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionPharmacist.InsertOne(item);
            return item;
        }

        public Pharmacist DeletePharmacist(string pharmacistId)
        {
            return _collectionPharmacist.FindOneAndDelete(item => item.PharmacistId == pharmacistId);
        }

        public Pharmacist FindByIdPharmacist(string pharmacistId)
        {
            return _collectionPharmacist.Find(item => item.PharmacistId == pharmacistId).FirstOrDefault();
        }

        public Pharmacist FindByVideoCallIdPharmacist(string videoCallId)
        {
            return _collectionPharmacist.Find(item => item.VideoCallId == videoCallId).FirstOrDefault();
        }

        public List<Pharmacist> ListPharmacist(int pageIndex = 0, int pageSize = 99)
        {
            return _collectionPharmacist.Find(item => true).Skip(pageIndex * pageSize).Limit(pageSize).ToList();
        }

        public Pharmacist RestorePharmacist(Pharmacist item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.PharmacistId = (!string.IsNullOrEmpty(item.PharmacistId) ? item.PharmacistId : Guid.NewGuid().ToString());
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionPharmacist.InsertOne(item);
            return item;
        }

        [Obsolete]
        public long TotalPharmacist()
        {
            return _collectionPharmacist.Count(item => true);
        }

        public Pharmacist UpdatePharmacist(Pharmacist item)
        {
            Pharmacist _item = _collectionPharmacist.Find(x => x.PharmacistId == item.PharmacistId).FirstOrDefault();
            if (_item == null) return null;

            var update = Builders<Pharmacist>.Update
                .Set("PharmacistId", item.PharmacistId)
                .Set("TypeLogin", item.TypeLogin)
                .Set("Confirm", item.Confirm)
                .Set("Active", item.Active)
                .Set("PhoneNumber", item.PhoneNumber)
                .Set("Email", item.Email)
                .Set("Username", item.Username)
                .Set("Password", item.Password)

                .Set("TypePharmacist", item.TypePharmacist)
                .Set("Specialist", item.Specialist)
                .Set("Firstname", item.Firstname)
                .Set("Lastname", item.Lastname)
                .Set("NamePharmacist", item.NamePharmacist)
                .Set("Birthdate", item.Birthdate)
                .Set("Address", item.Address)
                .Set("HomePhone", item.HomePhone)
                .Set("Avatar", item.Avatar)
                .Set("Race", item.Race)
                .Set("Gender", item.Gender)
                .Set("Language", item.Language)
                .Set("WorkPlace", item.WorkPlace)

                .Set("Num", item.Num)
                .Set("Exp", item.Exp)
                .Set("Rate", item.Rate)
                .Set("Info", item.Info)

                .Set("TwoFactorEnabled", item.TwoFactorEnabled)
                .Set("IsOnline", item.IsOnline)
                .Set("VideoCallId", item.VideoCallId)
                .Set("VideoCallToken", item.VideoCallToken)
                .Set("Notes", item.Notes)

                .Set("Created", _item.Created)
                .Set("Updated", DateTimes.Now());

            return _collectionPharmacist.FindOneAndUpdate(x => x.Id == _item.Id, update);
        }

        public Pharmacist FindByEmailPharmacist(string email)
        {
            return _collectionPharmacist.Find(item => item.Email == email).FirstOrDefault();
        }

    }

}
