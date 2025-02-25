using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NMCPipedGasLineAPI.Models
{
    public class SalesTransactionItem
    {

        public string SaleTransactionItemId { get; set; }

        public string SaleTransactionItemName { get; set; }

        public decimal Quantity { get; set; }

        public decimal Rate { get; set; }

        public decimal TotalAmount { get; set; }

        public string SaleId { get; set; }

        public DateTime CreatedDate { get; set; }

    }
}