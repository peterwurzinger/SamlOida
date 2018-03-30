using Microsoft.AspNetCore.Http;
using SamlOida.Binding;
using SamlOida.MessageHandler.Parser;

namespace SamlOida.MessageHandler
{
    public abstract class IncomingSamlMessageHandler<THandlingResult, TMessageContext>
    {
        protected internal ISamlMessageParser<TMessageContext> MessageParser { get; }

        protected internal ISamlBindingStrategy Binding { get; }

        protected IncomingSamlMessageHandler(ISamlMessageParser<TMessageContext> messageParser, ISamlBindingStrategy binding)
        {
            MessageParser = messageParser;
            Binding = binding;
        }

        public THandlingResult Handle(SamlOptions options, HttpContext context)
        {
            var result = Binding.ExtractMessage(context);

            var messageContext = MessageParser.Parse(result.Message);

            Validate(options, result, messageContext);

            return HandleInternal(options, context, messageContext);
        }

        protected internal virtual void Validate(SamlOptions options, ExtractionResult extractionResult,
            TMessageContext messageContext)
        {
            //TODO: Check if  Incoming messages need to be signed
            //TODO: Check contained signature - either in ExtractionResult or embedded in XmlDocument
        }

        protected internal abstract THandlingResult HandleInternal(SamlOptions options, HttpContext httpContext, TMessageContext messageContext);
    }
}
