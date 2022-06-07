using System;
using System.Collections.Generic;
using System.Globalization;
using App.Core.Entities;
using App.Core.Services;
using App.SharedLib.Repository;
using log4net;
using MongoDB.Bson;
using MongoDB.Driver;
using Sophie.Repository.Interface;
using Sophie.Resource.Dtos.Shop;
using Sophie.Resource.Entities;
using Sophie.Resource.Entities.Shop;
using Sophie.Resource.Model;
using Sophie.Units;
using static Sophie.Controllers.API.ProductController;

namespace Sophie.Repository
{
    public class ProductRepository : BaseRepository, IProductRepository
    {
        private readonly ILog _log4net = log4net.LogManager.GetLogger(typeof(CategoryRepository));
        private readonly LogMongoService _logMongoService;

        private readonly IMongoCollection<Product> _collectionProduct;

        public ProductRepository(LogMongoService logMongoService) : base()
        {
            _logMongoService = logMongoService;
            _collectionProduct = _database.GetCollection<Product>($"Product");

            var indexOptions = new CreateIndexOptions();
            var indexKeys_1 = Builders<Product>.IndexKeys.Ascending(item => item.ProductId);
            var indexKeys_2 = Builders<Product>.IndexKeys.Ascending(item => item.CategoryId);
            var indexKeys_3 = Builders<Product>.IndexKeys.Ascending(item => item.SubCategoryId);
            var indexModelCategory = new List<CreateIndexModel<Product>>();
            indexModelCategory.Add(new CreateIndexModel<Product>(indexKeys_1, indexOptions));
            indexModelCategory.Add(new CreateIndexModel<Product>(indexKeys_2, indexOptions));
            indexModelCategory.Add(new CreateIndexModel<Product>(indexKeys_3, indexOptions));
            _collectionProduct.Indexes.CreateMany(indexModelCategory);
        }

        public Product CreateProduct(Product item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            if (string.IsNullOrEmpty(item.ProductId))
                item.ProductId = Guid.NewGuid().ToString();
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();
            item.IsSale = item.ProductRealPrice > item.ProductPrice;
            _collectionProduct.InsertOne(item);
            return item;
        }

        public void CreateProducts(List<Product> products)
        {
            foreach (Product product in products)
            {
                ObjectId objectId = ObjectId.GenerateNewId();
                product.Id = new BsonObjectId(objectId).ToString();
                if (string.IsNullOrEmpty(product.ProductId))
                    product.ProductId = Guid.NewGuid().ToString();
                product.Created = DateTimes.Now();
                product.Updated = DateTimes.Now();
                product.IsSale = product.ProductRealPrice > product.ProductPrice;
            }
            _collectionProduct.InsertMany(products);
        }

        public Product DeleteProduct(string productId)
        {
            return _collectionProduct.FindOneAndDelete(item => item.ProductId == productId);
        }

        public Product FindByIdProduct(string productId)
        {
            return _collectionProduct.Find(item => item.ProductId == productId).FirstOrDefault();
        }

        public List<Product> FindByProductIds(string productIds = "")
        {
            var ListIds = productIds.Split(',');
            var filter = Builders<Product>.Filter.In(x => x.ProductId, ListIds);
            return _collectionProduct.Find(filter).ToList();
        }

        public PagingResult<Product> ListProduct(Paging paging)
        {
            PagingResult<Product> result = new PagingResult<Product>();
            var filter = Builders<Product>.Filter.Where(p => true);
            if (!string.IsNullOrEmpty(paging.search))
            {
                filter = Builders<Product>.Filter.Regex(x => x.ProductName, new BsonRegularExpression(paging.search, "i"));
            }
            var query = _collectionProduct.Find(filter);
            result.Total = query.ToList().Count;
            result.Result = query.Sort($"{{{paging.sortName}: {(paging.sort == "asc" ? 1 : -1)}}}")
                .Skip(paging.PageIndex * paging.PageSize)
                .Limit(paging.PageSize)
                .ToList();
            return result;
        }
        public PagingResult<Product> ListSearchProduct(Paging paging)
        {
            PagingResult<Product> result = new PagingResult<Product>();
            var filter = Builders<Product>.Filter.Where(p => p.Type == TypeEnum.Actived);
            if (!string.IsNullOrEmpty(paging.search))
            {
                filter = Builders<Product>.Filter.Regex(x => x.ProductName, new BsonRegularExpression(paging.search, "i"));
            }
            var query = _collectionProduct.Find(filter);
            result.Total = query.ToList().Count;
            result.Result = query.Sort($"{{{paging.sortName}: {(paging.sort == "asc" ? 1 : -1)}}}")
                .Skip(paging.PageIndex * paging.PageSize)
                .Limit(paging.PageSize)
                .ToList();
            return result;
        }

