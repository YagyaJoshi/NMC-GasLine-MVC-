using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NMCDataAccesslayer.DataModel
{
    public class CompanyDataModel
    {
        public string Id { get; set; }
        public string CompanyName { get; set; }
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

        public List<CountryDataModel> Country { get; set; }
        public List<StateDataModel> State { get; set; }
        public List<CityDataModel> City { get; set; }

        public List<CompanyDataModel> CompanyMasterList { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int RecordCount { get; set; }

        public Int32 DelayDays { get; set; }
        public Int32? DelayDaysLimit { get; set; }
        public Int32 DueDays { get; set; }

        public decimal DelayMinAmount { get; set; }
        public Int32 TotalRows { get; set; }

        public decimal LatePaymentFee { get; set; }
        public decimal MinimumGasCharges { get; set; }
        public decimal ReConnectionFee { get; set; }
        public decimal InstallationCharges { get; set; }
        public decimal ServiceCharges { get; set; }
        public decimal CallCharges { get; set; }
        public decimal ConnectionFee { get; set; }
        public decimal DetailsChangesFee { get; set; }
        public DateTime CreatedAt { get; set; }
        public string MerchantId { get; set; }
        public string SecurityId { get; set; }
    }

    public class CompaniesPaymentModel
    {

        public string Id { get; set; }
        public string CompanyName { get; set; }
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
        public List<CompanyDataModel> Company { get; set; }
    //    public List<CompaniesPaymentModel> Company1 { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int RecordCount { get; set; }

        public Int32 DelayDays { get; set; }
        public Int32? DelayDaysLimit { get; set; }
        public Int32 DueDays { get; set; }

        public decimal DelayMinAmount { get; set; }
        public Int32 TotalRows { get; set; }

        public decimal LatePaymentFee { get; set; }
        public decimal MinimumGasCharges { get; set; }
        public decimal ReConnectionFee { get; set; }
        public decimal InstallationCharges { get; set; }
        public decimal ServiceCharges { get; set; }
        public decimal CallCharges { get; set; }
        public decimal ConnectionFee { get; set; }
        public decimal DetailsChangesFee { get; set; }
        public DateTime CreatedAt { get; set; }
        public string MerchantId { get; set; }
        public string SecurityId { get; set; }
      
        public string ChecksumKey { get; set; }
        
        public string CompanyId { get; set; }
      
       
       
    }
}