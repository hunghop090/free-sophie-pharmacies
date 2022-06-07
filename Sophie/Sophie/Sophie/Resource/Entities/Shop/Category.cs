using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sophie.Units;

namespace Sophie.Resource.Entities.Shop
{
    public enum TypeCategory
    {
        Actived,
        Draft,
        Trash
    }

    public enum TypeSubCategory
    {
        Actived,
        Draft,
        Trash
    }

    [BsonIgnoreExtraElements]
    public class SubCategory
    {
        //[NotMapped]
        //[Newtonsoft.Json.JsonIgnore]
        //[System.Text.Json.Serialization.JsonIgnore]

        [Display(Name = "SubCategoryId")]
        [BsonElement("SubCategoryId")]
        [BsonRepresentation(BsonType.String)]
        public string SubCategoryId { get; set; }

        [Display(Name = "SubCategoryName")]
        [BsonElement("SubCategoryName")]
        [BsonRepresentation(BsonType.String)]
        public string SubCategoryName { get; set; }

        [Display(Name = "SubCategoryLevel")]
        [BsonElement("SubCategoryLevel")]
        [BsonRepresentation(BsonType.String)]
        public string SubCategoryLevel { get; set; } = "2";

        [Display(Name = "SubCategoryIcon")]
        [BsonElement("SubCategoryIcon")]
        [BsonRepresentation(BsonType.String)]
        public string SubCategoryIcon { get; set; }

        [Display(Name = "Type")]
        [BsonElement("Type")]
        [BsonRepresentation(BsonType.String)]       // Mongo
        [JsonConverter(typeof(StringEnumConverter))]// Newtonsoft.Json
        public TypeSubCategory Type { get; set; } = TypeSubCategory.Actived; // [Actived, Draft, Trash]



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

    [BsonIgnoreExtraElements]
    public class Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Display(Name = "CategoryId")]
        [BsonElement("CategoryId")]
        [BsonRepresentation(BsonType.String)]
        public string CategoryId { get; set; }

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
        public List<SubCategory> ListSubCategory { get; set; } = new List<SubCategory>();

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
