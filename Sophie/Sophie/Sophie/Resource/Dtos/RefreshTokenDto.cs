using System;
using System.ComponentModel.DataAnnotations;
using App.Core.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sophie.Units;

namespace Sophie.Resource.Dtos
{
    public class RefreshTokenDto
    {
        [Display(Name = "AccountId")]
        [BsonElement("AccountId")]
        [BsonRepresentation(BsonType.String)]
        public string AccountId { get; set; }

        [Display(Name = "TypeDevice")]
        [BsonElement("TypeDevice")]
        [BsonRepresentation(BsonType.String)]       // Mongo
        [JsonConverter(typeof(StringEnumConverter))]// Newtonsoft.Json
        public TypeDevice TypeDevice { get; set; }  // [WEB, MOBILE]

        [Display(Name = "DeviceName")]
        [BsonElement("DeviceName")]
        [BsonRepresentation(BsonType.String)]
        public string DeviceName { get; set; }

        [Display(Name = "DeviceId")]
        [BsonElement("DeviceId")]
        [BsonRepresentation(BsonType.String)]
        public string DeviceId { get; set; }

        [Display(Name = "Token")]
        [BsonElement("Token")]
        [BsonRepresentation(BsonType.String)]
        public string Token { get; set; }

        [Display(Name = "Expired")]
        [BsonElement("Expired")]
        [BsonRepresentation(BsonType.String)]
        public DateTime Expired { get; set; }

        [Display(Name = "TotalRefresh")]
        [BsonElement("TotalRefresh")]
        [BsonRepresentation(BsonType.String)]
        public int TotalRefresh { get; set; }

        [Display(Name = "TokenFCM")]
        [BsonElement("TokenFCM")]
        [BsonRepresentation(BsonType.String)]
        public string TokenFCM { get; set; }

        [Display(Name = "Location")]
        [BsonElement("Location")]
        [BsonRepresentation(BsonType.String)]
        public string Location { get; set; }



        [Display(Name = "Created")]
        [BsonElement("Created")]
        [BsonRepresentation(BsonType.String)]
        public DateTime? Created { get; set; } = DateTimes.Now();

        [Display(Name = "Updated")]
        [BsonElement("Updated")]
        [BsonRepresentation(BsonType.String)]
        public DateTime? Updated { get; set; } = DateTimes.Now();
    }
}
