using Microsoft.AspNetCore.Http;
using SamlOida.MessageHandler;
using SamlOida.Test.Binding;
using SamlOida.Test.MessageHandler.MessageFactory;
using SamlOida.Test.Model;

namespace SamlOida.Test.MessageHandler
{
    internal class FakeOutgoingSamlMessageHandler : OutgoingSamlMessageHandler<FakeSamlMessage>
    {
        public FakeOutgoingSamlMessageHandler(FakeSamlMessageFactory factory, FakeBinding binding) : base(factory, binding)
        {
        }

        public bool HandleCalled { get; private set; }
        public new void Handle(SamlOptions options, HttpContext context, FakeSamlMessage messageContext, string target,
            string relayState = null)
        {
            HandleCalled = true;
            base.Handle(options, context, messageContext, target, relayState);
        }

        public string FlowKeyToReturn { get; set; }
        protected override string FlowKey => FlowKeyToReturn ?? SamlAuthenticationDefaults.SamlRequestKey;
    }
}