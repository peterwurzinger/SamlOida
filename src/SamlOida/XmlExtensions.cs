using System.IO;
using System.IO.Compression;
using System.Xml;

namespace SamlOida
{
    public static class XmlExtensions
    {
        public static byte[] Deflate(this XmlDocument document)
        {
            byte[] binary;

            using (var stream = new MemoryStream())
            {
                using (var deflate = new DeflateStream(stream, CompressionMode.Compress, true))
                {
                    document.Save(deflate);
                }

                binary = stream.ToArray();
            }
            return binary;
        }

        public static XmlDocument InflateToXmlDocument(this byte[] binary)
        {
            using (var stream = new MemoryStream(binary))
            {
                using (var deflate = new DeflateStream(stream, CompressionMode.Decompress))
                {
                    var document = new XmlDocument();
                    document.Load(deflate);
                    return document;
                }
            }
        }

    }
}
