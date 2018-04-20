using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using SamlOida.Binding;
using SamlOida.MessageHandler;
using SamlOida.MessageHandler.MessageFactory;
using SamlOida.MessageHandler.Parser;
using System;

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

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ISamlBindingStrategy, HttpPostBindingHandler>());
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ISamlBindingStrategy, HttpRedirectBindingHandler>());

            builder.Services.AddScoped<AuthnRequestHandler>();
            builder.Services.AddScoped<AuthnResponseHandler>();
            builder.Services.AddScoped<IdpInitiatedLogoutResponseHandler>();
            builder.Services.AddScoped<IdpInitiatedLogoutRequestHandler>();
            builder.Services.AddScoped<SpInitiatedLogoutRequestHandler>();
            builder.Services.AddScoped<SpInitiatedLogoutResponseHandler>();

            builder.Services.AddSingleton<AuthnRequestFactory>();
            builder.Services.AddSingleton<LogoutRequestFactory>();
            builder.Services.AddSingleton<LogoutResponseFactory>();

            builder.Services.AddSingleton<AuthnResponseParser>();
            builder.Services.AddSingleton<LogoutRequestParser>();
            builder.Services.AddSingleton<LogoutResponseParser>();

            return builder.AddRemoteScheme<SamlOptions, SamlHandler>(authenticationScheme, displayName, configureOptions);
        }

    }
}
