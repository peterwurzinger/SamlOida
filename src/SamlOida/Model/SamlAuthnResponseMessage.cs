using System;
using System.Collections.Generic;

namespace SamlOida.Model
{
    public class SamlAuthnResponseMessage
    {
        internal bool Success { get; set; }
        internal DateTime IssueInstant { get; set; }
        internal bool IsSigned { get; set; }
        internal IEnumerable<SamlAssertion> Assertions { get; set; }
    }
}
