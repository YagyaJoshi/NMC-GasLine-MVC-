using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NMCPipedGasLineAPI.Models
{
    public class CustomerPayment
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
        public string TransactionFree { get; set; }
    }
}