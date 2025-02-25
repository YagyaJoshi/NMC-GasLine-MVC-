using NMCDataAccesslayer.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NMCDataAccesslayer.DataModel;

namespace NMCDataAccesslayer.BL
{
    public class AreaBL
    {
        string result;
        AreaDAL obj = new AreaDAL ();
        public List<AreaDataModel> GetArea(string CompanyId)
        {
            List<AreaDataModel> Area = new List<AreaDataModel>();
            return Area = obj.GetArea(CompanyId);
        }

        public List<AreaDataModel> GetAreaBillWise(string CompanyId)
        {
            List<AreaDataModel> Area = new List<AreaDataModel>();
            return Area = obj.GetAreaBillWise(CompanyId);
        }

        public void DeleteArea(string UserId, Int16 IsActive)
        {
            obj.DeleteArea(UserId, IsActive);
        }


        public string Save(AreaDataModel objModel)
        {
          return  result= obj.Save(objModel);
        }

        public string Update(AreaDataModel objModel)
        {
         return   result= obj.Update(objModel);
        }

        public List<AreaDataModel> GetAllArea(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey, string CityId, string AdminId)
        {
            List<AreaDataModel> Area = new List<AreaDataModel>();
            return Area = obj.GetAllArea(PageNo,  PageSize, q, sortby,  sortkey, CityId, AdminId);
        }

        public AreaDataModel GetAreaById(string AreaId)
        {
            AreaDataModel Area = new AreaDataModel();
            return Area = obj.GetAreaById(AreaId);
        }



        public void ErrorSave(string ErrorDescription, string ErrorScope, string Action, string AdminId)
        {
            obj.ErrorSave(ErrorDescription, ErrorScope,  Action, AdminId);
        }



        
    }
}
