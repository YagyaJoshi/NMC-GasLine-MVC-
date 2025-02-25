using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NMCDataAccesslayer.DataModel;
using NMCPipedGasLineAPI.Models;

namespace NMCDataAccesslayer.DataModel
{
    public class UserDataModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string CountryId { get; set; }
        public List<CountryDataModel> Country { get; set; }

        public string StateId { get; set; }
        public List<StateDataModel> State { get; set; }

        public string CityId { get; set; }
        public List<CityDataModel> City { get; set; }

        public string AreaId { get; set; }
        public List<AreaDataModel> Area { get; set; }
        public string Pincode { get; set; }
        public DateTime CreatedDate { get; set; }
        public Int32 CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int isActive { get; set; }
        public string CompanyId { get; set; }
        public List<CompanyDataModel> Company { get; set; }
        public string CompanyName { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string AreaName { get; set; }
        public string UpdatePassword { get; set; }
        public string RoleName { get; set; }
        
        public string GoDownId { get; set; }
        public List<GodownDataModel> GoDown { get; set; }

        public List<UserDataModel> UserList { get; set; }
        public List<StockItemMasterDataModel> StockItemList { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int RecordCount { get; set; }
        public Int32 TotalRows { get; set; }
        public List<RoleDataModel> Role { get; set; }
        public string RoleId { get; set; }
        public string oldRoleName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string BillFileName { get; set; }
        public string PaymentFileName { get; set; }
        public string filedate { get; set; }
        public int? EmpCode { get; set; }
    
    }
}

