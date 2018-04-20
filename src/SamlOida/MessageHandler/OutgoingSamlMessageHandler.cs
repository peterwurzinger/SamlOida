using Microsoft.AspNetCore.Http;
using SamlOida.Binding;
using SamlOida.MessageHandler.MessageFactory;
using SamlOida.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SamlOida.MessageHandler
{
    public abstract class OutgoingSamlMessageHandler<TMessageContext>
        where TMessageContext : SamlMessage
    {
        protected ISamlMessageFactory<TMessageContext> MessageFactory;
        protected IEnumerable<ISamlBindingStrategy> Bindings;

        protected abstract string FlowKey { get; }

        protected OutgoingSamlMessageHandler(ISamlMessageFactory<TMessageContext> messageFactory, IEnumerable<ISamlBindingStrategy> bindings)
        {
            MessageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
            Bindings = bindings ?? throw new ArgumentNullException(nameof(bindings));
        }

        public void Handle(SamlOptions options, HttpContext context, TMessageContext messageContext, string target, SamlBindingBehavior bindingBehavior, string relayState = null)
        {
            //TODO: Extract creation and preparation of messageContext to a separate method.

            var document = MessageFactory.CreateMessage(options, messageContext);

            GetBinding(bindingBehavior).BindMessage(document, context, target, FlowKey, options, relayState);
        }

        protected virtual ISamlBindingStrategy GetBinding(SamlBindingBehavior bindingBehavior)
        {
            Type bindingType;
            switch (bindingBehavior)
            {
                case SamlBindingBehavior.HttpPost:
                    bindingType = typeof(HttpPostBindingHandler);
                    break;
                case SamlBindingBehavior.HttpRedirect:
                    bindingType = typeof(HttpRedirectBindingHandler);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(bindingBehavior));
            }

            return Bindings.Single(b => b.GetType() == bindingType);
        }
    }
}