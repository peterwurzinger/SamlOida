using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace SamlOida
{
    public static class PvpIdentityMapper
    {
        public const string UserNameKey = "GID";
        public const string EmailKey = "MAIL";
        public const string FirstNameKey = "GIVEN-NAME";
        public const string LastNameKey = "PRINCIPAL-NAME";

        internal static ISet<Claim> Map(ResponseParsingResult result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            if (result.Attributes == null)
                throw new ArgumentNullException(nameof(result.Attributes));

            var userName = result.Attributes.Get(UserNameKey);
            var mail = result.Attributes.Get(EmailKey);
            var firstName = result.Attributes.Get(FirstNameKey);
            var lastName = result.Attributes.Get(LastNameKey);

            var claims = new HashSet<Claim>();

            claims.UnionWith(MapEachValue(userName, ClaimTypes.NameIdentifier));
            claims.UnionWith(MapEachValue(mail, ClaimTypes.Email));
            claims.UnionWith(MapEachValue(firstName, ClaimTypes.GivenName));
            claims.UnionWith(MapEachValue(lastName, ClaimTypes.Name));

            return claims;
        }

        private static IEnumerable<Claim> MapEachValue(SamlAttribute attribute, string claimType)
        {
            return !attribute.Values.Any() 
                ? new [] { new Claim(claimType, null) } 
                : attribute.Values.Select(value => new Claim(claimType, value));
        }
    }
}
