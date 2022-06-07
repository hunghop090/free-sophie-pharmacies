using System;
using System.Collections.Generic;
using System.Linq;
using App.SharedLib.Repository;
using MongoDB.Bson;
using MongoDB.Driver;
using Sophie.Repository.Interface;
using Sophie.Resource.Entities.Shop;
using Sophie.Resource.Model;
using Sophie.Units;

namespace Sophie.Repository
{
    public class TransportPromotionRepository : BaseRepository, ITransportPromotionRepository
    {
        private readonly IMongoCollection<TransportPromotion> _collectionTransportPromotion;

        public TransportPromotionRepository() : base()
        {
            _collectionTransportPromotion = _database.GetCollection<TransportPromotion>($"TransportPromotion");

            var indexOptions = new CreateIndexOptions();
            var indexKeys_1 = Builders<TransportPromotion>.IndexKeys.Ascending(item => item.TransportPromotionId);
            var indexModelTransportPromotion = new List<CreateIndexModel<TransportPromotion>>();
            indexModelTransportPromotion.Add(new CreateIndexModel<TransportPromotion>(indexKeys_1, indexOptions));
            _collectionTransportPromotion.Indexes.CreateMany(indexModelTransportPromotion);
        }

        public TransportPromotion CreateTransportPromotion(TransportPromotion item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.TransportPromotionId = Guid.NewGuid().ToString();
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();
            _collectionTransportPromotion.InsertOne(item);
            return item;
        }

        public TransportPromotion DeleteTransportPromotion(string transportPromotionId)
        {
            return _collectionTransportPromotion.FindOneAndDelete(item => item.TransportPromotionId == transportPromotionId);
        }

        public TransportPromotion FindByIdTransportPromotion(string transportPromotionId)
        {
            return _collectionTransportPromotion.Find(x => x.TransportPromotionId == transportPromotionId).FirstOrDefault();
        }

        public List<TransportPromotion> FindByTransportPromotionIds(TypePay? typePay, List<string> transportPromotionIds)
        {
            var filter = Builders<TransportPromotion>.Filter.And(Builders<TransportPromotion>.Filter.In(x => x.TransportPromotionId, transportPromotionIds),
                            Builders<TransportPromotion>.Filter.Where(p => p.StartDate < DateTime.Now),
                            Builders<TransportPromotion>.Filter.Where(p => p.EndDate > DateTime.Now),
                            Builders<TransportPromotion>.Filter.Where(p => typePay == null || p.TypePay == typePay || (p.TypePay == TypePay.ZaloMomo && (typePay == TypePay.Zalo || typePay == TypePay.Momo))),
                            Builders<TransportPromotion>.Filter.Where(p => p.Type == TypeEnum.Actived));
            return _collectionTransportPromotion.Find(filter).ToEnumerable().Where(x => x.QuantityUsed < x.TransportPromotionQuantity).ToList();
        }

        public PagingResult<TransportPromotion> ListTransportPromotion(Paging paging)
        {
            PagingResult<TransportPromotion> result = new PagingResult<TransportPromotion>();
            var filter = Builders<TransportPromotion>.Filter.Or(Builders<TransportPromotion>.Filter.Regex(x => x.TransportPromotionCode, new BsonRegularExpression(paging.search, "i")),
                Builders<TransportPromotion>.Filter.Regex(x => x.TransportPromotionName, new BsonRegularExpression(paging.search, "i")));
            var query = _collectionTransportPromotion.Find(filter);
            result.Total = query.ToList().Count;
            result.Result = query.Sort($"{{{paging.sortName}: {(paging.sort == "asc" ? 1 : -1)}}}").Skip(paging.PageIndex * paging.PageSize).Limit(paging.PageSize).ToList();
            return result;
        }
        public PagingResult<TransportPromotion> ListTransportPromotionByShopIds(FilterWithId filter)
        {
            string[] listId = { };
            if (!string.IsNullOrEmpty(filter.Id))
            {
                listId = filter.Id.Split(",");
            }
            PagingResult<TransportPromotion> result = new PagingResult<TransportPromotion>();
            var filterBuilders = Builders<TransportPromotion>.Filter.And(
                                        Builders<TransportPromotion>.Filter.Where(p => p.StartDate < DateTime.Now),
                                        Builders<TransportPromotion>.Filter.Where(p => p.EndDate > DateTime.Now),
                                        Builders<TransportPromotion>.Filter.Where(p => p.Type == TypeEnum.Actived),
                                        Builders<TransportPromotion>.Filter.Or(
                                        Builders<TransportPromotion>.Filter.Where(x => string.IsNullOrEmpty(filter.Id)),
                                        Builders<TransportPromotion>.Filter.In(x => x.ShopId, listId)
                                         )
                                        );
            var query = _collectionTransportPromotion.Find(filterBuilders);
            result.Total = query.ToList().Count;
            result.Result = query.Sort("{Created:  1 }").Skip(filter.PageIndex * filter.PageSize).Limit(filter.PageSize).ToList();
            return result;
        }

