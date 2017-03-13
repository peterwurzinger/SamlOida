using System;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SamlOida
{
    public class SamlMiddleware : AuthenticationMiddleware<SamlOptions>
    {

        public SamlMiddleware(RequestDelegate next, IOptions<SamlOptions> options, ILoggerFactory loggerFactory, UrlEncoder encoder, IOptions<SharedAuthenticationOptions> sharedOptions) : base(next, options, loggerFactory, encoder)
        {
            if (string.IsNullOrEmpty(options.Value.LogOnUrl))
                throw new ArgumentNullException(nameof(options.Value.LogOnUrl));

            if (string.IsNullOrEmpty(Options.SignInScheme))
                Options.SignInScheme = sharedOptions.Value.SignInScheme;
        }
        
        protected override AuthenticationHandler<SamlOptions> CreateHandler()
        {
            //TODO: Differentiate between GET- and POST-Profile based on SAML-Options
            return new SamlHandler();
        }
    }

}
