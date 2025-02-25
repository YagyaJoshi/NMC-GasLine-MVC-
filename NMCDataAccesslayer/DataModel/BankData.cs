using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMCDataAccesslayer.DataModel
{
  public  class BankData
    {
        public string Id { get; set; }

        public string BankName { get; set; }
        public string AdminId { get; set; }
        public Int32 TotalRows { get; set; }
        public Int32 IsActive { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int RecordCount { get; set; }
        public List<BankData> BankMasterList { get; set; }
    }
}
