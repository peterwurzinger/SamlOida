using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace SamlOida
{
    public static class SamlAppBuilderExtensions
    {
        public static IApplicationBuilder UseSaml(this IApplicationBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.UseMiddleware<SamlMiddleware>();
        }

        public static IApplicationBuilder UseSaml(this IApplicationBuilder builder, SamlOptions options)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            //My father, my father, he's touching me now!
            return builder.UseMiddleware<SamlMiddleware>(Options.Create(options));
        }

    }
}
