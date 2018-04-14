namespace SamlOida
{
    public static class SamlAuthenticationDefaults
    {
        public const string SamlProtocolNsPrefix = "samlp";
        public const string SamlAssertionNsPrefix = "saml";
        public const string XmlSignatureNsPrefix = "ds";

        public const string SamlProtocolNamespace = "urn:oasis:names:tc:SAML:2.0:protocol";
        public const string SamlAssertionNamespace = "urn:oasis:names:tc:SAML:2.0:assertion";

        public const string DeflateEncoding = "urn:oasis:names:tc:SAML:2.0:bindings:URL-Encoding:DEFLATE";

        public const string AuthenticationScheme = "Saml";

        //Uppercase is important!
        public const string SamlRequestKey = "SAMLRequest";
        public const string SamlResponseKey = "SAMLResponse";
        public const string RelayStateKey = "RelayState";
        public const string SamlEncodingKey = "SAMLEncoding";
        public const string SignatureKey = "Signature";
        public const string SignatureAlgorithmKey = "SigAlg";

        public const string SessionIndexClaimType = "http://samloida.com/schemas/claims/sessionIndex";

    }
}
