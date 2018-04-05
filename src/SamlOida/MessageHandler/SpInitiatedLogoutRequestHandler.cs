using SamlOida.Binding;
using SamlOida.MessageHandler.MessageFactory;
using SamlOida.Model;

namespace SamlOida.MessageHandler
{
    public class SpInitiatedLogoutRequestHandler : OutgoingSamlMessageHandler<SamlLogoutRequestMessage>
    {
        protected override string FlowKey => SamlAuthenticationDefaults.SamlRequestKey;

        //TODO: Binding
        public SpInitiatedLogoutRequestHandler(LogoutRequestFactory messageFactory, HttpRedirectBindingHandler binding) : base(messageFactory, binding)
        {
        }

        //TODO Implement
    }
}
