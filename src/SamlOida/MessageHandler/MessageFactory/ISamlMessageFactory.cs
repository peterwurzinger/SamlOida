using System.Xml;

namespace SamlOida.MessageHandler.MessageFactory
{
    public interface ISamlMessageFactory<in TMessageContext>
    {
        XmlDocument CreateMessage(TMessageContext messageContext);
    }
}