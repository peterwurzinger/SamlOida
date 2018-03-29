using Microsoft.AspNetCore.Http;
using SamlOida.Binding;
using SamlOida.MessageHandler.MessageFactory;

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

        public void Handle(HttpContext context, TMessageContext messageContext, string relayState = null)
        {
            var document = MessageFactory.CreateMessage(messageContext);

            Binding.SendMessage(context, document, relayState);
        }
    }
}