using System.Collections.Generic;
using System.Xml;

namespace SamlOida.Binding
{
    public class ExtractionResult
    {
        public XmlDocument Message { get; set; }

        public string RelayState { get; set; }
        
        public IReadOnlyList<byte> Signature { get; set; }

        public string SignatureAlgorithm { get; set; }
    }
}