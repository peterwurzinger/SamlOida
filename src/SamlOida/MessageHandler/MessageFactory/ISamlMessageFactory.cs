using System.Xml;
using SamlOida.Model;

namespace SamlOida.MessageHandler.MessageFactory
{
    public interface ISamlMessageFactory<in TMessageContext>
        where TMessageContext : SamlMessage
    {
        XmlDocument CreateMessage(SamlOptions options, TMessageContext message);
    }
}