using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sophie.Units;

namespace Sophie.Resource.Entities.Health
{
    [BsonIgnoreExtraElements]
    public class HealthWeight
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

        [Display(Name = "Type")]
        [BsonElement("Type")]
        [BsonRepresentation(BsonType.String)]       // Mongo
        [JsonConverter(typeof(StringEnumConverter))]// Newtonsoft.Json
        public TypeHealth Type { get; set; } = TypeHealth.Weight; // [BloodPressure, BloodSugar, SpO2, Weight, MenstrualCycle, HBA1C]

        [Display(Name = "Name")]
        [BsonElement("Name")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue(NameHealth.NameHealth_Weight)]
        public string? Name { get; set; } = NameHealth.NameHealth_Weight;



        [Display(Name = "MinUnit")]
        [BsonElement("MinUnit")]
        [BsonRepresentation(BsonType.String)]
        public double? MinUnit { get; set; }

        [Display(Name = "AverageUnit")]
        [BsonElement("AverageUnit")]
        [BsonRepresentation(BsonType.String)]
        public double? AverageUnit { get; set; }

        [Display(Name = "MaxUnit")]
        [BsonElement("MaxUnit")]
        [BsonRepresentation(BsonType.String)]
        public double? MaxUnit { get; set; }

        [Display(Name = "Unit")]
        [BsonElement("Unit")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("kg")]
        public string? Unit { get; set; } = "kg";

        [Display(Name = "Date")]
        [BsonElement("Date")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Unspecified)]
        [BsonRepresentation(BsonType.String)]
        public DateTime? Date { get; set; }

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
