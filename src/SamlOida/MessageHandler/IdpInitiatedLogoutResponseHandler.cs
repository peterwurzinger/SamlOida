using SamlOida.Binding;
using SamlOida.MessageHandler.MessageFactory;
using SamlOida.Model;

namespace SamlOida.MessageHandler
{
    public class IdpInitiatedLogoutResponseHandler : OutgoingSamlMessageHandler<SamlLogoutResponseMessage>
    {
        public IdpInitiatedLogoutResponseHandler(ISamlMessageFactory<SamlLogoutResponseMessage> messageFactory, ISamlBindingStrategy binding) : base(messageFactory, binding)
        {
        }

        //TODO
    }
}
