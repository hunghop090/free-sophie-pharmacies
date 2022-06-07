﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sophie.Units;

namespace Sophie.Resource.Entities.Health
{
    public enum TypeBloodSugar
    {
        Normal,
        Hungry,
        AfterMeal,
        AfterTestHBA1C
    }

    [BsonIgnoreExtraElements]
    public class HealthBloodSugar
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

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
        public TypeHealth Type { get; set; } = TypeHealth.BloodSugar; // [BloodPressure, BloodSugar, SpO2, Weight, MenstrualCycle, HBA1C]

        [Display(Name = "Name")]
        [BsonElement("Name")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue(NameHealth.NameHealth_BloodSugar)]
        public string? Name { get; set; } = NameHealth.NameHealth_BloodSugar;



        [Required]
        [Display(Name = "TypeBloodSugar")]
        [BsonElement("TypeBloodSugar")]
        [BsonRepresentation(BsonType.String)]       // Mongo
        [JsonConverter(typeof(StringEnumConverter))]// Newtonsoft.Json
        public TypeBloodSugar TypeBloodSugar { get; set; } = TypeBloodSugar.Normal; // [Normal, Hungry, AfterMeal, AfterTestHBA1C]

        [Required]
        [Display(Name = "MinUnit")]
        [BsonElement("MinUnit")]
        [BsonRepresentation(BsonType.String)]
        public double? MinUnit { get; set; } = 0;

        [Required]
        [Display(Name = "AverageUnit")]
        [BsonElement("AverageUnit")]
        [BsonRepresentation(BsonType.String)]
        public double? AverageUnit { get; set; } = 0;

        [Required]
        [Display(Name = "MaxUnit")]
        [BsonElement("MaxUnit")]
        [BsonRepresentation(BsonType.String)]
        public double? MaxUnit { get; set; } = 0;

        [Required]
        [Display(Name = "Unit")]
        [BsonElement("Unit")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("mg/dL")]
        public string? Unit { get; set; } = "mg/dL";

        [Required]
        [Display(Name = "Date")]
        [BsonElement("Date")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Unspecified)]
        [BsonRepresentation(BsonType.String)]
        public DateTime? Date { get; set; }

        [Required]
        [Display(Name = "Time")]
        [BsonElement("Time")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Unspecified)]
        [BsonRepresentation(BsonType.String)]
        public DateTime? Time { get; set; }



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
