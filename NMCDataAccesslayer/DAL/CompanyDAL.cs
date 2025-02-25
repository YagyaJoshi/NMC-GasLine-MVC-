using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using NMCDataAccesslayer.DataModel;
using System.Data.Common;
using DbProviderFactorie;

namespace NMCDataAccesslayer.DAL
{
    public class CompanyDAL
    {
        DbProvider _db;
        string result;

        /// <summary>
        /// Get Company List CityWise
        /// </summary>
        /// <param name="CityId"></param>
        /// <returns></returns>
        public List<BillTypeData> GetBillType()
        {
            List<BillTypeData> BillTypeData = new List<BillTypeData>();
            try
            {
                _db = new DbProvider();

                DbDataReader sdr = _db.ExecuteDataReader("GetBillType", CommandType.StoredProcedure);
                while (sdr.Read())
                {
                    BillTypeData.Add(new BillTypeData
                    {
                        Id = Convert.ToString(sdr["Id"]),
                        Type = sdr["type"].ToString()
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
            return BillTypeData;
        }



        /// <summary>
        /// Get Company List CityWise
        /// </summary>
        /// <param name="CityId"></param>
        /// <returns></returns>
        public List<CompanyDataModel> GetCompany(string adminId, string CityId)
        {
            List<CompanyDataModel> Company = new List<CompanyDataModel>();

            try
            {
                _db = new DbProvider();
              
                _db.AddParameter("@CityId", Convert.ToString(CityId));
                _db.AddParameter("@adminId", Convert.ToString(adminId));
                DbDataReader sdr = _db.ExecuteDataReader("GetCompany", CommandType.StoredProcedure);
                while (sdr.Read())
                {
                    Company.Add(new CompanyDataModel
                    {
                        Id = Convert.ToString(sdr["Id"]),
                        CompanyName = sdr["CompanyName"].ToString(),
                        Value = Convert.ToString(sdr["Id"]),
                        Text = sdr["CompanyName"].ToString()
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
            return Company;
        }

        /// <summary>
        /// Create New Company
        /// </summary>
        /// <param name="model">Company Data Model</param>
        public string SaveCompany(CompanyDataModel model)
        {
            _db = new DbProvider();
            try
            {
                _db.AddParameter("@Id", Guid.NewGuid().ToString());
                _db.AddParameter("@CompanyName", model.CompanyName != null ? model.CompanyName.Trim() : model.CompanyName);
                _db.AddParameter("@Address", model.Address != null ? model.Address.Trim() : model.Address);
                _db.AddParameter("@CityId", model.CityId);
                _db.AddParameter("@LatePaymentFee", model.LatePaymentFee);
                _db.AddParameter("@MinimumGasCharges", model.MinimumGasCharges);
                _db.AddParameter("@ReConnectionFee", model.ReConnectionFee);
                _db.AddParameter("@InstallationCharges", model.InstallationCharges);
                _db.AddParameter("@ServiceCharges", model.ServiceCharges);
                _db.AddParameter("@CreatedBy", model.AdminId);
                _db.AddParameter("@CreatedAt", DateTime.Now);
                _db.AddParameter("@DelayMinAmount", model.DelayMinAmount);
                _db.AddParameter("@DelayDaysLimit", model.DelayDaysLimit != null ? model.DelayDaysLimit : 0);
                _db.AddParameter("@DelayDays", model.DelayDays);
                _db.AddParameter("@DueDays", model.DueDays);
                _db.AddParameter("@CallCharges", model.CallCharges);
                _db.AddParameter("@ConnectionFee", model.ConnectionFee);
                _db.AddParameter("@DetailsChangesFee", model.DetailsChangesFee);

                //_db.ExecuteNonQuery("InsertCompany", CommandType.StoredProcedure);
                result = _db.ExecuteScalar("InsertCompany", CommandType.StoredProcedure).ToString();
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
        /// Update Company
        /// </summary>
        /// <param name="model">Company Data Model</param>
        public string UpdateCompany(CompanyDataModel model)
        {
            _db = new DbProvider();
            try
            {
                _db.AddParameter("@CompanyName", model.CompanyName != null ? model.CompanyName.Trim() : model.CompanyName);
                _db.AddParameter("@Address", model.Address != null ? model.Address.Trim() : model.Address);
                _db.AddParameter("@CityId", model.CityId);
                _db.AddParameter("@LatePaymentFee", model.LatePaymentFee);
                _db.AddParameter("@MinimumGasCharges", model.MinimumGasCharges);
                _db.AddParameter("@ReConnectionFee", model.ReConnectionFee);
                _db.AddParameter("@InstallationCharges", model.InstallationCharges);
                _db.AddParameter("@ServiceCharges", model.ServiceCharges);
                _db.AddParameter("@CompanyId", model.Id);
                _db.AddParameter("@UpdateAt", DateTime.Now);
                _db.AddParameter("@DelayMinAmount", model.DelayMinAmount);
                _db.AddParameter("@DelayDaysLimit", model.DelayDaysLimit != null ? model.DelayDaysLimit : 0);
                _db.AddParameter("@DelayDays", model.DelayDays);
                _db.AddParameter("@DueDays", model.DueDays);
                _db.AddParameter("@CallCharges", model.CallCharges);
                _db.AddParameter("@ConnectionFee", model.ConnectionFee);
                _db.AddParameter("@DetailsChangesFee", model.DetailsChangesFee);

                //_db.ExecuteNonQuery("UpdateCompany", CommandType.StoredProcedure);
                result = _db.ExecuteScalar("UpdateCompany", CommandType.StoredProcedure).ToString();
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
        /// Get Company List
        /// </summary>
        /// <returns></returns>
        public List<CompanyDataModel> GetCompany(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey, string CityId, string AdminId)
        {
            _db = new DbProvider();
            List<CompanyDataModel> Company = new List<CompanyDataModel>();
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
                DbDataReader sdr = _db.ExecuteDataReader("GetAllCompany", CommandType.StoredProcedure);
                while (sdr.Read())
                {
                    Company.Add(new CompanyDataModel
                    {
                        CompanyName = sdr["CompanyName"].ToString(),
                        Address = sdr["Address"].ToString(),
                        CityId = Convert.ToString(sdr["CityId"]),
                        CityName = sdr["CityName"].ToString(),
                        Id = Convert.ToString(sdr["Id"]),
                        IsActive = Convert.ToInt16(sdr["IsActive"]),
                        LatePaymentFee = Convert.ToDecimal(sdr["LatePaymentFee"]),
                        MinimumGasCharges = Convert.ToDecimal(sdr["MinimumGasCharges"]),
                        ReConnectionFee = Convert.ToDecimal(sdr["ReConnectionFee"]),
                        CallCharges = Convert.ToDecimal(sdr["CallCharges"]),
                        ConnectionFee = Convert.ToDecimal(sdr["ConnectionFee"]),
                        DetailsChangesFee = Convert.ToDecimal(sdr["DetailsChangesFee"]),
                        InstallationCharges = Convert.ToDecimal(sdr["InstallationCharges"]),
                        DelayDaysLimit = Convert.ToInt16(sdr["DelayDaysLimit"]),
                        DueDays = Convert.ToInt16(sdr["DueDays"]),
                        DelayDays = Convert.ToInt16(sdr["DelayDays"]),
                        DelayMinAmount = Convert.ToDecimal(sdr["DelayMinAmount"]),
                        TotalRows = TotalRows = sdr["TotalRows"] != DBNull.Value ? Convert.ToInt16(sdr["TotalRows"]) : 0,
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
            return Company;
        }

        
        /// <summary>
        /// Get Company Details by id
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        public CompanyDataModel GetCompanyById(string CompanyId)
        {
            _db = new DbProvider();
            CompanyDataModel Company = new CompanyDataModel();
            try
            {

                _db.AddParameter("@CompanyId", CompanyId);
                DbDataReader sdr = _db.ExecuteDataReader("GetCompanyById", CommandType.StoredProcedure);
                while (sdr.Read())
                {
                    Company.CompanyName = sdr["CompanyName"].ToString();
                    Company.Address = sdr["Address"].ToString();
                    Company.CountryId = Convert.ToString(sdr["CountryId"]);
                    Company.Id = Convert.ToString(sdr["Id"]);
                    Company.StateId = Convert.ToString(sdr["StateId"]);
                    Company.CityId = Convert.ToString(sdr["CityId"]);
                    Company.LatePaymentFee = Convert.ToDecimal(sdr["LatePaymentFee"]);
                    Company.MinimumGasCharges = Convert.ToDecimal(sdr["MinimumGasCharges"]);
                    Company.ReConnectionFee = Convert.ToDecimal(sdr["ReConnectionFee"]);
                    Company.CallCharges = Convert.ToDecimal(sdr["CallCharges"]);
                    Company.ConnectionFee = Convert.ToDecimal(sdr["ConnectionFee"]);
                    Company.DetailsChangesFee = Convert.ToDecimal(sdr["DetailsChangesFee"]);
                    Company.InstallationCharges = Convert.ToDecimal(sdr["InstallationCharges"]);
                    Company.DelayDaysLimit = Convert.ToInt16(sdr["DelayDaysLimit"]);
                    Company.DueDays = Convert.ToInt16(sdr["DueDays"]);
                    Company.DelayDays = Convert.ToInt16(sdr["DelayDays"]);
                    Company.DelayMinAmount = Convert.ToDecimal(sdr["DelayMinAmount"]);
                    Company.ServiceCharges = Convert.ToDecimal(sdr["ServiceCharges"]);
                    

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
            return Company;
        }
      
        /// <summary>
        /// Active/InActive Company 
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <param name="IsActive"></param>
        public void DeleteCompany(string CompanyId, Int16 IsActive)
        {
            _db = new DbProvider();
            try
            {
                _db.AddParameter("@CompanyId", CompanyId);
                _db.AddParameter("@IsActive", IsActive);
                _db.ExecuteNonQuery("DeleteCompany", CommandType.StoredProcedure);

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

        public string InsertCompaniesPaymentSetup(CompaniesPaymentModel model)
        {
            _db = new DbProvider();
            try
            {
                _db.AddParameter("@Id", Guid.NewGuid().ToString());
                _db.AddParameter("@CompanyId", model.CompanyId);
                _db.AddParameter("@MerchantId", model.MerchantId);
                _db.AddParameter("@SecurityId", model.SecurityId);
                _db.AddParameter("@ChecksumKey", model.ChecksumKey);
                _db.AddParameter("@CreatedAt", DateTime.Now);
                result = _db.ExecuteScalar("InsertCompanyWisePaymentSetup", CommandType.StoredProcedure).ToString();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                //throw;

            }
            finally
            {
                _db.Dispose();
            }
            return result;
        }


        public string UpdateCompaniesPaymentSetup(CompaniesPaymentModel model)
        {
            _db = new DbProvider();
            try
            {
                    _db.AddParameter("@Id", model.Id);
                _db.AddParameter("@CompanyId", model.CompanyId);
                _db.AddParameter("@MerchantId", model.MerchantId);
                _db.AddParameter("@SecurityId", model.SecurityId);
                _db.AddParameter("@ChecksumKey", model.ChecksumKey);
                _db.AddParameter("@UpdateAt", DateTime.Now);
                result = _db.ExecuteScalar("UpdateCompaniesPaymentSetup", CommandType.StoredProcedure).ToString();

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

        public List<CompaniesPaymentModel> GetAllCompaniesPaymentSetup(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey, string CityId, string AdminId)
        {
            _db = new DbProvider();
            List<CompaniesPaymentModel> Company = new List<CompaniesPaymentModel>();
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
                DbDataReader sdr = _db.ExecuteDataReader("GetAllCompaniesPaymentSetup", CommandType.StoredProcedure);
                while (sdr.Read())
                {
                    Company.Add(new CompaniesPaymentModel
                    {
                        Id = Convert.ToString(sdr["Id"]),
                        CityName = sdr["CityName"].ToString(),
                        MerchantId = Convert.ToString(sdr["MerchantId"]),
                        SecurityId = Convert.ToString(sdr["SecurityId"]),
                        ChecksumKey = Convert.ToString(sdr["ChecksumKey"]),
                        TotalRows = TotalRows = sdr["TotalRows"] != DBNull.Value ? Convert.ToInt16(sdr["TotalRows"]) : 0,
                    });
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                //throw;
            }
            finally
            {
                _db.Dispose();
            }
            return Company;
        }

        public CompaniesPaymentModel GetCompaniesPaymentById(string Id)
        {
            _db = new DbProvider();
            CompaniesPaymentModel Company = new CompaniesPaymentModel();
            try
            {

                _db.AddParameter("@Id", Id);
                DbDataReader sdr = _db.ExecuteDataReader("GetCompaniesPaymentById", CommandType.StoredProcedure);
                while (sdr.Read())
                {

                    Company.Id = Convert.ToString(sdr["Id"]);
                    Company.CompanyId = Convert.ToString(sdr["CompanyId"]);
                    Company.CityId = Convert.ToString(sdr["CityId"]);
                    Company.MerchantId = Convert.ToString(sdr["MerchantId"]);
                    Company.SecurityId = Convert.ToString(sdr["SecurityId"]);
                    Company.ChecksumKey = Convert.ToString(sdr["ChecksumKey"]);
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
            return Company;
        }
    }
}
