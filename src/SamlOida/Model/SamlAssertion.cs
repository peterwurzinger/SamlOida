using System.Collections.Generic;

namespace SamlOida.Model
{
    internal class SamlAssertion
    {

        internal IEnumerable<SamlAttribute> Attributes { get; set; }

        internal bool IsSigned { get; set; }

    }
}
