using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sophie.Units;

namespace Sophie.Resource.Entities.MedicalAppointment
{
    public enum TypeMedicalAppointment
    {
        ByPhone,     // Đặt khám qua điện thoại
        ByHome,      // Đặt khám tại nhà
        ByHospital,  // Đặt khám tại phòng khám
        ByFastPhone, // Khám nhanh qua điện thoại

        ByBabyBath,        // Dịch vụ tắm cho bé
        ByCareSick,        // Dịch vụ chăm sóc người ốm
        ByBloodCollection, // Dịch vụ Lấy máu tại nhà
        ByPhysiotherapy,   // Dịch vụ Vật lý trị liệu
    }
    
    public enum TypeStatusMedicalAppointment
    {
        Pending,
        Successed,
        Failed,
    }

    [BsonIgnoreExtraElements]
    public class MedicalAppointment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

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
        [DefaultValue("ByPhone")]
        public TypeMedicalAppointment TypeMedicalAppointment { get; set; } = TypeMedicalAppointment.ByPhone; //[ByPhone, ByHome, ByHospital, ByFastPhone, BabyBathService, CareSickService, BloodCollectionService, PhysiotherapyService]

        [Display(Name = "AccountId")]
        [BsonElement("AccountId")]
        [BsonRepresentation(BsonType.String)]
        public string AccountId { get; set; }

        [Display(Name = "HospitalId")]
        [BsonElement("HospitalId")]
        [BsonRepresentation(BsonType.String)]
        public string? HospitalId { get; set; }

        [Display(Name = "Specialist")]
        [BsonElement("Specialist")]
        [BsonRepresentation(BsonType.String)]
        public string? Specialist { get; set; } // Đa Khoa, Tim Mạch, Da Liễu, Nội Tiết, Thần Kinh

        [Display(Name = "DoctorId")]
        [BsonElement("DoctorId")]
        [BsonRepresentation(BsonType.String)]
        public string? DoctorId { get; set; }

        [Display(Name = "AddressId")]
        [BsonElement("AddressId")]
        [BsonRepresentation(BsonType.String)]
        public string? AddressId { get; set; }

        [Display(Name = "DescriptionOfSymptoms")]
        [BsonElement("DescriptionOfSymptoms")]
        [BsonRepresentation(BsonType.String)]
        public string? DescriptionOfSymptoms { get; set; }

        [Required]
        [Display(Name = "Date")]
        [BsonElement("Date")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Unspecified)]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("2021-12-30T00:00:00")]
        public DateTime? Date { get; set; }

        [Required]
        [Display(Name = "Time")]
        [BsonElement("Time")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Unspecified)]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("2021-11-01T15:00:00")]
        public DateTime? Time { get; set; }

        [Display(Name = "TypeStatusMedicalAppointment")]
        [BsonElement("TypeStatusMedicalAppointment")]
        [BsonRepresentation(BsonType.String)]       // Mongo
        [JsonConverter(typeof(StringEnumConverter))]// Newtonsoft.Json
        [DefaultValue("Pending")]
        public TypeStatusMedicalAppointment TypeStatusMedicalAppointment { get; set; } = TypeStatusMedicalAppointment.Pending; // [Pending, Successed, Failed]



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
