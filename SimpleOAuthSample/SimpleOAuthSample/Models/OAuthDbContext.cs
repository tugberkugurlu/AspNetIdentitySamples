using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SimpleOAuthSample.Models
{
    public class OAuthDbContext : IdentityDbContext
    {
        public OAuthDbContext()
            : base("OAuthDbContext")
        {
        }

        public DbSet<Client> Clients { get; set; }
    }
}