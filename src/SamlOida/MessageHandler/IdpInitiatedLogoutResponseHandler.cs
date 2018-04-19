using SamlOida.Binding;
using SamlOida.MessageHandler.MessageFactory;
using SamlOida.Model;

namespace SamlOida.MessageHandler
{
    public class IdpInitiatedLogoutResponseHandler : OutgoingSamlMessageHandler<SamlLogoutResponseMessage>
    {
        protected override string FlowKey => SamlAuthenticationDefaults.SamlResponseKey;
        
        //Binding
        public IdpInitiatedLogoutResponseHandler(LogoutResponseFactory messageFactory, HttpRedirectBindingHandler binding) : base(messageFactory, binding)
        {
        }

    }
}
