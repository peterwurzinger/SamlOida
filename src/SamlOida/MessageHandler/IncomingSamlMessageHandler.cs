using Microsoft.AspNetCore.Http;
using SamlOida.Binding;
using SamlOida.MessageHandler.Parser;
using SamlOida.Model;
using System;

namespace SamlOida.MessageHandler
{
    public abstract class IncomingSamlMessageHandler<THandlingResult, TMessageContext>
        where TMessageContext : SamlMessage, new()
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

            var result = ExtractMessage(context, options);

            var messageContext = MessageParser.Parse(result.Message, options);

            Validate(options, result, messageContext);

            return HandleInternal(options, context, messageContext, result.RelayState);
        }

        private static ExtractionResult ExtractMessage(HttpContext context, SamlOptions options)
        {
            if (HttpMethods.IsGet(context.Request.Method))
                return ExtractionHelper.ExtractHttpRedirect(context.Request.Query, options.IdentityProviderCertificate);

            if (HttpMethods.IsPost(context.Request.Method))
                return ExtractionHelper.ExtractHttpPost(context.Request.Form, options.IdentityProviderCertificate);

            //HTTP-Methods other than GET and POST are not supported
            throw new InvalidOperationException($"HTTP-Method {context.Request.Method} is not supported.");
        }

        protected internal virtual void Validate(SamlOptions options, ExtractionResult extractionResult,
            TMessageContext messageContext)
        {
            if (options.AcceptSignedMessagesOnly && (!extractionResult.HasValidSignature && !messageContext.HasValidSignature))
                throw new SamlException("Messages without or with invalid signatures are not accepted.");
        }

        protected internal abstract THandlingResult HandleInternal(SamlOptions options, HttpContext httpContext, TMessageContext messageContext, string relayState = null);
    }
}
