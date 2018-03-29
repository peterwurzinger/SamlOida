namespace SamlOida.Model
{
    public class SamlAuthnRequestMessage
    {
        internal string Destination { get; set; }

        internal string AssertionConsumerServiceUrl { get; set; }

        internal string Issuer { get; set; }
    }
}
