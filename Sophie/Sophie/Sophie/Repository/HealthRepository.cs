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
using Sophie.Resource.Entities.Health;
using Sophie.Units;
using MongoDB.Driver.Linq;
using System.Linq;

namespace Sophie.Repository
{
    public class HealthRepository : BaseRepository, IHealthRepository
    {
        private readonly ILog _log4net = log4net.LogManager.GetLogger(typeof(HealthRepository));
        private readonly LogMongoService _logMongoService;

        private readonly IMongoCollection<HealthBloodPressure> _collectionHealthBloodPressure;
        private readonly IMongoCollection<HealthBloodSugar> _collectionHealthBloodSugar;
        private readonly IMongoCollection<HealthHBA1C> _collectionHealthHBA1C;
        private readonly IMongoCollection<HealthMenstrualCycle> _collectionHealthMenstrualCycle;
        private readonly IMongoCollection<HealthSpO2> _collectionHealthSpO2;
        private readonly IMongoCollection<HealthWeight> _collectionHealthWeight;

        public HealthRepository(LogMongoService logMongoService) : base()
        {
            _logMongoService = logMongoService;

            // BloodPressure (Huyết áp)
            _collectionHealthBloodPressure = _database.GetCollection<HealthBloodPressure>($"HealthBloodPressure");
            // BloodSugar (Đường huyết)
            _collectionHealthBloodSugar = _database.GetCollection<HealthBloodSugar>($"HealthBloodSugar");
            // HBA1C (Chỉ số đái tháo đường)
            _collectionHealthHBA1C = _database.GetCollection<HealthHBA1C>($"HealthHBA1C");
            // MenstrualCycle (Chu kỳ kinh nguyệt)
            _collectionHealthMenstrualCycle = _database.GetCollection<HealthMenstrualCycle>($"HealthMenstrualCycle");
            // SpO2 (Nồng độ O2 trong máu)
            _collectionHealthSpO2 = _database.GetCollection<HealthSpO2>($"HealthSpO2");
            // Weight (Cân nặng)
            _collectionHealthWeight = _database.GetCollection<HealthWeight>($"HealthWeight");

            var indexOptions = new CreateIndexOptions();
            var indexKeys_1 = Builders<HealthBloodPressure>.IndexKeys.Ascending(item => item.AccountId);
            var indexKeys_2 = Builders<HealthBloodPressure>.IndexKeys.Ascending(item => item.HealthId);
            var indexModelBloodPressure = new List<CreateIndexModel<HealthBloodPressure>>();
            indexModelBloodPressure.Add(new CreateIndexModel<HealthBloodPressure>(indexKeys_1, indexOptions));
            indexModelBloodPressure.Add(new CreateIndexModel<HealthBloodPressure>(indexKeys_2, indexOptions));
            _collectionHealthBloodPressure.Indexes.CreateMany(indexModelBloodPressure);

            var indexKeys_3 = Builders<HealthBloodSugar>.IndexKeys.Ascending(item => item.AccountId);
            var indexKeys_4 = Builders<HealthBloodSugar>.IndexKeys.Ascending(item => item.HealthId);
            var indexModelBloodSugar = new List<CreateIndexModel<HealthBloodSugar>>();
            indexModelBloodSugar.Add(new CreateIndexModel<HealthBloodSugar>(indexKeys_3, indexOptions));
            indexModelBloodSugar.Add(new CreateIndexModel<HealthBloodSugar>(indexKeys_4, indexOptions));
            _collectionHealthBloodSugar.Indexes.CreateMany(indexModelBloodSugar);

            var indexKeys_5 = Builders<HealthHBA1C>.IndexKeys.Ascending(item => item.AccountId);
            var indexKeys_6 = Builders<HealthHBA1C>.IndexKeys.Ascending(item => item.HealthId);
            var indexModelHBA1C = new List<CreateIndexModel<HealthHBA1C>>();
            indexModelHBA1C.Add(new CreateIndexModel<HealthHBA1C>(indexKeys_5, indexOptions));
            indexModelHBA1C.Add(new CreateIndexModel<HealthHBA1C>(indexKeys_6, indexOptions));
            _collectionHealthHBA1C.Indexes.CreateMany(indexModelHBA1C);

            var indexKeys_7 = Builders<HealthMenstrualCycle>.IndexKeys.Ascending(item => item.AccountId);
            var indexKeys_8 = Builders<HealthMenstrualCycle>.IndexKeys.Ascending(item => item.HealthId);
            var indexModelMenstrualCycle = new List<CreateIndexModel<HealthMenstrualCycle>>();
            indexModelMenstrualCycle.Add(new CreateIndexModel<HealthMenstrualCycle>(indexKeys_7, indexOptions));
            indexModelMenstrualCycle.Add(new CreateIndexModel<HealthMenstrualCycle>(indexKeys_8, indexOptions));
            _collectionHealthMenstrualCycle.Indexes.CreateMany(indexModelMenstrualCycle);

            var indexKeys_9 = Builders<HealthSpO2>.IndexKeys.Ascending(item => item.AccountId);
            var indexKeys_10 = Builders<HealthSpO2>.IndexKeys.Ascending(item => item.HealthId);
            var indexModelSpO2 = new List<CreateIndexModel<HealthSpO2>>();
            indexModelSpO2.Add(new CreateIndexModel<HealthSpO2>(indexKeys_9, indexOptions));
            indexModelSpO2.Add(new CreateIndexModel<HealthSpO2>(indexKeys_10, indexOptions));
            _collectionHealthSpO2.Indexes.CreateMany(indexModelSpO2);

            var indexKeys_11 = Builders<HealthWeight>.IndexKeys.Ascending(item => item.AccountId);
            var indexKeys_12 = Builders<HealthWeight>.IndexKeys.Ascending(item => item.HealthId);
            var indexModelWeight = new List<CreateIndexModel<HealthWeight>>();
            indexModelWeight.Add(new CreateIndexModel<HealthWeight>(indexKeys_11, indexOptions));
            indexModelWeight.Add(new CreateIndexModel<HealthWeight>(indexKeys_12, indexOptions));
            _collectionHealthWeight.Indexes.CreateMany(indexModelWeight);

        }

