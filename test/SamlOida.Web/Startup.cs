using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Claims;
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
            var spCert = new X509Certificate2(File.ReadAllBytes("spPrivateCertificate.pfx"), pw);
            var idpCert = new X509Certificate2(File.ReadAllBytes("idpPublicCertificate.cer"));

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = SamlAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignOutScheme = SamlAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                })
                .AddSaml(options =>
                {
                    options.IssueInstantExpiration = TimeSpan.FromMinutes(20);
                    options.ServiceProviderEntityId = "SamlOida";

                    options.IdentityProviderSignOnUrl = "https://idp.ssocircle.com:443/sso/SSORedirect/metaAlias/publicidp";
                    options.CallbackPath = "/saml-auth";


                    options.IdentityProviderLogOutUrl = "https://idp.ssocircle.com:443/sso/IDPSloRedirect/metaAlias/publicidp";
                    options.LogoutPath = "/saml-idpSignout";
                    //options.SignoutCallbackPath = "/saml-spSignoutCallback";

                    options.AcceptSignedMessagesOnly = false;
                    options.SignOutgoingMessages = true;
                    options.ServiceProviderCertificate = spCert;
                    options.IdentityProviderCertificate = idpCert;

                    options.ClaimsSelector = (attributes) =>
                        {
                            return attributes.Select(attr => new Claim(attr.Name, attr.Values.FirstOrDefault()))
                                .ToList();
                        };

                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
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
