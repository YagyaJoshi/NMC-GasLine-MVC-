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
using System.Reflection;

namespace NMCDataAccesslayer.DAL
{
    public class GodownDAL
    {
        DbProvider _db;
        string result;
        /// <summary>
        /// Get Godown by Area wise for Employee
        /// </summary>
        /// <param name="AreaId"></param>
        /// <returns></returns>
        public async Task<List<GodownDataModel>> GetGoDownAreawise(string AreaId)
        {
            _db = new DbProvider();
            List<GodownDataModel> listGodown = new List<GodownDataModel>();
            try
            {
                _db.AddParameter("@AreaId", AreaId);
                DbDataReader dr = await _db.ExecuteDataReaderAsync("GetgodowniwthArea", CommandType.StoredProcedure);
                while (dr.Read())
                {
                    GodownDataModel model = new GodownDataModel();
                    model.Id = Convert.ToString(dr["id"]);
                    model.Name = Convert.ToString(dr["name"]);
                    model.InputRate = Convert.ToDecimal(dr["InputRate"]);
                    model.Value = Convert.ToString(dr["id"]);
                    model.Text = Convert.ToString(dr["name"]);
                    listGodown.Add(model);
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
            return listGodown;
        }
        /// <summary>
        /// create godown 
        /// </summary>
        /// <param name="model"></param>
        public string Save(GodownDataModel model)
        {
            _db = new DbProvider();
            try
            {
                string n = RandomString(2);

                string GodownId = Guid.NewGuid().ToString();
                _db.AddParameter("@Id", GodownId);
                _db.AddParameter("@Name", model.Name.Trim());
                _db.AddParameter("@ShortName", model.ShortName != null ? model.ShortName.Trim() : model.ShortName);
                _db.AddParameter("@AdminId", model.AdminId);
                _db.AddParameter("@CompanyId", model.CompanyId);
                _db.AddParameter("@InputRate", model.InputRate);
                _db.AddParameter("@NewServiceCharge", model.NewServiceCharge);
                _db.AddParameter("@AliasName", model.AliasName);
                _db.AddParameter("@CreatedAt", DateTime.Now);
                result = _db.ExecuteScalar("InsertGodownMaster", CommandType.StoredProcedure).ToString();
                if (result == "1")
                {
                    foreach (var item in model.AreaId)
                    {
                        _db = new DbProvider();
                        _db.AddParameter("@GodownId", GodownId);
                        _db.AddParameter("@AreaId", item);
                        _db.ExecuteNonQuery("InsertGodownArea", CommandType.StoredProcedure);
                    }
                }
                else
                {
                    if (result == "3")
                    {
                        result = "Society  Name Already Exist";
                    }
                    else  if(result=="4")
                    {
                        result = "Society Alias Name Already Exist";
                    }
                    else
                    { result = "Society  Code Already Exist"; }


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
            return result;
        }

        public static System.Data.DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
        /// <summary>
        /// update Godown
        /// </summary>
        /// <param name="model"></param>
        public string Update(GodownDataModel model)
        {
            _db = new DbProvider();
            try
            {
                _db.AddParameter("@Name", model.Name.Trim());
                _db.AddParameter("@ShortName", model.ShortName != null ? model.ShortName.Trim() : model.ShortName);
                _db.AddParameter("@GodownMasterId", model.Id);
                _db.AddParameter("@CompanyId", model.CompanyId);
                _db.AddParameter("@InputRate", model.InputRate);
                _db.AddParameter("@NewServiceCharge", model.NewServiceCharge);
                _db.AddParameter("@UpdatedAt", DateTime.Now);
                _db.AddParameter("@AliasName", model.AliasName);
                result = _db.ExecuteScalar("UpdateGodownMaster", CommandType.StoredProcedure).ToString();
                if (result == "1")
                {
                    var firstNotSecond = model.AreaId.Except(model.UAreadId).ToList();
                    var firstNot = model.UAreadId.Except(model.AreaId).ToList();

                    if (firstNotSecond.Count() != 0)
                    {
                        foreach (var item in firstNotSecond)
                        {
                            _db = new DbProvider();
                            _db.AddParameter("@GodownId", model.Id);
                            _db.AddParameter("@AreaId", item);
                            _db.ExecuteNonQuery("InsertGodownArea", CommandType.StoredProcedure);
                        }
                    }

                    if (firstNot.Count() != 0)
                    {
                        foreach (var item in firstNot)
                        {
                            _db = new DbProvider();
                            _db.AddParameter("@GodownId", model.Id);
                            _db.AddParameter("@AreaId", item);
                            _db.ExecuteNonQuery("deleteGodownArea", CommandType.StoredProcedure);
                        }

                    }
                }
                else
                {
                    if (result == "3")
                    {
                        result = "Society  Name Already Exist";
                    }
                    else if (result == "4")
                    {
                        result = "Society Alias Name Already Exist";
                    }
                    else
                    {
                        result = "Society  Code Already Exist";
                    }

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
            return result;
        }

        /// <summary>
        /// Get Godown List
        /// </summary>
        /// <returns></returns>
        public List<GodownDataModel> GetGodown(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey, string CityId, string AdminId)
        {
            _db = new DbProvider();
            List<GodownDataModel> Godown = new List<GodownDataModel>();
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
                DbDataReader sdr = _db.ExecuteDataReader("GetGodownMaster", CommandType.StoredProcedure);
                int TotalRows = 0;
                while (sdr.Read())
                {
                    Godown.Add(new GodownDataModel
                    {
                        Name = sdr["Name"].ToString(),
                        ShortName = sdr["ShortName"].ToString(),
                        AliasName = sdr["AliasName"].ToString(),
                        InputRate = Convert.ToDecimal(sdr["InputRate"]),
                        NewServiceCharge = Convert.ToDecimal(sdr["NewServiceCharge"]),
                        Id = Convert.ToString(sdr["Id"]),
                        CompanyId = Convert.ToString(sdr["CompanyId"]),
                        CompanyName = Convert.ToString(sdr["CompanyName"]),
                        IsActive = Convert.ToInt16(sdr["IsActive"]),
                        TotalRows = TotalRows = sdr["TotalRows"] != DBNull.Value ? Convert.ToInt16(sdr["TotalRows"]) : 0,
                        RecordCount =  sdr["Customercount"] != DBNull.Value ? Convert.ToInt16(sdr["Customercount"]) : 0,
                        disCustomercount = sdr["disCustomercount"] != DBNull.Value ? Convert.ToInt16(sdr["disCustomercount"]) : 0,
                        CreatedAt = Convert.ToDateTime(sdr["CreatedAt"])
                    });
                }


            }
            catch (Exception ex)
            {
                throw;

            }
            finally
            {
                _db.Dispose();
            }
            return Godown;
        }

        /// <summary>
        /// Get Godown By Id
        /// </summary>
        /// <param name="GodownMasterId"></param>
        /// <returns></returns>
        public GodownDataModel GetGodownById(string GodownMasterId)
        {
            _db = new DbProvider();
            GodownDataModel Godown = new GodownDataModel();
            try
            {
                _db.AddParameter("@GodownMasterId", GodownMasterId);
                DbDataReader sdr = _db.ExecuteDataReader("GetGodownMasterById", CommandType.StoredProcedure);
                while (sdr.Read())
                {
                    Godown.Name = sdr["Name"].ToString();
                    Godown.ShortName = sdr["ShortName"].ToString();
                    Godown.AliasName = sdr["AliasName"].ToString();
                    Godown.CompanyId = Convert.ToString(sdr["CompanyId"]);
                    Godown.Id = Convert.ToString(sdr["Id"]);
                    Godown.InputRate = Convert.ToDecimal(sdr["InputRate"]);
                    Godown.NewServiceCharge = Convert.ToDecimal(sdr["NewServiceCharge"]);
                    Godown.AreaIdedit = sdr["AreaId"].ToString();
                    if (!string.IsNullOrEmpty(Godown.AreaIdedit))
                    {
                        Godown.AreaId = Godown.AreaIdedit.Split(',').Select(n => Convert.ToString(n)).ToArray();
                        Godown.UAreadId = Godown.AreaIdedit.Split(',').Select(n => Convert.ToString(n)).ToArray();
                    }

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
            return Godown;

        }

        /// <summary>
        /// Active & InActive godown
        /// </summary>
        /// <param name="GodownMasterId"></param>
        /// <param name="IsActive"></param>
        public void DeleteGodown(string GodownMasterId, Int16 IsActive)
        {
            _db = new DbProvider();
            try
            {

                _db.AddParameter("@GodownMasterId", GodownMasterId);
                _db.AddParameter("@IsActive", IsActive);
                _db.ExecuteNonQuery("DeleteGodownMaster", CommandType.StoredProcedure);

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

        public void DeleteGodownSociety(string GodownId) 
        {
            _db = new DbProvider();
            try
            {
                _db.AddParameter("@GodownId", GodownId);               
                _db.ExecuteNonQuery("DeleteGodownData", CommandType.StoredProcedure);
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
        /// Get Godown by Area wise for Employee
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        public List<GodownDataModel> GetGoDownCompanywise(string CompanyId)
        {
            _db = new DbProvider();
            List<GodownDataModel> listGodown = new List<GodownDataModel>();
            try
            {
                _db.AddParameter("@CompanyId", CompanyId);
                DbDataReader dr = _db.ExecuteDataReader("GetGodownMaster_Cust", CommandType.StoredProcedure);
                while (dr.Read())
                {
                    GodownDataModel model = new GodownDataModel();
                    model.Id = Convert.ToString(dr["Id"]);
                    model.Name = Convert.ToString(dr["Name"]);
                    model.Value = Convert.ToString(dr["Id"]);
                    model.Text = Convert.ToString(dr["Name"]);
                    listGodown.Add(model);
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
            return listGodown;
        }

        public async Task<List<GodownDataModel>> GetGoDownBillWise(string AreaId)
        {
            _db = new DbProvider();
            List<GodownDataModel> listGodown = new List<GodownDataModel>();
            try
            {
                _db.AddParameter("@AreaId", AreaId);
                DbDataReader dr = await _db.ExecuteDataReaderAsync("GetGodownBillWise", CommandType.StoredProcedure);
                while (dr.Read())
                {
                    GodownDataModel model = new GodownDataModel();
                    model.Id = Convert.ToString(dr["id"]);
                    model.Name = Convert.ToString(dr["name"]);
                    //model.InputRate = Convert.ToDecimal(dr["InputRate"]);
                    model.Value = Convert.ToString(dr["id"]);
                    model.Text = Convert.ToString(dr["name"]);
                    listGodown.Add(model);
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
            return listGodown;
        }

        public Random random = new Random();
        public string RandomString(int length)
        {
            const string pool = "abcdefghijklmnopqrstuvwxyz";
            var chars = Enumerable.Range(0, length)
                .Select(x => pool[random.Next(0, pool.Length)]);
            return new string(chars.ToArray());
        }

    }
}
