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

        //TODO: Add Options here (?)
    }
}
