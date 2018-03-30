using System.Collections.Generic;

namespace SamlOida.Model
{
    public class SamlAuthnResponseMessage : SamlMessage
    {
        internal bool Success { get; set; }
        internal bool IsSigned { get; set; }
        internal IEnumerable<SamlAssertion> Assertions { get; set; }
    }
}
