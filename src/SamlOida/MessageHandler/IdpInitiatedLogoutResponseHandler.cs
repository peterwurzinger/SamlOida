using SamlOida.Binding;
using SamlOida.MessageHandler.MessageFactory;
using SamlOida.Model;
using System.Collections.Generic;

namespace SamlOida.MessageHandler
{
    public class IdpInitiatedLogoutResponseHandler : OutgoingSamlMessageHandler<SamlLogoutResponseMessage>
    {
        protected override string FlowKey => SamlAuthenticationDefaults.SamlResponseKey;

        public IdpInitiatedLogoutResponseHandler(LogoutResponseFactory messageFactory, IEnumerable<ISamlBindingStrategy> bindings) : base(messageFactory, bindings)
        {
        }

    }
}
