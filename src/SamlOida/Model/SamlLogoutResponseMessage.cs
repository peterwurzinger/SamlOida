namespace SamlOida.Model
{
    public class SamlLogoutResponseMessage : SamlMessage
    {
        public bool Success { get; set; }

        public string InResponseTo { get; set; }
    }
}