        public PagingResult<Product> ListProductByPharmacistId(Paging paging, string pharmacistId)
        {
            PagingResult<Product> result = new PagingResult<Product>();
            var filter = Builders<Product>.Filter.Where(p => p.PharmacistId == pharmacistId);
            if (!string.IsNullOrEmpty(paging.search))
            {
                filter = Builders<Product>.Filter.And(
                                Builders<Product>.Filter.Where(p => p.PharmacistId == pharmacistId),
                                Builders<Product>.Filter.Regex(x => x.ProductName, new BsonRegularExpression(paging.search, "i"))
                                );
            }
            var query = _collectionProduct.Find(filter);
            result.Total = query.ToList().Count;
            result.Result = query.SortByDescending(item => item.PurchasedNumber)
                .Skip(paging.PageIndex * paging.PageSize)
                .Limit(paging.PageSize)
                .ToList();
            return result;
        }

        public PagingResult<Product> ListProductFilter(FilterProduct filter, SortType? sort, SortFilterProduct? sortName, int limit, int pageIndex)
        {
            PagingResult<Product> result = new PagingResult<Product>();
            var filterBuilders = Builders<Product>.Filter.And(
                Builders<Product>.Filter.Where(x => string.IsNullOrEmpty(filter.CategoryId) || x.CategoryId == filter.CategoryId),
                Builders<Product>.Filter.Where(x => string.IsNullOrEmpty(filter.SubCategoryId) || x.SubCategoryId == filter.SubCategoryId),
                Builders<Product>.Filter.Where(x => filter.MinPrice == 0 || x.ProductPrice >= filter.MinPrice),
                Builders<Product>.Filter.Where(x => filter.MaxPrice == 0 || x.ProductPrice <= filter.MaxPrice),
                Builders<Product>.Filter.Where(x => filter.IsSale == null || filter.IsSale == false || x.IsSale.Value)
                );
            var query = _collectionProduct.Find(filterBuilders);
            result.Total = query.ToList().Count;
            if (filter.IsRelated == true && !string.IsNullOrEmpty(filter.SubCategoryId) && string.IsNullOrEmpty(filter.CategoryId) && result.Total == 0)
            {
                filterBuilders = Builders<Product>.Filter.And(
                Builders<Product>.Filter.Where(x => string.IsNullOrEmpty(filter.CategoryId) || x.CategoryId == filter.CategoryId),
                Builders<Product>.Filter.Where(x => filter.MinPrice == 0 || x.ProductPrice >= filter.MinPrice),
                Builders<Product>.Filter.Where(x => filter.MaxPrice == 0 || x.ProductPrice <= filter.MaxPrice),
                Builders<Product>.Filter.Where(x => filter.IsSale == null || filter.IsSale == false || x.IsSale.Value)
                );
                query = _collectionProduct.Find(filterBuilders);
                result.Total = query.ToList().Count;
            }
            result.Result = query.Sort($"{{{sortName}: {(sort == SortType.asc ? 1 : -1)}}}").Skip(pageIndex * limit).Limit(limit).ToList();
            return result;
        }

        public PagingResult<Product> ListTopSold(Paging paging)
        {
            PagingResult<Product> result = new PagingResult<Product>();
            var query = _collectionProduct.Find(item => true);
            result.Total = query.ToList().Count;
            var ListDate = query.Skip(paging.PageIndex * paging.PageSize).Limit(paging.PageSize).Sort("{PurchasedNumber: -1}").ToList();
            result.Result = ListDate;
            return result;
        }

        public List<Product> FindAll()
        {
            return _collectionProduct.Find(item => item.Type == TypeEnum.Actived).SortByDescending(item => item.Updated).ToList();
        }

        public Product RestoreProduct(Product item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.ProductId = (!string.IsNullOrEmpty(item.ProductId) ? item.ProductId : Guid.NewGuid().ToString());
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionProduct.InsertOne(item);
            return item;
        }

        [Obsolete]
        public long TotalProduct()
        {
            return _collectionProduct.Count(item => true);
        }

        public Product UpdateProduct(Product item)
        {
            var update = Builders<Product>.Update
                .Set("CategoryId", item.CategoryId)
                .Set("SubCategoryId", item.SubCategoryId)
                .Set("ProductName", item.ProductName)
                .Set("ProductImages", item.ProductImages)
                .Set("ProductPrice", item.ProductPrice)
                .Set("ProductDiscounts", item.ProductDiscounts)
                .Set("ProductRealPrice", item.ProductRealPrice)
                .Set("ProductNumber", item.ProductNumber)
                .Set("ProductInfo", item.ProductInfo)
                .Set("SellOver", item.SellOver)
                .Set("Type", item.Type)
                .Set("Created", item.Created)
                .Set("IsSale", item.ProductRealPrice > item.ProductPrice)
                .Set("Updated", DateTimes.Now());

            return _collectionProduct.FindOneAndUpdate(x => x.Id == item.Id, update);
        }

        public void UpdatePurchasedNumber(List<ProductOrder> listProduct)
        {
            foreach (var item in listProduct)
            {
                var oldProduct = _collectionProduct.Find(x => x.ProductId == item.ProductId).FirstOrDefault();
                var update = Builders<Product>.Update.Set("PurchasedNumber", oldProduct.PurchasedNumber + item.Quantity);
                _collectionProduct.FindOneAndUpdate(x => x.Id == oldProduct.Id, update);
            }
        }

    }
}
