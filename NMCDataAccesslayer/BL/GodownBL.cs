using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NMCDataAccesslayer.DAL;
using NMCDataAccesslayer.DataModel;
using System.Data;

namespace NMCDataAccesslayer.BL
{
    public class GodownBL
    {
        string result;
        GodownDAL objgodown = new GodownDAL();
        public async Task<List<GodownDataModel>> GetGoDownAreawise(string AreaId)
        {
            return await objgodown.GetGoDownAreawise(AreaId);
        }
        public void DeleteGodown(string GodownMasterId, Int16 IsActive)
        {
            objgodown.DeleteGodown(GodownMasterId, IsActive);
        }
        public void DeleteGodownSociety(string GodownId)
        {
            objgodown.DeleteGodownSociety(GodownId);
        }
        public string Save(GodownDataModel objModel)
        {
          return  objgodown.Save(objModel);
        }

        public string Update(GodownDataModel objModel)
        {
            return objgodown.Update(objModel);
        }

        public List<GodownDataModel> GetAllGodown(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey, string CityId, string AdminId)
        {
            List<GodownDataModel> Godown = new List<GodownDataModel>();
            return Godown = objgodown.GetGodown(PageNo,  PageSize,  q, sortby, sortkey,  CityId, AdminId);
        }

        public GodownDataModel GetGodownById(string GodownMasterId)
        {
            GodownDataModel Godown = new GodownDataModel();
            return Godown = objgodown.GetGodownById(GodownMasterId);
        }

        public List<GodownDataModel> GetGodownByCompanyId(string CompanyId)
        {
            return objgodown.GetGoDownCompanywise(CompanyId);
        }

        public async Task<List<GodownDataModel>> GetGoDownBillWise(string AreaId)
        {
            return await objgodown.GetGoDownBillWise(AreaId);
        }
      
    }
}
