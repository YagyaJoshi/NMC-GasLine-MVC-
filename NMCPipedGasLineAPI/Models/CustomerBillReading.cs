using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace NMCPipedGasLineAPI.Models
{
    public class CustomerBillReading
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        [Required(ErrorMessage = "Current Reading is required.")]
        //[RegularExpression(@"^[1-9]\d*(\.\d+)?$", ErrorMessage = "Invaild Reading .")]
       
        [DataType("decimal(18 ,2")]
        public decimal CurrentRedg { get; set; }
        public string CurrentRedgDate { get; set; }
        //[Required(ErrorMessage = "Meter Reading Image is required.")]
        public string MeterRedgImage { get; set; }
        public string BillId { get; set; }
        public string BillNo { get; set; }
        public string BillDate { get; set; }
        //[Required(ErrorMessage = "Meter Reading Image is required.")]
        public HttpPostedFileBase ImageFile { get; set; }
        public Int32 TotalRows { get; set; }

        public string UpdateMeterRedgImage { get; set; }
        public string Name { get; set; }
        public string AreaName { get; set; }
        public string CompanyName { get; set; }
        public string GodownName { get; set; }
        public decimal PreviousRedg { get; set; }
        public string PreviousBillDate { get; set; }
        public string Type { get; set; }
    }

}