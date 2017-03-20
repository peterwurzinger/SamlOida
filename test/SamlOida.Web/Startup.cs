using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SamlOida.Web
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(sharedOptions => sharedOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme);
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            app.UseCookieAuthentication();

            app.UseSaml(new SamlOptions
            {
                AutomaticChallenge = true,
                ServiceProviderEntityId = "https://localhost:50000/stdportal-sp/test-pv@stdp.gv.at/samloida@bmspot.gv.at",
                SamlBindingOptions = new SamlBindingOptions
                {
                    BindingBehavior = SamlBindingBehavior.HttpPost,
                    IdentityProviderSignOnUrl = "https://unitarytest/stdportal-idp/test-pv@stdp.gv.at/profile/SAML2/Redirect/SSO"
                }
            });

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
