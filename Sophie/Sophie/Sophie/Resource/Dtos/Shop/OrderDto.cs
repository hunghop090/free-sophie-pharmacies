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
    public class OrderGroupBy
    {
        public string AccountId { get; set; }
        public string TransactionId { get; set; }
        public List<Order> ListOrder { get; set; }
        public DateTime Created { get; set; }
        public TypeStatusOrder TypeStatusOrder { get; set; }
    }

    public class OrderGroupByDto
    {
        public string AccountId { get; set; }
        public string TransactionId { get; set; }
        public string AccountName { get; set; }
        public List<OrderDto> ListOrder { get; set; }
        public long Price { get; set; }
        public long PromotionPrice { get; set; }
        public long TransportPrice { get; set; }
        public long TransportPromotionPrice { get; set; }
        public DateTime Created { get; set; }
        public TypeStatusOrder TypeStatusOrder { get; set; }
    }
    public class OrderDto
    {
        //[NotMapped]
        //[Newtonsoft.Json.JsonIgnore]
        //[System.Text.Json.Serialization.JsonIgnore]

        [Display(Name = "OrderId")]
        [BsonElement("OrderId")]
        [BsonRepresentation(BsonType.String)]
        public string OrderId { get; set; }

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

        [Display(Name = "ShopId")]
        [BsonElement("ShopId")]
        [BsonRepresentation(BsonType.String)]
        public string ShopId { get; set; }

        [Display(Name = "ShopName")]
        [BsonElement("ShopName")]
        [BsonRepresentation(BsonType.String)]
        public string ShopName { get; set; }

        [Display(Name = "PromotionIds")]
        [BsonElement("PromotionIds")]
        public List<string> PromotionIds { get; set; } = new List<string>();

        [Display(Name = "Promotion")]
        [BsonElement("Promotion")]
        public List<Promotion> Promotion { get; set; } = new List<Promotion>();

        [Display(Name = "TransportPromotionIds")]
        [BsonElement("TransportPromotionIds")]
        [BsonRepresentation(BsonType.String)]
        public List<string> TransportPromotionIds { get; set; } = new List<string>();

        [Display(Name = "TransportPromotion")]
        [BsonElement("TransportPromotion")]
        public List<TransportPromotion> TransportPromotion { get; set; } = new List<TransportPromotion>();

        [Display(Name = "TransportPrice")]
        [BsonElement("TransportPrice")]
        [BsonRepresentation(BsonType.Int64)]
        public long TransportPrice { get; set; }

        [Required]
        [Display(Name = "ListProduct")]
        [BsonElement("ListProduct")]
        public List<ProductOrder> ListProduct { get; set; }

        [Display(Name = "TransportStatus")]
        [BsonElement("TransportStatus")]
        [BsonRepresentation(BsonType.String)]
        public string TransportStatus { get; set; }

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

        [Display(Name = "TypeStatusOrder")]
        [BsonElement("TypeStatusOrder")]
        [BsonRepresentation(BsonType.String)]
        [JsonConverter(typeof(StringEnumConverter))]
        public TypeStatusOrder TypeStatusOrder { get; set; } = TypeStatusOrder.Pending;

        [Display(Name = "TypePay")]
        [BsonElement("TypePay")]
        [BsonRepresentation(BsonType.String)]
        [JsonConverter(typeof(StringEnumConverter))]
        public TypePay TypePay { get; set; } = TypePay.Other;

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
    public class OrderInListDto
    {
        //[NotMapped]
        //[Newtonsoft.Json.JsonIgnore]
        //[System.Text.Json.Serialization.JsonIgnore]

        [Display(Name = "OrderId")]
        [BsonElement("OrderId")]
        [BsonRepresentation(BsonType.String)]
        public string OrderId { get; set; }

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

        [Display(Name = "ShopId")]
        [BsonElement("ShopId")]
        [BsonRepresentation(BsonType.String)]
        public string ShopId { get; set; }

        [Display(Name = "TransportStatus")]
        [BsonElement("TransportStatus")]
        [BsonRepresentation(BsonType.String)]
        public string TransportStatus { get; set; }

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

        [Display(Name = "TypeStatusOrder")]
        [BsonElement("TypeStatusOrder")]
        [BsonRepresentation(BsonType.String)]
        [JsonConverter(typeof(StringEnumConverter))]
        public TypeStatusOrder TypeStatusOrder { get; set; } = TypeStatusOrder.Pending;

        [Display(Name = "TypePay")]
        [BsonElement("TypePay")]
        [BsonRepresentation(BsonType.String)]
        [JsonConverter(typeof(StringEnumConverter))]
        public TypePay TypePay { get; set; } = TypePay.Other;

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
