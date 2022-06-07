using System;

namespace Sophie.Model
{
    public enum TypeAction
    {
        Load,
        Update,
        Delete
    }

    public enum TypeStatus
    {
        Success,
        Failure
    }

    public class LogUserEvent
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string Datetime { get; set; }

        public TypeAction Action { get; set; }
        public string Description { get; set; }
        public TypeStatus Status { get; set; }
        public string DeviceName { get; set; }
        public string DeviceId { get; set; }
        public string DeviceLocation { get; set; }
        public string DeviceLanguage { get; set; }
        public string DataJson { get; set; }
    }
}
