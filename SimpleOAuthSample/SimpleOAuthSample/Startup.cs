using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using SimpleOAuthSample.Models;

[assembly: OwinStartup(typeof(SimpleOAuthSample.Startup))]
namespace SimpleOAuthSample
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext<OAuthDbContext>(() => new OAuthDbContext());
            app.CreatePerOwinContext<UserManager<IdentityUser>>(CreateManager);
        }

        private static UserManager<IdentityUser> CreateManager(IdentityFactoryOptions<UserManager<IdentityUser>> options, IOwinContext context)
        {
            var userStore = new UserStore<IdentityUser>(context.Get<OAuthDbContext>());
            var manager = new UserManager<IdentityUser>(userStore);

            return manager;
        }
    }
}
