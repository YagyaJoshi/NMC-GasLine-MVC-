using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMCDataAccesslayer.DataModel
{
  public  class CustomerPaymentDataModel
    {
        public string MerchantID { get; set; }
        public string CustomerID { get; set; }
        public string TxnReferenceNo { get; set; }
        public string BankReferenceNo { get; set; }
        public string TxnAmount { get; set; }
        public string BankID { get; set; }
        public string BankMerchantID { get; set; }
        public string TxnType { get; set; }
        public string CurrencyName { get; set; }
        public string ItemCode { get; set; }
        public string SecurityType { get; set; }
        public string SecurityID { get; set; }
        public string SecurityPassword { get; set; }
        public string TxnDate { get; set; }
        public string AuthStatus { get; set; }
        public string SettlementType { get; set; }
        public string AdditionalInfo1 { get; set; }
        public string AdditionalInfo2 { get; set; }
        public string AdditionalInfo3 { get; set; }
        public string AdditionalInfo4 { get; set; }
        public string AdditionalInfo5 { get; set; }
        public string AdditionalInfo6 { get; set; }
        public string AdditionalInfo7 { get; set; }
        public string ErrorStatus { get; set; }
        public string ErrorDescription { get; set; }
        public string CheckSum { get; set; }
        public string ResponseMsg { get; set; }
        
    }

    public class CustomerPaymentData
    {
        public string MerchantID { get; set; }
        public string BillId { get; set; }
        public string CustomerID { get; set; }
        public string TxnReferenceNo { get; set; }
        public string TxnAmount { get; set; }
        public string VendorId { get; set; }
        public string TxnDate { get; set; }
        public string AuthStatus { get; set; }
        public string Auth1 { get; set; }
        public string Auth2 { get; set; }
        public string Auth3 { get; set; }
        public string Auth4 { get; set; }
        public string Auth5 { get; set; }




        public string BillNo { get; set; }

        public string BillDate { get; set; }

        public string PaymentDate { get; set; }
        public string PaymentId { get; set; }

        public string ReceiptNo { get; set; }

        public string Address { get; set; }
        public decimal latefee { get; set; }
        public string Society { get; set; }
        public decimal LatePaymentFee { get; set; }
        public decimal DelayMinAmount { get; set; }

        public decimal closingBalance { get; set; }

        public decimal BalanceDue { get; set; }
        public decimal Delaychg { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaymentDue { get; set; }
        public decimal PaidAmount { get; set; }
        public Int32 UniqueReceiptNo { get; set; }

        public string PaymentTypeId { get; set; }
        public string shortname { get; set; }
        public string YEAR { get; set; }
        public string MONTH { get; set; }




        public string CheckNo { get; set; }

        public string AccountNo { get; set; }

        public string BankName { get; set; }

        public string BankDetail { get; set; }

        public int CreatedBy { get; set; }
        public int DelayDays { get; set; }

        public DateTime CreatedDate { get; set; }

        public string XmlTally { get; set; }

        public int XmlSendToTally { get; set; }

        public int TallyResponseSuccess { get; set; }
        public string PaymentType { get; set; }

        public bool isPaid { get; set; }
        public Int32 TotalRows { get; set; }

       
        public string GodownId { get; set; }
        public decimal PreviousDue { get; set; }
        public decimal CustomerclosingBalance { get; set; }
        public string Number { get; set; }
        public Int32 PaymentFrom { get; set; }
        public string CutomerName { get; set; }
        public string CutomerEmail { get; set; }
        public string Logimg { get; set; }
        public decimal TransactionFree { get; set; }
        public bool istxt { get; set; }
        public decimal BillAmount { get; set; }
        public decimal PreviousLateFree { get; set; }
        public string CustNumber { get; set; }
        public decimal Amount { get; set; }


    }
}
