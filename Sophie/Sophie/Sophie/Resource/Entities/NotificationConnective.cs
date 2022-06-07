using System;
using System.Collections.Generic;
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
    public class NotificationConnective
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Display(Name = "NotificationConnectiveId")]
        [BsonElement("NotificationConnectiveId")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("")]
        public string? NotificationConnectiveId { get; set; }

        [Display(Name = "NotificationId")]
        [BsonElement("NotificationId")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("")]
        public string? NotificationId { get; set; }

        [Display(Name = "AccountId")]
        [BsonElement("AccountId")]
        [BsonRepresentation(BsonType.String)]
        public string? AccountId { get; set; }

        [Display(Name = "DoctorId")]
        [BsonElement("DoctorId")]
        [BsonRepresentation(BsonType.String)]
        public string? DoctorId { get; set; }

        [Display(Name = "IsRead")]
        [BsonElement("IsRead")]
        [BsonRepresentation(BsonType.Boolean)]
        public bool IsRead { get; set; } = false;



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
