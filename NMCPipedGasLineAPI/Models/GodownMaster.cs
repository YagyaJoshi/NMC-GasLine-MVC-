using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
namespace NMCPipedGasLineAPI.Models
{
    public class GodownMaster
    {
        public string Id { get; set; }
        [StringLength(50, ErrorMessage = "Name must be between 1 and 50 char")]
        [Required(ErrorMessage = "Please Enter  Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please Enter Input Rate")]
        [Range(0, 9999999999999999.99)]
        [RegularExpression(@"^-{0,1}\d+\.{0,1}\d*$", ErrorMessage = "Please Enter Rate valid rate")]    
        public decimal InputRate { get; set; }
        public Int32 IsActive { get; set; }
        [Required(ErrorMessage = "Please Select Company Name")]
        public string CompanyId { get; set; }
        public string AdminId { get; set; }
        public decimal NewServiceCharge { get; set; }
        public List<CompanyMaster> Company { get; set; }
        public string CompanyName { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }

        public List<AreaMaster> Area { get; set; }
        [Required(ErrorMessage = "Please Select Area Name")]
        public string[] AreaId { get; set; }
        public string[] UAreadId { get; set; }

        public List<GodownMaster> GodownMasterList { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int RecordCount { get; set; }
        public int disCustomercount { get; set; }

        public string AreaIdedit { get; set; }
        [Required(ErrorMessage = "Please Enter  Code")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        public string ShortName { get; set; }
        public Int32 TotalRows { get; set; }
        [Required(ErrorMessage = "Please Enter Alias Name")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        public string AliasName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}