using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sophie.Resource.Entities;
using Sophie.Units;

namespace Sophie.Resource.Dtos
{
    public class NotificationDto
    {
        //[NotMapped]
        //[Newtonsoft.Json.JsonIgnore]
        //[System.Text.Json.Serialization.JsonIgnore]
        [Display(Name = "NotificationId")]
        [BsonElement("NotificationId")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("")]
        public string? NotificationId { get; set; }

        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [Display(Name = "Type")]
        [BsonElement("Type")]
        [BsonRepresentation(BsonType.String)]       // Mongo
        [JsonConverter(typeof(StringEnumConverter))]// Newtonsoft.Json
        public TypeNotification Type { get; set; } = TypeNotification.Draft; // [Actived, Draft]

        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [Display(Name = "TypeFor")]
        [BsonElement("TypeFor")]
        [BsonRepresentation(BsonType.String)]       // Mongo
        [JsonConverter(typeof(StringEnumConverter))]// Newtonsoft.Json
        public TypeForNotification TypeFor { get; set; } = TypeForNotification.Account; // [Account, Doctor]

        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [Display(Name = "TypeSend")]
        [BsonElement("TypeSend")]
        [BsonRepresentation(BsonType.String)]       // Mongo
        [JsonConverter(typeof(StringEnumConverter))]// Newtonsoft.Json
        public TypeSendNotification TypeSend { get; set; } = TypeSendNotification.All; // [All, Group]



        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [BsonIgnore]
        [Display(Name = "UserId")]
        [BsonElement("UserId")]
        [BsonRepresentation(BsonType.String)]
        public List<string>? UserId { get; set; }

        [Display(Name = "Icon")]
        [BsonElement("Icon")]
        [BsonRepresentation(BsonType.String)]
        public string? Icon { get; set; }

        [Display(Name = "Images")]
        [BsonElement("Images")]
        [BsonRepresentation(BsonType.String)]
        public List<string>? Images { get; set; }

        [Display(Name = "Subject")]
        [BsonElement("Subject")]
        [BsonRepresentation(BsonType.String)]
        public string? Subject { get; set; }

        [Display(Name = "SubjectType")]
        [BsonElement("SubjectType")]
        [BsonRepresentation(BsonType.String)]
        public string? SubjectType { get; set; }

        [Display(Name = "DateTime")]
        [BsonElement("DateTime")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Unspecified)]
        [BsonRepresentation(BsonType.String)]
        public DateTime? DateTime { get; set; }

        [Display(Name = "Content")]
        [BsonElement("Content")]
        [BsonRepresentation(BsonType.String)]
        public string? Content { get; set; }



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
