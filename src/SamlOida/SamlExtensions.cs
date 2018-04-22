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
        /// <summary>
        /// Adds the SAML-Handler with default authentication scheme name as authentication scheme and default options
        /// </summary>
        /// <param name="builder">The authentication builder obtained from <see cref="AuthenticationServiceCollectionExtensions.AddAuthentication(Microsoft.Extensions.DependencyInjection.IServiceCollection)"/></param>
        /// <returns>The configured authentication builder</returns>
        public static AuthenticationBuilder AddSaml(this AuthenticationBuilder builder)
            => builder.AddSaml(SamlAuthenticationDefaults.AuthenticationScheme, _ => { });

        /// <summary>
        /// Add the SAML-Handler with default authentication scheme name but custom options configurator
        /// </summary>
        /// <param name="builder">The authentication builder obtained from <see cref="AuthenticationServiceCollectionExtensions.AddAuthentication(Microsoft.Extensions.DependencyInjection.IServiceCollection)"/></param>
        /// <param name="configureOptions">The action to configure <see cref="SamlOptions"/></param>
        /// <returns>The configured authentication builder</returns>
        public static AuthenticationBuilder AddSaml(this AuthenticationBuilder builder, Action<SamlOptions> configureOptions)
            => builder.AddSaml(SamlAuthenticationDefaults.AuthenticationScheme, configureOptions);

        /// <summary>
        /// Add the SAML-Handler with custom authentication scheme name and custom options
        /// </summary>
        /// <param name="builder">The authentication builder obtained from <see cref="AuthenticationServiceCollectionExtensions.AddAuthentication(Microsoft.Extensions.DependencyInjection.IServiceCollection)"/></param>
        /// <param name="authenticationScheme">The authentication scheme name of the SAML-Handler instance</param>
        /// <param name="configureOptions">The action to configure <see cref="SamlOptions"/></param>
        /// <returns>The configured authentication builder</returns>
        public static AuthenticationBuilder AddSaml(this AuthenticationBuilder builder, string authenticationScheme, Action<SamlOptions> configureOptions)
            => builder.AddSaml(authenticationScheme, "SAML", configureOptions);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder">The authentication builder obtained from <see cref="AuthenticationServiceCollectionExtensions.AddAuthentication(Microsoft.Extensions.DependencyInjection.IServiceCollection)"/></param>
        /// <param name="authenticationScheme">The authentication scheme name of the SAML-Handler instance</param>
        /// <param name="displayName">The display name of this SAML-Handler instance</param>
        /// <param name="configureOptions">The action to configure <see cref="SamlOptions"/></param>
        /// <returns>The configured authentication builder</returns>
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