        // BloodPressure (Huyết áp)
        public HealthBloodPressure CreateHealthBloodPressure(HealthBloodPressure item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.HealthId = Guid.NewGuid().ToString();
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionHealthBloodPressure.InsertOne(item);
            return item;
        }

        public HealthBloodPressure RestoreHealthBloodPressure(HealthBloodPressure item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.HealthId = (!string.IsNullOrEmpty(item.HealthId) ? item.HealthId : Guid.NewGuid().ToString());
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionHealthBloodPressure.InsertOne(item);
            return item;
        }

        public HealthBloodPressure DeleteHealthBloodPressure(string healthId)
        {
            return _collectionHealthBloodPressure.FindOneAndDelete(item => item.HealthId == healthId);
        }
        
        public HealthBloodPressure UpdateHealthBloodPressure(HealthBloodPressure item)
        {
            HealthBloodPressure _item = _collectionHealthBloodPressure.Find(x => x.HealthId == item.HealthId).FirstOrDefault();
            if (_item == null) return null;

            var update = Builders<HealthBloodPressure>.Update
                .Set("HealthId", item.HealthId)
                .Set("AccountId", item.AccountId)
                .Set("Type", item.Type)
                .Set("Name", item.Name)

                .Set("MinUnit", item.MinUnit)
                .Set("AverageUnit", item.AverageUnit)
                .Set("MaxUnit", item.MaxUnit)
                .Set("Unit", item.Unit)
                .Set("Date", item.Date)
                .Set("Time", item.Time)

                .Set("Created", _item.Created)
                .Set("Updated", DateTimes.Now());

            return _collectionHealthBloodPressure.FindOneAndUpdate(x => x.Id == _item.Id, update);
        }

