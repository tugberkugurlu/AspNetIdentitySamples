using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;

namespace SimpleOAuthSample.Controllers
{
    [Authorize]
    public class UsersController : ApiController
    {
        [Route("api/users/me")]
        public IEnumerable<ClaimModel> GetMe()
        {
            ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
            return identity.Claims.Select(claim => new ClaimModel { Type = claim.Type, Value = claim.Value }).ToList();
        }
    }

    public class ClaimModel
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}