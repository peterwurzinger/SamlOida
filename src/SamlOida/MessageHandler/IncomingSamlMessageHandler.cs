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

        public THandlingResult Handle(HttpContext context)
        {
            var result = Binding.ExtractMessage(context);

            var messageContext = MessageParser.Parse(result.Message);

            Validate(messageContext);

            return HandleInternal(context, messageContext);
        }

        protected internal virtual void Validate(TMessageContext messageContext) { }

        protected internal abstract THandlingResult HandleInternal(HttpContext httpContext, TMessageContext messageContext);
    }
}
