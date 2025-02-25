using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NMCPipedGasLineAPI.Models
{
    public class RateImport
    {
        [Required]
        [Display(Name = "ImportPath")]
        public string ImportPath { get; set; }
    }
}