﻿using Microsoft.AspNetCore.Http;
using System;
using System.Xml;

namespace SamlOida.Binding
{
    public interface ISamlBindingStrategy
    {
        void BindMessage(XmlDocument message, HttpContext context, Uri target, string flowKey, SamlOptions options,
            string relayState = null);
    }
}