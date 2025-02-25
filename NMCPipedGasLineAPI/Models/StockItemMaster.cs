using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NMCPipedGasLineAPI.Models
{
    public class StockItemMaster : IValidatableObject
    {
        public string StockItemId { get; set; }
        [StringLength(50, ErrorMessage = "Name must be between 1 and 50 char")]
        [Required(ErrorMessage = "Please Enter Stock Item ")]
        public string StockItemName { get; set; }
        [Required(ErrorMessage = "Please Select Company Name")]
        public string CompanyId { get; set; }
        public List<CompanyMaster> Company { get; set; }
        public string CompanyName { get; set; }
        //, ErrorMessage = "Please Enter Rate valid rate"
        [Required(ErrorMessage = "Please Enter Rate")]
        [Range(0, 9999999999999999.99)]
        [RegularExpression(@"^-{0,1}\d+\.{0,1}\d*$")]
        public decimal Rate { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
        public Int32 month { get; set; }
        public Int32 Year { get; set; }
        public decimal ReConnectionFee { get; set; }
        public string StockItemGasRateId { get; set; }
        public bool IsGas { get; set; }
        [Required(ErrorMessage = "Please Enter weight")]
        public decimal weight { get; set; }
        public string day { get; set; }
        public string CreatedDate { get; set; }
        public Int32 NoEdit { get; set; }
        public List<StockItemMaster> StockItemMasterList { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int RecordCount { get; set; }
        public Int32 TotalRows { get; set; }
       
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RateDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; }

        public Int32 IsShow { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ToDate < RateDate)
            {
                yield return new ValidationResult(
                    errorMessage: "To Date must be greater than From Date",
                    memberNames: new[] { "ToDate" }
               );
            }
        }
    }
}