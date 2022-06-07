using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Sophie.Resource.Model
{
    public class AppleLoginSocialCommand
    {
        [Required]
        [DefaultValue("eyJraWQiOiJZdXlYb1kiLCJhbGciOiJSUzI1NiJ9.eyJpc3MiOiJodHRwczovL2FwcGxlaWQuYXBwbGUuY29tIiwiYXVkIjoiY29tLmhlYWx0aGNhcmUuc29waGllLmRldiIsImV4cCI6MTYzNzk5MDQ3NiwiaWF0IjoxNjM3OTA0MDc2LCJzdWIiOiIwMDE5NzIuNDkxYWYxZGIxNmQ5NGVkZDhjZjU0MDg2ODNkMTVkZGMuMDQxNCIsImNfaGFzaCI6IlJ3b3lUaFM0NENFek5DVUE2TGI2Z0EiLCJlbWFpbCI6ImNudHRpdGszNkBnbWFpbC5jb20iLCJlbWFpbF92ZXJpZmllZCI6InRydWUiLCJhdXRoX3RpbWUiOjE2Mzc5MDQwNzYsIm5vbmNlX3N1cHBvcnRlZCI6dHJ1ZX0.O2eUzdgZi5qiolkfGA8R4mWdgea22NXBKVEOxlkALYim_fo-wtwHf6ncLoUAAVyMlNSg-sI_5z8fbhJSUrDMW1yOo-M2SaL4keQWcvnoGAb6edNF4k4-FmTH4EnIAmC7mU-XzthtYTXbfBRAkc1M_mKUxtMYwsopNoyZ1tEm98aFrxY5RNngBX-BLD_7id_C18I7C9VUCIK6PbD5QGQqEf0RpAZrPaktei69cm_wwCDzrDAQFcBAN-b0TFXUgnlG7GCo_cQgMnMTwGPfBtAj1rftMgJZJx_NsI7SEH7ESH7kYVdMV7TGj0jBg_4EKn9Ly_DCdRWX5P7UJx-8K189iw")]
        public string IdentityToken { get; set; }

        [Required]
        [DefaultValue("eyJraWQiOiJZdXlYb1kiLCJhbGciOiJSUzI1NiJ9.eyJpc3MiOiJodHRwczovL2FwcGxlaWQ")]
        public string? AuthorizationCode { get; set; }

        public AppleModel Profile { get; set; }
    }

    public class AppleTokenResponse
    {
        public AppleModel Profile { get; set; }
        public List<Claim> Claims { get; set; }
        public List<string> Error { get; set; }
        public bool IsExists { get; set; }
    }

    public class AppleModel
    {
        public string? Aud { get; set; }

        public long? Exp { get; set; }

        public long? Iat { get; set; }

        public string? Sub { get; set; }

        public string? Nonce { get; set; }

        public string? C_hash { get; set; }

        [DefaultValue("Trần Việt Thức")]
        public string? Name { get; set; }

        [DefaultValue("Trần Việt")]
        public string? Family_name { get; set; }

        [DefaultValue("Thức")]
        public string? Given_name { get; set; }

        [DefaultValue("")]
        public string? Picture { get; set; }

        [DefaultValue("cnttitk36@gmail.com")]
        public string? Email { get; set; }

        public bool? Email_verified { get; set; }

        [DefaultValue("+84389955141")]
        public string? Mobile { get; set; }

        [DefaultValue("+84")]
        public string? MobileCountryCode { get; set; }

        [DefaultValue("")]
        public string? Nationality { get; set; }

        public bool? Is_private_email { get; set; }

        public long? Auth_time { get; set; }

        public bool? Nonce_supported { get; set; }
    }
}
