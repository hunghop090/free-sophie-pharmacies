using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sophie.Resource.Entities.Shop;
using Sophie.Units;

namespace Sophie.Resource.Dtos.Shop
{
    //[NotMapped]
    //[Newtonsoft.Json.JsonIgnore]
    //[System.Text.Json.Serialization.JsonIgnore]
    [BsonIgnoreExtraElements]
    public class ShopDto
    {
        [Required]
        [Display(Name = "ShopId")]
        [BsonElement("ShopId")]
        [BsonRepresentation(BsonType.String)]
        public string ShopId { get; set; }

        [Required]
        [Display(Name = "PharmacistId")]
        [BsonElement("PharmacistId")]
        [BsonRepresentation(BsonType.String)]
        public string PharmacistId { get; set; }

        [Required]
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

        [Display(Name = "ShopAddress")]
        [BsonElement("ShopAddress")]
        [BsonRepresentation(BsonType.String)]
        public string Address { get; set; }

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
