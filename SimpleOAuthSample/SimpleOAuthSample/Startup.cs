using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using SimpleOAuthSample.Models;
using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(SimpleOAuthSample.Startup))]
namespace SimpleOAuthSample
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext<OAuthDbContext>(() => new OAuthDbContext());
            app.CreatePerOwinContext<UserManager<IdentityUser>>(CreateManager);

            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("oauth/token"),
                Provider = new MyOAuthAuthorizationServerProvider(),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
#if DEBUG
                AllowInsecureHttp = true,
#endif
            });

            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }

        private static UserManager<IdentityUser> CreateManager(IdentityFactoryOptions<UserManager<IdentityUser>> options, IOwinContext context)
        {
            var userStore = new UserStore<IdentityUser>(context.Get<OAuthDbContext>());
            var manager = new UserManager<IdentityUser>(userStore);

            return manager;
        }
    }

    public class MyOAuthAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;

            if (context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                UserManager<IdentityUser> userManager = context.OwinContext.GetUserManager<UserManager<IdentityUser>>();
                OAuthDbContext dbContext = context.OwinContext.Get<OAuthDbContext>();

                try
                {
                    Client client = await dbContext
                        .Clients
                        .FirstOrDefaultAsync(clientEntity => clientEntity.Id == clientId);

                    if (client != null &&
                        userManager.PasswordHasher.VerifyHashedPassword(
                            client.ClientSecretHash, clientSecret) == PasswordVerificationResult.Success)
                    {
                        // Client has been verified.
                        context.OwinContext.Set<Client>("oauth:client", client);
                        context.Validated(clientId);
                    }
                    else
                    {
                        // Client could not be validated.
                        context.SetError("invalid_client", "Client credentials are invalid.");
                        context.Rejected();
                    }
                }
                catch
                {
                    // Could not get the client through the IClientManager implementation.
                    context.SetError("server_error");
                    context.Rejected();
                }
            }
            else
            {
                // The client credentials could not be retrieved.
                context.SetError("invalid_client", "Client credentials could not be retrieved through the Authorization header.");
                context.Rejected();
            }
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            Client client = context.OwinContext.Get<Client>("oauth:client");
            if (client.AllowedGrant == OAuthGrant.ResourceOwner)
            {
                // Client flow matches the requested flow. Continue...
                UserManager<IdentityUser> userManager = context.OwinContext.GetUserManager<UserManager<IdentityUser>>();

                IdentityUser user;
                try
                {
                    user = await userManager.FindAsync(context.UserName, context.Password);
                }
                catch
                {
                    // Could not retrieve the user.
                    context.SetError("server_error");
                    context.Rejected();

                    // Return here so that we don't process further. Not ideal but needed to be done here.
                    return;
                }

                if (user != null)
                {
                    try
                    {
                        // User is found. Signal this by calling context.Validated
                        ClaimsIdentity identity = await userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ExternalBearer);
                        context.Validated(identity);
                    }
                    catch
                    {
                        // The ClaimsIdentity could not be created by the UserManager.
                        context.SetError("server_error");
                        context.Rejected();
                    }
                }
                else
                {
                    // The resource owner credentials are invalid or resource owner does not exist.
                    context.SetError("access_denied", "The resource owner credentials are invalid or resource owner does not exist.");
                    context.Rejected();
                }
            }
            else
            {
                // Client is not allowed for the 'Resource Owner Password Credentials Grant'.
                context.SetError("invalid_grant", "Client is not allowed for the 'Resource Owner Password Credentials Grant'");
                context.Rejected();
            }
        }
    }
}