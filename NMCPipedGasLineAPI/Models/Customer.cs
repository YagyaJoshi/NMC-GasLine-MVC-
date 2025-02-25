using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Configuration;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NMCPipedGasLineAPI.Models
{

    public class Customer
    {
        public string CustomerID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        //[Required(ErrorMessage = "Please Enter FlatNo")]
        public string FlatNo { get; set; }
        public decimal ClosingBalance { get; set; }

        public string CustomerType { get; set; }
        public string BillNo { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string Phone { get; set; }
        
       
        public string PaymentTypeId { get; set; }
        public string MeterNo { get; set; }
        public decimal NewMeterReading { get; set; }
        public DateTime? PreviousBillDate { get; set; }
        public decimal PreviousRedg { get; set; }

        public DateTime? CreatedDate { get; set; }
        public Int32 CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int isActive { get; set; }
        public string XmlTally { get; set; }
        public int XmlSendToTally { get; set; }
        public int TallyResponseSuccess { get; set; }
        public string TallyName { get; set; }
        public decimal InstallationAmount { get; set; }
        public decimal InputRate { get; set; }
        public decimal LastImportedBy { get; set; }
        public DateTime LastImportdate { get; set; }
        public string ContactNos { get; set; }
        public string EmailId { get; set; }
        [Required(ErrorMessage = "Please Select Area")]
        public string AreaId { get; set; }
        public List<AreaMaster> Area { get; set; }
        [Required(ErrorMessage = "Please Select Society")]
        public string GodownId { get; set; }
        public List<GodownMaster> GoDown { get; set; }
        public string AreaName { get; set; }
        public string CompanyName { get; set; }
        public string GodownName { get; set; }
        [Required(ErrorMessage = "Please Select Company")]
        public string CompanyId { get; set; }
        public List<CompanyMaster> Company { get; set; }
        [Required(ErrorMessage = "Please Select Bill Type")]
        public string BillTypeId { get; set; }
        public List<BillType> BillTypeList { get; set; }
        //[CustomDate]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateofInstallation { get; set; }
        public List<Customer> CustomerList { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int RecordCount { get; set; }
        public string AccountNo { get; set; }
        public string BankName { get; set; }
        public string BankDetail { get; set; }
        public decimal MinAmt { get; set; }
        public decimal ReconnectionAmt { get; set; }
        public decimal Amount { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public string StockItemId { get; set; }
        public decimal Rate { get; set; }
        public string OwnerId { get; set; }
        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }
        public string OwnerAddress { get; set; }
        public string OwnerPhone { get; set; }
        public bool isPaid { get; set; }
        public List<StockItemMaster> StockItem { get; set; }
        public string ChequeNo { get; set; }
        public decimal ClosingRedg { get; set; }
        public decimal? PreviousDue { get; set; }
        public string BillDate { get; set; }
        public decimal? CurrentScm { get; set; }
        public decimal? CurrentKGS { get; set; }
        public decimal? PaymentDue { get; set; }
        public string DueDate { get; set; }
        public decimal ConsumeUnit { get; set; }
        public decimal ServiceAmt { get; set; }
        public string BillMonth { get; set; }
        public decimal LateFee { get; set; }
        public Int32 TotalRows { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public string ReadingDate { get;set;}
        public string CustomerNumber { get; set; }
        public string PassWord { get; set; }
        public string ConnectionType { get; set; }
        public DateTime CreatedAt { get; set; }
        public string MaxBillDate { get; set; }
        public string AliasName { get; set; }
    }
    public class CustomDateAttribute : RangeAttribute
    {
        public CustomDateAttribute()
          : base(typeof(DateTime),
                  DateTime.Now.AddYears(-6).ToShortDateString(),
                  DateTime.Now.AddYears(6).ToShortDateString())
        { }
    }

    public class ImportCustomer
    {

        public string AreaId { get; set; }
        public string GodownId { get; set; }
        public string TallyName { get; set; }
        [DataType("decimal(18 ,2")]
        public decimal ClosingBalance { get; set; }
        public DateTime? PrevBillDate { get; set; }
        [DataType("decimal(18 ,4")]
        public decimal PrevReading { get; set; }
        public string ContactNos { get; set; }
        public string EmailId { get; set; }

        [DataType("decimal(18 ,2")]
        public decimal PreviousDue { get; set; }
        public string CustomerNumber { get; set; }
        public long Customerunique { get; set; }
        public Int32 IsActive { get; set; }


        public string FlatNo { get; set; }
        public string AliasName { get; set; }

    }


    public class ExcelImportCustomer
    {
        [Required(ErrorMessage = "Please Select Company")]
        public string CompanyId { get; set; }

        //[Required(ErrorMessage = "Please Select Bill Type")]
        //public string BillTypeId { get; set; }
        
    }

    public class ExcelExportCustomerWithoutBill
    {
        [Required(ErrorMessage = "Please Select Society")]
        public string GodownId { get; set; }
        public string CompanyId { get; set; }
        public string AreaId { get; set; }
    }



    public class StockRate
    {
        public string month { get; set; }
        public string rate { get; set; }
        public string year { get; set; }
        public string day { get; set; }
        public string StockItemId { get; set; }
        public string CompanyId { get; set; }
        public decimal PreviousRedg { get; set; }
        public decimal ClosingRedg { get; set; }
        public decimal InputRate { get; set; }
    }


    public class ExportCustomer
    {

        public string PartyName { get; set; }
        public decimal ClosingBalance { get; set; }
        public string BillDate { get; set; }
        public string PreviousBillDate { get; set; }
        public decimal PreviousRedg { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

    }

   

    public class ImCustomer
    {
        public string CustomerID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        //[Required(ErrorMessage = "Please Enter FlatNo")]
        public string FlatNo { get; set; }
        public decimal ClosingBalance { get; set; }

        public string CustomerType { get; set; }
        public string BillNo { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string Phone { get; set; }


        public string PaymentTypeId { get; set; }
        public string MeterNo { get; set; }
        public decimal NewMeterReading { get; set; }
        public DateTime? PreviousBillDate { get; set; }
        public decimal PreviousRedg { get; set; }

        public DateTime? CreatedDate { get; set; }
        public Int32 CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int isActive { get; set; }
        public string XmlTally { get; set; }
        public int XmlSendToTally { get; set; }
        public int TallyResponseSuccess { get; set; }
        public string TallyName { get; set; }
        public decimal InstallationAmount { get; set; }
        public decimal InputRate { get; set; }
        public decimal LastImportedBy { get; set; }
        public DateTime LastImportdate { get; set; }
        public string ContactNos { get; set; }
        public string EmailId { get; set; }
        [Required(ErrorMessage = "Please Select Area")]
        public string AreaId { get; set; }
        public List<AreaMaster> Area { get; set; }
        [Required(ErrorMessage = "Please Select Society")]
        public string GodownId { get; set; }
        public List<GodownMaster> GoDown { get; set; }
        public string AreaName { get; set; }
        public string CompanyName { get; set; }
        public string GodownName { get; set; }
        [Required(ErrorMessage = "Please Select Company")]
        public string CompanyId { get; set; }
        public List<CompanyMaster> Company { get; set; }
        // [Required(ErrorMessage = "Please Select Bill Type")]
        public string BillTypeId { get; set; }
        public List<BillType> BillTypeList { get; set; }
        //[CustomDate]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateofInstallation { get; set; }
        public List<Customer> CustomerList { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int RecordCount { get; set; }
        public string AccountNo { get; set; }
        public string BankName { get; set; }
        public string BankDetail { get; set; }
        public decimal MinAmt { get; set; }
        public decimal ReconnectionAmt { get; set; }
        public decimal Amount { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public string StockItemId { get; set; }
        public decimal Rate { get; set; }
        public string OwnerId { get; set; }
        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }
        public string OwnerAddress { get; set; }
        public string OwnerPhone { get; set; }
        public bool isPaid { get; set; }
        public List<StockItemMaster> StockItem { get; set; }
        public string ChequeNo { get; set; }
        public decimal ClosingRedg { get; set; }
        public decimal? PreviousDue { get; set; }
        public string BillDate { get; set; }
        public decimal? CurrentScm { get; set; }
        public decimal? CurrentKGS { get; set; }
        public decimal? PaymentDue { get; set; }
        public string DueDate { get; set; }
        public decimal ConsumeUnit { get; set; }
        public decimal ServiceAmt { get; set; }
        public string BillMonth { get; set; }
        public decimal LateFee { get; set; }
        public Int32 TotalRows { get; set; }

    }
}
