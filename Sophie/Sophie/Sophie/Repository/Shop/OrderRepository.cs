using System;
using System.Collections.Generic;
using System.Linq;
using App.SharedLib.Repository;
using MongoDB.Bson;
using MongoDB.Driver;
using Sophie.Repository.Interface;
using Sophie.Resource.Dtos.Shop;
using Sophie.Resource.Entities.Shop;
using Sophie.Resource.Model;
using Sophie.Units;

namespace Sophie.Repository
{
    public class OrderRepository : BaseRepository, IOrderRepository
    {
        private readonly IMongoCollection<Order> _collectionOrder;

        public OrderRepository() : base()
        {
            _collectionOrder = _database.GetCollection<Order>($"Order");

            var indexOptions = new CreateIndexOptions();
            var indexKeys_1 = Builders<Order>.IndexKeys.Ascending(item => item.OrderId);
            var indexModelOrder = new List<CreateIndexModel<Order>>();
            indexModelOrder.Add(new CreateIndexModel<Order>(indexKeys_1, indexOptions));
            _collectionOrder.Indexes.CreateMany(indexModelOrder);
        }

        public Order CreateOrder(Order item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.OrderId = Guid.NewGuid().ToString();
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();
            _collectionOrder.InsertOne(item);
            return item;
        }

        public List<Order> CreateListOrder(List<Order> orders)
        {
            foreach (var order in orders)
            {
                ObjectId objectId = ObjectId.GenerateNewId();
                order.Id = new BsonObjectId(objectId).ToString();
                order.OrderId = Guid.NewGuid().ToString();
                order.Created = DateTimes.Now();
                order.Updated = DateTimes.Now();
            }
            _collectionOrder.InsertMany(orders);
            return orders;
        }

        public bool DeleteSameTransactionId(string transactionId, string accountId)
        {
            _collectionOrder.DeleteMany(x => x.TransactionId == transactionId && x.AccountId == accountId && x.TypeStatusOrder != TypeStatusOrder.Successed);
            return true;
        }

        public Order DeleteOrder(string orderId)
        {
            return _collectionOrder.FindOneAndDelete(item => item.OrderId == orderId);
        }

        public Order FindByIdOrder(string orderId)
        {
            return _collectionOrder.Find(item => item.OrderId == orderId).FirstOrDefault();
        }

        public PagingResult<OrderGroupBy> FindByAccountId(FilterWithId filter)
        {
            PagingResult<OrderGroupBy> result = new PagingResult<OrderGroupBy>();
            var filterBuilders = Builders<OrderGroupBy>.Filter.Where(x => x.AccountId == filter.Id);
            var query = _collectionOrder.Aggregate().Sort("{Created: -1}").Group(BsonDocument.Parse("{ _id: '$TransactionId', Items: { '$push': '$$ROOT'}}"))
    .Project<OrderGroupBy>(BsonDocument.Parse("{ _id: 0, TransactionId: '$_id', AccountId: { $max: '$Items.AccountId' } ,  ListOrder: '$Items'}")).Match(filterBuilders);
            result.Total = query.ToList().Count;
            result.Result = query.Skip((int)filter.PageIndex * (int)filter.PageSize).Limit((int)filter.PageSize).ToList();
            return result;
        }



        public Order FindExitsByShopId(string shopId)
        {
            return _collectionOrder.Find(item => item.ShopId == shopId).FirstOrDefault();
        }

        public PagingResult<OrderGroupBy> ListOrder(Paging paging)
        {
            PagingResult<OrderGroupBy> result = new PagingResult<OrderGroupBy>();
            var filter = Builders<OrderGroupBy>.Filter.Where(p => true);
            if (!string.IsNullOrEmpty(paging.search))
            {
                filter = Builders<OrderGroupBy>.Filter.Regex(x => x.TransactionId, new BsonRegularExpression(paging.search, "i"));
            }
            var query = _collectionOrder.Aggregate().Sort("{Created: -1}").Group(BsonDocument.Parse("{ _id: '$TransactionId', Items: { '$push': '$$ROOT'}}"))
   .Project<OrderGroupBy>(BsonDocument.Parse("{ _id: 0, TransactionId: '$_id', AccountId: { $max: '$Items.AccountId' },TypeStatusOrder :{ $max: '$Items.TypeStatusOrder' },Created :{ $max: '$Items.Created' },  ListOrder: '$Items'}")).Match(filter);
            result.Total = query.ToList().Count;
            result.Result = query.Sort($"{{{paging.sortName}: {(paging.sort == "asc" ? 1 : -1)}}}").Skip((int)paging.PageIndex * (int)paging.PageSize).Limit((int)paging.PageSize).ToList();
            return result;
        }

        public List<Order> FindAll(string pharmacistId)
        {
            return _collectionOrder.Find(item => item.PharmacistId == pharmacistId).SortByDescending(item => item.Updated).ToList();
        }

        public Order UpdateOrder(Order item)
        {
            Order _item = _collectionOrder.Find(x => x.OrderId == item.OrderId).FirstOrDefault();
            if (_item == null) return null;

            var update = Builders<Order>.Update
                .Set("Type", item.Type)
                .Set("Created", _item.Created)
                .Set("Updated", DateTimes.Now());
            return _collectionOrder.FindOneAndUpdate(x => x.Id == _item.Id, update);
        }

        public bool UpdateListOrder(List<Order> listOrder, TypeStatusOrder typeStatusOrder)
        {
            try
            {
                var a = _collectionOrder.UpdateMany(x =>
                        x.TransactionId == listOrder[0].TransactionId,
                         Builders<Order>.Update.Set(p => p.TypeStatusOrder, typeStatusOrder),
                         new UpdateOptions { IsUpsert = false }
                     );
                return true;
            }
            catch
            {
                return false;
            }
        }

        [Obsolete]
        public long TotalOrder()
        {
            return _collectionOrder.Count(item => true);
        }

        public List<Order> FindByIdTransactionId(string transactionId)
        {
            return _collectionOrder.Find(item => item.TransactionId == transactionId).ToList();
        }

    }
}
