using SamlOida.Binding;
using SamlOida.MessageHandler.MessageFactory;
using SamlOida.Model;

namespace SamlOida.MessageHandler
{
    public class IdpInitiatedLogoutResponseHandler : OutgoingSamlMessageHandler<SamlLogoutResponseMessage>
    {
        //TODO: Binding
        public IdpInitiatedLogoutResponseHandler(LogoutResponseFactory messageFactory, HttpPostBindingHandler binding) : base(messageFactory, binding)
        {
        }

        //TODO: Implement
    }
}
