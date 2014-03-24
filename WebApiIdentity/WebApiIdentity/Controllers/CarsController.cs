using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace WebApiIdentity.Controllers
{
    public class CarsController : ApiController
    {
        [Authorize]
        public string[] Get()
        {
            var user = User as ClaimsPrincipal;
            return new[] 
            {
                "Foo 1",
                "Foo 1",
                "Foo 1"
            };
        }
    }
}