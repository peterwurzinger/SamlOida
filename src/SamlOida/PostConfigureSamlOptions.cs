using Microsoft.Extensions.Options;

namespace SamlOida
{
    public class PostConfigureSamlOptions : IPostConfigureOptions<SamlOptions>
    {
        public void PostConfigure(string name, SamlOptions options)
        {
            //TODO do the post-configure dance
        }
    }
}
