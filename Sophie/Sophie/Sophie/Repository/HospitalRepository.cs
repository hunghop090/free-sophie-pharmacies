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
    public class HospitalRepository : BaseRepository, IHospitalRepository
    {
        private readonly ILog _log4net = log4net.LogManager.GetLogger(typeof(HospitalRepository));
        private readonly LogMongoService _logMongoService;

        private readonly IMongoCollection<Hospital> _collectionHospital;

        public HospitalRepository(LogMongoService logMongoService) : base()
        {
            _logMongoService = logMongoService;
            _collectionHospital = _database.GetCollection<Hospital>($"Hospital");

            var indexOptions = new CreateIndexOptions();
            var indexKeys_1 = Builders<Hospital>.IndexKeys.Ascending(item => item.HospitalId);
            var indexKeys_2 = Builders<Hospital>.IndexKeys.Ascending(item => item.Specialist);
            var indexKeys_3 = Builders<Hospital>.IndexKeys.Ascending(item => item.NameHospital);
            var indexModelHospital = new List<CreateIndexModel<Hospital>>();
            indexModelHospital.Add(new CreateIndexModel<Hospital>(indexKeys_1, indexOptions));
            indexModelHospital.Add(new CreateIndexModel<Hospital>(indexKeys_2, indexOptions));
            indexModelHospital.Add(new CreateIndexModel<Hospital>(indexKeys_3, indexOptions));
            _collectionHospital.Indexes.CreateMany(indexModelHospital);
        }

        public Hospital CreateHospital(Hospital item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.HospitalId = Guid.NewGuid().ToString();
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionHospital.InsertOne(item);
            return item;
        }

        public Hospital RestoreHospital(Hospital item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.HospitalId = (!string.IsNullOrEmpty(item.HospitalId) ? item.HospitalId : Guid.NewGuid().ToString());
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionHospital.InsertOne(item);
            return item;
        }

        public Hospital DeleteHospital(string hospitalId)
        {
            return _collectionHospital.FindOneAndDelete(item => item.HospitalId == hospitalId);
        }

        public List<Hospital> ListHospital(int pageIndex = 0, int pageSize = 99)
        {
            return _collectionHospital.Find(item => true).Skip(pageIndex * pageSize).Limit(pageSize).ToList();
        }

        public Hospital UpdateHospital(Hospital item)
        {
            Hospital _item = _collectionHospital.Find(x => x.HospitalId == item.HospitalId).FirstOrDefault();
            if (_item == null) return null;

            var update = Builders<Hospital>.Update
                .Set("HospitalId", item.HospitalId)
                .Set("TypeLogin", item.TypeLogin)
                .Set("Confirm", item.Confirm)
                .Set("Active", item.Active)
                .Set("PhoneNumber", item.PhoneNumber)
                .Set("Email", item.Email)
                .Set("Username", item.Username)
                .Set("Password", item.Password)

                .Set("Specialist", item.Specialist)
                .Set("NameHospital", item.NameHospital)
                .Set("Avatar", item.Avatar)
                .Set("Language", item.Language)

                .Set("Province", item.Province)
                .Set("City", item.City)
                .Set("District", item.District)
                .Set("Wards", item.Wards)
                .Set("AddressHospital", item.AddressHospital)

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
            
            return _collectionHospital.FindOneAndUpdate(x => x.Id == _item.Id, update);
        }

        [Obsolete]
        public long TotalHospital()
        {
            return _collectionHospital.Count(item => true);
        }

        public Hospital FindByIdHospital(string hospitalId)
        {
            return _collectionHospital.Find(item => item.HospitalId == hospitalId).FirstOrDefault();
        }

    }
}
