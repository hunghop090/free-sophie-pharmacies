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
    public class AnalysisRepository : BaseRepository, IAnalysisRepository
    {
        private readonly ILog _log4net = log4net.LogManager.GetLogger(typeof(AnalysisRepository));
        private readonly LogMongoService _logMongoService;

        private readonly IMongoCollection<Analysis> _collectionAnalysis;

        public AnalysisRepository(LogMongoService logMongoService) : base()
        {
            _logMongoService = logMongoService;
            _collectionAnalysis = _database.GetCollection<Analysis>($"Analysis");

            var indexOptions = new CreateIndexOptions();
            var indexKeys_1 = Builders<Analysis>.IndexKeys.Ascending(item => item.AnalysisId);
            var indexKeys_2 = Builders<Analysis>.IndexKeys.Ascending(item => item.Specialist);
            var indexKeys_3 = Builders<Analysis>.IndexKeys.Ascending(item => item.NameAnalysis);
            var indexModelAnalysis = new List<CreateIndexModel<Analysis>>();
            indexModelAnalysis.Add(new CreateIndexModel<Analysis>(indexKeys_1, indexOptions));
            indexModelAnalysis.Add(new CreateIndexModel<Analysis>(indexKeys_2, indexOptions));
            indexModelAnalysis.Add(new CreateIndexModel<Analysis>(indexKeys_3, indexOptions));
            _collectionAnalysis.Indexes.CreateMany(indexModelAnalysis);
        }

        public Analysis CreateAnalysis(Analysis item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.AnalysisId = Guid.NewGuid().ToString();
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionAnalysis.InsertOne(item);
            return item;
        }

        public Analysis DeleteAnalysis(string analysisId)
        {
            return _collectionAnalysis.FindOneAndDelete(item => item.AnalysisId == analysisId);
        }

        public Analysis FindByIdAnalysis(string analysisId)
        {
            return _collectionAnalysis.Find(item => item.AnalysisId == analysisId).FirstOrDefault();
        }

        public Analysis FindByVideoCallIdAnalysis(string videoCallId)
        {
            return _collectionAnalysis.Find(item => item.VideoCallId == videoCallId).FirstOrDefault();
        }

        public List<Analysis> ListAnalysis(int pageIndex = 0, int pageSize = 99)
        {
            return _collectionAnalysis.Find(item => true).Skip(pageIndex * pageSize).Limit(pageSize).ToList();
        }

        public Analysis RestoreAnalysis(Analysis item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.AnalysisId = (!string.IsNullOrEmpty(item.AnalysisId) ? item.AnalysisId : Guid.NewGuid().ToString());
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionAnalysis.InsertOne(item);
            return item;
        }

        [Obsolete]
        public long TotalAnalysis()
        {
            return _collectionAnalysis.Count(item => true);
        }

        public Analysis UpdateAnalysis(Analysis item)
        {
            Analysis _item = _collectionAnalysis.Find(x => x.AnalysisId == item.AnalysisId).FirstOrDefault();
            if (_item == null) return null;

            var update = Builders<Analysis>.Update
                .Set("AnalysisId", item.AnalysisId)
                .Set("TypeLogin", item.TypeLogin)
                .Set("Confirm", item.Confirm)
                .Set("Active", item.Active)
                .Set("PhoneNumber", item.PhoneNumber)
                .Set("Email", item.Email)
                .Set("Username", item.Username)
                .Set("Password", item.Password)

                .Set("Specialist", item.Specialist)
                .Set("NameAnalysis", item.NameAnalysis)
                .Set("Avatar", item.Avatar)
                .Set("Language", item.Language)

                .Set("Province", item.Province)
                .Set("City", item.City)
                .Set("District", item.District)
                .Set("Wards", item.Wards)
                .Set("AddressAnalysis", item.AddressAnalysis)

                .Set("Num", item.Num)
                .Set("Exp", item.Exp)
                .Set("Rate", item.Rate)
                .Set("Info", item.Info)

                .Set("TwoFactorEnabled", item.TwoFactorEnabled)
                .Set("IsOnline", item.IsOnline)
                .Set("VideoCallId", item.VideoCallId)
                .Set("VideoCallToken", item.VideoCallToken)
                .Set("Notes", item.Notes)

                .Set("Created", _item.Created)
                .Set("Updated", DateTimes.Now());

            return _collectionAnalysis.FindOneAndUpdate(x => x.Id == _item.Id, update);
        }
    }
}
