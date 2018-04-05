using Microsoft.AspNetCore.Http;
using SamlOida.Binding;
using SamlOida.MessageHandler.Parser;
using SamlOida.Model;
using System;

namespace SamlOida.MessageHandler
{
    public abstract class IncomingSamlMessageHandler<THandlingResult, TMessageContext>
        where TMessageContext : SamlMessage
    {
        protected readonly ISamlMessageParser<TMessageContext> MessageParser;

        protected IncomingSamlMessageHandler(ISamlMessageParser<TMessageContext> messageParser)
        {
            MessageParser = messageParser ?? throw new ArgumentNullException(nameof(messageParser));
        }

        public THandlingResult Handle(SamlOptions options, HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var result = ExtractMessage(context);

            var messageContext = MessageParser.Parse(result.Message);

            Validate(options, result, messageContext);

            return HandleInternal(options, context, messageContext);
        }

        private static ExtractionResult ExtractMessage(HttpContext context)
        {
            if (HttpMethods.IsGet(context.Request.Method))
                return ExtractionHelper.ExtractHttpRedirect(context.Request.Query);

            if (HttpMethods.IsPost(context.Request.Method))
                return ExtractionHelper.ExtractHttpPost(context.Request.Form);

            //HTTP-Methods other than GET and POST are not supported
            throw new NotImplementedException();
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
