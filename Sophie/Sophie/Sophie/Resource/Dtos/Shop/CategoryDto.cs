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
using Sophie.Units;

namespace Sophie.Resource.Dtos.Shop
{
    public class CategoryDto
    {
        //[NotMapped]
        //[Newtonsoft.Json.JsonIgnore]
        //[System.Text.Json.Serialization.JsonIgnore]

        [Display(Name = "CategoryId")]
        [BsonElement("CategoryId")]
        [BsonRepresentation(BsonType.String)]
        public string CategoryId { get; set; }

        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [Display(Name = "PharmacistId")]
        [BsonElement("PharmacistId")]
        [BsonRepresentation(BsonType.String)]
        public string PharmacistId { get; set; }

        [Display(Name = "CategoryName")]
        [BsonElement("CategoryName")]
        [BsonRepresentation(BsonType.String)]
        public string CategoryName { get; set; }

        [Display(Name = "CategoryLevel")]
        [BsonElement("CategoryLevel")]
        [BsonRepresentation(BsonType.String)]
        public string CategoryLevel { get; set; } = "1";

        [Display(Name = "CategoryIcon")]
        [BsonElement("CategoryIcon")]
        [BsonRepresentation(BsonType.String)]
        public string CategoryIcon { get; set; }

        [Required]
        [Display(Name = "ListSubCategory")]
        [BsonElement("ListSubCategory")]
        //[BsonRepresentation(BsonType.String)]
        public List<SubCategory> ListSubCategory { get; set; } // Danh sách danh mục con

        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [Display(Name = "Type")]
        [BsonElement("Type")]
        [BsonRepresentation(BsonType.String)]       // Mongo
        [JsonConverter(typeof(StringEnumConverter))]// Newtonsoft.Json
        public TypeCategory Type { get; set; } = TypeCategory.Actived; // [Actived, Draft, Trash]



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
