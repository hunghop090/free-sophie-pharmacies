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
    public class AccountDto
    {
        //[NotMapped]
        //[Newtonsoft.Json.JsonIgnore]
        //[System.Text.Json.Serialization.JsonIgnore]
        [Display(Name = "AccountId")]
        [BsonElement("AccountId")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("")]
        public string? AccountId { get; set; }

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



        [Display(Name = "Firstname")]
        [BsonElement("Firstname")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("Trần Việt")]
        public string? Firstname { get; set; }

        [Display(Name = "Lastname")]
        [BsonElement("Lastname")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("Thức")]
        public string? Lastname { get; set; }

        [Display(Name = "Fullname")]
        [BsonElement("Fullname")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("Trần Việt Thức")]
        public string? Fullname { get; set; }

        [Display(Name = "Birthdate")]
        [BsonElement("Birthdate")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("1989-07-26T00:00:00")]
        public DateTime? Birthdate { get; set; }

        [Display(Name = "Address")]
        [BsonElement("Address")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("")]
        public string? Address { get; set; }

        [Display(Name = "HomePhone")]
        [BsonElement("HomePhone")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("")]
        public string? HomePhone { get; set; }

        [Display(Name = "Avatar")]
        [BsonElement("Avatar")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("")]
        public string? Avatar { get; set; }

        [Display(Name = "Race")]
        [BsonElement("Race")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("Kinh")]
        public string? Race { get; set; }

        [Display(Name = "Gender")]
        [BsonElement("Gender")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("Male")]
        public string? Gender { get; set; }

        [Display(Name = "Language")]
        [BsonElement("Language")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue(EnumLanguage.VI)]
        public string? Language { get; set; }



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
        public DateTime? Created { get; set; } = DateTimes.Now();

        [Display(Name = "Updated")]
        [BsonElement("Updated")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Unspecified)]
        [BsonRepresentation(BsonType.String)]
        public DateTime? Updated { get; set; } = DateTimes.Now();
    }
}
