using System;

namespace SimpleOAuthSample.Models
{
    public class Client
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ClientSecretHash { get; set; }
        public AllowedGrant AllowedGrant { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
    }
}