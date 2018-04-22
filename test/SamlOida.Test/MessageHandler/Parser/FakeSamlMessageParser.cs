using SamlOida.MessageHandler.Parser;
using SamlOida.Test.Model;
using System.Xml;

namespace SamlOida.Test.MessageHandler.Parser
{
    internal class FakeSamlMessageParserBase : ISamlMessageParser<FakeSamlMessage>
    {
        public bool ParseCalled { get; private set; }
        public FakeSamlMessage ParseResult { get; set; }
        public FakeSamlMessage Parse(XmlDocument message, SamlOptions options)
        {
            ParseCalled = true;
            return ParseResult ?? new FakeSamlMessage();
        }
    }
}