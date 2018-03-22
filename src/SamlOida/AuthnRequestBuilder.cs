using System;
using System.Globalization;
using System.Xml;

namespace SamlOida
{
    public static class AuthnRequestBuilder
    {
        
        public static XmlDocument Build(string samlLogOnUri, string assertionConsumerUrl, string issuer)
        {
            var doc = new XmlDocument();
            
            var authnRequestElement = doc.CreateElement(SamlDefaults.SamlProtocolNsPrefix, "AuthnRequest", SamlDefaults.SamlProtocolNamespace);

            //Set namespaces explicitly avoiding to include namespace declarations in child elements
            authnRequestElement.SetAttribute($"xmlns:{SamlDefaults.SamlAssertionNsPrefix}", SamlDefaults.SamlAssertionNamespace);
            authnRequestElement.SetAttribute($"xmlns:{SamlDefaults.SamlProtocolNsPrefix}", SamlDefaults.SamlProtocolNamespace);

            //TODO: Use something else than Guid.NewGuid ?
            authnRequestElement.SetAttribute("ID", $"Issuer_{Guid.NewGuid()}");
            authnRequestElement.SetAttribute("Version", "2.0");
            
            //Does the Standardportal differ from SAML-Standard?
            authnRequestElement.SetAttribute("IssueInstant", DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture));

            authnRequestElement.SetAttribute("Destination", samlLogOnUri);
            authnRequestElement.SetAttribute("AssertionConsumerServiceURL", assertionConsumerUrl);

            var issuerElement = doc.CreateElement(SamlDefaults.SamlAssertionNsPrefix, "Issuer", SamlDefaults.SamlAssertionNamespace);
            issuerElement.InnerText = issuer;

            authnRequestElement.AppendChild(issuerElement);
            doc.AppendChild(authnRequestElement);

            return doc;
        }

    }
}
