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
    public class BankDAL
    {

        string result;
        DbProvider _db;

        /// <summary>
        /// Create New area
        /// </summary>
        /// <param name="model">area data model </param>
        public string Save(BankData model)
        {
            _db = new DbProvider();
            try
            {

                _db.AddParameter("@Id", Guid.NewGuid().ToString());
                _db.AddParameter("@BankName", model.BankName != null ? model.BankName.Trim() : model.BankName);
                _db.AddParameter("@AdminId", model.AdminId);
                _db.AddParameter("@CreatedAt", DateTime.Now);
                result = _db.ExecuteScalar("InsertBank", CommandType.StoredProcedure).ToString();
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
        public string Update(BankData model)
        {
            _db = new DbProvider();
            try
            {
                _db.AddParameter("@Id", model.Id);
                _db.AddParameter("@BankName", model.BankName != null ? model.BankName.Trim() : model.BankName);
                _db.AddParameter("@UpdatedAt", DateTime.Now);
                result = _db.ExecuteScalar("UpdateBank", CommandType.StoredProcedure).ToString();
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
        public List<BankData> GetAllBank(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey)
        {
            _db = new DbProvider();
            List<BankData> Bank = new List<BankData>();
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
                DbDataReader sdr = _db.ExecuteDataReader("GetAllBank", CommandType.StoredProcedure);
                while (sdr.Read())
                {
                    Bank.Add(new BankData
                    {
                        Id = Convert.ToString(sdr["Id"]),
                        BankName = sdr["BankName"].ToString(),
                        TotalRows = TotalRows = sdr["TotalRows"] != DBNull.Value ? Convert.ToInt16(sdr["TotalRows"]) : 0,

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
            return Bank;

        }

        /// <summary>
        /// Get area details by area Id
        /// </summary>
        /// <param name="BankId"></param>
        /// <returns></returns>
        public BankData GetBankById(string BankId)
        {
            _db = new DbProvider();
            BankData Bank = new BankData();
            try
            {
                _db.AddParameter("@BankId", BankId);
                DbDataReader sdr = _db.ExecuteDataReader("GetBankById", CommandType.StoredProcedure);
                while (sdr.Read())
                {
                    Bank.BankName = sdr["BankName"].ToString();
                    Bank.Id = Convert.ToString(sdr["Id"]);
                    Bank.IsActive = Convert.ToInt16(sdr["isActive"]);


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
            return Bank;
        }

        /// <summary>
        ///  Active/InActive of Bank
        /// </summary>
        /// <param name="BankId"></param>
        /// <param name="IsActive"></param>
        public void DeleteBank(string BankId, Int16 IsActive)
        {
            _db = new DbProvider();
            try
            {
                _db.AddParameter("@BankId", BankId);
                _db.AddParameter("@IsActive", IsActive);
                _db.ExecuteNonQuery("DeleteBank", CommandType.StoredProcedure);
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

    }
}
