﻿using SamlOida.MessageHandler.Parser;
using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace SamlOida.MessageHandler
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

	    public static XmlNode RemoveSignature(this XmlDocument document)
	    {
	        var signatureNode = document.DocumentElement.SelectSingleNode($"{SamlAuthenticationDefaults.XmlSignatureNsPrefix}:Signature", SamlXmlExtensions.NamespaceManager);

            if (signatureNode != null)
                return document.DocumentElement.RemoveChild(signatureNode);

            return null;
	    }

		public static XmlDocument ToXmlDocument(this byte[] binary, bool tryInflate = true)
		{
			using (var stream = new MemoryStream(binary, false))
			{
				var document = new XmlDocument();
                if (tryInflate) {
                    try
                    {
                        using (var deflate = new DeflateStream(stream, CompressionMode.Decompress))
                        {
                            //Try to load from deflated stream
                            document.Load(deflate);
                        }
                    }
                    catch (Exception)
                    {
                        //Load from non-deflated stream
                        return binary.ToXmlDocument(false);
                    }
                    return document;
                }
			    document.Load(stream);
                return document;
			}
		}
	}
}
