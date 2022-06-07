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
    public class ProductDto
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
        public string? ProductName { get; set; }

        [Display(Name = "ProductImages")]
        [BsonElement("ProductImages")]
        public List<string>? ProductImages { get; set; }

        [Display(Name = "ListImageInfo")]
        [BsonElement("ListImageInfo")]
        //[BsonRepresentation(BsonType.String)]
        public List<ImageInfo> ListImageInfo { get; set; } = new List<ImageInfo>();

        [Display(Name = "ProductPrice")]
        [BsonElement("ProductPrice")]
        [BsonRepresentation(BsonType.Double)]
        public double? ProductPrice { get; set; } = 0.0;

        [Display(Name = "ProductDiscounts")]
        [BsonElement("ProductDiscounts")]
        [BsonRepresentation(BsonType.Double)]
        public double? ProductDiscounts { get; set; } = 0.0;

        [Display(Name = "ProductRealPrice")]
        [BsonElement("ProductRealPrice")]
        [BsonRepresentation(BsonType.Double)]
        public double? ProductRealPrice { get; set; } = 0.0;

        [Display(Name = "ProductNumber")]
        [BsonElement("ProductNumber")]
        [BsonRepresentation(BsonType.Int32)]
        public int? ProductNumber { get; set; } = 0;

        [Required]
        [Display(Name = "ProductInfo")]
        [BsonElement("ProductInfo")]
        public List<ProductInfo> ProductInfo { get; set; } // Danh sách thông tin sản phẩm

        [Display(Name = "PurchasedNumber")]
        [BsonElement("PurchasedNumber")]
        [BsonRepresentation(BsonType.Int32)]
        public int? PurchasedNumber { get; set; } = 0;

        [Display(Name = "Rating")]
        [BsonElement("Rating")]
        [BsonRepresentation(BsonType.String)]
        public double Rating { get; set; } = 1.0;

        [Display(Name = "SellOver")]
        [BsonElement("SellOver")]
        [BsonRepresentation(BsonType.String)]
        public bool SellOver { get; set; } = false;

        [Display(Name = "Type")]
        [BsonElement("Type")]
        [BsonRepresentation(BsonType.String)]       // Mongo
        [JsonConverter(typeof(StringEnumConverter))]// Newtonsoft.Json
        public TypeEnum Type { get; set; } = TypeEnum.Actived; // [Actived, Draft, Trash]


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
