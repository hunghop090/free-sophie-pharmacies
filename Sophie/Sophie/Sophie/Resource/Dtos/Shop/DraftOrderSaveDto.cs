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
using Sophie.Resource.Entities;
using Sophie.Resource.Entities.Shop;
using Sophie.Resource.Model;
using Sophie.Units;

namespace Sophie.Resource.Dtos.Shop
{

    public class TransportPrice
    {
        public string ShopId { get; set; }
        public long Price { get; set; }
    }

    public class DraftOrderSaveDto
    {
        //[NotMapped]
        //[Newtonsoft.Json.JsonIgnore]
        //[System.Text.Json.Serialization.JsonIgnore]

        [Display(Name = "DraftOrderId")]
        [BsonElement("DraftOrderId")]
        [BsonRepresentation(BsonType.String)]
        public string DraftOrderId { get; set; }

        [Display(Name = "TransactionId")]
        [BsonElement("TransactionId")]
        [BsonRepresentation(BsonType.String)]
        public string? TransactionId { get; set; }

        [Display(Name = "PharmacistId")]
        [BsonElement("PharmacistId")]
        [BsonRepresentation(BsonType.String)]
        public string? PharmacistId { get; set; }

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
        [BsonRepresentation(BsonType.String)]
        public List<string>? PromotionIds { get; set; }

        [Display(Name = "TransportPromotionIds")]
        [BsonElement("TransportPromotionIds")]
        [BsonRepresentation(BsonType.String)]
        public List<string>? TransportPromotionIds { get; set; }

        [Required]
        [Display(Name = "ListProduct")]
        [BsonElement("ListProduct")]
        public List<ProductOrder> ListProduct { get; set; }

        [Display(Name = "Price")]
        [BsonElement("Price")]
        [BsonRepresentation(BsonType.Int64)]
        public long Price { get; set; }

        [Display(Name = "TypePay")]
        [BsonElement("TypePay")]
        [BsonRepresentation(BsonType.String)]
        [JsonConverter(typeof(StringEnumConverter))]
        public TypePay TypePay { get; set; } = TypePay.Other;
    }
}
