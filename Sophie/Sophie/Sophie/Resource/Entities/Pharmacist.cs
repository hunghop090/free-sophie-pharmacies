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
    public enum TypePharmacist
    {
        Admin,
        User,
    }

    [BsonIgnoreExtraElements]
    public class Pharmacist
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Display(Name = "PharmacistId")]
        [BsonElement("PharmacistId")]
        [BsonRepresentation(BsonType.String)]
        public string PharmacistId { get; set; }

        [Display(Name = "TypeLogin")]
        [BsonElement("TypeLogin")]
        [BsonRepresentation(BsonType.String)]       // Mongo
        [JsonConverter(typeof(StringEnumConverter))]// Newtonsoft.Json
        public TypeLogin TypeLogin { get; set; } = TypeLogin.Phone; // [Phone, Email, Google, Facebook, Apple, Other]

        [Display(Name = "Confirm")]
        [BsonElement("Confirm")]
        [BsonRepresentation(BsonType.Boolean)]
        public bool Confirm { get; set; } = false;

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

        [Display(Name = "Password")]
        [BsonElement("Password")]
        [BsonRepresentation(BsonType.String)]
        public string Password { get; set; }



        [Display(Name = "TypePharmacist")]
        [BsonElement("TypePharmacist")]
        [BsonRepresentation(BsonType.String)]       // Mongo
        [JsonConverter(typeof(StringEnumConverter))]// Newtonsoft.Json
        public TypePharmacist TypePharmacist { get; set; } = TypePharmacist.User; // [Admin, User]

        [Display(Name = "Specialist")]
        [BsonElement("Specialist")]
        [BsonRepresentation(BsonType.String)]
        public string? Specialist { get; set; } // Đa Khoa, Tim Mạch, Da Liễu, Nội Tiết, Thần Kinh

        [Display(Name = "Firstname")]
        [BsonElement("Firstname")]
        [BsonRepresentation(BsonType.String)]
        public string? Firstname { get; set; }

        [Display(Name = "Lastname")]
        [BsonElement("Lastname")]
        [BsonRepresentation(BsonType.String)]
        public string? Lastname { get; set; }

        [Display(Name = "NamePharmacist")]
        [BsonElement("NamePharmacist")]
        [BsonRepresentation(BsonType.String)]
        public string? NamePharmacist { get; set; }

        [Display(Name = "Birthdate")]
        [BsonElement("Birthdate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Unspecified)]
        [BsonRepresentation(BsonType.String)]
        public DateTime? Birthdate { get; set; }

        [Display(Name = "Address")]
        [BsonElement("Address")]
        [BsonRepresentation(BsonType.String)]
        public string? Address { get; set; }

        [Display(Name = "HomePhone")]
        [BsonElement("HomePhone")]
        [BsonRepresentation(BsonType.String)]
        public string? HomePhone { get; set; }

        [Display(Name = "Avatar")]
        [BsonElement("Avatar")]
        [BsonRepresentation(BsonType.String)]
        public string? Avatar { get; set; }

        [Display(Name = "Race")]
        [BsonElement("Race")]
        [BsonRepresentation(BsonType.String)]
        public string? Race { get; set; }

        [Display(Name = "Gender")]
        [BsonElement("Gender")]
        [BsonRepresentation(BsonType.String)]
        public string? Gender { get; set; }

        [Display(Name = "Language")]
        [BsonElement("Language")]
        [BsonRepresentation(BsonType.String)]
        public string? Language { get; set; }

        [Display(Name = "WorkPlace")]
        [BsonElement("WorkPlace")]
        [BsonRepresentation(BsonType.String)]
        public string? WorkPlace { get; set; }



        [Display(Name = "Num")]
        [BsonElement("Num")]
        [BsonRepresentation(BsonType.String)]
        public double Num { get; set; } = 0;

        [Display(Name = "Exp")]
        [BsonElement("Exp")]
        [BsonRepresentation(BsonType.String)]
        public double Exp { get; set; } = 1;

        [Display(Name = "Rate")]
        [BsonElement("Rate")]
        [BsonRepresentation(BsonType.String)]
        public double Rate { get; set; } = 1.0;

        [Display(Name = "ReferralVideo")]
        [BsonElement("ReferralVideo")]
        [BsonRepresentation(BsonType.String)]
        public string? ReferralVideo { get; set; }

        [Display(Name = "Info")]
        [BsonElement("Info")]
        [BsonRepresentation(BsonType.String)]
        public string? Info { get; set; }



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
        public string? VideoCallId { get; set; }

        [Display(Name = "VideoCallToken")]
        [BsonElement("VideoCallToken")]
        [BsonRepresentation(BsonType.String)]
        public string? VideoCallToken { get; set; }

        [Display(Name = "Notes")]
        [BsonElement("Notes")]
        [BsonRepresentation(BsonType.String)]
        public string? Notes { get; set; }

        [Display(Name = "DynamicField")]
        [BsonElement("DynamicField")]
        [BsonRepresentation(BsonType.String)]
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
