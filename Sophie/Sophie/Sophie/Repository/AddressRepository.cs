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
    public class AddressRepository: BaseRepository, IAddressRepository
    {
        private readonly ILog _log4net = log4net.LogManager.GetLogger(typeof(AddressRepository));
        private readonly LogMongoService _logMongoService;

        private readonly IMongoCollection<Address> _collectionAddress;

        public AddressRepository(LogMongoService logMongoService) : base()
        {
            _logMongoService = logMongoService;
            _collectionAddress = _database.GetCollection<Address>($"Address");

            var indexOptions = new CreateIndexOptions();
            var indexKeys_1 = Builders<Address>.IndexKeys.Ascending(item => item.AddressId);
            var indexKeys_2 = Builders<Address>.IndexKeys.Ascending(item => item.AccountId);
            var indexModelAddress = new List<CreateIndexModel<Address>>();
            indexModelAddress.Add(new CreateIndexModel<Address>(indexKeys_1, indexOptions));
            indexModelAddress.Add(new CreateIndexModel<Address>(indexKeys_2, indexOptions));
            _collectionAddress.Indexes.CreateMany(indexModelAddress);
        }

        public Address CreateAddress(Address item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.AddressId = Guid.NewGuid().ToString();
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionAddress.InsertOne(item);
            return item;
        }

        public Address RestoreAddress(Address item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.AddressId = (!string.IsNullOrEmpty(item.AddressId) ? item.AddressId : Guid.NewGuid().ToString());
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionAddress.InsertOne(item);
            return item;
        }

        public Address DeleteAddress(string addressId)
        {
            return _collectionAddress.FindOneAndDelete(item => item.AddressId == addressId);
        }

        public List<Address> ListAddress(int pageIndex = 0, int pageSize = 99)
        {
            return _collectionAddress.Find(item => true).Skip(pageIndex * pageSize).Limit(pageSize).ToList();
        }

        public Address UpdateAddress(Address item)
        {
            Address _item = _collectionAddress.Find(x => x.AddressId == item.AddressId).FirstOrDefault();
            if (_item == null) return null;

            var update = Builders<Address>.Update
                .Set("AddressId", item.AddressId)
                .Set("AccountId", item.AccountId)
                .Set("IsDefault", item.IsDefault)
                .Set("Title", item.Title)
                .Set("FullName", item.FullName)
                .Set("Phone", item.Phone)

                .Set("Province", item.Province)
                .Set("City", item.City)
                .Set("District", item.District)
                .Set("Wards", item.Wards)
                .Set("AddressAccount", item.AddressAccount)
                .Set("TypeAddress", item.TypeAddress)

                .Set("Created", _item.Created)
                .Set("Updated", DateTimes.Now());

            return _collectionAddress.FindOneAndUpdate(x => x.Id == _item.Id, update);
        }

        [Obsolete]
        public long TotalAddress()
        {
            return _collectionAddress.Count(item => true);
        }

        public Address FindByIdAddress(string addressId)
        {
            return _collectionAddress.Find(item => item.AddressId == addressId).FirstOrDefault();
        }

        public List<Address> FindByIdAccount(string accountId)
        {
            return _collectionAddress.Find(item => item.AccountId == accountId).ToList();
        }

    }
}
