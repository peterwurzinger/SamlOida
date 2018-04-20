using Microsoft.AspNetCore.Http;
using SamlOida.Binding;
using SamlOida.MessageHandler;
using SamlOida.Test.MessageHandler.MessageFactory;
using SamlOida.Test.Model;
using System.Collections.Generic;
using System.Linq;

namespace SamlOida.Test.MessageHandler
{
    internal class FakeOutgoingSamlMessageHandler : OutgoingSamlMessageHandler<FakeSamlMessage>
    {
        public FakeOutgoingSamlMessageHandler(FakeSamlMessageFactory factory, IEnumerable<ISamlBindingStrategy> bindings) : base(factory, bindings)
        {
        }

        public bool HandleCalled { get; private set; }
        public new void Handle(SamlOptions options, HttpContext context, FakeSamlMessage messageContext, string target,
            SamlBindingBehavior bindingBehavior, string relayState = null)
        {
            HandleCalled = true;
            base.Handle(options, context, messageContext, target, bindingBehavior, relayState);
        }

        protected override ISamlBindingStrategy GetBinding(SamlBindingBehavior bindingBehavior)
        {
            return Bindings.First();
        }

        public string FlowKeyToReturn { get; set; }
        protected override string FlowKey => FlowKeyToReturn ?? SamlAuthenticationDefaults.SamlRequestKey;
    }
}