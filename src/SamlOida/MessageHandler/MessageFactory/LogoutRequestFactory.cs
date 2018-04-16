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

            SamlXmlExtensions.PropagateStandardElements(logoutRequestElement, message);

            var nameIdElement = doc.CreateElement(SamlAuthenticationDefaults.SamlAssertionNsPrefix, "NameID",
                SamlAuthenticationDefaults.SamlAssertionNamespace);
            nameIdElement.InnerText = message.NameId;

            var sessionIndexElement = doc.CreateElement(SamlAuthenticationDefaults.SamlProtocolNsPrefix, "SessionIndex",
                SamlAuthenticationDefaults.SamlProtocolNamespace);
            sessionIndexElement.InnerText = message.SessionIndex;


            logoutRequestElement.AppendChild(nameIdElement);
            logoutRequestElement.AppendChild(sessionIndexElement);

            doc.AppendChild(logoutRequestElement);

            if (options.SignOutgoingMessages)
                SamlXmlExtensions.SignElement(logoutRequestElement, options);

            return doc;
        }
    }
}
