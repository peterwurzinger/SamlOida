using Microsoft.AspNetCore.Http;
using SamlOida.Binding;
using SamlOida.MessageHandler;
using SamlOida.MessageHandler.Parser;
using SamlOida.Test.Model;

namespace SamlOida.Test.MessageHandler
{
    internal class FakeIncomingSamlMessageHandler : IncomingSamlMessageHandler<object, FakeSamlMessage>
    {
        public FakeIncomingSamlMessageHandler(ISamlMessageParser<FakeSamlMessage> messageParserBase) : base(messageParserBase)
        {
        }

        public bool HandleCalled { get; private set; }
        public new object Handle(SamlOptions options, HttpContext context)
        {
            HandleCalled = true;
            return base.Handle(options, context);
        }

        public bool ValidateCalled { get; private set; }
        protected internal override void Validate(SamlOptions options, ExtractionResult extractionResult,
            FakeSamlMessage messageContext)
        {
            ValidateCalled = true;
            base.Validate(options, extractionResult, messageContext);
        }

        public bool HandleInternalCalled { get; private set; }
        protected internal override object HandleInternal(SamlOptions options, HttpContext httpContext, FakeSamlMessage messageContext,
            string relayState = null)
        {
            HandleInternalCalled = true;
            return new object();
        }
    }
}