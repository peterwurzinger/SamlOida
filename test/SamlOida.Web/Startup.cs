using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SamlOida.Binding;
using System;
using System.IO;
using System.Security;
using System.Security.Cryptography.X509Certificates;

namespace SamlOida.Web
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var pw = new SecureString();
            pw.AppendChar('t'); pw.AppendChar('e'); pw.AppendChar('s'); pw.AppendChar('t');
            pw.MakeReadOnly();
            var cer = new X509Certificate2(File.ReadAllBytes("spPrivateCertificate.pfx"), pw);

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
                    options.CallbackPath = "/saml-auth";
                    options.AuthnRequestBinding = SamlBindingBehavior.HttpRedirect;
                    //options.IdentityProviderSignOnUrl = new Uri("https://capriza.github.io/samling/samling.html");
                    options.IdentityProviderSignOnUrl = "https://idp.ssocircle.com:443/sso/SSORedirect/metaAlias/publicidp";
                    options.IssueInstantExpiration = TimeSpan.FromMinutes(20);
                    options.ServiceProviderEntityId = "SamlOida";

                    //options.SignoutCallbackPath = "/saml-spSignoutCallback";
                    //options.SignoutPath = "/saml-idpSignout";

                    options.AcceptSignedMessagesOnly = false;
                    options.SignOutgoingMessages = true;
                    options.ServiceProviderCertificate = cer;
                });
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //loggerFactory.AddConsole();
            app.UseDeveloperExceptionPage();

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