        [Obsolete]
        public long TotalHealthBloodPressure()
        {
            return _collectionHealthBloodPressure.Count(item => true);
        }

        public HealthBloodPressure FindByIdHealthBloodPressure(string healthId)
        {
            return _collectionHealthBloodPressure.Find(item => item.HealthId == healthId).FirstOrDefault();
        }

        public List<HealthBloodPressure> ListHealthBloodPressure(int? year, int pageIndex = 0, int pageSize = 99)
        {
            int _year = year ?? DateTimes.Now().Year;
            DateTime startDate = new DateTime(_year, 1, 1);
            DateTime endDate = new DateTime(_year + 1, 1, 1);
            FilterDefinition<HealthBloodPressure> filter = Builders<HealthBloodPressure>.Filter.Gte(x => x.Date, startDate) &
                                                           Builders<HealthBloodPressure>.Filter.Lt(x => x.Date, endDate);
            return _collectionHealthBloodPressure.Find(filter).Skip(pageIndex * pageSize).Limit(pageSize).ToList();
        }

        public List<HealthBloodPressure> FindByIdAccountForHealthBloodPressure(int? year, string accountId)
        {
            int _year = year ?? DateTimes.Now().Year;
            DateTime startDate = new DateTime(_year, 1, 1);
            DateTime endDate = new DateTime(_year + 1, 1, 1);
            FilterDefinition<HealthBloodPressure> filter = Builders<HealthBloodPressure>.Filter.Eq(x => x.AccountId, accountId) &
                                                           Builders<HealthBloodPressure>.Filter.Gte(x => x.Date, startDate) &
                                                           Builders<HealthBloodPressure>.Filter.Lt(x => x.Date, endDate);
            return _collectionHealthBloodPressure.Find(filter).ToList();
        }



        // BloodSugar (Đường huyết)
        public HealthBloodSugar CreateHealthBloodSugar(HealthBloodSugar item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.HealthId = Guid.NewGuid().ToString();
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionHealthBloodSugar .InsertOne(item);
            return item;
        }

        public HealthBloodSugar RestoreHealthBloodSugar(HealthBloodSugar item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.HealthId = (!string.IsNullOrEmpty(item.HealthId) ? item.HealthId : Guid.NewGuid().ToString());
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionHealthBloodSugar.InsertOne(item);
            return item;
        }

        public HealthBloodSugar DeleteHealthBloodSugar(string healthId)
        {
            return _collectionHealthBloodSugar.FindOneAndDelete(item => item.HealthId == healthId);
        }

        public HealthBloodSugar UpdateHealthBloodSugar(HealthBloodSugar item)
        {
            HealthBloodSugar _item = _collectionHealthBloodSugar.Find(x => x.HealthId == item.HealthId).FirstOrDefault();
            if (_item == null) return null;

            var update = Builders<HealthBloodSugar>.Update
                .Set("HealthId", item.HealthId)
                .Set("AccountId", item.AccountId)
                .Set("Type", item.Type)
                .Set("Name", item.Name)

                .Set("MinUnit", item.MinUnit)
                .Set("AverageUnit", item.AverageUnit)
                .Set("MaxUnit", item.MaxUnit)
                .Set("Unit", item.Unit)
                .Set("Date", item.Date)
                .Set("Time", item.Time)

                .Set("Created", _item.Created)
                .Set("Updated", DateTimes.Now());

            return _collectionHealthBloodSugar.FindOneAndUpdate(x => x.Id == _item.Id, update);
        }

        [Obsolete]
        public long TotalHealthBloodSugar()
        {
            return _collectionHealthBloodSugar.Count(item => true);
        }

