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
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        private readonly ILog _log4net = log4net.LogManager.GetLogger(typeof(AccountRepository));
        private readonly LogMongoService _logMongoService;

        private readonly IMongoCollection<Account> _collectionAccount;
        private readonly IMongoCollection<RefreshToken> _collectionRefreshToken;

        public AccountRepository(LogMongoService logMongoService) : base()
        {
            _logMongoService = logMongoService;
            _collectionAccount = _database.GetCollection<Account>($"Account");
            _collectionRefreshToken = _database.GetCollection<RefreshToken>($"RefreshToken");

            var indexOptions = new CreateIndexOptions();
            var indexKeys_1 = Builders<Account>.IndexKeys.Ascending(item => item.AccountId);
            var indexKeys_2 = Builders<Account>.IndexKeys.Ascending(item => item.PhoneNumber);
            var indexKeys_3 = Builders<Account>.IndexKeys.Ascending(item => item.Email);
            var indexKeys_4 = Builders<Account>.IndexKeys.Ascending(item => item.Username);
            var indexModelAccount = new List<CreateIndexModel<Account>>();
            indexModelAccount.Add(new CreateIndexModel<Account>(indexKeys_1, indexOptions));
            indexModelAccount.Add(new CreateIndexModel<Account>(indexKeys_2, indexOptions));
            indexModelAccount.Add(new CreateIndexModel<Account>(indexKeys_3, indexOptions));
            indexModelAccount.Add(new CreateIndexModel<Account>(indexKeys_4, indexOptions));
            _collectionAccount.Indexes.CreateMany(indexModelAccount);

            var indexKeys_5 = Builders<RefreshToken>.IndexKeys.Ascending(item => item.UserId);
            var indexKeys_6 = Builders<RefreshToken>.IndexKeys.Ascending(item => item.Token);
            var indexModelRefreshToken = new List<CreateIndexModel<RefreshToken>>();
            indexModelRefreshToken.Add(new CreateIndexModel<RefreshToken>(indexKeys_5, indexOptions));
            indexModelRefreshToken.Add(new CreateIndexModel<RefreshToken>(indexKeys_6, indexOptions));
            _collectionRefreshToken.Indexes.CreateMany(indexModelRefreshToken);
        }

        //=== Account

        public Account CreateAccount(Account item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.AccountId = Guid.NewGuid().ToString();
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionAccount.InsertOne(item);
            return item;
        }

        public Account RestoreAccount(Account item)
        {
            ObjectId objectId = ObjectId.GenerateNewId();
            item.Id = new BsonObjectId(objectId).ToString();
            item.AccountId = (!string.IsNullOrEmpty(item.AccountId) ? item.AccountId : Guid.NewGuid().ToString());
            item.Created = DateTimes.Now();
            item.Updated = DateTimes.Now();

            _collectionAccount.InsertOne(item);
            return item;
        }

        public Account DeleteAccount(string accountId)
        {
            return _collectionAccount.FindOneAndDelete(item => item.AccountId == accountId);
        }

        public List<Account> ListAccount(int pageIndex = 0, int pageSize = 99)
        {
            return _collectionAccount.Find(item => true).Skip(pageIndex * pageSize).Limit(pageSize).ToList();
        }

        public Account UpdateAccount(Account item)
        {
            Account _item = _collectionAccount.Find(x => x.AccountId == item.AccountId).FirstOrDefault();
            if (_item == null) return null;

            var update = Builders<Account>.Update
                .Set("AccountId", item.AccountId)
                .Set("TypeLogin", item.TypeLogin)
                .Set("Confirm", item.Confirm)
                .Set("Active", item.Active)
                .Set("PhoneNumber", item.PhoneNumber)
                .Set("Email", item.Email)
                .Set("Username", item.Username)
                .Set("Password", item.Password)

                .Set("Firstname", item.Firstname)
                .Set("Lastname", item.Lastname)
                .Set("Fullname", item.Fullname)
                .Set("Birthdate", item.Birthdate)
                .Set("Address", item.Address)
                .Set("HomePhone", item.HomePhone)
                .Set("Avatar", item.Avatar)
                .Set("Race", item.Race)
                .Set("Gender", item.Gender)
                .Set("Language", item.Language)

                .Set("TwoFactorEnabled", item.TwoFactorEnabled)
                .Set("IsOnline", item.IsOnline)
                .Set("VideoCallId", item.VideoCallId)
                .Set("VideoCallToken", item.VideoCallToken)
                .Set("Notes", item.Notes)

                .Set("Created", _item.Created)
                .Set("Updated", DateTimes.Now());

            return _collectionAccount.FindOneAndUpdate(x => x.Id == _item.Id, update);
        }

        [Obsolete]
        public long TotalAccount()
        {
            return _collectionAccount.Count(item => true);
        }

        public Account FindByIdAccount(string accountId)
        {
            return _collectionAccount.Find(item => item.AccountId == accountId).FirstOrDefault();
        }

        public Account FindByEmailAccount(string email, TypeLogin? typeLogin = null)
        {
            if (typeLogin != null)
            {
                return _collectionAccount.Find(item => item.Email == email && item.TypeLogin == typeLogin).FirstOrDefault();
            }

            return _collectionAccount.Find(item => item.Email == email).FirstOrDefault();
        }

        public Account FindByPhoneAccount(string phonenumber, TypeLogin? typeLogin = null)
        {
            if (typeLogin != null)
            {
                return _collectionAccount.Find(item => item.PhoneNumber == phonenumber && item.TypeLogin == typeLogin).FirstOrDefault();
            }

            return _collectionAccount.Find(item => item.PhoneNumber == phonenumber).FirstOrDefault();
        }

        public Account FindByUsernameAccount(string username)
        {
            return _collectionAccount.Find(item => item.Username == username).FirstOrDefault();
        }

        public Account FindByVideoCallIdAccount(string videoCallId)
        {
            return _collectionAccount.Find(item => item.VideoCallId == videoCallId).FirstOrDefault();
        }

        //=== RefreshToken

        public RefreshToken CreateToken(RefreshToken refreshToken)
        {
            // Remove old token
            _collectionRefreshToken.DeleteMany(item => item.UserId == refreshToken.UserId && item.DeviceId == refreshToken.DeviceId && item.DeviceName == refreshToken.DeviceName);

            // Create new token
            ObjectId objectId = ObjectId.GenerateNewId();
            RefreshToken item = new RefreshToken
            {
                Id = new BsonObjectId(objectId).ToString(),
                UserId = refreshToken.UserId,
                TypeUser = refreshToken.TypeUser,
                TypeDevice = refreshToken.TypeDevice,
                DeviceName = refreshToken.DeviceName,
                DeviceId = refreshToken.DeviceId,
                Token = refreshToken.Token,
                Expired = refreshToken.Expired,
                TotalRefresh = refreshToken.TotalRefresh,
                TokenFCM = refreshToken.TokenFCM,
                Location = refreshToken.Location,
                Created = DateTimes.Now(),
                Updated = DateTimes.Now()
            };
            _collectionRefreshToken.InsertOne(item);

            return item;
        }

        public RefreshToken UpdateToken(RefreshToken refreshToken)
        {
            RefreshToken _item = _collectionRefreshToken.Find(x => x.Id == refreshToken.Id).FirstOrDefault();
            if (_item == null) return null;

            var update = Builders<RefreshToken>.Update
                .Set("UserId", refreshToken.UserId)
                .Set("TypeUser", refreshToken.TypeUser)
                .Set("TypeDevice", refreshToken.TypeDevice)
                .Set("DeviceId", refreshToken.DeviceId)
                .Set("DeviceName", refreshToken.DeviceName)
                .Set("Token", refreshToken.Token)
                .Set("Expired", refreshToken.Expired)
                .Set("TotalRefresh", _item.TotalRefresh)
                .Set("TokenFCM", refreshToken.TokenFCM)
                .Set("Location", refreshToken.Location)
                .Set("Created", _item.Created)
                .Set("Updated", DateTimes.Now());

            return _collectionRefreshToken.FindOneAndUpdate(x => x.Id == _item.Id, update);
        }

        public List<RefreshToken> FindTokenByIdAccount(string userId)
        {
            return _collectionRefreshToken.Find(item => item.UserId == userId).ToList();
        }

        public RefreshToken FindByRefreshToken(string token)
        {
            return _collectionRefreshToken.Find(item => item.Token == token).FirstOrDefault();
        }

        public RefreshToken RefreshToken(string token, string deviceName, string deviceId, int dayExpiration)
        {
            RefreshToken _item = _collectionRefreshToken.Find(x => x.Token == token).FirstOrDefault();
            if (_item == null) return null;

            var update = Builders<RefreshToken>.Update
                .Set("DeviceId", deviceId)
                .Set("DeviceName", deviceName)
                .Set("Token", token)
                .Set("Expired", DateTimes.Now().AddDays(dayExpiration))
                .Set("TotalRefresh", _item.TotalRefresh++)
                .Set("Created", DateTimes.Now())
                .Set("Updated", DateTimes.Now());
            
            return _collectionRefreshToken.FindOneAndUpdate(x => x.Id == _item.Id, update);
        }

        public RefreshToken RevokeToken(string token)
        {
            return _collectionRefreshToken.FindOneAndDelete(x => x.Token == token);
        }

        public List<RefreshToken> ListAllToken()
        {
            List<RefreshToken> listToken = _collectionRefreshToken.Find(item => true).ToList();
            return listToken;
        }

        public List<RefreshToken> RemoveAllToken()
        {
            List<RefreshToken> listToken = _collectionRefreshToken.Find(item => true).ToList();
            foreach (var item in listToken)
            {
                _collectionRefreshToken.FindOneAndDelete(x => x.Id == item.Id);
            }
            //DeleteResult deleteResult = _collectionRefreshToken.DeleteMany(x => true);

            return listToken;
        }

        public List<RefreshToken> RemoveAllToken(string accountId)
        {
            List<RefreshToken> listToken = _collectionRefreshToken.Find(item => item.UserId == accountId).ToList();
            foreach(var item in listToken)
            {
                _collectionRefreshToken.FindOneAndDelete(x => x.Id == item.Id);
            }
            //DeleteResult deleteResult = _collectionRefreshToken.DeleteMany(x => x.AccountId == accountId);

            return listToken;
        }

        //=== 

    }
}
