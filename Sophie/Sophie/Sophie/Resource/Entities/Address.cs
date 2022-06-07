using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sophie.Units;

namespace Sophie.Resource.Entities
{
    public enum TypeAddress
    {
        Home, // Nhà riêng
        Organ, // Cơ quan
    }

    [BsonIgnoreExtraElements]
    public class Address
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Display(Name = "AddressId")]
        [BsonElement("AddressId")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("")]
        public string? AddressId { get; set; }

        [Display(Name = "AccountId")]
        [BsonElement("AccountId")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("")]
        public string AccountId { get; set; }

        [Display(Name = "IsDefault")]
        [BsonElement("IsDefault")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("false")]
        public bool? IsDefault { get; set; } = false;



        [Display(Name = "Title")]
        [BsonElement("Title")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("")]
        public string? Title { get; set; }

        [Display(Name = "FullName")]
        [BsonElement("FullName")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("")]
        public string? FullName { get; set; }

        [Display(Name = "Phone")]
        [BsonElement("Phone")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("")]
        public string? Phone { get; set; }

        [Display(Name = "Province")]
        [BsonElement("Province")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("")]
        public string? Province { get; set; }

        [Display(Name = "City")]
        [BsonElement("City")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("Hồ Chí Minh")]
        public string? City { get; set; }

        [Display(Name = "District")]
        [BsonElement("District")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("Quận 1")]
        public string? District { get; set; }

        [Display(Name = "Wards")]
        [BsonElement("Wards")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("Phường 16")]
        public string? Wards { get; set; }

        [Display(Name = "AddressAccount")]
        [BsonElement("AddressAccount")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("234 Hai Bà Trưng, phường 16, quận 1, TP. Hồ Chí Minh")]
        public string? AddressAccount { get; set; }

        [Display(Name = "TypeAddress")]
        [BsonElement("TypeAddress")]
        [BsonRepresentation(BsonType.String)]       // Mongo
        [JsonConverter(typeof(StringEnumConverter))]// Newtonsoft.Json
        public TypeAddress TypeAddress { get; set; } = TypeAddress.Home; // [Home, Organ]



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
