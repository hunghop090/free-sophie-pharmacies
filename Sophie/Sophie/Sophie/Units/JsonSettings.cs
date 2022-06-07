using System;
using System.Collections.Generic;
using App.Core.Units;

namespace Sophie.Units
{
    public static class JsonSettings
    {
        public static Newtonsoft.Json.JsonSerializerSettings SettingForNewtonsoft = new Newtonsoft.Json.JsonSerializerSettings
        {
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
            PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None,
            Formatting = Newtonsoft.Json.Formatting.None,
            Converters = new List<Newtonsoft.Json.JsonConverter> {
                new Newtonsoft.Json.Converters.StringEnumConverter(),
                new NewtonsoftJsonConverterDateTimeNullable()
            },
            ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
            DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc
        };

        public static Newtonsoft.Json.JsonSerializerSettings SettingForNewtonsoftPretty = new Newtonsoft.Json.JsonSerializerSettings
        {
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
            PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None,
            Formatting = Newtonsoft.Json.Formatting.Indented,
            Converters = new List<Newtonsoft.Json.JsonConverter> {
                new Newtonsoft.Json.Converters.StringEnumConverter(),
                new NewtonsoftJsonConverterDateTimeNullable()
            },
            ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
            DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc
        };
    }
}
