using System;

namespace SamlOida
{
    public class SamlException : Exception
    {

        internal SamlException(string message) : base(message)
        {
            
        }

        internal SamlException(string message, Exception innerException) : base(message, innerException)
        {
            
        }

    }
}