        public HealthBloodSugar FindByIdHealthBloodSugar(string healthId)
        {
            return _collectionHealthBloodSugar.Find(item => item.HealthId == healthId).FirstOrDefault();
        }

        public List<HealthBloodSugar> ListHealthBloodSugar(int? year, int pageIndex = 0, int pageSize = 99)
        {
            int _year = year ?? DateTimes.Now().Year;
            DateTime startDate = new DateTime(_year, 1, 1);
            DateTime endDate = new DateTime(_year + 1, 1, 1);
            FilterDefinition<HealthBloodSugar> filter = Builders<HealthBloodSugar>.Filter.Gte(x => x.Date, startDate) &
                                                        Builders<HealthBloodSugar>.Filter.Lt(x => x.Date, endDate);
            return _collectionHealthBloodSugar.Find(filter).Skip(pageIndex * pageSize).Limit(pageSize).ToList();
        }

        public List<HealthBloodSugar> FindByIdAccountForHealthBloodSugar(int? year, string accountId)
        {
            int _year = year ?? DateTimes.Now().Year;
            DateTime startDate = new DateTime(_year, 1, 1);
            DateTime endDate = new DateTime(_year + 1, 1, 1);
            FilterDefinition<HealthBloodSugar> filter = Builders<HealthBloodSugar>.Filter.Eq(x => x.AccountId, accountId) &
                                                        Builders<HealthBloodSugar>.Filter.Gte(x => x.Date, startDate) &
                                                        Builders<HealthBloodSugar>.Filter.Lt(x => x.Date, endDate);
            return _collectionHealthBloodSugar.Find(filter).ToList();
        }



        // HBA1C (Chỉ số đái tháo đường)
        public HealthHBA1C CreateHealthHBA1C(HealthHBA1C item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.HealthId = Guid.NewGuid().ToString();
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionHealthHBA1C .InsertOne(item);
            return item;
        }

        public HealthHBA1C RestoreHealthHBA1C(HealthHBA1C item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.HealthId = (!string.IsNullOrEmpty(item.HealthId) ? item.HealthId : Guid.NewGuid().ToString());
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionHealthHBA1C.InsertOne(item);
            return item;
        }

        public HealthHBA1C DeleteHealthHBA1C(string healthId)
        {
            return _collectionHealthHBA1C.FindOneAndDelete(item => item.HealthId == healthId);
        }

        public HealthHBA1C UpdateHealthHBA1C(HealthHBA1C item)
        {
            HealthHBA1C _item = _collectionHealthHBA1C.Find(x => x.HealthId == item.HealthId).FirstOrDefault();
            if (_item == null) return null;

            var update = Builders<HealthHBA1C>.Update
                .Set("HealthId", item.HealthId)
                .Set("AccountId", item.AccountId)
                .Set("Type", item.Type)
                .Set("Name", item.Name)

                .Set("MinUnit", item.MinUnit)
                .Set("AverageUnit", item.AverageUnit)
                .Set("MaxUnit", item.MaxUnit)
                .Set("Unit", item.Unit)
                .Set("Date", item.Date)
                .Set("Time", item.Time)

                .Set("Created", _item.Created)
                .Set("Updated", DateTimes.Now());

            return _collectionHealthHBA1C.FindOneAndUpdate(x => x.Id == _item.Id, update);
        }

        [Obsolete]
        public long TotalHealthHBA1C()
        {
            return _collectionHealthHBA1C.Count(item => true);
        }

        public HealthHBA1C FindByIdHealthHBA1C(string healthId)
        {
            return _collectionHealthHBA1C.Find(item => item.HealthId == healthId).FirstOrDefault();
        }

