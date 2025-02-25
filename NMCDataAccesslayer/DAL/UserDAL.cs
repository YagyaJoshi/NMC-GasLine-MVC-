using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbProviderFactorie;
using System.Data.SqlClient;
using System.Data;
using NMCDataAccesslayer.DataModel;
using System.Data.Common;
using System.Web;

namespace NMCDataAccesslayer.DAL
{
    public class UserDAL
    {
        DbProvider _db;
        /// <summary>
        /// get Country List for dropdown
        /// </summary>
        /// <returns></returns>
        public List<CountryDataModel> GetCountry()
        {
            List<CountryDataModel> Country = new List<CountryDataModel>();
            _db = new DbProvider();
            try
            {

                DbDataReader sdr = _db.ExecuteDataReader("GetCountry", CommandType.StoredProcedure);
                while (sdr.Read())
                {
                    Country.Add(new CountryDataModel
                    {
                        CountryId = Convert.ToString(sdr["Id"]),
                        CountryName = sdr["CountryName"].ToString()
                    });
                }
               

            }
            catch (Exception)
            {
                throw;

            }
            finally
            {
                _db.Dispose();
            }
            return Country;
        }

        /// <summary>
        /// get Country List for dropdown
        /// </summary>
        /// <returns></returns>
        public List<NMCPipedGasLineAPI.Models.RoleDataModel> GetRole()
        {
            List<NMCPipedGasLineAPI.Models.RoleDataModel> Country = new List<NMCPipedGasLineAPI.Models.RoleDataModel>();
            _db = new DbProvider();
            try
            {

                DbDataReader sdr = _db.ExecuteDataReader("GetRole", CommandType.StoredProcedure);
                while (sdr.Read())
                {
                    Country.Add(new NMCPipedGasLineAPI.Models.RoleDataModel
                    {
                        Id = Convert.ToString(sdr["Name"]),
                        Name = sdr["Name"].ToString()
                    });
                }


            }
            catch (Exception)
            {
                throw;

            }
            finally
            {
                _db.Dispose();
            }
            return Country;
        }


        /// <summary>
        /// get State List for dropdown Country wise
        /// </summary>
        /// <param name="CountryId"></param>
        /// <returns></returns>
        public List<StateDataModel> GetState(string CountryId, string cityId)
        {
            List<StateDataModel> State = new List<StateDataModel>();
            _db = new DbProvider();
            try
            {

                _db.AddParameter("@CountryId", CountryId);
                _db.AddParameter("@cityId", cityId);
                DbDataReader sdr = _db.ExecuteDataReader("GetState", CommandType.StoredProcedure);

                while (sdr.Read())
                {
                    State.Add(new StateDataModel
                    {
                        StateId = Convert.ToString(sdr["Id"]),
                        StateName = sdr["StateName"].ToString(),
                        Value = Convert.ToString(sdr["Id"]),
                        Text = sdr["StateName"].ToString()
                    });
                }
              

            }
            catch (Exception)
            {
                throw;

            }
            finally
            {
                _db.Dispose();
            }
            return State;
        }
        /// <summary>
        /// get city List for dropdown state wise
        /// </summary>
        /// <param name="StateId"></param>
        /// <returns></returns>
        public List<CityDataModel> GetCity(string StateId, string CityId)
        {
            List<CityDataModel> City = new List<CityDataModel>();
            _db = new DbProvider();
            try
            {

                _db.AddParameter("@StateId", StateId);
                _db.AddParameter("@CityId", CityId);
                DbDataReader sdr = _db.ExecuteDataReader("GetCity", CommandType.StoredProcedure);

                while (sdr.Read())
                {
                    City.Add(new CityDataModel
                    {
                        CityId = Convert.ToString(sdr["Id"]),
                        CityName = sdr["CityName"].ToString(),
                        Value = Convert.ToString(sdr["Id"]),
                        Text = sdr["CityName"].ToString()
                    });
                }
                

            }
            catch (Exception)
            {
                throw;

            }
            finally
            {
                _db.Dispose();
            }
            return City;
        }



