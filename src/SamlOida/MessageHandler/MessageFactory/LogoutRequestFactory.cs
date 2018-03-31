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

            //TODO: Implement Creation of LogoutRequest
            //<saml:NameID SPNameQualifier="http://sp.example.com/demo1/metadata.php" Format="urn:oasis:names:tc:SAML:2.0:nameid-format:transient">ONELOGIN_f92cc1834efc0f73e9c09f482fce80037a6251e7</saml:NameID>

            doc.AppendChild(logoutRequestElement);

            if (options.SignOutgoingMessages)
                XmlExtensions.SignDocument(doc, options);

            return doc;
        }
    }
}
