using Microsoft.AspNetCore.Http;
using SamlOida.Binding;
using SamlOida.MessageHandler.MessageFactory;
using System;

namespace SamlOida.MessageHandler
{
    public abstract class OutgoingSamlMessageHandler<TMessageContext>
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
            var document = MessageFactory.CreateMessage(options, messageContext);

            Binding.SendMessage(options, context, document, target, relayState);
        }
    }
}