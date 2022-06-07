using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sophie.Resource.Entities.MedicalAppointment;
using Sophie.Units;

namespace Sophie.Resource.Dtos.MedicalAppointment
{
    public class MedicalAppointmentByHospitalDto
    {
        //[NotMapped]
        //[Newtonsoft.Json.JsonIgnore]
        //[System.Text.Json.Serialization.JsonIgnore]
        [Display(Name = "MedicalAppointmentId")]
        [BsonElement("MedicalAppointmentId")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("")]
        public string MedicalAppointmentId { get; set; }

        [Required]
        [Display(Name = "TypeMedicalAppointment")]
        [BsonElement("TypeMedicalAppointment")]
        [BsonRepresentation(BsonType.String)]       // Mongo
        [JsonConverter(typeof(StringEnumConverter))]// Newtonsoft.Json
        [DefaultValue("ByHospital")]
        public TypeMedicalAppointment TypeMedicalAppointment { get; set; } = TypeMedicalAppointment.ByHospital; // [ByPhone, ByHome, ByHospital, ByFastPhone]

        [Display(Name = "HospitalId")]
        [BsonElement("HospitalId")]
        [BsonRepresentation(BsonType.String)]
        public string? HospitalId { get; set; }

        [Display(Name = "Specialist")]
        [BsonElement("Specialist")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("Đa Khoa")]
        public string? Specialist { get; set; } // Đa Khoa, Tim Mạch, Da Liễu, Nội Tiết, Thần Kinh

        //[Display(Name = "DoctorId")]
        //[BsonElement("DoctorId")]
        //[BsonRepresentation(BsonType.String)]
        //public string? DoctorId { get; set; }

        //[Display(Name = "AddressId")]
        //[BsonElement("AddressId")]
        //[BsonRepresentation(BsonType.String)]
        //public string? AddressId { get; set; }

        //[Display(Name = "DescriptionOfSymptoms")]
        //[BsonElement("DescriptionOfSymptoms")]
        //[BsonRepresentation(BsonType.String)]
        //public string? DescriptionOfSymptoms { get; set; }

        [Required]
        [Display(Name = "Date")]
        [BsonElement("Date")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("2021-11-01T00:00:00")]
        public DateTime? Date { get; set; }

        [Required]
        [Display(Name = "Time")]
        [BsonElement("Time")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("2021-11-01T15:00:00")]
        public DateTime? Time { get; set; }

        //[Display(Name = "TypeStatusMedicalAppointment")]
        //[BsonElement("TypeStatusMedicalAppointment")]
        //[BsonRepresentation(BsonType.String)]       // Mongo
        //[JsonConverter(typeof(StringEnumConverter))]// Newtonsoft.Json
        //[DefaultValue("Pending")]
        //public TypeStatusMedicalAppointment TypeStatusMedicalAppointment { get; set; } = TypeStatusMedicalAppointment.Pending; // [Pending, Successed, Failed]



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
