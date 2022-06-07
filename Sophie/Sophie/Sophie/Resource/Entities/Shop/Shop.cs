using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sophie.Units;

namespace Sophie.Resource.Entities.Shop
{
    public enum TypeShop
    {
        Actived,
        Draft,
        Trash
    }

    [BsonIgnoreExtraElements]
    public class Shop
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Display(Name = "ShopId")]
        [BsonElement("ShopId")]
        [BsonRepresentation(BsonType.String)]
        public string ShopId { get; set; }

        [Display(Name = "PharmacistId")]
        [BsonElement("PharmacistId")]
        [BsonRepresentation(BsonType.String)]
        public string PharmacistId { get; set; }

        [Display(Name = "ShopName")]
        [BsonElement("ShopName")]
        [BsonRepresentation(BsonType.String)]
        public string ShopName { get; set; }

        [Display(Name = "Description")]
        [BsonElement("Description")]
        [BsonRepresentation(BsonType.String)]
        public string Description { get; set; }

        [Display(Name = "ShopImage")]
        [BsonElement("ShopImage")]
        [BsonRepresentation(BsonType.String)]
        public string ShopImage { get; set; }

        [Display(Name = "TransportPrice")]
        [BsonElement("TransportPrice")]
        public long TransportPrice { get; set; }

        [Display(Name = "ShopAddress")]
        [BsonElement("ShopAddress")]
        [BsonRepresentation(BsonType.String)]
        public string ShopAddress { get; set; }

        [Display(Name = "Type")]
        [BsonElement("Type")]
        [BsonRepresentation(BsonType.String)]
        [JsonConverter(typeof(StringEnumConverter))]
        public TypeShop Type { get; set; } = TypeShop.Actived;

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
