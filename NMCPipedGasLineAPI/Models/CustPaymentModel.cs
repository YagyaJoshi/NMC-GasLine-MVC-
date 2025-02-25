using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NMCPipedGasLineAPI.Models
{
    public class CustPaymentModel
    {
        public string MerchantID { get; set; }
        public string CustomerID { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyType { get; set; }
        public string TypeField1 { get; set; }
        public string SecurityID { get; set; }
        public string TypeField2 { get; set; }
        public string AdditionalInfo1 { get; set; }
        public string AdditionalInfo2 { get; set; }
        public string RU { get; set; }
        public string ActionURL { get; set; }
    }


    
}