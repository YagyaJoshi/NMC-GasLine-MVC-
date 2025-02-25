using NMCDataAccesslayer.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NMCPipedGasLineAPI.Models
{
    public class GetDataFromExcel
    {
        public string PartyName { get; set; }
        public string ClosingBalance { get; set; }
        public string PrevBillDate { get; set; }
        public string PrevReading { get; set; }
        public string ContactNos { get; set; }
        public string EmailId { get; set; }

        [Required(ErrorMessage = "Please Select Area Name")]
        public string AreaId { get; set; }
        public List<AreaMaster> Area { get; set; }

        [Required(ErrorMessage = "Please Select Godown Name")]
        public string GoDownId { get; set; }
        public List<GodownMaster> GoDown { get; set; }


        [Required(ErrorMessage = "Please Select Company Name")]
        public string CompanyId { get; set; }
        public List<CompanyDataModel> Company { get; set; }

        public List<GetDataFromExcel> list { get; set; }
    }
}