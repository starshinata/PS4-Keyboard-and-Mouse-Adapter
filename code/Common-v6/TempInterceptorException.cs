using System;
using System.Runtime.Serialization;

namespace Pizza.KeyboardAndMouseAdapter.Backend.Injection
{
    [Serializable]
    public class TempInterceptorException : Exception
    {
        public TempInterceptorException()
        {
        }

        public TempInterceptorException(string message) : base(message)
        {
        }

        public TempInterceptorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TempInterceptorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
