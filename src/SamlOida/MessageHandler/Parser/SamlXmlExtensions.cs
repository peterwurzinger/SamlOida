using SamlOida.Model;
using System;
using System.Globalization;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace SamlOida.MessageHandler.Parser
{
    public static class SamlXmlExtensions
    {

        private static XmlNamespaceManager _namespaceManager;
        public static XmlNamespaceManager NamespaceManager
        {
            get
            {
                if (_namespaceManager != null)
                    return _namespaceManager;

                //TODO: Lock?
                _namespaceManager = new XmlNamespaceManager(new NameTable());
                _namespaceManager.AddNamespace(SamlAuthenticationDefaults.SamlProtocolNsPrefix, SamlAuthenticationDefaults.SamlProtocolNamespace);
                _namespaceManager.AddNamespace(SamlAuthenticationDefaults.SamlAssertionNsPrefix, SamlAuthenticationDefaults.SamlAssertionNamespace);
                _namespaceManager.AddNamespace(SamlAuthenticationDefaults.XmlSignatureNsPrefix, SignedXml.XmlDsigNamespaceUrl);

                return _namespaceManager;
            }
        }

        internal static void PropagateStandardElements(XmlElement element, SamlMessage message)
        {
            element.SetAttribute($"xmlns:{SamlAuthenticationDefaults.SamlAssertionNsPrefix}", SamlAuthenticationDefaults.SamlAssertionNamespace);
            element.SetAttribute($"xmlns:{SamlAuthenticationDefaults.SamlProtocolNsPrefix}", SamlAuthenticationDefaults.SamlProtocolNamespace);

            //TODO: Use something else than Guid.NewGuid ?
            element.SetAttribute("ID", $"{Guid.NewGuid()}");
            element.SetAttribute("Version", "2.0");

            //Does the Standardportal differ from SAML-Standard?
            element.SetAttribute("IssueInstant", DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture));

            element.SetAttribute("Destination", message.Destination);

            var issuerElement = element.OwnerDocument.CreateElement(SamlAuthenticationDefaults.SamlAssertionNsPrefix, "Issuer", SamlAuthenticationDefaults.SamlAssertionNamespace);
            issuerElement.InnerText = message.Issuer;

            element.AppendChild(issuerElement);

        }

        internal static void ParseStandardElements(XmlElement element, SamlMessage message)
        {
            message.Id = element.GetAttribute("ID");

            var issueInstantNode = element.SelectSingleNode("@IssueInstant", NamespaceManager);
            if (issueInstantNode == null || string.IsNullOrEmpty(issueInstantNode.InnerText))
                throw new ParsingException("Attribute 'IssueInstant' missing");

            if (!DateTime.TryParse(issueInstantNode.InnerText, out var issueInstant))
                throw new ParsingException("Issue instant cannot be parsed");

            message.IssueInstant = issueInstant;

            message.Destination = element.GetAttribute("Destination");

            message.Issuer = element.GetElementsByTagName("Issuer", SamlAuthenticationDefaults.SamlAssertionNamespace)[0].InnerText;
        }

        

    }
}