        public List<HealthHBA1C> ListHealthHBA1C(int? year, int pageIndex = 0, int pageSize = 99)
        {
            int _year = year ?? DateTimes.Now().Year;
            DateTime startDate = new DateTime(_year, 1, 1);
            DateTime endDate = new DateTime(_year + 1, 1, 1);
            FilterDefinition<HealthHBA1C> filter = Builders<HealthHBA1C>.Filter.Gte(x => x.Date, startDate) &
                                                   Builders<HealthHBA1C>.Filter.Lt(x => x.Date, endDate);
            return _collectionHealthHBA1C.Find(filter).Skip(pageIndex * pageSize).Limit(pageSize).ToList();
        }

        public List<HealthHBA1C> FindByIdAccountForHealthHBA1C(int? year, string accountId)
        {
            int _year = year ?? DateTimes.Now().Year;
            DateTime startDate = new DateTime(_year, 1, 1);
            DateTime endDate = new DateTime(_year + 1, 1, 1);
            FilterDefinition<HealthHBA1C> filter = Builders<HealthHBA1C>.Filter.Eq(x => x.AccountId, accountId) &
                                                   Builders<HealthHBA1C>.Filter.Gte(x => x.Date, startDate) &
                                                   Builders<HealthHBA1C>.Filter.Lt(x => x.Date, endDate);
            return _collectionHealthHBA1C.Find(filter).ToList();
        }



        // MenstrualCycle (Chu kỳ kinh nguyệt)
        public HealthMenstrualCycle CreateHealthMenstrualCycle(HealthMenstrualCycle item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.HealthId = Guid.NewGuid().ToString();
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionHealthMenstrualCycle.InsertOne(item);
            return item;
        }

        public HealthMenstrualCycle RestoreHealthMenstrualCycle(HealthMenstrualCycle item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.HealthId = (!string.IsNullOrEmpty(item.HealthId) ? item.HealthId : Guid.NewGuid().ToString());
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionHealthMenstrualCycle.InsertOne(item);
            return item;
        }

        public HealthMenstrualCycle DeleteHealthMenstrualCycle(string healthId)
        {
            return _collectionHealthMenstrualCycle.FindOneAndDelete(item => item.HealthId == healthId);
        }

        public HealthMenstrualCycle UpdateHealthMenstrualCycle(HealthMenstrualCycle item)
        {
            HealthMenstrualCycle _item = _collectionHealthMenstrualCycle.Find(x => x.HealthId == item.HealthId).FirstOrDefault();
            if (_item == null) return null;

            var update = Builders<HealthMenstrualCycle>.Update
                .Set("HealthId", item.HealthId)
                .Set("AccountId", item.AccountId)
                .Set("Type", item.Type)
                .Set("Name", item.Name)

                .Set("Month1", item.Month1)
                .Set("Month2", item.Month2)
                .Set("Month3", item.Month3)
                .Set("Month4", item.Month4)
                .Set("Month5", item.Month5)
                .Set("Month6", item.Month6)
                .Set("Month7", item.Month7)
                .Set("Month8", item.Month8)
                .Set("Month9", item.Month9)
                .Set("Month10", item.Month10)
                .Set("Month11", item.Month11)
                .Set("Month12", item.Month12)

                .Set("DateYear", item.DateYear)

                .Set("Created", _item.Created)
                .Set("Updated", DateTimes.Now());

            return _collectionHealthMenstrualCycle.FindOneAndUpdate(x => x.Id == _item.Id, update);
        }

        [Obsolete]
        public long TotalHealthMenstrualCycle()
        {
            return _collectionHealthMenstrualCycle.Count(item => true);
        }

        public HealthMenstrualCycle FindByIdHealthMenstrualCycle(string healthId)
        {
            return _collectionHealthMenstrualCycle.Find(item => item.HealthId == healthId).FirstOrDefault();
        }