        public List<TransportPromotion> FindAll()
        {
            return _collectionTransportPromotion.Find(item => true).SortByDescending(item => item.Updated).ToList();
        }

        public TransportPromotion UpdateTransportPromotion(TransportPromotion item)
        {
            TransportPromotion _item = _collectionTransportPromotion.Find(x => x.TransportPromotionId == item.TransportPromotionId).FirstOrDefault();
            if (_item == null) return null;

            var update = Builders<TransportPromotion>.Update
                .Set("TransportPromotionName", item.TransportPromotionName)
                .Set("TransportPromotionCode", item.TransportPromotionCode)
                .Set("Title", item.Title)
                .Set("StartDate", item.StartDate)
                .Set("EndDate", item.EndDate)
                .Set("TransportPromotionQuantity", item.TransportPromotionQuantity)
                .Set("MinBuget", item.MinBuget)
                .Set("Price", item.Price)
                .Set("Discount", item.Discount)
                .Set("MaxPriceDiscount", item.MaxPriceDiscount)
                .Set("Type", item.Type)
                .Set("TypePay", item.TypePay)
                .Set("TypeDiscount", item.TypeDiscount)
                .Set("Created", _item.Created)
                .Set("Updated", DateTimes.Now());
            return _collectionTransportPromotion.FindOneAndUpdate(x => x.Id == _item.Id, update);
        }

        [Obsolete]
        public long TotalTransportPromotion()
        {
            return _collectionTransportPromotion.Count(item => true);
        }

        public TransportPromotion FindByCode(string transportPromotionCode)
        {
            return _collectionTransportPromotion.Find(item => item.TransportPromotionCode == transportPromotionCode).FirstOrDefault();
        }

        public PagingResult<TransportPromotion> ListPromotionActive(Paging paging)
        {
            PagingResult<TransportPromotion> result = new PagingResult<TransportPromotion>();
            var filter = Builders<TransportPromotion>.Filter.And(
                            Builders<TransportPromotion>.Filter.Where(p => p.StartDate < DateTime.Now),
                            Builders<TransportPromotion>.Filter.Where(p => p.EndDate > DateTime.Now),
                            Builders<TransportPromotion>.Filter.Where(p => p.Type == TypeEnum.Actived),
                            Builders<TransportPromotion>.Filter.Regex(x => x.TransportPromotionName, new BsonRegularExpression(paging.search, "i"))
                            );
            var query = _collectionTransportPromotion.Find(filter);
            result.Total = query.ToList().Count;
            result.Result = query.Sort("{Created:  1 }").Skip(paging.PageIndex * paging.PageSize).Limit(paging.PageSize).ToList();
            return result;
        }

        public void UpdateUsed(List<string> listPromotion)
        {
            foreach (var promotion in listPromotion)
            {
                var oldPromotion = _collectionTransportPromotion.Find(item => item.TransportPromotionId == promotion).FirstOrDefault();
                var update = Builders<TransportPromotion>.Update.Set("QuantityUsed", ++oldPromotion.QuantityUsed);
                _collectionTransportPromotion.FindOneAndUpdate(x => x.TransportPromotionId == promotion, update);
            }
        }

    }
}
