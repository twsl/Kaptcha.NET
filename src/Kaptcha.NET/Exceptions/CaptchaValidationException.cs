using System;
using System.Runtime.Serialization;

namespace KaptchaNET.Exceptions
{
    public class CaptchaValidationException : Exception
    {
        public bool InvalidResponse { get; set; } = true;

        public CaptchaValidationException() { }

        public CaptchaValidationException(string message) : base(message) { }

        public CaptchaValidationException(string message, Exception innerException) : base(message, innerException) { }

        protected CaptchaValidationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
