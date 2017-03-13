﻿using System;
using System.Xml;

namespace SamlOida
{
    public static class AuthnRequestBuilder
    {

        //TODO: Extract those to SamlOptions
        private const string SamlLogOff = "https://logoff-uri.com";
        public const string SamlLogOn = "https://logon-uri.com";
        private const string Issuer = "https://issuer.com";

        public static XmlDocument Build(string assertionConsumerUrl)
        {
            var doc = new XmlDocument();
            
            var authnRequestElement = doc.CreateElement(SamlDefaults.SamlProtocolNsPrefix, "AuthnRequest", SamlDefaults.SamlProtocolNamespace);

            //Set namespaces explicitly avoiding to include namespace declarations in child elements
            authnRequestElement.SetAttribute($"xmlns:{SamlDefaults.SamlAssertionNsPrefix}", SamlDefaults.SamlAssertionNamespace);
            authnRequestElement.SetAttribute($"xmlns:{SamlDefaults.SamlProtocolNsPrefix}", SamlDefaults.SamlProtocolNamespace);

            //TODO: Use something else than Guid.NewGuid ?
            authnRequestElement.SetAttribute("ID", $"Issuer_{Guid.NewGuid()}");
            authnRequestElement.SetAttribute("Version", "2.0");
            authnRequestElement.SetAttribute("IssueInstant", DateTime.UtcNow.ToString("u"));
            authnRequestElement.SetAttribute("Destination", SamlLogOn);
            authnRequestElement.SetAttribute("AssertionConsumerServiceURL", assertionConsumerUrl);

            var issuerElement = doc.CreateElement(SamlDefaults.SamlAssertionNsPrefix, "Issuer", SamlDefaults.SamlAssertionNamespace);
            issuerElement.InnerText = Issuer;

            authnRequestElement.AppendChild(issuerElement);
            doc.AppendChild(authnRequestElement);

            return doc;
        }

    }
}
