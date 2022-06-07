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
    public class PromotionRepository : BaseRepository, IPromotionRepository
    {
        private readonly IMongoCollection<Promotion> _collectionPromotion;

        public PromotionRepository() : base()
        {
            _collectionPromotion = _database.GetCollection<Promotion>($"Promotion");

            var indexOptions = new CreateIndexOptions();
            var indexKeys_1 = Builders<Promotion>.IndexKeys.Ascending(item => item.PromotionId);
            var indexModelPromotion = new List<CreateIndexModel<Promotion>>();
            indexModelPromotion.Add(new CreateIndexModel<Promotion>(indexKeys_1, indexOptions));
            _collectionPromotion.Indexes.CreateMany(indexModelPromotion);
        }

        public Promotion CreatePromotion(Promotion item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.PromotionId = Guid.NewGuid().ToString();
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();
            _collectionPromotion.InsertOne(item);
            return item;
        }

        public Promotion DeletePromotion(string promotionId)
        {
            return _collectionPromotion.FindOneAndDelete(item => item.PromotionId == promotionId);
        }

        public Promotion FindByIdPromotion(string promotionId)
        {
            return _collectionPromotion.Find(item => item.PromotionId == promotionId).FirstOrDefault();
        }

        public PagingResult<Promotion> ListPromotion(Paging paging)
        {
            PagingResult<Promotion> result = new PagingResult<Promotion>();
            var filter = Builders<Promotion>.Filter.Or(Builders<Promotion>.Filter.Regex(x => x.PromotionName, new BsonRegularExpression(paging.search, "i")),
                Builders<Promotion>.Filter.Regex(x => x.PromotionCode, new BsonRegularExpression(paging.search, "i")));
            var query = _collectionPromotion.Find(filter);
            result.Total = query.ToList().Count;
            result.Result = query.Sort($"{{{paging.sortName}: {(paging.sort == "asc" ? 1 : -1)}}}").Skip(paging.PageIndex * paging.PageSize).Limit(paging.PageSize).ToList();
            return result;
        }

        public PagingResult<Promotion> ListPromotionActive(Paging paging)
        {
            PagingResult<Promotion> result = new PagingResult<Promotion>();
            var filter = Builders<Promotion>.Filter.And(
                            Builders<Promotion>.Filter.Where(p => p.StartDate < DateTime.Now),
                            Builders<Promotion>.Filter.Where(p => p.EndDate > DateTime.Now),
                            Builders<Promotion>.Filter.Where(p => p.Type == TypeEnum.Actived),
                            Builders<Promotion>.Filter.Regex(x => x.PromotionName, new BsonRegularExpression(paging.search, "i"))
                            );
            var query = _collectionPromotion.Find(filter);
            result.Total = query.ToList().Count;
            result.Result = query.Sort("{Created:  1 }").Skip(paging.PageIndex * paging.PageSize).Limit(paging.PageSize).ToList();
            return result;
        }

        public PagingResult<Promotion> ListPromotionByShopIds(FilterWithId filter)
        {
            string[] listId = { };
            if (!string.IsNullOrEmpty(filter.Id))
            {
                listId = filter.Id.Split(",");
            }
            PagingResult<Promotion> result = new PagingResult<Promotion>();
            var filterBuilders = Builders<Promotion>.Filter.And(
                                        Builders<Promotion>.Filter.Where(p => p.StartDate < DateTime.Now),
                                        Builders<Promotion>.Filter.Where(p => p.EndDate > DateTime.Now),
                                        Builders<Promotion>.Filter.Where(p => p.Type == TypeEnum.Actived),
                                        Builders<Promotion>.Filter.Or(
                                             Builders<Promotion>.Filter.Where(x => string.IsNullOrEmpty(filter.Id)),
                                             Builders<Promotion>.Filter.In(x => x.ShopId, listId)
                                         )
                                        );
            var query = _collectionPromotion.Find(filterBuilders);
            result.Total = query.ToList().Count;
            result.Result = query.Sort("{Created:  1 }").Skip(filter.PageIndex * filter.PageSize).Limit(filter.PageSize).ToList();
            return result;
        }

        public List<Promotion> FindAll()
        {
            return _collectionPromotion.Find(item => true).SortByDescending(item => item.Updated).ToList();
        }

        public Promotion UpdatePromotion(Promotion item)
        {
            Promotion _item = _collectionPromotion.Find(x => x.PromotionId == item.PromotionId).FirstOrDefault();
            if (_item == null) return null;

            var update = Builders<Promotion>.Update
                .Set("PromotionName", item.PromotionName)
                .Set("PromotionCode", item.PromotionCode)
                .Set("Title", item.Title)
                .Set("StartDate", item.StartDate)
                .Set("EndDate", item.EndDate)
                .Set("Banner", item.Banner)
                .Set("Content", item.Content)
                .Set("PromotionQuantity", item.PromotionQuantity)
                .Set("MinBuget", item.MinBuget)
                .Set("Price", item.Price)
                .Set("Discount", item.Discount)
                .Set("MaxPriceDiscount", item.MaxPriceDiscount)
                .Set("Type", item.Type)
                .Set("TypePay", item.TypePay)
                .Set("TypeDiscount", item.TypeDiscount)
                .Set("Created", _item.Created)
                .Set("Updated", DateTimes.Now());
            return _collectionPromotion.FindOneAndUpdate(x => x.Id == _item.Id, update);
        }

        [Obsolete]
        public long TotalPromotion()
        {
            return _collectionPromotion.Count(item => true);
        }

        public Promotion FindByCode(string promotionCode)
        {
            return _collectionPromotion.Find(item => item.PromotionCode == promotionCode).FirstOrDefault();
        }

        public List<Promotion> FindByPromotionIds(TypePay? typePay, List<string> promotionIds)
        {
            var filter = Builders<Promotion>.Filter.And(Builders<Promotion>.Filter.In(x => x.PromotionId, promotionIds),
                            Builders<Promotion>.Filter.Where(p => p.StartDate < DateTime.Now),
                            Builders<Promotion>.Filter.Where(p => p.EndDate > DateTime.Now),
                            Builders<Promotion>.Filter.Where(p => typePay == null || p.TypePay == typePay || (p.TypePay == TypePay.ZaloMomo && (typePay == TypePay.Zalo || typePay == TypePay.Momo))),
                            Builders<Promotion>.Filter.Where(p => p.Type == TypeEnum.Actived));
            return _collectionPromotion.Find(filter).ToEnumerable().Where(x => x.QuantityUsed < x.PromotionQuantity).ToList();
        }

        public void UpdateUsed(List<string> listPromotion)
        {
            foreach (var promotion in listPromotion)
            {
                var oldPromotion = _collectionPromotion.Find(item => item.PromotionId == promotion).FirstOrDefault();
                var update = Builders<Promotion>.Update.Set("QuantityUsed", ++oldPromotion.QuantityUsed);
                _collectionPromotion.FindOneAndUpdate(x => x.PromotionId == promotion, update);
            }
        }
    }

}
