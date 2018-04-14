using SamlOida.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SamlOida
{
    public static class SamlAttributeExtensions
    {
        public static SamlAttribute Get(this ISet<SamlAttribute> attributes, string key) 
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return attributes.SingleOrDefault(attr => key.Equals(attr.FriendlyName, StringComparison.Ordinal))
                ?? attributes.SingleOrDefault(attr => key.Equals(attr.Name, StringComparison.Ordinal));
        }
    }
}
