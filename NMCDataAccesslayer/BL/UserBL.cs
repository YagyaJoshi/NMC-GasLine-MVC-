using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NMCDataAccesslayer.DAL;
using NMCDataAccesslayer.DataModel;
using System.Data;
using NMCPipedGasLineAPI.Models;

namespace NMCDataAccesslayer.BL
{
    public class UserBL
    {
        UserDAL obj = new UserDAL();

        public List<CountryDataModel> GetCountry()
        {
            List<CountryDataModel> Country = new List<CountryDataModel>();
            return Country = obj.GetCountry();
        }


        public List<RoleDataModel> GetRole()
        {
            List<RoleDataModel> Role = new List<RoleDataModel>();
            return Role = obj.GetRole();
        }

        public List<StateDataModel> GetState(string CountryId, string cityId)
        {
            List<StateDataModel> State = new List<StateDataModel>();
            return State = obj.GetState(CountryId, cityId);
        }

        public List<CityDataModel> GetCity(string StateId, string cityId)
        {
            List<CityDataModel> City = new List<CityDataModel>();
            return City = obj.GetCity(StateId, cityId);
        }


        public List<StateDataModel> AllGetState()
        {
            List<StateDataModel> State = new List<StateDataModel>();
            return State = obj.AllGetState();
        }

        public List<CityDataModel> AllGetCity()
        {
            List<CityDataModel> City = new List<CityDataModel>();
            return City = obj.AllGetCity();
        }


        public void SaveUser(UserDataModel objModel)
        {
            obj.SaveUser(objModel);
        }

        public void UpdateUser(UserDataModel objModel)
        {
            obj.UpdateUser(objModel);
        }

        public List<UserDataModel> GetAllUser(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey, string CityId, string AdminId)
        {
            List<UserDataModel> User = new List<UserDataModel>();
            return User = obj.GetUser(PageNo,  PageSize,  q, sortby, sortkey,  CityId,  AdminId);
        }

        public UserDataModel GetUser(string UserId)
        {
            UserDataModel User = new UserDataModel();
            return User = obj.GetUserById(UserId);
        }

        public void DeleteUser(string UserId, Int16 IsActive)
        {
            obj.DeleteUser(UserId, IsActive);
        }

        public async Task<UserDataModel> GetUserDetail(string id)
        {
            return await obj.GetUserDetail(id);
        }


        public List<UserDataModel> GetEmployeeFile(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey)
        {
            List<UserDataModel> User = new List<UserDataModel>();
            return User = obj.GetEmployeeFile(PageNo, PageSize, q, sortby, sortkey);
        }
        public UserDataModel GetMaxEmpCode()
        {
            UserDataModel EmpCode = new UserDataModel();
            return EmpCode = obj.GetMaxEmpCode();
        }
    }
}
