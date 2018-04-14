namespace SamlOida.Model
{
    public class SamlLogoutRequestMessage : SamlMessage
    {
        public string NameId { get; set; }

        public string SessionIndex { get; set; }
    }
}
