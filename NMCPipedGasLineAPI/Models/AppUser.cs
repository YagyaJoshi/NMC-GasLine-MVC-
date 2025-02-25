using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace NMCPipedGasLineAPI.Models
{
    public class AppUser : IdentityUser
    {
        public string Country { get; set; }
    }
}