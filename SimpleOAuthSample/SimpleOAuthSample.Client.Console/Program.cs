using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Client;

namespace SimpleOAuthSample.Client.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = "http://localhost:53523/";
            OAuth2Client client = new OAuth2Client(new Uri(baseAddress), "42ff5dad3c274c97a3a7c3d44b67bb42", "client123456");
            TokenResponse tokenResponse = client.RequestResourceOwnerPasswordAsync("Tugberk", "user123456").Result;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(baseAddress);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
                HttpResponseMessage response = httpClient.GetAsync("api/users/me").Result;
            }
        }
    }
}