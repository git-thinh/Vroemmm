using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Vroemmm.GoogleAuth;

namespace Vroemmm.Models
{
    public class VroemmmContext : DbContext
    {
        public VroemmmContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<StoredCredentials> StoredCredentialSet { get; set; }
    }
}