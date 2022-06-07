using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Claims;

namespace Sophie.Resource.Model
{
    public class GoogleSocialCommand
    {
        [DefaultValue("ya29.a0ARrdaM8gG02Uc1gf888Vh_NDl8s-X6TyrC7fxLAT4fpOrTZD2UxSVKax9dzf8DEivnaC1C0ywfM4Hhnc2ZCxWANoiUKeEzkyKJ93D9YStVsmU_iGA2PyRDJYvJPoNK2pYmkv9BAZTyYjP-TjV1Uvy2tRzyR8")]
        public string access_token { get; set; }

        [DefaultValue("")]
        public string id_token { get; set; }

        public GoogleModel Profile { get; set; }
    }

    public class GoogleTokenResponse
    {
        public GoogleModel Profile { get; set; }
        public List<Claim> Claims { get; set; }
        public List<string> Error { get; set; }
        public bool IsExists { get; set; }
    }

    public class GoogleModel
    {
        [DefaultValue("110717747109610451318")]
        public string? Id { get; set; }

        [DefaultValue("Trần Việt Thức")]
        public string? Name { get; set; }

        [DefaultValue("Trần Việt")]
        public string? Family_name { get; set; }

        [DefaultValue("Thức")]
        public string? Given_name { get; set; }

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
