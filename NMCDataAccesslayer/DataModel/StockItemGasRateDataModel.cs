using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMCDataAccesslayer.DataModel
{
    public class StockItemGasRateDataModel
    {
        public string StockItemId { get; set; }
        public string Ratemonth { get; set; }
        public string createdate { get; set; }
        public string ToCreatedDate { get; set; }
        public decimal Rate { get; set; }
        public string RateYear { get; set; }
        public Int32 TotalRows { get; set; }
        public decimal weight { get; set; }
    }
}
