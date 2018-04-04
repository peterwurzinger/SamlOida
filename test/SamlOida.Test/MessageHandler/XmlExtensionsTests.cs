using SamlOida.MessageHandler;
using System.Security.Cryptography.Xml;
using System.Xml;
using Xunit;

namespace SamlOida.Test.MessageHandler
{
    public class XmlExtensionsTests
    {

        [Fact]
        public void RemoveSignatureShouldRemoveSignatures()
        {
            var xmlDoc = new XmlDocument();
            var rootElement = xmlDoc.CreateElement("Message");
            var signatureElement = xmlDoc.CreateElement("ds", "Signature", SignedXml.XmlDsigNamespaceUrl);
            rootElement.AppendChild(signatureElement);
            xmlDoc.AppendChild(rootElement);

            xmlDoc.RemoveSignature();

            Assert.Null(signatureElement.ParentNode); 
            Assert.False(rootElement.HasChildNodes);
        }
    }
}