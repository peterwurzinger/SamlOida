using System.Xml;
using SamlOida.MessageHandler.MessageFactory;
using SamlOida.Test.Model;

namespace SamlOida.Test.MessageHandler.MessageFactory
{
    internal class FakeSamlMessageFactory : ISamlMessageFactory<FakeSamlMessage>
    {
        public XmlDocument DocumentToReturn { get; set; }
        public bool CreateMessageCalled { get; private set; }
        public XmlDocument CreateMessage(SamlOptions options, FakeSamlMessage message)
        {
            CreateMessageCalled = true;
            return DocumentToReturn ?? new XmlDocument();
        }
    }
}