using System.Collections.Generic;

namespace SamlOida.Model
{
    internal class SamlAttribute
    {
        internal string Name { get; set; }
        internal string NameFormat { get; set; }
        internal string FriendlyName { get; set; }
        internal bool IsEncrypted { get; set; }
        internal ISet<string> Values { get; }

        internal SamlAttribute()
        {
            Values = new HashSet<string>();
        }

        internal SamlAttribute(string name, string value) : this()
        {
            Name = name;
            Values.Add(value);
        }
    }
}
