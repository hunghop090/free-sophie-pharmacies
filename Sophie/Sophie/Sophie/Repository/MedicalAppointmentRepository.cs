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
using Sophie.Resource.Entities.MedicalAppointment;
using Sophie.Units;

namespace Sophie.Repository
{
    public class MedicalAppointmentRepository : BaseRepository, IMedicalAppointmentRepository
    {
        private readonly ILog _log4net = log4net.LogManager.GetLogger(typeof(MedicalAppointmentRepository));
        private readonly LogMongoService _logMongoService;

        private readonly IMongoCollection<MedicalAppointment> _collectionMedicalAppointment;

        public MedicalAppointmentRepository(LogMongoService logMongoService) : base()
        {
            _logMongoService = logMongoService;
            _collectionMedicalAppointment = _database.GetCollection<MedicalAppointment>($"MedicalAppointment");

            //var indexOptions = new CreateIndexOptions();
            //var indexKeys_1 = Builders<Address>.IndexKeys.Ascending(item => item.AccountId);
            //var indexKeys_2 = Builders<Address>.IndexKeys.Ascending(item => item.PhoneNumber);
            //var indexKeys_3 = Builders<Address>.IndexKeys.Ascending(item => item.Email);
            //var indexModelAddress = new List<CreateIndexModel<Address>>();
            //indexModelAddress.Add(new CreateIndexModel<Address>(indexKeys_1, indexOptions));
            //indexModelAddress.Add(new CreateIndexModel<Address>(indexKeys_2, indexOptions));
            //indexModelAddress.Add(new CreateIndexModel<Address>(indexKeys_3, indexOptions));
            //_collectionAddress.Indexes.CreateMany(indexModelAddress);
        }

        public MedicalAppointment CreateMedicalAppointment(MedicalAppointment item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.MedicalAppointmentId = Guid.NewGuid().ToString();
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionMedicalAppointment.InsertOne(item);
            return item;
        }

        public MedicalAppointment RestoreMedicalAppointment(MedicalAppointment item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.MedicalAppointmentId = (!string.IsNullOrEmpty(item.MedicalAppointmentId) ? item.MedicalAppointmentId : Guid.NewGuid().ToString());
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionMedicalAppointment.InsertOne(item);
            return item;
        }

        public MedicalAppointment DeleteMedicalAppointment(string medicalAppointmentId)
        {
            return _collectionMedicalAppointment.FindOneAndDelete(item => item.MedicalAppointmentId == medicalAppointmentId);
        }

        public List<MedicalAppointment> ListMedicalAppointment(int pageIndex = 0, int pageSize = 99)
        {
            return _collectionMedicalAppointment.Find(item => true).Skip(pageIndex * pageSize).Limit(pageSize).ToList();
        }

        public MedicalAppointment UpdateMedicalAppointment(MedicalAppointment item)
        {
            MedicalAppointment _item = _collectionMedicalAppointment.Find(x => x.MedicalAppointmentId == item.MedicalAppointmentId).FirstOrDefault();
            if (_item == null) return null;

            var update = Builders<MedicalAppointment>.Update
                .Set("MedicalAppointmentId", item.MedicalAppointmentId)
                .Set("TypeMedicalAppointment", item.TypeMedicalAppointment)
                .Set("AccountId", item.AccountId)
                .Set("HospitalId", item.HospitalId)
                .Set("Specialist", item.Specialist)
                .Set("DoctorId", item.DoctorId)
                .Set("AddressId", item.AddressId)
                .Set("DescriptionOfSymptoms", item.DescriptionOfSymptoms)
                .Set("Date", item.Date)
                .Set("Time", item.Time)
                .Set("TypeStatusMedicalAppointment", item.TypeStatusMedicalAppointment)
                .Set("Created", _item.Created)
                .Set("Updated", DateTimes.Now());
            
            return _collectionMedicalAppointment.FindOneAndUpdate(x => x.Id == _item.Id, update);
        }

        [Obsolete]
        public long TotalMedicalAppointment()
        {
            return _collectionMedicalAppointment.Count(item => true);
        }

        public MedicalAppointment FindByIdMedicalAppointment(string medicalAppointmentId)
        {
            return _collectionMedicalAppointment.Find(item => item.MedicalAppointmentId == medicalAppointmentId).FirstOrDefault();
        }

        public List<MedicalAppointment> FindByIdAccount(string accountId)
        {
            return _collectionMedicalAppointment.Find(item => item.AccountId == accountId).ToList();
        }

    }
}
