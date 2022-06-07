using System;
using System.ComponentModel.DataAnnotations;
using App.Core.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sophie.Units;

namespace Sophie.Resource.Entities
{
    //public enum TypeDevice
    //{
    //    WEB,
    //    MOBILE
    //}

    public enum TypeUser
    {
        ACCOUNT,
        DOCTOR
    }

    /// <summary>
    /// Lưu trữ các token đã đăng nhập
    /// </summary>
    [BsonIgnoreExtraElements]
    public class RefreshToken
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Display(Name = "UserId")]
        [BsonElement("UserId")]
        [BsonRepresentation(BsonType.String)]
        public string? UserId { get; set; }

        [Display(Name = "TypeUser")]
        [BsonElement("TypeUser")]
        [BsonRepresentation(BsonType.String)]       // Mongo
        [JsonConverter(typeof(StringEnumConverter))]// Newtonsoft.Json
        public TypeUser? TypeUser { get; set; }  // [ACCOUNT, DOCTOR]

        [Display(Name = "TypeDevice")]
        [BsonElement("TypeDevice")]
        [BsonRepresentation(BsonType.String)]       // Mongo
        [JsonConverter(typeof(StringEnumConverter))]// Newtonsoft.Json
        public TypeDevice? TypeDevice { get; set; }  // [WEB, MOBILE]

        [Display(Name = "DeviceId")]
        [BsonElement("DeviceId")]
        [BsonRepresentation(BsonType.String)]
        public string? DeviceId { get; set; }

        [Display(Name = "DeviceName")]
        [BsonElement("DeviceName")]
        [BsonRepresentation(BsonType.String)]
        public string? DeviceName { get; set; }

        [Display(Name = "Token")]
        [BsonElement("Token")]
        [BsonRepresentation(BsonType.String)]
        public string Token { get; set; }

        [Display(Name = "Expired")]
        [BsonElement("Expired")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Unspecified)]
        [BsonRepresentation(BsonType.String)]
        public DateTime? Expired { get; set; }

        [Display(Name = "TotalRefresh")]
        [BsonElement("TotalRefresh")]
        [BsonRepresentation(BsonType.String)]
        public int? TotalRefresh { get; set; }

        [Display(Name = "TokenFCM")]
        [BsonElement("TokenFCM")]
        [BsonRepresentation(BsonType.String)]
        public string? TokenFCM { get; set; }

        [Display(Name = "Location")]
        [BsonElement("Location")]
        [BsonRepresentation(BsonType.String)]
        public string? Location { get; set; }



        [Display(Name = "Created")]
        [BsonElement("Created")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Unspecified)]
        [BsonRepresentation(BsonType.String)]
        public DateTime Created { get; set; } = DateTimes.Now();

        [Display(Name = "Updated")]
        [BsonElement("Updated")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Unspecified)]
        [BsonRepresentation(BsonType.String)]
        public DateTime Updated { get; set; } = DateTimes.Now();
    }
}
