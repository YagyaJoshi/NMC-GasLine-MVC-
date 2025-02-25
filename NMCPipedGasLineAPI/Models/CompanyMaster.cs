using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NMCPipedGasLineAPI.Models
{
    public class CompanyMaster
    {
        public string Id { get; set; }
        [StringLength(50, ErrorMessage = "Name must be between 1 and 50 char")]
        [Required(ErrorMessage = "Please Select Company Name")]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        
        [Required(ErrorMessage = "Please Select Country Name")]
        public string CountryId { get; set; }
        public List<CountryMaster> Country { get; set; }
        [Required(ErrorMessage = "Please Select State Name")]
        public string StateId { get; set; }
        public List<StateMaster> State { get; set; }
        [Required(ErrorMessage = "Please Select City Name")]
        public string CityId { get; set; }
        public List<City> City { get; set; }
        [StringLength(100, ErrorMessage = "Address must be between 1 and 100 char")]
        public string Address { get; set; }
        [DataType("decimal(18 ,2")]
        [Required(ErrorMessage = "Please Enter Late Payment Fee")]
     
        public decimal LatePaymentFee { get; set; }
        [Required(ErrorMessage = "Please Enter Minimum Gas Charges")]
        [DataType("decimal(18 ,2")]
        public decimal MinimumGasCharges { get; set; }
        [Required(ErrorMessage = "Please Enter Re Connection Fee")]
        [DataType("decimal(18 ,2")]
        public decimal ReConnectionFee { get; set; }
        [Required(ErrorMessage = "Please Enter Installation Charges")]
        [DataType("decimal(18 ,2")]
        public decimal InstallationCharges { get; set; }
        [Required(ErrorMessage = "Please Enter Service Charges")]
        [DataType("decimal(18 ,2")]
        public decimal ServiceCharges { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
        public Int32 IsActive { get; set; }
        public string AdminId { get; set; }

        public List<CompanyMaster> CompanyMasterList { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int RecordCount { get; set; }
        [Required(ErrorMessage = "Please Enter Delay Days")]
        public Int32 DelayDays { get; set; }
       
        public Int32? DelayDaysLimit { get; set; }
        [Required(ErrorMessage = "Please Enter Due Days")]
        public Int32 DueDays { get; set; }
        [DataType("decimal(18 ,2")]
        [Required(ErrorMessage = "Please Enter Delay Min Amount")]
        public decimal DelayMinAmount { get; set; }
        public Int32 TotalRows { get; set; }

        [Required(ErrorMessage = "Please Enter Delay Call Charges")]
        public decimal CallCharges { get; set; }
        [Required(ErrorMessage = "Please Enter Connection Fee")]
        public decimal ConnectionFee { get; set; }
        [Required(ErrorMessage = "Please Enter Details Changes Fee")]
        public decimal DetailsChangesFee { get; set; }
        public DateTime CreatedAt { get; set; }
        public string MerchantId { get; set; }
        public string SecurityId { get; set; }
    }

    public class CompanyPayment
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Please Select Company")]
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public List<CompanyMaster> Company { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int RecordCount { get; set; }
        public Int32 TotalRows { get; set; }
        [Required(ErrorMessage = "Please Enter Merchant Id")]
        public string MerchantId { get; set; }
        [Required(ErrorMessage = "Please Enter Security Id")]
        public string SecurityId { get; set; }
        [Required(ErrorMessage = "Please Enter ChecksumKey")]
        public string ChecksumKey { get; set; }

        public string StateId { get; set; }
        public string CityId { get; set; }
        public string CountryId { get; set; }
        public string Address { get; set; }

        public string AdminId { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
        public Int32 IsActive { get; set; }

        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
    

        public Int32 DelayDays { get; set; }
        public Int32? DelayDaysLimit { get; set; }
        public Int32 DueDays { get; set; }

        public decimal DelayMinAmount { get; set; }
        public decimal LatePaymentFee { get; set; }
        public decimal MinimumGasCharges { get; set; }
        public decimal ReConnectionFee { get; set; }
        public decimal InstallationCharges { get; set; }
        public decimal ServiceCharges { get; set; }
        public decimal CallCharges { get; set; }
        public decimal ConnectionFee { get; set; }
        public decimal DetailsChangesFee { get; set; }
        public DateTime CreatedAt { get; set; }
      
    }
}