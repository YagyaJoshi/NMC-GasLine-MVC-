using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMCDataAccesslayer.DataModel
{
    public class StockItemRateDataModel
    {
        public string StockItemId { get; set; }
        public string Rate { get; set; }
        public string Ratemonth { get; set; }
        public string RateYear { get; set; }
        public string CreatedDate { get; set; }
        public string weight { get; set; }
        public string ToCreatedDate { get; set; }
    }
}
