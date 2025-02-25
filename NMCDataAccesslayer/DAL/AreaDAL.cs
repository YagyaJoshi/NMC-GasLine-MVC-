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

namespace NMCDataAccesslayer.DAL
{
    public class AreaDAL
    {
        string result;
        DbProvider _db;
        /// <summary>
        /// Get Area List Company Wise
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns>List of area list(value & text for drop down which is filling by ajax)</returns>
        public List<AreaDataModel> GetArea(string CompanyId)
        {
            List<AreaDataModel> area = new List<AreaDataModel>();
            _db = new DbProvider();
            try
            {

                _db.AddParameter("@CompanyId", Convert.ToString(CompanyId));
                DbDataReader sdr = _db.ExecuteDataReader("GetArea", CommandType.StoredProcedure);
                while (sdr.Read())
                {
                    area.Add(new AreaDataModel
                    {
                        AreaId = Convert.ToString(sdr["AreaId"]),
                        AreaName = sdr["AreaName"].ToString(),
                        Value = Convert.ToString(sdr["AreaId"]),
                        Text = sdr["AreaName"].ToString()
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
            return area;
        }
        /// <summary>
        /// Create New area
        /// </summary>
        /// <param name="model">area data model </param>
        public string Save(AreaDataModel model)
        {
            _db = new DbProvider();
            try
            {
               
                _db.AddParameter("@Id", Guid.NewGuid().ToString());
                _db.AddParameter("@AreaName", model.AreaName!=null? model.AreaName.Trim(): model.AreaName);
                _db.AddParameter("@AdminId", model.AdminId);
                _db.AddParameter("@CompanyId", model.CompanyId);
                _db.AddParameter("@CreatedAt", DateTime.Now);
                //_db.ExecuteNonQuery("InsertArea", CommandType.StoredProcedure);
                result= _db.ExecuteScalar("InsertArea", CommandType.StoredProcedure).ToString();
            }
            catch (Exception)
            {
                throw;

            }
            finally
            {
                _db.Dispose();
            }
            return result;
        }
        /// <summary>
        /// Update area 
        /// </summary>
        /// <param name="model">area data model</param>
        public string Update(AreaDataModel model)
        {
            _db = new DbProvider();
            try
            {

                _db.AddParameter("@AreaName", model.AreaName != null ? model.AreaName.Trim() : model.AreaName);
                _db.AddParameter("@AreaId", model.AreaId);
                _db.AddParameter("@CompanyId", model.CompanyId);
                _db.AddParameter("@UpdatedAt", DateTime.Now);
                result = _db.ExecuteScalar("UpdateArea", CommandType.StoredProcedure).ToString();
            }
            catch (Exception)
            {
                throw;

            }
            finally
            {
                _db.Dispose();
            }
            return result;
        }

        /// <summary>
        /// Get List Of Area
        /// </summary>
        /// <returns></returns>
        public List<AreaDataModel> GetAllArea(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey,string CityId, string AdminId)
        {
            _db = new DbProvider();
            List<AreaDataModel> Area = new List<AreaDataModel>();
            try
            {
                _db.AddParameter("@PageNo", PageNo);
                if (PageSize != 0)
                    _db.AddParameter("@PageSize", PageSize);
                if (!string.IsNullOrWhiteSpace(q))
                    _db.AddParameter("@q", q);
                _db.AddParameter("@sortkey ", sortby);
                _db.AddParameter("@sortby", sortkey != "desc" ? 1 : 0);
                _db.AddParameter("@CityId", CityId);
                _db.AddParameter("@AdminId", AdminId);
                int TotalRows = 0;
                DbDataReader sdr = _db.ExecuteDataReader("GetAllArea", CommandType.StoredProcedure);
                while (sdr.Read())
                {
                    Area.Add(new AreaDataModel
                    {
                        AreaId = Convert.ToString(sdr["Id"]),
                        AreaName = sdr["AreaName"].ToString(),
                        CompanyId = Convert.ToString(sdr["CompanyId"]),
                        CompanyName = sdr["CompanyName"].ToString(),
                        TotalRows = TotalRows = sdr["TotalRows"] != DBNull.Value ? Convert.ToInt16(sdr["TotalRows"]) : 0,
                        EmployeeName = sdr["employeename"].ToString(),
                        GodownName = sdr["godownname"].ToString(),
                        CreatedAt = Convert.ToDateTime(sdr["CreatedAt"]),
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
            return Area;

        }

        /// <summary>
        /// Get area details by area Id
        /// </summary>
        /// <param name="AreaId"></param>
        /// <returns></returns>
        public AreaDataModel GetAreaById(string AreaId)
        {
            _db = new DbProvider();
            AreaDataModel Area = new AreaDataModel();
            try
            {
                _db.AddParameter("@AreaId", AreaId);
                DbDataReader sdr = _db.ExecuteDataReader("GetAreaById", CommandType.StoredProcedure);
                while (sdr.Read())
                {
                    Area.AreaName = sdr["AreaName"].ToString();
                    Area.AreaId = Convert.ToString(sdr["Id"]);
                    Area.IsActive = Convert.ToInt16(sdr["isActive"]);
                    Area.CompanyId = Convert.ToString(sdr["CompanyId"]);

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
            return Area;
        }

        /// <summary>
        ///  Active/InActive of Area
        /// </summary>
        /// <param name="AreaId"></param>
        /// <param name="IsActive"></param>
        public void DeleteArea(string AreaId, Int16 IsActive)
        {
            _db = new DbProvider();
            try
            {
                _db.AddParameter("@AreaId", AreaId);
                _db.AddParameter("@IsActive", IsActive);
                _db.ExecuteNonQuery("DeleteArea", CommandType.StoredProcedure);
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
        /// Create Error Log
        /// </summary>
        /// <param name="ErrorDescription">What is error</param>
        /// <param name="ErrorScope"></param>
        /// <param name="Action"></param>
        /// <param name="AdminId"></param>
        public void ErrorSave(string ErrorDescription, string ErrorScope, string Action, string AdminId)
        {
            _db = new DbProvider();
            try
            {
                _db.AddParameter("@ErrorDescription", ErrorDescription);
                _db.AddParameter("@ErrorScope", ErrorScope);
                _db.AddParameter("@Action", Action);
                _db.AddParameter("@AdminId", AdminId);
                _db.AddParameter("@CreatedAt", DateTime.Now);
                _db.ExecuteNonQuery("sp_InsertErrorLog", CommandType.StoredProcedure);
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
        public List<AreaDataModel> GetAreaBillWise(string CompanyId)
        {
            List<AreaDataModel> area = new List<AreaDataModel>();
            _db = new DbProvider();
            try
            {

                _db.AddParameter("@CompanyId", Convert.ToString(CompanyId));
                DbDataReader sdr = _db.ExecuteDataReader("GetAreaBillWise", CommandType.StoredProcedure);
                while (sdr.Read())
                {
                    area.Add(new AreaDataModel
                    {
                        AreaId = Convert.ToString(sdr["AreaId"]),
                        AreaName = sdr["AreaName"].ToString(),
                        Value = Convert.ToString(sdr["AreaId"]),
                        Text = sdr["AreaName"].ToString()
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
            return area;
        }
    }
}
