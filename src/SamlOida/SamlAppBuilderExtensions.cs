using Microsoft.AspNetCore.Authentication;
using System;

namespace SamlOida
{
    public static class SamlAppBuilderExtensions
    {
        public static AuthenticationBuilder AddSaml(this AuthenticationBuilder builder)
            => builder.AddSaml(SamlDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddSaml(this AuthenticationBuilder builder, Action<SamlOptions> configureOptions)
            => builder.AddSaml(SamlDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddSaml(this AuthenticationBuilder builder, string authenticationScheme, Action<SamlOptions> configureOptions)
            => builder.AddSaml(authenticationScheme, "SAML", configureOptions);

        public static AuthenticationBuilder AddSaml(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<SamlOptions> configureOptions)
        {
            //TODO: Add PostConfigureOptions
            //builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<SamlOptions>, SamlPostConfigureOptions>());
            return builder.AddRemoteScheme<SamlOptions, SamlHandler>(authenticationScheme, displayName, configureOptions);
        }

    }
}
