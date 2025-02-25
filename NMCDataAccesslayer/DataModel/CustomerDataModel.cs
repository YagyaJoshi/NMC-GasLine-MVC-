using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMCDataAccesslayer.DataModel
{
    public class CustomerDataModel
    {

        public string CustomerID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string FlatNo { get; set; }
        public decimal ClosingBalance { get; set; }

        public string CustomerType { get; set; }
        public string BillNo { get; set; }
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
        public string AreaId { get; set; }
        public List<AreaDataModel> Area { get; set; }
        public string GodownId { get; set; }
        public List<GodownDataModel> GoDown { get; set; }
        public string AreaName { get; set; }
        public string CompanyName { get; set; }
        public string GodownName { get; set; }
        public string ConnectionType { get; set; }
        
        public string CompanyId { get; set; }
        public List<CompanyDataModel> Company { get; set; }
        public DateTime DateofInstallation { get; set; }
        public List<CustomerDataModel> CustomerList { get; set; }
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
        public List<StockItemMasterDataModel> StockItem { get; set; }
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
        public string BilltypeId { get; set; }
        public List<BillTypeData> BillTypeList { get; set; }
        public string ReadingDate { get; set; }
        public string CustomerNumber { get; set; }
        public string PassWord { get; set; }
        public DateTime CreatedAt { get; set; }
        public string MaxBillDate { get; set; }
        public string AliasName { get; set; }
        public string MerchantId { get; set; }
        public string SecurityId { get; set; }
        public string ChecksumKey { get; set; }
    }



    public class ImportCustomerData
    {
        public string AreaId { get; set; }
        public string GodownId { get; set; }
        public string TallyName { get; set; }
        public decimal ClosingBalance { get; set; }
        public DateTime? PrevBillDate { get; set; }
        public decimal PrevReading { get; set; }
        public string ContactNos { get; set; }
        public string EmailId { get; set; }
        public decimal PreviousDue { get; set; }
        public string CustomerNumber { get; set; }
        public long Customerunique { get; set; }
        public Int32 IsActive { get; set; }
        public string FlatNo { get; set; }
        public string AliasName { get; set; }
    }



    public class ExportCustomerData
    {
        public string Id { get; set; }
        public decimal ClosingBalance { get; set; }
      
        public string PreviousBillDate { get; set; }
        public decimal PreviousRedg { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string ALIAS { get; set; }
      
        public string BILLWISEDETAILS { get; set; }
        public string DUEDATE { get; set; }
        public string Address { get; set; }
        public string COUNTRY { get; set; }
        public string State { get; set; }
        public string PINCODE { get; set; }
        
        public string CONTACTPERSON { get; set; }
        public string MOBILE { get; set; }
        public string EMAIL { get; set; }

        public string BillType { get; set; }
        public string GodownMastername { get; set; }
        public string ReceiptNo { get; set; }
        public string BankName { get; set; }
        public string AccountNo { get; set; }
        public string ChequeNo { get; set; }

        public string PaymentType { get; set; }
        public string TallyName { get; set; }
        public decimal InitialMeterReading { get; set; }


        public string DepositeBankName { get; set; }
        public string DepositeAccountNo { get; set; }
        public string Voucherdate { get; set; }

        public string CustomerNumber { get; set; }
        public string MailEMAIL { get; set; }
        public string EmailSend { get; set; }
        public string Type { get; set; }
    }

    public class ExportWithoutBillData
    {
        public string Name { get; set; }
        public string AliasName { get; set; }
        public string PaymentDate { get; set; }
        public string Narration { get; set; }
        public decimal Amount { get; set; }
    }
    
    public class ExportCustomerWithoutBill
    {
        public string TallyName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string FlatNo { get; set; }
       public string AliasName { get; set; }
    }
    public class CustomerList
    {

        public string CompanyId { get; set; }
        public string AreaId { get; set; }
        public string GodownId { get; set; }
        public string AliasName { get; set; }

    }
}
