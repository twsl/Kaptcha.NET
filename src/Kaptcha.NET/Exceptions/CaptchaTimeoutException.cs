using System;
using System.Runtime.Serialization;

namespace KaptchaNET.Exceptions
{
    public class CaptchaTimeoutException : CaptchaValidationException
    {
        public CaptchaTimeoutException()
            : this("Captcha timeout exception")
        {
            InvalidResponse = false;
        }

        public CaptchaTimeoutException(string message) : base(message)
        {
            InvalidResponse = false;
        }

        public CaptchaTimeoutException(string message, Exception innerException) : base(message, innerException)
        {
            InvalidResponse = false;
        }

        protected CaptchaTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            InvalidResponse = false;
        }
    }
}
