using System;
using System.Collections.Generic;
using System.Linq;
using SamlOida.Model;

namespace SamlOida
{
    internal static class SamlAttributeExtensions
    {
        internal static SamlAttribute Get(this ISet<SamlAttribute> attributes, string key) 
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return attributes.SingleOrDefault(attr => key.Equals(attr.FriendlyName, StringComparison.Ordinal))
                ?? attributes.SingleOrDefault(attr => key.Equals(attr.Name, StringComparison.Ordinal));
        }
    }
}
