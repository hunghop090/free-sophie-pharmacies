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
    public class DoctorRepository : BaseRepository, IDoctorRepository
    {
        private readonly ILog _log4net = log4net.LogManager.GetLogger(typeof(DoctorRepository));
        private readonly LogMongoService _logMongoService;

        private readonly IMongoCollection<Doctor> _collectionDoctor;

        public DoctorRepository(LogMongoService logMongoService) : base()
        {
            _logMongoService = logMongoService;
            _collectionDoctor = _database.GetCollection<Doctor>($"Doctor");

            var indexOptions = new CreateIndexOptions();
            var indexKeys_1 = Builders<Doctor>.IndexKeys.Ascending(item => item.DoctorId);
            var indexKeys_2 = Builders<Doctor>.IndexKeys.Ascending(item => item.Specialist);
            var indexKeys_3 = Builders<Doctor>.IndexKeys.Ascending(item => item.NameDoctor);
            var indexModelDoctor = new List<CreateIndexModel<Doctor>>();
            indexModelDoctor.Add(new CreateIndexModel<Doctor>(indexKeys_1, indexOptions));
            indexModelDoctor.Add(new CreateIndexModel<Doctor>(indexKeys_2, indexOptions));
            indexModelDoctor.Add(new CreateIndexModel<Doctor>(indexKeys_3, indexOptions));
            _collectionDoctor.Indexes.CreateMany(indexModelDoctor);
        }

        public Doctor CreateDoctor(Doctor item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.DoctorId = Guid.NewGuid().ToString();
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionDoctor.InsertOne(item);
            return item;
        }

        public Doctor RestoreDoctor(Doctor item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.DoctorId = (!string.IsNullOrEmpty(item.DoctorId) ? item.DoctorId : Guid.NewGuid().ToString());
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionDoctor.InsertOne(item);
            return item;
        }

        public Doctor DeleteDoctor(string doctorId)
        {
            return _collectionDoctor.FindOneAndDelete(item => item.DoctorId == doctorId);
        }

        public List<Doctor> ListDoctor(int pageIndex = 0, int pageSize = 99)
        {
            return _collectionDoctor.Find(item => true).Skip(pageIndex * pageSize).Limit(pageSize).ToList();
        }

        public Doctor UpdateDoctor(Doctor item)
        {
            Doctor _item = _collectionDoctor.Find(x => x.DoctorId == item.DoctorId).FirstOrDefault();
            if (_item == null) return null;

            var update = Builders<Doctor>.Update
                .Set("DoctorId", item.DoctorId)
                .Set("TypeLogin", item.TypeLogin)
                .Set("Confirm", item.Confirm)
                .Set("Active", item.Active)
                .Set("PhoneNumber", item.PhoneNumber)
                .Set("Email", item.Email)
                .Set("Username", item.Username)
                .Set("Password", item.Password)

                .Set("Specialist", item.Specialist)
                .Set("Firstname", item.Firstname)
                .Set("Lastname", item.Lastname)
                .Set("NameDoctor", item.NameDoctor)
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
            
            return _collectionDoctor.FindOneAndUpdate(x => x.Id == _item.Id, update);
        }

        [Obsolete]
        public long TotalDoctor()
        {
            return _collectionDoctor.Count(item => true);
        }

        public Doctor FindByIdDoctor(string doctorId)
        {
            return _collectionDoctor.Find(item => item.DoctorId == doctorId).FirstOrDefault(); 
        }

        public Doctor FindByVideoCallIdDoctor(string videoCallId)
        {
            return _collectionDoctor.Find(item => item.VideoCallId == videoCallId).FirstOrDefault();
        }
    }
}
