﻿using System;

namespace SamlOida
{
    internal class ParsingException : SamlException
    {
        internal ParsingException(string message) : base(message)
        {
        }

        internal ParsingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
