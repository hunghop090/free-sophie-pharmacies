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
    public class ProductOrder
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

        [Display(Name = "ProductPrice")]
        [BsonElement("ProductPrice")]
        [BsonRepresentation(BsonType.Double)]
        public long ProductPrice { get; set; }

        [Display(Name = "Quantity")]
        [BsonElement("Quantity")]
        [BsonRepresentation(BsonType.Int32)]
        public int Quantity { get; set; }
    }
}
