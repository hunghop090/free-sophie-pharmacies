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
    public class DraftOrderRepository : BaseRepository, IDraftOrderRepository
    {
        private readonly IMongoCollection<DraftOrder> _collectionDraftOrder;

        public DraftOrderRepository() : base()
        {
            _collectionDraftOrder = _database.GetCollection<DraftOrder>($"DraftOrder");

            var indexOptions = new CreateIndexOptions();
            var indexKeys_1 = Builders<DraftOrder>.IndexKeys.Ascending(item => item.DraftOrderId);
            var indexModelDraftOrder = new List<CreateIndexModel<DraftOrder>>();
            indexModelDraftOrder.Add(new CreateIndexModel<DraftOrder>(indexKeys_1, indexOptions));
            _collectionDraftOrder.Indexes.CreateMany(indexModelDraftOrder);
        }

        public DraftOrder CreateDraftOrder(DraftOrder item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.DraftOrderId = Guid.NewGuid().ToString();
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();
            _collectionDraftOrder.InsertOne(item);
            return item;
        }

        public DraftOrder DeleteDraftOrder(string draftOrderId)
        {
            return _collectionDraftOrder.FindOneAndDelete(item => item.DraftOrderId == draftOrderId);
        }

        public DraftOrder FindByIdDraftOrder(string draftOrderId)
        {
            return _collectionDraftOrder.Find(item => item.DraftOrderId == draftOrderId).FirstOrDefault();
        }

        public DraftOrder FindByTransactionId(string transactionId)
        {
            return _collectionDraftOrder.Find(item => item.TransactionId == transactionId).FirstOrDefault();
        }

        public DraftOrder FindByAccountId(string accountId)
        {
            return _collectionDraftOrder.Find(item => item.AccountId == accountId).FirstOrDefault();
        }

        public PagingResult<DraftOrder> ListDraftOrder(Paging paging)
        {
            PagingResult<DraftOrder> result = new PagingResult<DraftOrder>();
            var filter = Builders<DraftOrder>.Filter.Where(p => true);
            if (!string.IsNullOrEmpty(paging.search))
            {
                filter = Builders<DraftOrder>.Filter.Regex(x => x.AccountName, new BsonRegularExpression(paging.search, "i"));
            }
            var query = _collectionDraftOrder.Find(filter);
            result.Total = query.ToList().Count;
            result.Result = query.Sort($"{{{paging.sortName}: {(paging.sort == "asc" ? 1 : -1)}}}").Skip(paging.PageIndex * paging.PageSize).Limit(paging.PageSize).ToList();
            return result;
        }

        public List<DraftOrder> FindAll(string pharmacistId)
        {
            return _collectionDraftOrder.Find(item => item.PharmacistId == pharmacistId).SortByDescending(item => item.Updated).ToList();
        }

        public DraftOrder UpdateDraftOrder(DraftOrder item)
        {
            var update = Builders<DraftOrder>.Update
                .Set("DraftOrderId", item.DraftOrderId)
                .Set("AccountId", item.AccountId)
                .Set("AccountName", item.AccountName)
                .Set("AddressId", item.AddressId)
                .Set("AddressAccount", item.AddressAccount)
                .Set("ListProduct", item.ListProduct)
                .Set("Price", item.Price)
                .Set("PromotionPrice", item.PromotionPrice)
                .Set("PromotionPrice", item.PromotionPrice)
                .Set("TransportPromotionPrice", item.TransportPromotionPrice)
                .Set("PromotionIds", item.PromotionIds)
                .Set("TransportPromotionIds", item.TransportPromotionIds)
                .Set("Type", item.Type)
                .Set("Updated", DateTimes.Now());
            return _collectionDraftOrder.FindOneAndUpdate(x => x.DraftOrderId == item.DraftOrderId, update);
        }

        public DraftOrder UpdateDraftOrderById(DraftOrder item)
        {
            var update = Builders<DraftOrder>.Update
                .Set("DraftOrderId", item.DraftOrderId)
                .Set("AccountId", item.AccountId)
                .Set("AccountName", item.AccountName)
                .Set("AddressId", item.AddressId)
                .Set("AddressAccount", item.AddressAccount)
                .Set("ListProduct", item.ListProduct)
                .Set("Price", item.Price)
                .Set("PromotionPrice", item.PromotionPrice)
                .Set("PromotionPrice", item.PromotionPrice)
                .Set("TransportPromotionPrice", item.TransportPromotionPrice)
                .Set("PromotionIds", item.PromotionIds)
                .Set("TransportPromotionIds", item.TransportPromotionIds)
                .Set("Type", item.Type)
                .Set("Updated", DateTimes.Now());
            return _collectionDraftOrder.FindOneAndUpdate(x => x.Id == item.Id, update);
        }

        public DraftOrder UpdateTransactionId(string draftOrderId, string transactionId)
        {
            var update = Builders<DraftOrder>.Update
                .Set("TransactionId", transactionId)
                .Set("Updated", DateTimes.Now());
            return _collectionDraftOrder.FindOneAndUpdate(x => x.DraftOrderId == draftOrderId, update);
        }



        [Obsolete]
        public long TotalDraftOrder()
        {
            return _collectionDraftOrder.Count(item => true);
        }

    }
}