        /// <summary>
        /// get State List for dropdown Country wise
        /// </summary>
        /// <param name="CountryId"></param>
        /// <returns></returns>
        public List<StateDataModel> AllGetState()
        {
            List<StateDataModel> State = new List<StateDataModel>();
            _db = new DbProvider();
            try
            {

               
                DbDataReader sdr = _db.ExecuteDataReader("AllGetState", CommandType.StoredProcedure);

                while (sdr.Read())
                {
                    State.Add(new StateDataModel
                    {
                        StateId = Convert.ToString(sdr["Id"]),
                        StateName = sdr["StateName"].ToString(),
                        Value = Convert.ToString(sdr["Id"]),
                        Text = sdr["StateName"].ToString()
                    });
                }


            }
            catch (Exception)
            {
                throw;

            }
            finally
            {
                _db.Dispose();
            }
            return State;
        }
        /// <summary>
        /// get city List for dropdown state wise
        /// </summary>
        /// <param name="StateId"></param>
        /// <returns></returns>
        public List<CityDataModel> AllGetCity()
        {
            List<CityDataModel> City = new List<CityDataModel>();
            _db = new DbProvider();
            try
            {

               
                DbDataReader sdr = _db.ExecuteDataReader("AllGetCity", CommandType.StoredProcedure);

                while (sdr.Read())
                {
                    City.Add(new CityDataModel
                    {
                        CityId = Convert.ToString(sdr["Id"]),
                        CityName = sdr["CityName"].ToString(),
                        Value = Convert.ToString(sdr["Id"]),
                        Text = sdr["CityName"].ToString()
                    });
                }


            }
            catch (Exception)
            {
                throw;

            }
            finally
            {
                _db.Dispose();
            }
            return City;
        }



        /// <summary>
        /// create new Employee &  user 
        /// </summary>
        /// <param name="model"></param>
        public void SaveUser(UserDataModel model)
        {
            _db = new DbProvider();
            try
            {
                _db.AddParameter("@UserId", model.UserId);
                _db.AddParameter("@Name", model.Name !=null?model.Name.Trim():model.Name);
                _db.AddParameter("@Phone", model.Phone !=null?model.Phone.Trim(): model.Phone);
                _db.AddParameter("@Pincode", model.Pincode !=null ?model.Pincode.Trim(): model.Pincode);
                _db.AddParameter("@Address", model.Address !=null ?model.Address.Trim(): model.Address);
                _db.AddParameter("@CountryId", model.CountryId);
            
                _db.AddParameter("@StateId", model.StateId);
                _db.AddParameter("@CityId", model.CityId);
                _db.AddParameter("@AreaId", model.AreaId);
                _db.AddParameter("@CompanyId", model.CompanyId);
                _db.AddParameter("@CreatedAt", DateTime.Now);
                _db.AddParameter("@EmpCode", 0);
                _db.ExecuteNonQuery("sp_InsertUser", CommandType.StoredProcedure);

            }
            catch (Exception)
            {
                throw;

            }
            finally
            {
                _db.Dispose();
            }
        }

        /// <summary>
        /// Update Employee & user deatils
        /// </summary>
        /// <param name="model"></param>
        public void UpdateUser(UserDataModel model)
        {
            _db = new DbProvider();
            try
            {

                _db.AddParameter("@Id", model.Id);
                _db.AddParameter("@Name", model.Name != null ? model.Name.Trim() : model.Name);
                _db.AddParameter("@Phone", model.Phone != null ? model.Phone.Trim() : model.Phone);
                _db.AddParameter("@Pincode", model.Pincode != null ? model.Pincode.Trim() : model.Pincode);
                _db.AddParameter("@Address", model.Address != null ? model.Address.Trim() : model.Address);
                _db.AddParameter("@CountryId", model.CountryId);
                _db.AddParameter("@StateId", model.StateId);
                _db.AddParameter("@CityId", model.CityId);
                _db.AddParameter("@AreaId", model.AreaId);
              
                _db.AddParameter("@CompanyId", model.CompanyId);
                _db.AddParameter("@Password", model.Password);
                _db.AddParameter("@UserId", model.UserId);
                _db.AddParameter("@UpdatedAt", DateTime.Now);
                _db.ExecuteNonQuery("sp_UpdateUser", CommandType.StoredProcedure);

            }
            catch (Exception)
            {
                throw;

            }
            finally
            {
                _db.Dispose();
            }
        }

