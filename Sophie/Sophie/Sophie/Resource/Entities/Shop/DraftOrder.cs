using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sophie.Languages;
using Sophie.Resource.Dtos.Shop;
using Sophie.Resource.Entities;
using Sophie.Resource.Entities.Shop;
using Sophie.Resource.Model;
using Sophie.Units;

namespace Sophie.Resource.Entities.Shop
{
    public class DraftOrder
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Display(Name = "DraftOrderId")]
        [BsonElement("DraftOrderId")]
        [BsonRepresentation(BsonType.String)]
        public string DraftOrderId { get; set; }

        [Display(Name = "TransactionId")]
        [BsonElement("TransactionId")]
        [BsonRepresentation(BsonType.String)]
        public string TransactionId { get; set; }

        [Display(Name = "PharmacistId")]
        [BsonElement("PharmacistId")]
        [BsonRepresentation(BsonType.String)]
        public string PharmacistId { get; set; }

        [Display(Name = "AccountId")]
        [BsonElement("AccountId")]
        [BsonRepresentation(BsonType.String)]
        public string AccountId { get; set; }

        [Display(Name = "AccountName")]
        [BsonElement("AccountName")]
        [BsonRepresentation(BsonType.String)]
        public string AccountName { get; set; }

        [Display(Name = "AddressId")]
        [BsonElement("AddressId")]
        [BsonRepresentation(BsonType.String)]
        public string? AddressId { get; set; }

        [Display(Name = "AddressAccount")]
        [BsonElement("AddressAccount")]
        [BsonRepresentation(BsonType.String)]
        public string? AddressAccount { get; set; }

        [Display(Name = "PromotionIds")]
        [BsonElement("PromotionIds")]
        public List<string>? PromotionIds { get; set; }

        [Display(Name = "TransportPromotionIds")]
        [BsonElement("TransportPromotionIds")]
        public List<string>? TransportPromotionIds { get; set; }

        [Required]
        [Display(Name = "ListProduct")]
        [BsonElement("ListProduct")]
        public List<ProductOrder>? ListProduct { get; set; }

        [Display(Name = "Price")]
        [BsonElement("Price")]
        [BsonRepresentation(BsonType.Int64)]
        public long Price { get; set; }

        [Display(Name = "PromotionPrice")]
        [BsonElement("PromotionPrice")]
        [BsonRepresentation(BsonType.Int64)]
        public long PromotionPrice { get; set; }

        [Display(Name = "TransportPromotionPrice")]
        [BsonElement("TransportPromotionPrice")]
        [BsonRepresentation(BsonType.Int64)]
        public long TransportPromotionPrice { get; set; }

        [Display(Name = "TransportPrice")]
        [BsonElement("TransportPrice")]
        [BsonRepresentation(BsonType.Int64)]
        public long TransportPrice { get; set; }

        [Display(Name = "Type")]
        [BsonElement("Type")]
        [BsonRepresentation(BsonType.String)]
        [JsonConverter(typeof(StringEnumConverter))]
        public TypeEnum Type { get; set; } = TypeEnum.Actived;

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
