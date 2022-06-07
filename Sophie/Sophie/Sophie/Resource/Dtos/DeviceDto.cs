using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Core.Entities;
using App.Core.Units;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Sophie.Resource.Dtos
{
    public class DeviceDto
    {
        //[NotMapped]
        //[Newtonsoft.Json.JsonIgnore]
        //[System.Text.Json.Serialization.JsonIgnore]
        [Required]
        [Display(Name = "UserId")]
        [BsonElement("UserId")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("629052f5-e544-44ca-8de5-20e20af66ce0")]
        public string UserId { get; set; }

        [Display(Name = "TypeDevice")]
        [BsonElement("TypeDevice")]
        [BsonRepresentation(BsonType.String)]       // Mongo
        [JsonConverter(typeof(StringEnumConverter))]// Newtonsoft.Json
        [DefaultValue("MOBILE")]
        public TypeDevice TypeDevice { get; set; }  // [WEB, MOBILE]

        [Display(Name = "DeviceId")]
        [BsonElement("DeviceId")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("")]
        public string? DeviceId { get; set; }

        [Display(Name = "DeviceName")]
        [BsonElement("DeviceName")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("")]
        public string? DeviceName { get; set; }

        [Display(Name = "TokenFCM")]
        [BsonElement("TokenFCM")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("")]
        public string? TokenFCM { get; set; }

        [Display(Name = "Location")]
        [BsonElement("Location")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("")]
        public string? Location { get; set; }

        [Display(Name = "Timestamp")]
        [BsonElement("Timestamp")]
        [BsonRepresentation(BsonType.String)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Unspecified)]
        public DateTime? Timestamp { get; set; } = DateTimesEx.Now();
    }
}
