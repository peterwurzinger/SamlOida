using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using SamlOida.Binding;
using SamlOida.MessageHandler;
using SamlOida.MessageHandler.MessageFactory;
using SamlOida.MessageHandler.Parser;

namespace SamlOida
{
    public static class SamlExtensions
    {
        public static AuthenticationBuilder AddSaml(this AuthenticationBuilder builder)
            => builder.AddSaml(SamlAuthenticationDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddSaml(this AuthenticationBuilder builder, Action<SamlOptions> configureOptions)
            => builder.AddSaml(SamlAuthenticationDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddSaml(this AuthenticationBuilder builder, string authenticationScheme, Action<SamlOptions> configureOptions)
            => builder.AddSaml(authenticationScheme, "SAML", configureOptions);

        public static AuthenticationBuilder AddSaml(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<SamlOptions> configureOptions)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<SamlOptions>, PostConfigureSamlOptions>());


            builder.Services.AddSingleton<AuthnRequestHandler>();
            builder.Services.AddSingleton<AuthnResponseHandler>();

            builder.Services.AddSingleton<HttpPostBindingHandler>();
            builder.Services.AddSingleton<HttpRedirectBindingHandler>();

            builder.Services.AddSingleton<AuthnRequestFactory>();

            builder.Services.AddSingleton<AuthnResponseParser>();

            return builder.AddRemoteScheme<SamlOptions, SamlHandler>(authenticationScheme, displayName, configureOptions);
        }

    }
}
