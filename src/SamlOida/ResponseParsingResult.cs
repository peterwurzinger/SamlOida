using System.Security.Principal;

namespace SamlOida
{
    public class ResponseParsingResult
    {
        /// <summary>
        /// The identity parsed from the SAML-Response
        /// </summary>
        public IIdentity Identity { get; set; }

        public bool Succeeded { get; set; }

        /// <summary>
        /// True if the SAML-Response has been signed
        /// </summary>
        public bool Signed { get; set; }

    }
}
