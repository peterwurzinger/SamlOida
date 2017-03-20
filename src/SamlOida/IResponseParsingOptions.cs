using System;

namespace SamlOida
{
    internal interface IResponseParsingOptions
    {
        TimeSpan IssueInstantExpiration { get; set; }
        bool AcceptSignedAssertionsOnly { get; set; }
    }
}