using Microsoft.AspNetCore.Builder;

namespace SamlOida.FakeIdp
{
    public static class FakeIdpAppBuilderExtensions
    {
        public static IApplicationBuilder UseFakeIdp(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<FakeIdpMiddleware>();
            return builder;
        }

    }
}
