using System.Xml;

namespace SamlOida.Binding
{
    public class ExtractionResult
    {
        public XmlDocument Message { get; set; }

        public string RelayState { get; set; }

        public bool HasValidSignature { get; set; }
    }
}