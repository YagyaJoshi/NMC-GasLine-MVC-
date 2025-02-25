using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NMCPipedGasLineAPI.Models
{
    public class StockItemRate
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