        /// <summary>
        /// get user List
        /// </summary>
        /// <returns></returns>
        public List<UserDataModel> GetUser(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey, string CityId, string AdminId)
        {
            _db = new DbProvider();
            List<UserDataModel> User = new List<UserDataModel>();
            try
            {
                _db.AddParameter("@PageNo", PageNo);
                if (PageSize != 0)
                    _db.AddParameter("@PageSize", PageSize);
                if (!string.IsNullOrWhiteSpace(q))
                    _db.AddParameter("@q", q);
                _db.AddParameter("@sortkey ", sortby);
                _db.AddParameter("@sortby", sortkey != "desc" ? 1 : 0);
                _db.AddParameter("@CityId ", CityId);
                _db.AddParameter("@AdminId ", AdminId);
                int TotalRows = 0;
                DbDataReader sdr = _db.ExecuteDataReader("GetAllUser", CommandType.StoredProcedure);
                while (sdr.Read())
                {
                    User.Add(new UserDataModel
                    {
                        Id = sdr["Id"] != DBNull.Value ? Convert.ToString(sdr["Id"]) : string.Empty,
                        Name = sdr["Name"].ToString(),
                        Phone = sdr["Phone"].ToString(),
                        Address = sdr["Address"].ToString(),
                        Email = sdr["Email"].ToString(),
                        Pincode = sdr["Pincode"] != DBNull.Value ? Convert.ToString(sdr["Pincode"]) : "",
                        CountryId = sdr["CountryId"] != DBNull.Value ? Convert.ToString(sdr["CountryId"]) : "",
                        CompanyId = sdr["CompanyId"] != DBNull.Value ? Convert.ToString(sdr["CompanyId"]) : "",
                        StateName = sdr["StateName"].ToString(),
                        CountryName = sdr["CountryName"].ToString(),
                        StateId = sdr["StateId"] != DBNull.Value ? Convert.ToString(sdr["StateId"]) : "",
                        CompanyName = sdr["CompanyName"].ToString(),
                        CityId = sdr["CityId"] != DBNull.Value ? Convert.ToString(sdr["CityId"]) : "",
                        CityName = sdr["CityName"].ToString(),
                        AreaId = sdr["AreaId"] != DBNull.Value ? Convert.ToString(sdr["AreaId"]) : "",
                        AreaName = sdr["AreaName"].ToString(),
                        TotalRows = TotalRows = sdr["TotalRows"] != DBNull.Value ? Convert.ToInt16(sdr["TotalRows"]) : 0,
                        RoleName = sdr["rolename"].ToString(),
                        CreatedAt = Convert.ToDateTime(sdr["CreatedAt"])
                    });
                }
             

            }
            catch (Exception)
            {
                throw;

            }
            finally
            {
                _db.Dispose();
            }
            return User;
        }

        /// <summary>
        /// get user & employee  details by id
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public UserDataModel GetUserById(string UserId)
        {
            _db = new DbProvider();
            UserDataModel User = new UserDataModel();
            try
            {

                _db.AddParameter("@UserId", UserId);
                DbDataReader sdr = _db.ExecuteDataReader("GetUserById", CommandType.StoredProcedure);
                while (sdr.Read())
                {
                    User.Id = Convert.ToString(sdr["Id"]);
                    User.Name = sdr["Name"].ToString();
                    User.Phone = sdr["Phone"].ToString();
                    User.Pincode = Convert.ToString(sdr["Pincode"]);
                    User.Address = sdr["Address"].ToString();
                    User.isActive = Convert.ToInt16(sdr["isActive"]);
                    User.Email = sdr["Email"].ToString();
                    User.Password = sdr["PasswordHash"].ToString();
                    User.UserId = sdr["UserId"].ToString();
                    User.CityId = sdr["CityId"] != DBNull.Value ? Convert.ToString(sdr["CityId"]) : "";
                    User.CountryId = sdr["CountryId"] != DBNull.Value ? Convert.ToString(sdr["CountryId"]) : "";
                    User.CompanyId = sdr["CompanyId"] != DBNull.Value ? Convert.ToString(sdr["CompanyId"]) : "";
                    User.AreaId = sdr["AreaId"] != DBNull.Value ? Convert.ToString(sdr["AreaId"]) : "";
                    User.StateId = sdr["StateId"] != DBNull.Value ? Convert.ToString(sdr["StateId"]) : "";
                    User.RoleId = sdr["Rolename"] != DBNull.Value ? Convert.ToString(sdr["Rolename"]) : "";
                    User.RoleName = sdr["Rolename"] != DBNull.Value ? Convert.ToString(sdr["Rolename"]) : "";
                    User.oldRoleName = sdr["Rolename"] != DBNull.Value ? Convert.ToString(sdr["Rolename"]) : "";
                    User.EmpCode = sdr["EmpCode"] == DBNull.Value ? (int?)null : Convert.ToInt32(sdr["EmpCode"]);

                }
             
            }
            catch (Exception)
            {
                throw;

            }
            finally
            {
                _db.Dispose();
            }
            return User;
        }

