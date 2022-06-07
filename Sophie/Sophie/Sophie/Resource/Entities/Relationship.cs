using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sophie.Units;

namespace Sophie.Resource.Entities
{
    [BsonIgnoreExtraElements]
    public class Relationship
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Display(Name = "RelationshipId")]
        [BsonElement("RelationshipId")]
        [BsonRepresentation(BsonType.String)]
        public string RelationshipId { get; set; }

        [Display(Name = "HospitalId")]
        [BsonElement("HospitalId")]
        [BsonRepresentation(BsonType.String)]
        public string HospitalId { get; set; }

        [Display(Name = "DoctorId")]
        [BsonElement("DoctorId")]
        [BsonRepresentation(BsonType.String)]
        public string DoctorId { get; set; }

        [Display(Name = "NameHospital")]
        [BsonElement("NameHospital")]
        [BsonRepresentation(BsonType.String)]
        public string? NameHospital { get; set; }

        [Display(Name = "NameDoctor")]
        [BsonElement("NameDoctor")]
        [BsonRepresentation(BsonType.String)]
        public string? NameDoctor { get; set; }



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
