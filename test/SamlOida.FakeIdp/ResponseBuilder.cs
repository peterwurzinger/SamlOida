using System;
using System.Xml;

namespace SamlOida.FakeIdp
{
    public class ResponseBuilder
    {

        public XmlDocument Build(AuthnRequestParameters parameters)
        {
            var doc = new XmlDocument();

            var responseElement = doc.CreateElement("samlp", "Response", SamlAuthenticationDefaults.SamlProtocolNamespace);

            var issueInstantAttribute = doc.CreateAttribute("IssueInstant");
            issueInstantAttribute.InnerText = DateTime.Now.ToString("u");
            responseElement.Attributes.Append(issueInstantAttribute);

            var destinationAttribute = doc.CreateAttribute("Destination");
            destinationAttribute.InnerText = parameters.AssertionConsumerServiceURL;
            responseElement.Attributes.Append(destinationAttribute);

            var inResponseToAttribute = doc.CreateAttribute("InResponseTo");
            inResponseToAttribute.InnerText = parameters.RequestId;
            responseElement.Attributes.Append(inResponseToAttribute);

            var versionAttribute = doc.CreateAttribute("Version");
            versionAttribute.InnerText = "2.0";
            responseElement.Attributes.Append(versionAttribute);

            var statusElement = doc.CreateElement("samlp", "Status", SamlAuthenticationDefaults.SamlProtocolNamespace);
            var statusCodeElement = doc.CreateElement("samlp", "StatusCode", SamlAuthenticationDefaults.SamlProtocolNamespace);
            statusCodeElement.InnerText = "urn:oasis:names:tc:SAML:2.0:status:Success";
            statusElement.AppendChild(statusCodeElement);

            var assertionElement = doc.CreateElement("saml", "Assertion", SamlAuthenticationDefaults.SamlAssertionNamespace);
            assertionElement.Attributes.Append(issueInstantAttribute);
            assertionElement.Attributes.Append(versionAttribute);

            responseElement.AppendChild(assertionElement);
            responseElement.AppendChild(statusElement);

            doc.AppendChild(responseElement);
            return doc;
        }
    }
}
