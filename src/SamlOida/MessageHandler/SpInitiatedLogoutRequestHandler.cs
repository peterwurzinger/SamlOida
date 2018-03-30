using SamlOida.Binding;
using SamlOida.MessageHandler.MessageFactory;
using SamlOida.Model;

namespace SamlOida.MessageHandler
{
    public class SpInitiatedLogoutRequestHandler : OutgoingSamlMessageHandler<SamlLogoutRequestMessage>
    {
        public SpInitiatedLogoutRequestHandler(ISamlMessageFactory<SamlLogoutRequestMessage> messageFactory, ISamlBindingStrategy binding) : base(messageFactory, binding)
        {
        }

        //TODO
    }
}
