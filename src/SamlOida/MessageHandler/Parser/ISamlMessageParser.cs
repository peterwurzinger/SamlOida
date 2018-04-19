using System.Xml;
using SamlOida.Model;

namespace SamlOida.MessageHandler.Parser
{
    public interface ISamlMessageParser<out TMessageContext>
        where TMessageContext : SamlMessage, new()
    {
        TMessageContext Parse(XmlDocument message, SamlOptions options);
    }
}