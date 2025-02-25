using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NMCPipedGasLineAPI.Models
{
    public class StockItemGasRate
    {
        public string StockItemId { get; set; }
        public string Ratemonth { get; set; }
        public string createdate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ToCreatedDate { get; set; }
        public decimal Rate { get; set; }
        public string RateYear { get; set; }
        public decimal weight { get; set; }
        
        public Int32 TotalRows { get; set; }
    }
}