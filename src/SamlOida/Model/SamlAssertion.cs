using System.Collections.Generic;

namespace SamlOida.Model
{
    internal class SamlAssertion
    {
        internal IEnumerable<SamlAttribute> Attributes { get; set; }

        internal string SessionIndex { get; set; }

        internal string Issuer { get; set; }

        internal string SubjectNameId { get; set; }

        internal bool IsSigned { get; set; }

    }
}
