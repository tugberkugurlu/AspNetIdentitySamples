using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;

namespace WebApiIdentity
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.EnableSystemDiagnosticsTracing();

            // Handlers
            config.SuppressDefaultHostAuthentication();
            config.MessageHandlers.Add(new MyHandler());

            // Filters
            config.Filters.Add(new HostAuthenticationFilter(DefaultAuthenticationTypes.ExternalBearer));
        }
    }

    public class MyHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            IPrincipal principal = request.GetRequestContext().Principal;
            return base.SendAsync(request, cancellationToken);
        }
    }

    public class MyOwinAuthFilter : IAuthenticationFilter
    {
        private readonly string _authenticationType;

        public MyOwinAuthFilter(string authenticationType)
        {
            _authenticationType = authenticationType;
        }

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            HttpRequestMessage request = context.Request;
            IAuthenticationManager authenticationManager = GetAuthenticationManagerOrThrow(request);
            AuthenticateResult result = await authenticationManager.AuthenticateAsync(_authenticationType);

            if (result != null)
            {
                IIdentity identity = result.Identity;

                if (identity != null)
                {
                    context.Principal = new ClaimsPrincipal(identity);
                }
            }
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public bool AllowMultiple
        {
            get { return true; }
        }

        private static IAuthenticationManager GetAuthenticationManagerOrThrow(HttpRequestMessage request)
        {
            IAuthenticationManager authenticationManager = GetAuthenticationManager(request);

            if (authenticationManager == null)
            {
                throw new InvalidOperationException();
            }

            return authenticationManager;
        }

        internal static IAuthenticationManager GetAuthenticationManager(HttpRequestMessage request)
        {
            IOwinContext context = request.GetOwinContext();

            if (context == null)
            {
                return null;
            }

            return context.Authentication;
        }
    }
}