using System.Xml;

namespace SamlOida.MessageHandler.Parser
{
    public interface ISamlMessageParser<out TMessageContext>
    {
        TMessageContext Parse(XmlDocument message);
    }
}