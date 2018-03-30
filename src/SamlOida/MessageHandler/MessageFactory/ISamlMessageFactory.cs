using System.Xml;

namespace SamlOida.MessageHandler.MessageFactory
{
    public interface ISamlMessageFactory<in TMessageContext>
    {
        XmlDocument CreateMessage(SamlOptions options, TMessageContext messageContext);
    }
}