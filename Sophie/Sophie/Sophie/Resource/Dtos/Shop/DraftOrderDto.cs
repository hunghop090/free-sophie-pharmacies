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
    public class ProductOrderDto
    {
        //[NotMapped]
        //[Newtonsoft.Json.JsonIgnore]
        //[System.Text.Json.Serialization.JsonIgnore]

        [Display(Name = "ProductId")]
        [BsonElement("ProductId")]
        [BsonRepresentation(BsonType.String)]
        public string ProductId { get; set; }

        [Display(Name = "CategoryId")]
        [BsonElement("CategoryId")]
        [BsonRepresentation(BsonType.String)]
        public string CategoryId { get; set; }

        [Display(Name = "SubCategoryId")]
        [BsonElement("SubCategoryId")]
        [BsonRepresentation(BsonType.String)]
        public string? SubCategoryId { get; set; }

        [Display(Name = "ShopId")]
        [BsonElement("ShopId")]
        [BsonRepresentation(BsonType.String)]
        public string ShopId { get; set; }

        [Display(Name = "PharmacistId")]
        [BsonElement("PharmacistId")]
        [BsonRepresentation(BsonType.String)]
        public string PharmacistId { get; set; }

        [Display(Name = "ProductName")]
        [BsonElement("ProductName")]
        [BsonRepresentation(BsonType.String)]
        public string ProductName { get; set; }

        [Display(Name = "ProductImages")]
        [BsonElement("ProductImages")]
        [BsonRepresentation(BsonType.String)]
        public List<string>? ProductImages { get; set; } = new List<string>();

        [Display(Name = "ProductRealPrice")]
        [BsonElement("ProductRealPrice")]
        [BsonRepresentation(BsonType.Double)]
        public double? ProductRealPrice { get; set; } = 0.0;

        [Display(Name = "ProductNumber")]
        [BsonElement("ProductNumber")]
        [BsonRepresentation(BsonType.Int32)]
        public int? ProductNumber { get; set; } = 0;

        [Display(Name = "PurchasedNumber")]
        [BsonElement("PurchasedNumber")]
        [BsonRepresentation(BsonType.Int32)]
        public int? PurchasedNumber { get; set; } = 0;

        [Display(Name = "ProductPrice")]
        [BsonElement("ProductPrice")]
        [BsonRepresentation(BsonType.Double)]
        public long ProductPrice { get; set; }

        [Display(Name = "Quantity")]
        [BsonElement("Quantity")]
        [BsonRepresentation(BsonType.Int32)]
        public int Quantity { get; set; }

        [Display(Name = "SellOver")]
        [BsonElement("SellOver")]
        [BsonRepresentation(BsonType.String)]
        public bool? SellOver { get; set; } = false;
    }

    public class DraftOrderDto
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

        [Required]
        [Display(Name = "ListProduct")]
        [BsonElement("ListProduct")]
        public List<ProductOrderDto> ListProduct { get; set; }

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

        [Display(Name = "PromotionIds")]
        [BsonElement("PromotionIds")]
        public List<string>? PromotionIds { get; set; }

        [Display(Name = "TransportPromotionIds")]
        [BsonElement("TransportPromotionIds")]
        public List<string>? TransportPromotionIds { get; set; }

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
