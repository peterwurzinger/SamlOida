using Microsoft.AspNetCore.Builder;

namespace SamlOida
{
    public class SamlOptions : RemoteAuthenticationOptions
    {
        public SamlOptions()
        {
            AuthenticationScheme = SamlDefaults.AuthenticationScheme;
            CallbackPath = "/signin-saml";
        }

        public string LogOnUrl { get; set; }

        //TODO: Add Options here (?)
    }
}
