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
    public class ConfigurationsRepository : BaseRepository, IConfigurationsRepository
    {
        private readonly ILog _log4net = log4net.LogManager.GetLogger(typeof(ConfigurationsRepository));
        private readonly LogMongoService _logMongoService;

        private readonly IMongoCollection<Configurations> _collectionConfigurations;

        public ConfigurationsRepository(LogMongoService logMongoService) : base()
        {
            _logMongoService = logMongoService;
            _collectionConfigurations = _database.GetCollection<Configurations>($"Configurations");

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

        public Configurations CreateConfigurations(Configurations item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.ConfigurationsId = Guid.NewGuid().ToString();
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionConfigurations.InsertOne(item);
            return item;
        }

        public Configurations RestoreConfigurations(Configurations item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.ConfigurationsId = (!string.IsNullOrEmpty(item.ConfigurationsId) ? item.ConfigurationsId : Guid.NewGuid().ToString());
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionConfigurations.InsertOne(item);
            return item;
        }

        public Configurations DeleteConfigurations(string configurationsId)
        {
            return _collectionConfigurations.FindOneAndDelete(item => item.ConfigurationsId == configurationsId);
        }

        public List<Configurations> ListConfigurations(int pageIndex = 0, int pageSize = 99)
        {
            return _collectionConfigurations.Find(item => true).Skip(pageIndex * pageSize).Limit(pageSize).ToList();
        }

        public Configurations UpdateConfigurations(Configurations item)
        {
            Configurations _item = _collectionConfigurations.Find(x => x.ConfigurationsId == item.ConfigurationsId).FirstOrDefault();
            if (_item == null) return null;

            var update = Builders<Configurations>.Update
                .Set("ConfigurationsId", item.ConfigurationsId)
                .Set("Settings", item.Settings)

                .Set("Created", _item.Created)
                .Set("Updated", DateTimes.Now());
            
            return _collectionConfigurations.FindOneAndUpdate(x => x.Id == _item.Id, update);
        }
    }
}
