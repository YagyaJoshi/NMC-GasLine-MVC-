using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NMCPipedGasLineAPI.Models
{
    public class BankMaster
    {
        public string Id { get; set; }
        [StringLength(50, ErrorMessage = "Name must be between 1 and 50 char")]
        [Required(ErrorMessage = "Please Enter Bank Name")]
        public string BankName { get; set; }
        public string AdminId { get; set; }
        public Int32 IsActive { get; set; }
        public Int32 TotalRows { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int RecordCount { get; set; }
        public List<BankMaster> BankMasterList { get; set; }
    }
}