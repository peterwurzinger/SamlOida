using SamlOida.Model;
using System.Xml;

namespace SamlOida.MessageHandler.Parser
{
    public interface ISamlMessageParser<out TMessageContext>
        where TMessageContext : SamlMessage
    {
        TMessageContext Parse(XmlDocument message);
    }
}