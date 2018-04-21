using SamlOida.Binding;
using SamlOida.MessageHandler.MessageFactory;
using SamlOida.Model;
using System.Collections.Generic;

namespace SamlOida.MessageHandler
{
    public class SpInitiatedLogoutRequestHandler : OutgoingSamlMessageHandler<SamlLogoutRequestMessage>
    {
        protected override string FlowKey => SamlAuthenticationDefaults.SamlRequestKey;

        public SpInitiatedLogoutRequestHandler(LogoutRequestFactory messageFactory, IEnumerable<ISamlBindingStrategy> bindings) : base(messageFactory, bindings)
        {
        }

    }
}
