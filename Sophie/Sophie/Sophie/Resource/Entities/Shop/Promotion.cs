using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sophie.Languages;
using Sophie.Resource.Entities;
using Sophie.Resource.Entities.Shop;
using Sophie.Resource.Model;
using Sophie.Units;

namespace Sophie.Resource.Entities.Shop
{
    public class Promotion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Display(Name = "PromotionId")]
        [BsonElement("PromotionId")]
        [BsonRepresentation(BsonType.String)]
        public string PromotionId { get; set; }

        [Display(Name = "PharmacistId")]
        [BsonElement("PharmacistId")]
        [BsonRepresentation(BsonType.String)]
        public string PharmacistId { get; set; }

        [Display(Name = "ShopId")]
        [BsonElement("ShopId")]
        [BsonRepresentation(BsonType.String)]
        public string ShopId { get; set; }

        [Display(Name = "PromotionName")]
        [BsonElement("PromotionName")]
        [BsonRepresentation(BsonType.String)]
        public string PromotionName { get; set; }

        [Display(Name = "PromotionCode")]
        [BsonElement("PromotionCode")]
        [BsonRepresentation(BsonType.String)]
        public string PromotionCode { get; set; }

        [Display(Name = "PromotionQuantity")]
        [BsonElement("PromotionQuantity")]
        [BsonRepresentation(BsonType.Int32)]
        public int PromotionQuantity { get; set; }

        [Display(Name = "QuantityUsed")]
        [BsonElement("QuantityUsed")]
        [BsonRepresentation(BsonType.Int32)]
        public int QuantityUsed { get; set; }

        [Display(Name = "MinBuget")]
        [BsonElement("MinBuget")]
        [BsonRepresentation(BsonType.Int64)]
        public long MinBuget { get; set; }

        [Display(Name = "Price")]
        [BsonElement("Price")]
        [BsonRepresentation(BsonType.Int64)]
        public long? Price { get; set; }

        [Display(Name = "Discount")]
        [BsonElement("Discount")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int? Discount { get; set; }

        [Display(Name = "MaxPriceDiscount")]
        [BsonElement("MaxPriceDiscount")]
        [BsonRepresentation(BsonType.Int64)]
        public long? MaxPriceDiscount { get; set; }

        [Display(Name = "TypePay")]
        [BsonElement("TypePay")]
        [BsonRepresentation(BsonType.String)]       // Mongo
        [JsonConverter(typeof(StringEnumConverter))]// Newtonsoft.Json
        public TypePay TypePay { get; set; } = TypePay.Other;//[Other, Momo, Zalo, ZaloMomo]

        [Display(Name = "TypeDiscount")]
        [BsonElement("TypeDiscount")]
        [BsonRepresentation(BsonType.String)]       // Mongo
        [JsonConverter(typeof(StringEnumConverter))]// Newtonsoft.Json
        public TypePromotionsDiscount TypeDiscount { get; set; }//[TypePromotionsDiscount_1, TypePromotionsDiscount_2]



        [Display(Name = "Title")]
        [BsonElement("Title")]
        [BsonRepresentation(BsonType.String)]
        public string Title { get; set; }

        [Display(Name = "Banner")]
        [BsonElement("Banner")]
        public List<string> Banner { get; set; } = new List<string>();

        [Display(Name = "Content")]
        [BsonElement("Content")]
        [BsonRepresentation(BsonType.String)]
        public string Content { get; set; }

        [Display(Name = "StartDate")]
        [BsonElement("StartDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Unspecified)]
        [BsonRepresentation(BsonType.String)]
        [ModelBinder(typeof(DateTimeModelBinder))]
        public DateTime StartDate { get; set; }

        [Display(Name = "EndDate")]
        [BsonElement("EndDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Unspecified)]
        [BsonRepresentation(BsonType.String)]
        [ModelBinder(typeof(DateTimeModelBinder))]
        public DateTime EndDate { get; set; }

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
