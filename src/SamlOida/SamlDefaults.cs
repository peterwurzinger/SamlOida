﻿namespace SamlOida
{
    public static class SamlDefaults
    {
        public const string SamlProtocolNsPrefix = "samlp";
        public const string SamlAssertionNsPrefix = "saml";

        public const string SamlProtocolNamespace = "urn:oasis:names:tc:SAML:2.0:protocol";
        public const string SamlAssertionNamespace = "urn:oasis:names:tc:SAML:2.0:assertion";

        public const string AuthenticationScheme = "Saml";

        //Uppercase is important!
        public const string SamlRequestQueryStringKey = "SAMLRequest";
        public const string SamlResponseQueryStringKey = "SAMLResponse";
        public const string RelayStateQueryStringKey = "RelayState";

    }
}