        /// <summary>
        /// Active & InActive user 
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="IsActive"></param>
        public void DeleteUser(string UserId, Int16 IsActive)
        {
            _db = new DbProvider();
            try
            {

                _db.AddParameter("@UserId", UserId);
                _db.AddParameter("@IsActive", IsActive);
                _db.ExecuteNonQuery("DeleteUser", CommandType.StoredProcedure);

            }
            catch (Exception)
            {
                throw;

            }
            finally
            {
                _db.Dispose();
            }
        }
        /// <summary>
        /// Get User Details by id for mobile app
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<UserDataModel> GetUserDetail(string id)
        {
            _db = new DbProvider();
            UserDataModel model = new UserDataModel();
            try
            {
                _db.AddParameter("@UserId", id, DbType.String, ParameterDirection.Input);
                DbDataReader dr = await _db.ExecuteDataReaderAsync("getUserDetail", CommandType.StoredProcedure);
                dr.Read();
                if (dr.HasRows)
                {
                    model.Id = Convert.ToString(dr["Id"]);
                    model.Name = Convert.ToString(dr["Name"]);
                    model.Address = Convert.ToString(dr["Address"]);
                    model.Phone = Convert.ToString(dr["Phone"]);
                    model.AreaId = Convert.ToString(dr["AreaId"]);
                    model.CompanyName = Convert.ToString(dr["CompanyName"]);
                    model.CityName = Convert.ToString(dr["CityName"]);
                    model.AreaName = Convert.ToString(dr["AreaName"]);
                    model.StateName = Convert.ToString(dr["StateName"]);
                    model.CompanyId = Convert.ToString(dr["CompanyId"]);
                    model.CountryId = Convert.ToString(dr["CountryId"]);
                    model.CityId = Convert.ToString(dr["CityId"]);
                    model.StateId = Convert.ToString(dr["StateId"]);
                    model.RoleName = Convert.ToString(dr["rolename"]);
                    model.isActive = Convert.ToInt32(dr["isActive"]);
                    model.EmpCode = dr["EmpCode"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["EmpCode"]);

                }
                dr.Close();


            }
            catch (Exception)
            {
                throw;

            }
            finally
            {
                _db.Dispose();
            }
            return model;
        }



        public List<UserDataModel> GetEmployeeFile(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey)
        {
            _db = new DbProvider();
            List<UserDataModel> User = new List<UserDataModel>();
            try
            {
                _db.AddParameter("@PageNo", PageNo);
                if (PageSize != 0)
                    _db.AddParameter("@PageSize", PageSize);
                if (!string.IsNullOrWhiteSpace(q))
                    _db.AddParameter("@q", q);
                _db.AddParameter("@sortkey ", sortby);
                _db.AddParameter("@sortby", sortkey != "desc" ? 1 : 0);
              
                int TotalRows = 0;
                DbDataReader sdr = _db.ExecuteDataReader("GetEmployeeFile", CommandType.StoredProcedure);
                while (sdr.Read())
                {
                    User.Add(new UserDataModel
                    {
                        Id = sdr["Id"] != DBNull.Value ? Convert.ToString(sdr["Id"]) : string.Empty,
                        Name = sdr["Name"].ToString(),
                        BillFileName = sdr["BillFileName"].ToString(),
                        PaymentFileName = sdr["PaymentFileName"].ToString(),
                        filedate = Convert.ToString(sdr["CreatedAt"])
                    });
                }
                

            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
                _db.Dispose();
            }
            return User;
        }
        public UserDataModel GetMaxEmpCode()
        {
            UserDataModel EmpCode = new UserDataModel();
            _db = new DbProvider();
            try
            {

                DbDataReader sdr = _db.ExecuteDataReader("GetMaxEmpCode", CommandType.StoredProcedure);
                while (sdr.Read())
                {

                   EmpCode.EmpCode = sdr["EmpCode"] == DBNull.Value ? (int?)null : Convert.ToInt32(sdr["EmpCode"]);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _db.Dispose();
            }
            return EmpCode;
        }

    }
}
