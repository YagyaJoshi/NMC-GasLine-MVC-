using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NMCDataAccesslayer.DataModel
{
  public class CustomerReading
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string CurrentRedg { get; set; }
        public string Type { get; set; }
        public string CurrentRedgDate { get; set; }
        public string MeterRedgImage { get; set; }
        public string BillNo { get; set; }
        public string BillDate { get; set; }
        public string BillId { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
        public Int32 TotalRows { get; set; }
        public string UpdateMeterRedgImage { get; set; }
        public string Name { get; set; }
        public string AreaName { get; set; }
        public string CompanyName { get; set; }
        public string GodownName { get; set; }
        public decimal PreviousRedg { get; set; }
        public string PreviousBillDate { get; set; }

    }

    public class reading
    {
        public string CompanyId { get; set; }
        public string AreaId { get; set; }
        public string GodownId { get; set; }
        public string ReadingDate { get; set; }
    }
}
