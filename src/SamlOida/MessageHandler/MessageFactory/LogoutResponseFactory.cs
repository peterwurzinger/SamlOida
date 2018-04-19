using SamlOida.MessageHandler.Parser;
using SamlOida.Model;
using System.Xml;

namespace SamlOida.MessageHandler.MessageFactory
{
    public class LogoutResponseFactory : ISamlMessageFactory<SamlLogoutResponseMessage>
    {
        public XmlDocument CreateMessage(SamlOptions options, SamlLogoutResponseMessage message)
        {
            var doc = new XmlDocument();
            var logoutResponseElement = doc.CreateElement(SamlAuthenticationDefaults.SamlProtocolNsPrefix, "LogoutResponse", SamlAuthenticationDefaults.SamlProtocolNamespace);
            
            SamlXmlExtensions.PropagateStandardElements(logoutResponseElement, message);


            if (message.InResponseTo != null)
                logoutResponseElement.SetAttribute("InResponseTo", message.InResponseTo);

            var statusvalue = (message.Success)
                                    ? "urn:oasis:names:tc:SAML:2.0:status:Success"
                                    : "urn:oasis:names:tc:SAML:2.0:status:Responder";

            var statusElement = doc.CreateElement(SamlAuthenticationDefaults.SamlProtocolNsPrefix, "Status", SamlAuthenticationDefaults.SamlProtocolNamespace);

            var statusCodeElement = doc.CreateElement(SamlAuthenticationDefaults.SamlProtocolNsPrefix, "StatusCode", SamlAuthenticationDefaults.SamlProtocolNamespace);
            statusCodeElement.SetAttribute("Value", statusvalue);

            statusElement.AppendChild(statusCodeElement);
            logoutResponseElement.AppendChild(statusElement);
            doc.AppendChild(logoutResponseElement);

            if (options.SignOutgoingMessages)
                SamlXmlExtensions.SignElement(logoutResponseElement, options);

            return doc;
        }
    }
}
