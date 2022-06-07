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
using Sophie.Resource.Entities.Shop;
using Sophie.Units;

namespace Sophie.Repository
{
    public class CategoryRepository : BaseRepository, ICategoryRepository
    {
        private readonly ILog _log4net = log4net.LogManager.GetLogger(typeof(CategoryRepository));
        private readonly LogMongoService _logMongoService;

        private readonly IMongoCollection<Category> _collectionCategory;

        public CategoryRepository(LogMongoService logMongoService) : base()
        {
            _logMongoService = logMongoService;
            _collectionCategory = _database.GetCollection<Category>($"Category");

            var indexOptions = new CreateIndexOptions();
            var indexKeys_1 = Builders<Category>.IndexKeys.Ascending(item => item.CategoryId);
            var indexModelCategory = new List<CreateIndexModel<Category>>();
            indexModelCategory.Add(new CreateIndexModel<Category>(indexKeys_1, indexOptions));
            _collectionCategory.Indexes.CreateMany(indexModelCategory);
        }

        public Category CreateCategory(Category item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.CategoryId = Guid.NewGuid().ToString();
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionCategory.InsertOne(item);
            return item;
        }

        public Category DeleteCategory(string categoryId)
        {
            return _collectionCategory.FindOneAndDelete(item => item.CategoryId == categoryId);
        }

        public Category FindByIdCategory(string categoryId)
        {
            return _collectionCategory.Find(item => item.CategoryId == categoryId).FirstOrDefault();
        }

        public List<Category> ListCategory(int pageIndex = 0, int pageSize = 99)
        {
            return _collectionCategory.Find(item => true).SortByDescending(item => item.Updated).Skip(pageIndex * pageSize).Limit(pageSize).ToList();
        }

        public List<Category> ListCategoryActive()
        {
            return _collectionCategory.Find(item => item.Type == TypeCategory.Actived).SortByDescending(item => item.Updated).ToList();
        }

        public Category RestoreCategory(Category item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.CategoryId = (!string.IsNullOrEmpty(item.CategoryId) ? item.CategoryId : Guid.NewGuid().ToString());
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionCategory.InsertOne(item);
            return item;
        }

        [Obsolete]
        public long TotalCategory()
        {
            return _collectionCategory.Count(item => true);
        }

        public Category UpdateCategory(Category item)
        {
            Category _item = _collectionCategory.Find(x => x.CategoryId == item.CategoryId).FirstOrDefault();
            if (_item == null) return null;

            var update = Builders<Category>.Update
                .Set("CategoryId", item.CategoryId)
                .Set("CategoryName", item.CategoryName)
                .Set("CategoryLevel", item.CategoryLevel)
                .Set("CategoryIcon", item.CategoryIcon)
                .Set("ListSubCategory", item.ListSubCategory)
                .Set("Type", item.Type)

                .Set("Created", _item.Created)
                .Set("Updated", DateTimes.Now());

            return _collectionCategory.FindOneAndUpdate(x => x.Id == _item.Id, update);
        }
    }
}
