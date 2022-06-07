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
    public class RelationshipRepository : BaseRepository, IRelationshipRepository
    {
        private readonly ILog _log4net = log4net.LogManager.GetLogger(typeof(RelationshipRepository));
        private readonly LogMongoService _logMongoService;

        private readonly IMongoCollection<Relationship> _collectionRelationship;

        public RelationshipRepository(LogMongoService logMongoService) : base()
        {
            _logMongoService = logMongoService;
            _collectionRelationship = _database.GetCollection<Relationship>($"Relationship");

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

        public Relationship CreateRelationship(Relationship item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.RelationshipId = Guid.NewGuid().ToString();
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionRelationship.InsertOne(item);
            return item;
        }

        public Relationship RestoreRelationship(Relationship item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.RelationshipId = (!string.IsNullOrEmpty(item.RelationshipId) ? item.RelationshipId : Guid.NewGuid().ToString());
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionRelationship.InsertOne(item);
            return item;
        }

        public Relationship DeleteRelationship(string relationshipId)
        {
            return _collectionRelationship.FindOneAndDelete(item => item.RelationshipId == relationshipId);
        }

        public List<Relationship> ListRelationship(int pageIndex = 0, int pageSize = 99)
        {
            return _collectionRelationship.Find(item => true).Skip(pageIndex * pageSize).Limit(pageSize).ToList();
        }

        public Relationship UpdateRelationship(Relationship item)
        {
            Relationship _item = _collectionRelationship.Find(x => x.RelationshipId == item.RelationshipId).FirstOrDefault();
            if (_item == null) return null;

            var update = Builders<Relationship>.Update
                .Set("RelationshipId", item.RelationshipId)
                .Set("HospitalId", item.HospitalId)
                .Set("DoctorId", item.DoctorId)
                .Set("NameHospital", item.NameHospital)
                .Set("NameDoctor", item.NameDoctor)

                .Set("Created", _item.Created)
                .Set("Updated", DateTimes.Now());
            
            return _collectionRelationship.FindOneAndUpdate(x => x.Id == _item.Id, update);
        }

        [Obsolete]
        public long TotalRelationship()
        {
            return _collectionRelationship.Count(item => true);
        }

        public Relationship FindByIdRelationship(string relationshipId)
        {
            return _collectionRelationship.Find(item => item.RelationshipId == relationshipId).FirstOrDefault();
        }

        public List<Relationship> FindByIdHospital(string hospitalId)
        {
            return _collectionRelationship.Find(item => item.HospitalId == hospitalId).ToList();
        }

        public List<Relationship> FindByIdDoctor(string doctorId)
        {
            return _collectionRelationship.Find(item => item.DoctorId == doctorId).ToList();
        }

    }
}
