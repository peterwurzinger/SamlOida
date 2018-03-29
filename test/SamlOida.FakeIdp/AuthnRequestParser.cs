using System.Xml;

namespace SamlOida.FakeIdp
{
    public class AuthnRequestParser
    {


        public AuthnRequestParameters Parse(XmlDocument samlRequest)
        {
            var ns = new XmlNamespaceManager(samlRequest.NameTable);
            ns.AddNamespace("samlp", SamlAuthenticationDefaults.SamlProtocolNamespace);
            ns.AddNamespace("saml", SamlAuthenticationDefaults.SamlAssertionNamespace);

            var assertionConsumerUrl = samlRequest.SelectSingleNode("samlp:AuthnRequest/@AssertionConsumerServiceURL", ns);
            var requestId = samlRequest.SelectSingleNode("samlp:AuthnRequest/@ID", ns);

            return new AuthnRequestParameters
            {
                AssertionConsumerServiceURL = assertionConsumerUrl.InnerText,
                RequestId = requestId.InnerText
            };
        }

    }
}