        public HealthMenstrualCycle ListHealthMenstrualCycle(int? year, int pageIndex = 0, int pageSize = 99)
        {
            int _year = year ?? DateTimes.Now().Year;
            DateTime startDate = new DateTime(_year, 1, 1);
            DateTime endDate = new DateTime(_year + 1, 1, 1);
            FilterDefinition<HealthMenstrualCycle> filter = Builders<HealthMenstrualCycle>.Filter.Gte(x => x.DateYear, startDate) &
                                                            Builders<HealthMenstrualCycle>.Filter.Lt(x => x.DateYear, endDate);
            return _collectionHealthMenstrualCycle.Find(filter).Skip(pageIndex * pageSize).Limit(pageSize).FirstOrDefault();
        }

        public HealthMenstrualCycle FindByIdAccountForHealthMenstrualCycle(int? year, string accountId)
        {
            int _year = year ?? DateTimes.Now().Year;
            DateTime startDate = new DateTime(_year, 1, 1);
            DateTime endDate = new DateTime(_year + 1, 1, 1);
            FilterDefinition<HealthMenstrualCycle> filter = Builders<HealthMenstrualCycle>.Filter.Eq(x => x.AccountId, accountId) &
                                                            Builders<HealthMenstrualCycle>.Filter.Gte(x => x.DateYear, startDate) &
                                                            Builders<HealthMenstrualCycle>.Filter.Lt(x => x.DateYear, endDate);
            return _collectionHealthMenstrualCycle.Find(filter).FirstOrDefault();
        }



        // SpO2 (Nồng độ O2 trong máu)
        public HealthSpO2 CreateHealthSpO2(HealthSpO2 item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.HealthId = Guid.NewGuid().ToString();
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionHealthSpO2.InsertOne(item);
            return item;
        }

        public HealthSpO2 RestoreHealthSpO2(HealthSpO2 item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.HealthId = (!string.IsNullOrEmpty(item.HealthId) ? item.HealthId : Guid.NewGuid().ToString());
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionHealthSpO2.InsertOne(item);
            return item;
        }

        public HealthSpO2 DeleteHealthSpO2(string healthId)
        {
            return _collectionHealthSpO2.FindOneAndDelete(item => item.HealthId == healthId);
        }

        public HealthSpO2 UpdateHealthSpO2(HealthSpO2 item)
        {
            HealthSpO2 _item = _collectionHealthSpO2.Find(x => x.HealthId == item.HealthId).FirstOrDefault();
            if (_item == null) return null;

            var update = Builders<HealthSpO2>.Update
                .Set("HealthId", item.HealthId)
                .Set("AccountId", item.AccountId)
                .Set("Type", item.Type)
                .Set("Name", item.Name)

                .Set("MinUnit", item.MinUnit)
                .Set("AverageUnit", item.AverageUnit)
                .Set("MaxUnit", item.MaxUnit)
                .Set("Unit", item.Unit)
                .Set("Date", item.Date)
                .Set("Time", item.Time)

                .Set("Created", _item.Created)
                .Set("Updated", DateTimes.Now());

            return _collectionHealthSpO2.FindOneAndUpdate(x => x.Id == _item.Id, update);
        }

        [Obsolete]
        public long TotalHealthSpO2()
        {
            return _collectionHealthSpO2.Count(item => true);
        }

        public HealthSpO2 FindByIdHealthSpO2(string healthId)
        {
            return _collectionHealthSpO2.Find(item => item.HealthId == healthId).FirstOrDefault();
        }

        public List<HealthSpO2> ListHealthSpO2(int? year, int pageIndex = 0, int pageSize = 99)
        {
            int _year = year ?? DateTimes.Now().Year;
            DateTime startDate = new DateTime(_year, 1, 1);
            DateTime endDate = new DateTime(_year + 1, 1, 1);
            FilterDefinition<HealthSpO2> filter = Builders<HealthSpO2>.Filter.Gte(x => x.Date, startDate) &
                                                  Builders<HealthSpO2>.Filter.Lt(x => x.Date, endDate);
            return _collectionHealthSpO2.Find(filter).Skip(pageIndex * pageSize).Limit(pageSize).ToList();
        }

