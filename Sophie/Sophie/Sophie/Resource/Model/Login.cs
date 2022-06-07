using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sophie.Resource.Entities;

namespace Sophie.Resource.Model
{
    public class Login
    {
        [Required]
        [Display(Name = "TypeLogin")]
        [JsonConverter(typeof(StringEnumConverter))]// Newtonsoft.Json
        public TypeLogin TypeLogin { get; set; } = TypeLogin.Phone; // [Phone, Email, Google, Facebook, Apple, Other]

        [Display(Name = "PhoneNumber")]
        [DefaultValue("+84389955141")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Email")]
        [DefaultValue("")]
        public string? Email { get; set; }

        [Display(Name = "Username")]
        [DefaultValue("")]
        public string? Username { get; set; }

        [DataType(DataType.Password)]
        [DefaultValue("10b8e822d03fb4fd946188e852a4c3e2")]
        public string Password { get; set; }

        [DefaultValue("")]
        public string? Code2FA { get; set; }
    }
}
