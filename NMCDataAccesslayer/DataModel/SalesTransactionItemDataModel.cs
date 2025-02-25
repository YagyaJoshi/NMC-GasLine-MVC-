using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMCDataAccesslayer.DataModel
{
    class SalesTransactionItemDataModel
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
