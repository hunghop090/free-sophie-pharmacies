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
    public class InforRepository : BaseRepository, IInforRepository
    {
        private readonly ILog _log4net = log4net.LogManager.GetLogger(typeof(AddressRepository));
        private readonly LogMongoService _logMongoService;

        private readonly IMongoCollection<Info> _collectionInfo;

        public InforRepository(LogMongoService logMongoService) : base()
        {
            _logMongoService = logMongoService;
            _collectionInfo = _database.GetCollection<Info>($"Info");

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

        public Info CreateInfo(Info item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.InfoId = Guid.NewGuid().ToString();
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionInfo.InsertOne(item);
            return item;
        }

        public Info RestoreInfo(Info item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.InfoId = (!string.IsNullOrEmpty(item.InfoId) ? item.InfoId : Guid.NewGuid().ToString());
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionInfo.InsertOne(item);
            return item;
        }

        public Info DeleteInfo(string infoId)
        {
            return _collectionInfo.FindOneAndDelete(item => item.InfoId == infoId);
        }

        public List<Info> ListInfo(int pageIndex = 0, int pageSize = 99)
        {
            return _collectionInfo.Find(item => true).Skip(pageIndex * pageSize).Limit(pageSize).ToList();
        }

        public Info UpdateInfo(Info item)
        {
            Info _item = _collectionInfo.Find(x => x.InfoId == item.InfoId).FirstOrDefault();
            if (_item == null) return null;

            var update = Builders<Info>.Update
                .Set("InfoId", item.InfoId)
                .Set("AccountId", item.AccountId)
                .Set("Age", item.Age)
                .Set("Height", item.Height)
                .Set("Weight", item.Weight)
                .Set("WeightTarget", item.WeightTarget)
                .Set("BloodGroup", item.BloodGroup)
                .Set("ContactEmail", item.ContactEmail)
                .Set("Points", _item.Points ?? 0)
                .Set("Created", _item.Created)
                .Set("Updated", DateTimes.Now());
            
            return _collectionInfo.FindOneAndUpdate(x => x.Id == _item.Id, update);
        }

        [Obsolete]
        public long TotalInfo()
        {
            return _collectionInfo.Count(item => true);
        }

        public Info FindByIdInfo(string infoId)
        {
            return _collectionInfo.Find(item => item.InfoId == infoId).FirstOrDefault();
        }

        public Info FindByIdAccount(string accountId)
        {
            return _collectionInfo.Find(item => item.AccountId == accountId).FirstOrDefault();
        }
    }

}
