using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NMCPipedGasLineAPI.Models
{
    public class AreaMaster
    {
        public string AreaId { get; set; }
        [StringLength(50, ErrorMessage = "Name must be between 1 and 50 char")]
        [Required(ErrorMessage = "Please Enter Area Name")]
        public string AreaName { get; set; }
        public string CityId { get; set; }
        [Required(ErrorMessage = "Please Select Company Name")]
        public string CompanyId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CompanyName { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public Int32 IsActive { get; set; }
        public string AdminId { get; set; }
        public string CountryId { get; set; }
        public string StateId { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
        public List<CompanyMaster> Company { get; set; }

        public List<AreaMaster> AreaMasterList { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int RecordCount { get; set; }
        public Int32 TotalRows { get; set; }
        public string EmployeeName { get; set; }
        public string GodownName { get; set; }
    }
}