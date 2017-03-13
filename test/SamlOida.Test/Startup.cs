using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SamlOida.FakeIdp;

namespace SamlOida.Test
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(sharedOptions => sharedOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseCookieAuthentication();

            app.UseSaml(new SamlOptions
            {
                AutomaticChallenge = true,
                LogOnUrl = $"http://localhost:5000{FakeIdpMiddleware.LogOnPath}"
            });

            app.UseFakeIdp();

            app.Use((ctx, func) =>
            {
                if (!ctx.User.Identity.IsAuthenticated)
                { 
                    ctx.Response.StatusCode = 401;
                    return Task.FromResult(0);
                }

                ctx.Response.StatusCode = 200;
                return ctx.Response.WriteAsync("Hi!");
            });
        }

    }
}
