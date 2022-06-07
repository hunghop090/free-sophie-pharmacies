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

namespace Sophie.Resource.Dtos.Shop
{
    public class TransportPromotionDto
    {
        //[NotMapped]
        //[Newtonsoft.Json.JsonIgnore]
        //[System.Text.Json.Serialization.JsonIgnore]

        [Display(Name = "TransportPromotionId")]
        [BsonElement("TransportPromotionId")]
        [BsonRepresentation(BsonType.String)]
        public string TransportPromotionId { get; set; }

        [Display(Name = "PharmacistId")]
        [BsonElement("PharmacistId")]
        [BsonRepresentation(BsonType.String)]
        public string PharmacistId { get; set; }

        [Display(Name = "ShopId")]
        [BsonElement("ShopId")]
        [BsonRepresentation(BsonType.String)]
        public string ShopId { get; set; }

        [Display(Name = "TransportPromotionName")]
        [BsonElement("TransportPromotionName")]
        [BsonRepresentation(BsonType.String)]
        public string TransportPromotionName { get; set; }

        [Display(Name = "TransportPromotionCode")]
        [BsonElement("TransportPromotionCode")]
        [BsonRepresentation(BsonType.String)]
        public string TransportPromotionCode { get; set; }

        [Display(Name = "TransportPromotionQuantity")]
        [BsonElement("TransportPromotionQuantity")]
        [BsonRepresentation(BsonType.Int32)]
        public int TransportPromotionQuantity { get; set; }

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
        public long Price { get; set; }

        [Display(Name = "Discount")]
        [BsonElement("Discount")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int Discount { get; set; }

        [Display(Name = "MaxPriceDiscount")]
        [BsonElement("MaxPriceDiscount")]
        [BsonRepresentation(BsonType.Int64)]
        public long MaxPriceDiscount { get; set; }

        [Display(Name = "TypePay")]
        [BsonElement("TypePay")]
        [BsonRepresentation(BsonType.String)]       // Mongo
        [JsonConverter(typeof(StringEnumConverter))]// Newtonsoft.Json
        public TypePay TypePay { get; set; } = TypePay.Other;//[Other, Momo, Zalo, ZaloMomo]

        [Display(Name = "TypeDiscount")]
        [BsonElement("TypeDiscount")]
        [BsonRepresentation(BsonType.String)]       // Mongo
        [JsonConverter(typeof(StringEnumConverter))]// Newtonsoft.Json
        public TypeTransportPromotionsDiscount? TypeDiscount { get; set; }//[TypeTransportPromotionsDiscount_1, TypeTransportPromotionsDiscount_2]



        [Display(Name = "Title")]
        [BsonElement("Title")]
        [BsonRepresentation(BsonType.String)]
        public string Title { get; set; }

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

    public class TransportPromotionInListDto
    {
        //[NotMapped]
        //[Newtonsoft.Json.JsonIgnore]
        //[System.Text.Json.Serialization.JsonIgnore]

        [Display(Name = "TransportPromotionId")]
        [BsonElement("TransportPromotionId")]
        [BsonRepresentation(BsonType.String)]
        public string TransportPromotionId { get; set; }

        [Display(Name = "PharmacistId")]
        [BsonElement("PharmacistId")]
        [BsonRepresentation(BsonType.String)]
        public string PharmacistId { get; set; }

        [Display(Name = "ShopId")]
        [BsonElement("ShopId")]
        [BsonRepresentation(BsonType.String)]
        public string ShopId { get; set; }

        [Display(Name = "TransportPromotionName")]
        [BsonElement("TransportPromotionName")]
        [BsonRepresentation(BsonType.String)]
        public string TransportPromotionName { get; set; }

        [Display(Name = "TransportPromotionQuantity")]
        [BsonElement("TransportPromotionQuantity")]
        [BsonRepresentation(BsonType.Int32)]
        public int TransportPromotionQuantity { get; set; }

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
        public long Price { get; set; }

        [Display(Name = "Discount")]
        [BsonElement("Discount")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int Discount { get; set; }

        [Display(Name = "MaxPriceDiscount")]
        [BsonElement("MaxPriceDiscount")]
        [BsonRepresentation(BsonType.Int64)]
        public long MaxPriceDiscount { get; set; }

        [Display(Name = "TypePay")]
        [BsonElement("TypePay")]
        [BsonRepresentation(BsonType.String)]       // Mongo
        [JsonConverter(typeof(StringEnumConverter))]// Newtonsoft.Json
        public TypePay TypePay { get; set; } = TypePay.Other;

        [Display(Name = "TypeDiscount")]
        [BsonElement("TypeDiscount")]
        [BsonRepresentation(BsonType.String)]       // Mongo
        [JsonConverter(typeof(StringEnumConverter))]// Newtonsoft.Json
        public TypeTransportPromotionsDiscount? TypeDiscount { get; set; }



        [Display(Name = "TransportPromotionCode")]
        [BsonElement("TransportPromotionCode")]
        [BsonRepresentation(BsonType.String)]
        public string TransportPromotionCode { get; set; }

        [Display(Name = "Title")]
        [BsonElement("Title")]
        [BsonRepresentation(BsonType.String)]
        public string Title { get; set; }

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
