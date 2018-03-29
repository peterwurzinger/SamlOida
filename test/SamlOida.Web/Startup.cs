using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SamlOida.Binding;
using System;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace SamlOida.Web
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication(options =>
                {
                    //options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = SamlAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                })
                .AddSaml(options =>
                {
                    //TODO: Options configuration useless atm
                    //options.CallbackPath = "/saml-auth";
                    //options.SingleSignOnBinding = SamlBindingBehavior.HttpRedirect;
                    //options.IdentityProviderSignOnUrl = new Uri("https://capriza.github.io/samling/samling.html");
                    //options.IssueInstantExpiration = TimeSpan.FromMinutes(20);
                });
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //loggerFactory.AddConsole();

            app.UseAuthentication();

            //app.UseSaml(new SamlOptions
            //{
            //    AutomaticChallenge = true,
            //    ServiceProviderEntityId = "https://localhost:50000/stdportal-sp/test-pv@stdp.gv.at/samloida@bmspot.gv.at",
            //    SamlBindingOptions = new SamlBindingOptions
            //    {
            //        BindingBehavior = SamlBindingBehavior.HttpRedirect,
            //        IdentityProviderSignOnUrl = "https://unitarytest/stdportal-idp/test-pv@stdp.gv.at/profile/SAML2/Redirect/SSO"
            //    }
            //});

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseDeveloperExceptionPage();

        }
    }
}
