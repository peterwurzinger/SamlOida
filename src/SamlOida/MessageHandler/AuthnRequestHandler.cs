using SamlOida.Binding;
using SamlOida.MessageHandler.MessageFactory;
using SamlOida.Model;
using System.Collections.Generic;

namespace SamlOida.MessageHandler
{
    public class AuthnRequestHandler : OutgoingSamlMessageHandler<SamlAuthnRequestMessage>
    {
        protected override string FlowKey => SamlAuthenticationDefaults.SamlRequestKey;

        public AuthnRequestHandler(AuthnRequestFactory messageFactory, IEnumerable<ISamlBindingStrategy> bindings) : base(
            messageFactory, bindings)
        {
        }

    }
}
