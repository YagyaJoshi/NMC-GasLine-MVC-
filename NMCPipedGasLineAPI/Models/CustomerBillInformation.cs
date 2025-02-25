using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;

namespace NMCPipedGasLineAPI.Models
{
    public class CustomerBillInformation
    {
        //public string PartyName { get; set; }
        //public decimal ClosingBalance { get; set; }
        //public string BillDate { get; set; }
        //public string PreviousBillDate { get; set; }
        //public decimal PreviousRedg { get; set; }
        //public string DueDate { get; set; }
        //public decimal ConsumeUnit { get; set; }
        //public decimal InputRate { get; set; }
        //public string StockItemName { get; set; }
        //public decimal Rate { get; set; }
        //public string Month { get; set; }
        //public decimal ServiceAmt { get; set; }
        //public string Arrears { get; set; }
        //public string MinAmt { get; set; }
        //public decimal LateFee { get; set; }
        //public string GodownName { get; set; }
        //public decimal ReconnectionAmt { get; set; }
        //public string InvoiceValue { get; set; }
        //public string GASCOMSUMPTIONLed { get; set; }
        //public string Round { get; set; }
        //public string Diff { get; set; }
        //public decimal CGST { get; set; }
        //public decimal SGST { get; set; }
        //public string customerid { get; set; }
        //public string billid { get; set; }
        public decimal PaymentDue { get; set; }
        public decimal Amount { get; set; }
        public decimal ConnectionFee { get; set; }
        public decimal InstallationCharges { get; set; }
        public string PartyName { get; set; }
        public string BillNo { get; set; }
        public decimal ClosingBalance { get; set; }
        public string BillDate { get; set; }
        public string PreviousBillDate { get; set; }
        public decimal PreviousRedg { get; set; }
        public decimal ClosingRedg { get; set; }
        public string DueDate { get; set; }
        public decimal ConsumeUnit { get; set; }
        public decimal InputRate { get; set; }
        public string StockItemName { get; set; }
        public decimal Rate { get; set; }
        public string Month { get; set; }
        public decimal ServiceAmt { get; set; }
        public decimal Arrears { get; set; }
        public decimal MinAmt { get; set; }//public string MinAmt { get; set; }
        public decimal LateFee { get; set; }
        public string GodownName { get; set; }
        public decimal ReconnectionAmt { get; set; }
        [UIHint("Currency")]
        public decimal InvoiceValue { get; set; } // public string InvoiceValue { get; set; }
        public decimal GASCOMSUMPTIONLed { get; set; }
        public decimal Round { get; set; } //public string Round { get; set; }
        public string Diff { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal ConsumptioninKG { get; set; }
        public decimal diposit { get; set; }
        public decimal PreviousBalance { get; set; }
        public string Type { get; set; }
        public Int32 TotalRows { get; set; }
        public string customerid { get; set; }
        public string billid { get; set; }

        public string ExportStatus { get; set; }
        public string isPaid { get; set; }
    }

    public class BillInformation
    {
        
        public string BillId { get; set; }

        public string CustomerId { get; set; }
        public string StockItemId { get; set; }
        public string GodownId { get; set; }
        public string UserId { get; set; }
        public string BillNo { get; set; }
        public string BillDate { get; set; }
        public decimal ClosingBalance { get; set; }
        public decimal PreviousDue { get; set; }
        public decimal ClosingRedg { get; set; }
        public decimal PreviousBalance { get; set; }
        public decimal PreviousRedg { get; set; }
        public string DueDate { get; set; }
        public decimal ConsumeUnit { get; set; }
        public decimal Rate { get; set; }
        public Int32 BillMonth { get; set; }//current date of month
        public decimal ServiceAmt { get; set; }

        public Int32 isPaid { get; set; } //0
        public decimal ReconnectionAmt { get; set; }
        public decimal Arrears { get; set; }
        public decimal LateFee { get; set; }
        public decimal TotalAmt { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public string BillType { get; set; } // F2653C96-46D8-4609-A5ED-8568C129BAA3
        public decimal CurrentScm { get; set; }
        public decimal CurrentKGS { get; set; }
        public decimal PaymentDue { get; set; }
        public string BillPeriod { get; set; }
        public decimal MinAmt { get; set; }
        public string CreatedDate { get; set; }
        public decimal PreviousDiposite { get; set; }
        public float BillCount { get; set; }
        public decimal Round { get; set; }
        public decimal Diff { get; set; }
        public decimal TransactionFree { get; set; }
        public string delaychglist { get; set; }
        public string PartyName { get; set; }
        public string Address { get; set; }
        public string cloasingdate { get; set; }
        public string Previousbilldate { get; set; }
        public decimal Balcencedue { get; set; }
        public string msg { get; set; }
        public DateTime duedt { get; set; }
        public DateTime PreviousBill { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal LatePaymentFee { get; set; }
        public Int32 DelayDays { get; set; }
        public decimal DelayMinAmount { get; set; }
        public string obj1 { get; set; }
        public string obj2 { get; set; }
        public decimal gasConsume1 { get; set; }
        public decimal PreviousDuef { get; set; }

        public string PBillId { get; set; }
        public decimal PLateFee { get; set; }
        public decimal Dalychg { get; set; }
         public decimal PreviousLateFree { get; set; }

        public decimal Stotal { get; set; }
        public decimal SRound { get; set; }
        public decimal SDiff { get; set; }

        public decimal SCurrentScm { get; set; }
        public decimal SCurrentKGS { get; set; }
        public string Lastdat { get; set; }
    }
}