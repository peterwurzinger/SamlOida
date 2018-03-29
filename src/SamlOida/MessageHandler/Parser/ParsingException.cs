using System;

namespace SamlOida.MessageHandler.Parser
{
    internal class ParsingException : SamlException
    {
        internal ParsingException()
        {

        }

        internal ParsingException(string message) : base(message)
        {
        }

        internal ParsingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
