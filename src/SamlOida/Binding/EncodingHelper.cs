using System;
using System.Net;
using System.Xml;
using SamlOida.MessageHandler;

namespace SamlOida.Binding
{
    public static class EncodingHelper
    {

        public static string EncodeMessage(XmlDocument message)
        {
            return WebUtility.UrlEncode(Convert.ToBase64String(message.Deflate()));
        }

    }
}
