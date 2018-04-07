using System;

namespace SamlOida.Model
{
    public abstract class SamlMessage
    {
        public string Id { get; set; }

        public string Issuer { get; set; }

        public DateTime IssueInstant { get; set; }

        public string Destination { get; set; }
    }
}
