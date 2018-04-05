using Microsoft.AspNetCore.Http;
using SamlOida.Binding;
using SamlOida.MessageHandler.MessageFactory;
using System;
using SamlOida.Model;

namespace SamlOida.MessageHandler
{
    public abstract class OutgoingSamlMessageHandler<TMessageContext>
        where TMessageContext : SamlMessage
    {
        protected internal ISamlMessageFactory<TMessageContext> MessageFactory { get; }

        protected internal ISamlBindingStrategy Binding { get; }

        protected OutgoingSamlMessageHandler(ISamlMessageFactory<TMessageContext> messageFactory, ISamlBindingStrategy binding)
        {
            MessageFactory = messageFactory;
            Binding = binding;
        }

        public void Handle(SamlOptions options, HttpContext context, TMessageContext messageContext, Uri target, string relayState = null)
        {
            //TODO: Extract creation and preparation of messageContext to a separate method.

            var document = MessageFactory.CreateMessage(options, messageContext);

            Binding.BindMessage(document, context, target, options, relayState);
        }
    }
}