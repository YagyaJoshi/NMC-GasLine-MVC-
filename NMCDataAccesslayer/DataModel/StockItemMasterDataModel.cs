using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMCDataAccesslayer.DataModel
{
    public class StockItemMasterDataModel
    {
        public string StockItemId { get; set; }
        public string StockItemName { get; set; }
        public decimal Rate { get; set; }
        public string CompanyId { get; set; }
        public List<CompanyDataModel> Company { get; set; }
        public string CompanyName { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
        public decimal ReConnectionFee { get; set; }
        public Int32 month { get; set; }
        public Int32 Year { get; set; }
        public string StockItemGasRateId { get; set; }
        public bool IsGas { get; set; }
        public decimal weight { get; set; }
        public string day { get; set; }
        public Int32 NoEdit { get; set; }
        public string CreatedDate { get; set; }
        
        public List<StockItemMasterDataModel> StockItemMasterList { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int RecordCount { get; set; }
        public Int32 TotalRows { get; set; }
        public DateTime? RateDate { get; set; }
        public DateTime? ToDate { get; set; }
        public Int32 IsShow { get; set; }
    }
}
