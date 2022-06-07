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
    public class NewsDto
    {
        //[NotMapped]
        //[Newtonsoft.Json.JsonIgnore]
        //[System.Text.Json.Serialization.JsonIgnore]
        [Display(Name = "NewsId")]
        [BsonElement("NewsId")]
        [BsonRepresentation(BsonType.String)]
        [DefaultValue("")]
        public string? NewsId { get; set; }

        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [Display(Name = "Type")]
        [BsonElement("Type")]
        [BsonRepresentation(BsonType.String)]       // Mongo
        [JsonConverter(typeof(StringEnumConverter))]// Newtonsoft.Json
        public TypeNews Type { get; set; } = TypeNews.Trash; // [Actived, Trash]

        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [Display(Name = "TypeFor")]
        [BsonElement("TypeFor")]
        [BsonRepresentation(BsonType.String)]       // Mongo
        [JsonConverter(typeof(StringEnumConverter))]// Newtonsoft.Json
        public TypeForNews TypeFor { get; set; } = TypeForNews.Account; // [Account, Doctor]



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
