using System;

namespace Sophie.Views
{
    public class ErrorMVCModel
    {
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string StatusCode { get; set; }
        public bool ShowSatusCode => !string.IsNullOrEmpty(StatusCode);
    }
}
