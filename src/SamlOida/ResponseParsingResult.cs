using System.Collections.Generic;
using System.Security.Principal;

namespace SamlOida
{
    internal class ResponseParsingResult
    {

        public ResponseParsingResult()
        {
            Attributes = new HashSet<SamlAttribute>();
        }

        public ISet<SamlAttribute> Attributes { get; }

        public bool Succeeded { get; set; }

        /// <summary>
        /// True if the SAML-Response has been signed
        /// </summary>
        public bool Signed { get; set; }

    }
}
