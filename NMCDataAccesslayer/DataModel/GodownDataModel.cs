using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NMCDataAccesslayer.DataModel
{
    public class GodownDataModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal InputRate { get; set; }
        public Int32 IsActive { get; set; }
        public string CompanyId { get; set; }
        public string AdminId { get; set; }
        public decimal NewServiceCharge { get; set; }
        public List<CompanyDataModel> Company { get; set; }
        public string CompanyName { get; set; }
        public List<AreaDataModel> Area { get; set; }
        public string[] AreaId { get; set; }
        public string[] UAreadId { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
        //public List<SelectListItem> Fruits { get; set; }
        //public int[] FruitIds { get; set; }
        public List<GodownDataModel> GodownMasterList { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int RecordCount { get; set; }
        public int disCustomercount { get; set; }
        
        public string AreaIdedit { get; set; }
        public string ShortName { get; set; }
        public Int32 TotalRows { get; set; }
        public string AliasName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}