using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using NMCPipedGasLineAPI.Models;

namespace NMCPipedGasLineAPI.App_Start
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext()
            : base("Connectionstring")
        {
        }
    }
}