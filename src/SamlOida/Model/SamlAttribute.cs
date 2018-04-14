using System.Collections.Generic;

namespace SamlOida.Model
{
    public class SamlAttribute
    {
        public string Name { get; internal set; }
        public string NameFormat { get; internal set; }
        public string FriendlyName { get; internal set; }
        public bool IsEncrypted { get; internal set; }
        public ISet<string> Values { get; }

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
