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
    public class HospitalDto
    {
        //[NotMapped]
        //[Newtonsoft.Json.JsonIgnore]
        //[System.Text.Json.Serialization.JsonIgnore]
        [Display(Name = "HospitalId")]
        [BsonElement("HospitalId")]
        [BsonRepresentation(BsonType.String)]
        public string? HospitalId { get; set; }

        [Display(Name = "TypeLogin")]
        [BsonElement("TypeLogin")]
        [BsonRepresentation(BsonType.String)]       // Mongo
        [JsonConverter(typeof(StringEnumConverter))]// Newtonsoft.Json
        [DefaultValue("Phone")]
        public TypeLogin TypeLogin { get; set; } = TypeLogin.Phone; // [Phone, Email, Google, Facebook, Apple, Other]

        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [Display(Name = "Confirm")]
        [BsonElement("Confirm")]
        [BsonRepresentation(BsonType.Boolean)]
        public bool Confirm { get; set; } = false;

        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [Display(Name = "Active")]
        [BsonElement("Active")]
        [BsonRepresentation(BsonType.String)]       // Mongo
        [JsonConverter(typeof(StringEnumConverter))]// Newtonsoft.Json
        public TypeActive Active { get; set; } = TypeActive.Pending; // [Pending, Actived, InActived]

        [Display(Name = "PhoneNumber")]
        [BsonElement("PhoneNumber")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("+84389955141")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Email")]
        [BsonElement("Email")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("")]
        public string? Email { get; set; }

        [Display(Name = "Username")]
        [BsonElement("Username")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("")]
        public string? Username { get; set; }

        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [Display(Name = "Password")]
        [BsonElement("Password")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("Abc@1234")]
        public string? Password { get; set; }



        [Display(Name = "Specialist")]
        [BsonElement("Specialist")]
        [BsonRepresentation(BsonType.String)]
        public string? Specialist { get; set; } // Đa Khoa, Tim Mạch, Da Liễu, Nội Tiết, Thần Kinh

        [Display(Name = "NameHospital")]
        [BsonElement("NameHospital")]
        [BsonRepresentation(BsonType.String)]
        public string? NameHospital { get; set; }

        [Display(Name = "Avatar")]
        [BsonElement("Avatar")]
        [BsonRepresentation(BsonType.String)]
        public string? Avatar { get; set; }

        [Display(Name = "Language")]
        [BsonElement("Language")]
        [BsonRepresentation(BsonType.String)]
        public string? Language { get; set; }



        [Display(Name = "Province")]
        [BsonElement("Province")]
        [BsonRepresentation(BsonType.String)]
        public string? Province { get; set; }

        [Display(Name = "City")]
        [BsonElement("City")]
        [BsonRepresentation(BsonType.String)]
        public string? City { get; set; }

        [Display(Name = "District")]
        [BsonElement("District")]
        [BsonRepresentation(BsonType.String)]
        public string? District { get; set; }

        [Display(Name = "Wards")]
        [BsonElement("Wards")]
        [BsonRepresentation(BsonType.String)]
        public string? Wards { get; set; }

        [Display(Name = "AddressHospital")]
        [BsonElement("AddressHospital")]
        [BsonRepresentation(BsonType.String)]
        public string? AddressHospital { get; set; }



        [Display(Name = "Num")]
        [BsonElement("Num")]
        [BsonRepresentation(BsonType.String)]
        public double? Num { get; set; } = 0;

        [Display(Name = "Exp")]
        [BsonElement("Exp")]
        [BsonRepresentation(BsonType.String)]
        public double? Exp { get; set; } = 1;

        [Display(Name = "Rate")]
        [BsonElement("Rate")]
        [BsonRepresentation(BsonType.String)]
        public double? Rate { get; set; } = 1.0;

        [Display(Name = "Info")]
        [BsonElement("Info")]
        [BsonRepresentation(BsonType.String)]
        public string? Info { get; set; }



        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [Display(Name = "TwoFactorEnabled")]
        [BsonElement("TwoFactorEnabled")]
        [BsonRepresentation(BsonType.Boolean)]
        public bool TwoFactorEnabled { get; set; } = false;

        [Display(Name = "IsOnline")]
        [BsonElement("IsOnline")]
        [BsonRepresentation(BsonType.String)]
        public bool? IsOnline { get; set; }

        [Display(Name = "VideoCallId")]
        [BsonElement("VideoCallId")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("")]
        public string? VideoCallId { get; set; }

        [Display(Name = "VideoCallToken")]
        [BsonElement("VideoCallToken")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("")]
        public string? VideoCallToken { get; set; }

        [Display(Name = "Notes")]
        [BsonElement("Notes")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("")]
        public string? Notes { get; set; }

        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [Display(Name = "DynamicField")]
        [BsonElement("DynamicField")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("")]
        public string? DynamicField { get; set; }



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
