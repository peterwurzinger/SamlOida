using System;
using System.Collections.Generic;
using System.Linq;

namespace SamlOida
{
    internal static class SamlAttributeExtensions
    {
        internal static SamlAttribute Get(this ISet<SamlAttribute> attributes, string key) 
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return attributes.SingleOrDefault(attr => key.Equals(attr.FriendlyName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
