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
    public class NewsRepository : BaseRepository, INewsRepository
    {
        private readonly ILog _log4net = log4net.LogManager.GetLogger(typeof(NewsRepository));
        private readonly LogMongoService _logMongoService;

        private readonly IMongoCollection<News> _collectionNews;

        public NewsRepository(LogMongoService logMongoService) : base()
        {
            _logMongoService = logMongoService;
            _collectionNews = _database.GetCollection<News>($"News");

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

        public News CreateNews(News item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.NewsId = Guid.NewGuid().ToString();
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionNews.InsertOne(item);
            return item;
        }

        public News RestoreNews(News item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.NewsId = (!string.IsNullOrEmpty(item.NewsId) ? item.NewsId : Guid.NewGuid().ToString());
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionNews.InsertOne(item);
            return item;
        }

        public News DeleteNews(string newsId)
        {
            return _collectionNews.FindOneAndDelete(item => item.NewsId == newsId);
        }

        public List<News> ListNews(int pageIndex = 0, int pageSize = 99)
        {
            return _collectionNews.Find(item => true).Skip(pageIndex * pageSize).Limit(pageSize).ToList();
        }

        public News UpdateNews(News item)
        {
            News _item = _collectionNews.Find(x => x.NewsId == item.NewsId).FirstOrDefault();
            if (_item == null) return null;

            var update = Builders<News>.Update
                .Set("NewsId", item.NewsId)
                .Set("Type", item.Type)
                .Set("TypeFor", item.TypeFor)
                .Set("Images", item.Images)
                .Set("Subject", item.Subject)
                .Set("SubjectType", item.SubjectType)
                .Set("DateTime", item.DateTime)
                .Set("Content", item.Content)

                .Set("Created", _item.Created)
                .Set("Updated", DateTimes.Now());
            
            return _collectionNews.FindOneAndUpdate(x => x.Id == _item.Id, update);
        }

        [Obsolete]
        public long TotalNews()
        {
            return _collectionNews.Count(item => true);
        }

        public News FindByIdNews(string newsId)
        {
            return _collectionNews.Find(item => item.NewsId == newsId).FirstOrDefault();
        }
    }

}
