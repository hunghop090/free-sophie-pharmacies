using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Sophie.Resource.Model
{
    public class FacebookLoginSocialCommand
    {
        [Required]
        public string access_token { get; set; }
        public FacebookModel Profile { get; set; }
    }

    public class FacebookTokenResponse
    {
        public FacebookModel Profile { get; set; }
        public List<Claim> Claims { get; set; }
        public List<string> Error { get; set; }
        public bool IsExists { get; set; }
    }

    public class FacebookModel
    {
        [DefaultValue("110717747109610451318")]
        public string? Id { get; set; }

        [DefaultValue("Trần Việt Thức")]
        public string? Name { get; set; }

        [DefaultValue("Trần Việt")]
        public string? First_name { get; set; }

        [DefaultValue("Thức")]
        public string? Last_name { get; set; }

        [DefaultValue("")]
        public string? Picture { get; set; }

        [DefaultValue("")]
        public string? Locale { get; set; }

        [DefaultValue("cnttitk36@gmail.com")]
        public string? Email { get; set; }

        [DefaultValue("+84389955141")]
        public string? Mobile { get; set; }

        [DefaultValue("+84")]
        public string? MobileCountryCode { get; set; }

        [DefaultValue("")]
        public string? Nationality { get; set; }
    }
}
