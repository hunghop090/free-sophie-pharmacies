using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sophie.Resource.Entities.Health;
using Sophie.Units;

namespace Sophie.Resource.Dtos.Health
{
    [BsonIgnoreExtraElements]
    public class HealthMenstrualCycleDto
    {
        //[BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        //public string Id { get; set; }

        [Display(Name = "HealthId")]
        [BsonElement("HealthId")]
        [BsonRepresentation(BsonType.String)]
        public string HealthId { get; set; }

        [Display(Name = "AccountId")]
        [BsonElement("AccountId")]
        [BsonRepresentation(BsonType.String)]
        public string AccountId { get; set; }

        [Required]
        [Display(Name = "Type")]
        [BsonElement("Type")]
        [BsonRepresentation(BsonType.String)]       // Mongo
        [JsonConverter(typeof(StringEnumConverter))]// Newtonsoft.Json
        public TypeHealth Type { get; set; } = TypeHealth.MenstrualCycle; // [BloodPressure, BloodSugar, SpO2, Weight, MenstrualCycle, HBA1C]

        [Display(Name = "Name")]
        [BsonElement("Name")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue(NameHealth.NameHealth_MenstrualCycle)]
        public string? Name { get; set; } = NameHealth.NameHealth_MenstrualCycle;



        [Required]
        [Display(Name = "Month1")]
        [BsonElement("Month1")]
        //[BsonRepresentation(BsonType.String)]
        public HealthMenstrualCycleMonth Month1 { get; set; }

        [Required]
        [Display(Name = "Month2")]
        [BsonElement("Month2")]
        //[BsonRepresentation(BsonType.String)]
        public HealthMenstrualCycleMonth Month2 { get; set; }

        [Required]
        [Display(Name = "Month3")]
        [BsonElement("Month3")]
        //[BsonRepresentation(BsonType.String)]
        public HealthMenstrualCycleMonth Month3 { get; set; }

        [Required]
        [Display(Name = "Month4")]
        [BsonElement("Month4")]
        //[BsonRepresentation(BsonType.String)]
        public HealthMenstrualCycleMonth Month4 { get; set; }

        [Required]
        [Display(Name = "Month5")]
        [BsonElement("Month5")]
        //[BsonRepresentation(BsonType.String)]
        public HealthMenstrualCycleMonth Month5 { get; set; }

        [Required]
        [Display(Name = "Month6")]
        [BsonElement("Month6")]
        //[BsonRepresentation(BsonType.String)]
        public HealthMenstrualCycleMonth Month6 { get; set; }

        [Required]
        [Display(Name = "Month7")]
        [BsonElement("Month7")]
        //[BsonRepresentation(BsonType.String)]
        public HealthMenstrualCycleMonth Month7 { get; set; }

        [Required]
        [Display(Name = "Month8")]
        [BsonElement("Month8")]
        //[BsonRepresentation(BsonType.String)]
        public HealthMenstrualCycleMonth Month8 { get; set; }

        [Required]
        [Display(Name = "Month9")]
        [BsonElement("Month9")]
        //[BsonRepresentation(BsonType.String)]
        public HealthMenstrualCycleMonth Month9 { get; set; }

        [Required]
        [Display(Name = "Month10")]
        [BsonElement("Month10")]
        //[BsonRepresentation(BsonType.String)]
        public HealthMenstrualCycleMonth Month10 { get; set; }

        [Required]
        [Display(Name = "Month11")]
        [BsonElement("Month11")]
        //[BsonRepresentation(BsonType.String)]
        public HealthMenstrualCycleMonth Month11 { get; set; }

        [Required]
        [Display(Name = "Month12")]
        [BsonElement("Month12")]
        //[BsonRepresentation(BsonType.String)]
        public HealthMenstrualCycleMonth Month12 { get; set; }



        [Required]
        [Display(Name = "Unit")]
        [BsonElement("Unit")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("year")]
        public string? Unit { get; set; } = "year";

        [Required]
        [Display(Name = "DateYear")]
        [BsonElement("DateYear")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Unspecified)]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("2022-01-01T00:00:00")]
        public DateTime? DateYear { get; set; }



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