        public List<HealthSpO2> FindByIdAccountForHealthSpO2(int? year, string accountId)
        {
            int _year = year ?? DateTimes.Now().Year;
            DateTime startDate = new DateTime(_year, 1, 1);
            DateTime endDate = new DateTime(_year + 1, 1, 1);
            FilterDefinition<HealthSpO2> filter = Builders<HealthSpO2>.Filter.Eq(x => x.AccountId, accountId) &
                                                  Builders<HealthSpO2>.Filter.Gte(x => x.Date, startDate) &
                                                  Builders<HealthSpO2>.Filter.Lt(x => x.Date, endDate);
            return _collectionHealthSpO2.Find(filter).ToList();
        }



        // Weight (Cân nặng)
        public HealthWeight CreateHealthWeight(HealthWeight item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.HealthId = Guid.NewGuid().ToString();
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionHealthWeight.InsertOne(item);
            return item;
        }

        public HealthWeight RestoreHealthWeight(HealthWeight item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.HealthId = (!string.IsNullOrEmpty(item.HealthId) ? item.HealthId : Guid.NewGuid().ToString());
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionHealthWeight.InsertOne(item);
            return item;
        }

        public HealthWeight DeleteHealthWeight(string healthId)
        {
            return _collectionHealthWeight.FindOneAndDelete(item => item.HealthId == healthId);
        }

        public HealthWeight UpdateHealthWeight(HealthWeight item)
        {
            HealthWeight _item = _collectionHealthWeight.Find(x => x.HealthId == item.HealthId).FirstOrDefault();
            if (_item == null) return null;

            var update = Builders<HealthWeight>.Update
                .Set("HealthId", item.HealthId)
                .Set("AccountId", item.AccountId)
                .Set("Type", item.Type)
                .Set("Name", item.Name)

                .Set("MinUnit", item.MinUnit)
                .Set("AverageUnit", item.AverageUnit)
                .Set("MaxUnit", item.MaxUnit)
                .Set("Unit", item.Unit)
                .Set("Date", item.Date)
                .Set("Time", item.Time)

                .Set("Created", _item.Created)
                .Set("Updated", DateTimes.Now());

            return _collectionHealthWeight.FindOneAndUpdate(x => x.Id == _item.Id, update);
        }

        [Obsolete]
        public long TotalHealthWeight()
        {
            return _collectionHealthWeight.Count(item => true);
        }

        public HealthWeight FindByIdHealthWeight(string healthId)
        {
            return _collectionHealthWeight.Find(item => item.HealthId == healthId).FirstOrDefault();
        }

        public List<HealthWeight> ListHealthWeight(int? year, int pageIndex = 0, int pageSize = 99)
        {
            int _year = year ?? DateTimes.Now().Year;
            DateTime startDate = new DateTime(_year, 1, 1);
            DateTime endDate = new DateTime(_year + 1, 1, 1);
            FilterDefinition<HealthWeight> filter = Builders<HealthWeight>.Filter.Gte(x => x.Date, startDate) &
                                                    Builders<HealthWeight>.Filter.Lt(x => x.Date, endDate);
            return _collectionHealthWeight.Find(filter).Skip(pageIndex * pageSize).Limit(pageSize).ToList();
        }

        public List<HealthWeight> FindByIdAccountForHealthWeight(int? year, string accountId)
        {
            int _year = year ?? DateTimes.Now().Year;
            DateTime startDate = new DateTime(_year, 1, 1);
            DateTime endDate = new DateTime(_year + 1, 1, 1);
            FilterDefinition<HealthWeight> filter = Builders<HealthWeight>.Filter.Eq(x => x.AccountId, accountId) &
                                                    Builders<HealthWeight>.Filter.Gte(x => x.Date, startDate) &
                                                    Builders<HealthWeight>.Filter.Lt(x => x.Date, endDate);
            return _collectionHealthWeight.Find(filter).ToList();
        }

        // ...

    }

}
