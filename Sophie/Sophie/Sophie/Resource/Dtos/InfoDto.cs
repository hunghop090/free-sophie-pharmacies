using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sophie.Languages;
using Sophie.Resource.Entities;
using Sophie.Units;

namespace Sophie.Resource.Dtos
{
    [BsonIgnoreExtraElements]
    public class InfoDto
    {
        [Display(Name = "InfoId")]
        [BsonElement("InfoId")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("")]
        public string InfoId { get; set; }

        [Display(Name = "AccountId")]
        [BsonElement("AccountId")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("")]
        public string AccountId { get; set; }



        [Display(Name = "Age")]
        [BsonElement("Age")]
        [BsonRepresentation(BsonType.String)]
        public string? Age { get; set; } // Tuổi

        [Display(Name = "Height")]
        [BsonElement("Height")]
        [BsonRepresentation(BsonType.String)]
        public double? Height { get; set; } = 0;// Chiều cao

        [Display(Name = "Weight")]
        [BsonElement("Weight")]
        [BsonRepresentation(BsonType.String)]
        public double? Weight { get; set; } = 0;// Cân nặng

        [Display(Name = "WeightTarget")]
        [BsonElement("WeightTarget")]
        [BsonRepresentation(BsonType.String)]
        public double? WeightTarget { get; set; } = 0;// Cân nặng mục tiêu

        [Display(Name = "BloodGroup")]
        [BsonElement("BloodGroup")]
        [BsonRepresentation(BsonType.String)]       // Mongo
        [JsonConverter(typeof(StringEnumConverter))]// Newtonsoft.Json
        public TypeBloodGroup? BloodGroup { get; set; } // Nhóm máu [A, B, AB, O]

        [Display(Name = "ContactEmail")]
        [BsonElement("ContactEmail")]
        [BsonRepresentation(BsonType.String)]
        public string? ContactEmail { get; set; } // Email phụ liên hệ

        [Display(Name = "Points")]
        [BsonElement("Points")]
        [BsonRepresentation(BsonType.String)]
        public double? Points { get; set; } = 0; // Điểm



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
