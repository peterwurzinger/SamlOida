using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SamlOida.Binding;
using SamlOida.MessageHandler.Parser;
using SamlOida.Model;
using System;

namespace SamlOida.MessageHandler
{
    public class AuthnResponseHandler : IncomingSamlMessageHandler<AuthnResultContext, SamlAuthnResponseMessage>
    {
        private readonly IOptionsMonitor<SamlOptions> _options;

        //TODO: HttpPostBinding isn't always sufficient
        public AuthnResponseHandler(AuthnResponseParser messageParser, HttpPostBindingHandler binding, IOptionsMonitor<SamlOptions> options)
            : base(messageParser, binding)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        protected internal override void Validate(SamlAuthnResponseMessage messageContext)
        {
            if ((DateTime.UtcNow - messageContext.IssueInstant) > _options.CurrentValue.IssueInstantExpiration)
                throw new SamlException("Issue instant is too long ago.");
        }

        protected internal override AuthnResultContext HandleInternal(HttpContext httpContext, SamlAuthnResponseMessage messageContext)
        {
            return new AuthnResultContext
            {
                //TODO
            };
        }
    }

    public class AuthnResultContext
    {
        //TODO:
    }
}
