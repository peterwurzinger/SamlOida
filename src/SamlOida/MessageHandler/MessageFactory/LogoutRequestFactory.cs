using SamlOida.MessageHandler.Parser;
using SamlOida.Model;
using System.Xml;

namespace SamlOida.MessageHandler.MessageFactory
{
    public class LogoutRequestFactory : ISamlMessageFactory<SamlLogoutRequestMessage>
    {
        public XmlDocument CreateMessage(SamlOptions options, SamlLogoutRequestMessage message)
        {
            var doc = new XmlDocument();

            var logoutRequestElement = doc.CreateElement(SamlAuthenticationDefaults.SamlProtocolNsPrefix, "LogoutRequest", SamlAuthenticationDefaults.SamlProtocolNamespace);

            SamlXmlExtensions.PropagateStandardElements(doc, logoutRequestElement, message);

            var nameIdElement = doc.CreateElement(SamlAuthenticationDefaults.SamlAssertionNsPrefix, "NameID",
                SamlAuthenticationDefaults.SamlAssertionNamespace);

            nameIdElement.SetAttribute("SPNameQualifier", options.ServiceProviderEntityId);
            nameIdElement.SetAttribute("Format", "urn:oasis:names:tc:SAML:2.0:nameid-format:transient");

            //TODO
            nameIdElement.Value = "TODO";

            logoutRequestElement.AppendChild(nameIdElement);
            doc.AppendChild(logoutRequestElement);

            if (options.SignOutgoingMessages)
                XmlExtensions.SignDocument(doc, options);

            return doc;
        }
    }
}
