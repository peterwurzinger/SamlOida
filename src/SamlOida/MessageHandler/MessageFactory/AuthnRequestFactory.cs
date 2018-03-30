using SamlOida.MessageHandler.Parser;
using SamlOida.Model;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace SamlOida.MessageHandler.MessageFactory
{
    public class AuthnRequestFactory : ISamlMessageFactory<SamlAuthnRequestMessage>
    {
        public XmlDocument CreateMessage(SamlOptions options, SamlAuthnRequestMessage authnRequestMessage)
        {
            var doc = new XmlDocument();

            var authnRequestElement = doc.CreateElement(SamlAuthenticationDefaults.SamlProtocolNsPrefix, "AuthnRequest", SamlAuthenticationDefaults.SamlProtocolNamespace);

            SamlXmlExtensions.PropagateStandardElements(doc, authnRequestElement, authnRequestMessage);

            authnRequestElement.SetAttribute("AssertionConsumerServiceURL", authnRequestMessage.AssertionConsumerServiceUrl);

            doc.AppendChild(authnRequestElement);

            //TODO: Extract this?
            if (options.SignRequest)
            {
                var signedXml = new SignedXml(doc)
                {
                    SigningKey = options.ServiceProviderCertificate.PrivateKey
                };
                var reference = new Reference("");
                var env = new XmlDsigEnvelopedSignatureTransform();
                reference.AddTransform(env);

                signedXml.AddReference(reference);
                signedXml.ComputeSignature();
                var signature = signedXml.GetXml();

                doc.DocumentElement.AppendChild(doc.ImportNode(signature, true));
            }

            return doc;
        }
    }
}
