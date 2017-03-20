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
            if (options.Value.SamlBindingOptions == null)
                throw new ArgumentNullException(nameof(options.Value.SamlBindingOptions));

            if (string.IsNullOrEmpty(options.Value.SamlBindingOptions.IdentityProviderSignOnUrl))
                throw new ArgumentNullException(nameof(options.Value.SamlBindingOptions.IdentityProviderSignOnUrl));

            if (string.IsNullOrEmpty(options.Value.ServiceProviderEntityId))
                throw new ArgumentNullException(nameof(options.Value.ServiceProviderEntityId));

            if (string.IsNullOrEmpty(Options.SignInScheme))
                Options.SignInScheme = sharedOptions.Value.SignInScheme;
        }
        
        protected override AuthenticationHandler<SamlOptions> CreateHandler()
        {
            return new SamlHandler(Options.SamlBindingOptions);
        }
    }

}
