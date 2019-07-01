using System;

namespace KaptchaNET
{
    public class CaptchaParameters
    {
        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public string IpAddress { get; set; }

        public Guid CaptchaId { get; set; }
    }
}
