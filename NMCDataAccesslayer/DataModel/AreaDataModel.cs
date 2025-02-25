using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NMCDataAccesslayer.DataModel
{
    public class AreaDataModel
    {
        public string AreaId { get; set; }
        public string AreaName { get; set; }
        public string CityId { get; set; }
        public string CountryId { get; set; }
        public string StateId { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
        public string CompanyName { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public Int32 IsActive { get; set; }
        public string AdminId { get; set; }
        public string CompanyId { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<CompanyDataModel> Company { get; set; }
        public List<AreaDataModel> AreaMasterList { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int RecordCount { get; set; }

        public Int32 TotalRows { get; set; }
        public string EmployeeName { get; set; }
        public string GodownName { get; set; }

    }
}