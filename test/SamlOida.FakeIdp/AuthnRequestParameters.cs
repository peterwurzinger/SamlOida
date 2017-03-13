using System;
using System.Collections.Generic;
using System.Text;

namespace SamlOida.FakeIdp
{
    public class AuthnRequestParameters
    {
        public string RequestId { get; set; }
        public string AssertionConsumerServiceURL { get; set; }

    }
}
