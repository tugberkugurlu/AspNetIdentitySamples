using AspNetIdentityLogOutEverywhereSample.Models;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNetIdentityLogOutEverywhereSample.Infrastructure
{
    public class MyClaimsIdentityFactory : ClaimsIdentityFactory<ApplicationUser>
    {
        public override async Task<ClaimsIdentity> CreateAsync(UserManager<ApplicationUser> manager, ApplicationUser user, string authenticationType)
        {
            ClaimsIdentity identity = await base.CreateAsync(manager, user, authenticationType).ConfigureAwait(false);
            identity.AddClaim(new Claim("urn:AspNetIdentityLogOutEverywhereSample:securitystamp", user.SecurityStamp));

            return identity;
        }
    }
}