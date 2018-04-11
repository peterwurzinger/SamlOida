using Microsoft.AspNetCore.Http;
using SamlOida.Binding;
using SamlOida.MessageHandler.MessageFactory;
using SamlOida.Model;
using System;

namespace SamlOida.MessageHandler
{
    public abstract class OutgoingSamlMessageHandler<TMessageContext>
        where TMessageContext : SamlMessage
    {
        protected ISamlMessageFactory<TMessageContext> MessageFactory;
        protected ISamlBindingStrategy Binding;

        protected abstract string FlowKey { get; }

        protected OutgoingSamlMessageHandler(ISamlMessageFactory<TMessageContext> messageFactory, ISamlBindingStrategy binding)
        {
            MessageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
            Binding = binding ?? throw new ArgumentNullException(nameof(binding));
        }

        public void Handle(SamlOptions options, HttpContext context, TMessageContext messageContext, string target, string relayState = null)
        {
            //TODO: Extract creation and preparation of messageContext to a separate method.

            var document = MessageFactory.CreateMessage(options, messageContext);

            Binding.BindMessage(document, context, target, FlowKey, options, relayState);
        }
    }
}