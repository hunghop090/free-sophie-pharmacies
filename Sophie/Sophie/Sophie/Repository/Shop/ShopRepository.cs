using System;
using System.Collections.Generic;
using App.SharedLib.Repository;
using MongoDB.Bson;
using MongoDB.Driver;
using Sophie.Repository.Interface;
using Sophie.Resource.Entities.Shop;
using Sophie.Resource.Model;
using Sophie.Units;

namespace Sophie.Repository
{
    public class ShopRepository : BaseRepository, IShopRepository
    {
        private readonly IMongoCollection<Shop> _collectionShop;

        public ShopRepository() : base()
        {
            _collectionShop = _database.GetCollection<Shop>($"Shop");

            var indexOptions = new CreateIndexOptions();
            var indexKeys_1 = Builders<Shop>.IndexKeys.Ascending(item => item.ShopId);
            var indexModelShop = new List<CreateIndexModel<Shop>>();
            indexModelShop.Add(new CreateIndexModel<Shop>(indexKeys_1, indexOptions));
            _collectionShop.Indexes.CreateMany(indexModelShop);
        }

        public Shop CreateShop(Shop item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.ShopId = Guid.NewGuid().ToString();
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();
            _collectionShop.InsertOne(item);
            return item;
        }

        public Shop DeleteShop(string shopId)
        {
            return _collectionShop.FindOneAndDelete(item => item.ShopId == shopId);
        }

        public Shop FindByIdShop(string shopId)
        {
            return _collectionShop.Find(item => item.ShopId == shopId).FirstOrDefault();
        }

        public List<Shop> FindByIdShops(string shopIds)
        {
            var ListIds = shopIds.Split(',');
            var filter = Builders<Shop>.Filter.In(x => x.ShopId, ListIds);
            return _collectionShop.Find(filter).ToList();
        }

        public Shop FindByIdPharmacist(string pharmacistId)
        {
            return _collectionShop.Find(item => item.PharmacistId == pharmacistId).FirstOrDefault();
        }

        public PagingResult<Shop> ListShop(Paging paging)
        {
            PagingResult<Shop> result = new PagingResult<Shop>();
            var filter = Builders<Shop>.Filter.Regex(x => x.ShopName, new BsonRegularExpression(paging.search, "i"));
            var query = _collectionShop.Find(filter);
            result.Total = query.ToList().Count;
            result.Result = query.Sort($"{{{paging.sortName}: {(paging.sort == "asc" ? 1 : -1)}}}").Skip(paging.PageIndex * paging.PageSize).Limit(paging.PageSize).ToList();
            return result;
        }

        public List<Shop> FindAll()
        {
            return _collectionShop.Find(item => true).SortByDescending(item => item.Updated).ToList();
        }

        public Shop UpdateShop(Shop item)
        {
            Shop _item = _collectionShop.Find(x => x.ShopId == item.ShopId).FirstOrDefault();
            if (_item == null) return null;

            var update = Builders<Shop>.Update
                .Set("ShopName", item.ShopName)
                .Set("ShopIcon", item.ShopImage)
                .Set("Description", item.Description)
                .Set("ShopAddress", item.ShopAddress)
                .Set("TransportPrice", item.TransportPrice)
                .Set("Type", item.Type)
                .Set("Created", _item.Created)
                .Set("Updated", DateTimes.Now());
            return _collectionShop.FindOneAndUpdate(x => x.Id == _item.Id, update);
        }

        [Obsolete]
        public long TotalShop()
        {
            return _collectionShop.Count(item => true);
        }

    }
}
