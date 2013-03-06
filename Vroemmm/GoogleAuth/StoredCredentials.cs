using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using DotNetOpenAuth.OAuth2;
using System.ComponentModel.DataAnnotations;

namespace Vroemmm.GoogleAuth
{
    public class StoredCredentials
    {
        [Key]
        public string UserId { get; set; }

        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
