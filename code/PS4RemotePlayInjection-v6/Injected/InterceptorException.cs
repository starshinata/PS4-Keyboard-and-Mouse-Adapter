using System;

namespace PS4RemotePlayInjection.Injected
{
    //TODO WHY IS THIS EMPTY ?!
    public class InterceptorException : Exception
    {
        public InterceptorException()
        {
        }

        public InterceptorException(string message) : base(message)
        {
        }

        public InterceptorException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
