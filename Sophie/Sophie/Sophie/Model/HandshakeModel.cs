using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Sophie.Model
{
    public enum TypeDeviceLanguage
    {
        VI,
        EN
    }

    public enum TypeOperatingSystem
    {
        ANDROID,
        IOS,
        WEB
    }

    public class HandshakeModel
    {
        [Required]
        [StringLength(maximumLength: 100, ErrorMessage = "Must be at least 1 to 100 characters.", MinimumLength = 1)]
        [DefaultValue("c96b1299d7af6611405443a4e5c3e1d8")]
        public string clientId { get; set; }

        [Required]
        [StringLength(maximumLength: 100, ErrorMessage = "Must be at least 1 to 100 characters.", MinimumLength = 1)]
        [DefaultValue("0c1c0cf67a95b973f2ff18b56e925517")]
        public string clientSecret { get; set; }

        [Required]
        [StringLength(maximumLength: 100, ErrorMessage = "Must be at least 1 to 100 characters.", MinimumLength = 1)]
        [DefaultValue("3.1.154.100")]
        public string xForwardedFor { get; set; }

        [Required]
        [DefaultValue("2021-11-20T00:00:00")]
        public DateTime xDateTime { get; set; }

        [Required]
        public string deviceId { get; set; }

        [Required]
        public string deviceName { get; set; }

        [Required]
        public string deviceLocation { get; set; }

        [Required]
        [DefaultValue("VI")]
        public TypeDeviceLanguage deviceLanguage { get; set; } = TypeDeviceLanguage.VI;

        [Required]
        [DefaultValue("ANDROID")]
        public TypeOperatingSystem operatingSystem { get; set; } = TypeOperatingSystem.ANDROID;
    }
}
