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
    public class StockItemDAL
    {
        DbProvider _db;

        /// <summary>
        /// Create Stock Item 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="AdminId"></param>
        public void Save(StockItemMasterDataModel model, string AdminId)
        {
            _db = new DbProvider();
            try
            {

                int month = DateTime.Now.Month;
                int year = DateTime.Now.Year;

                _db.AddParameter("@Id", Guid.NewGuid().ToString());
                _db.AddParameter("@StockItemName", model.StockItemName.Trim());
                _db.AddParameter("@AdminId", AdminId);
                _db.AddParameter("@CompanyId", model.CompanyId);
                _db.AddParameter("@month", month);
                _db.AddParameter("@Year", year);
                _db.AddParameter("@Rate", model.Rate);
                _db.AddParameter("@IsGas", model.IsGas);
                _db.AddParameter("@weight", model.weight);
                _db.AddParameter("@CreatedDate", model.RateDate);
                _db.AddParameter("@ToDate", model.ToDate);
                _db.ExecuteNonQuery("InsertStockItem", CommandType.StoredProcedure);

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
        /// Update stock Item
        /// </summary>
        /// <param name="model"></param>
        /// <param name="AdminId"></param>
        public void Update2(StockItemMasterDataModel model, string AdminId)
        {
            _db = new DbProvider();
            try
            {

                int bmonth = 13;
                int byear = 13;
                if (model.RateDate.HasValue)
                {
                    byear = model.RateDate.Value.Year;
                    bmonth = model.RateDate.Value.Month;
                }

                int month = DateTime.Now.Month;
                int year = DateTime.Now.Year;
                string date = DateTime.Today.ToString("MM/dd/yy");
                _db.AddParameter("@StockItemGasRateId", model.StockItemGasRateId);
                _db.AddParameter("@StockItemId", model.StockItemId);
                _db.AddParameter("@StockItemName", model.StockItemName.Trim());
                _db.AddParameter("@AdminId", AdminId);
                _db.AddParameter("@CompanyId", model.CompanyId);

                _db.AddParameter("@Rate", model.Rate);
                _db.AddParameter("@IsGas", model.IsGas);
                _db.AddParameter("@weight", model.weight);
               
               

                if (month == model.month && year == model.Year)
                {
                    if (month != bmonth && year <= byear)
                    {
                        _db.AddParameter("@month", bmonth);
                        _db.AddParameter("@Year", byear);
                        _db.AddParameter("@updateRate", 0);
                        _db.AddParameter("@CreatedDate", model.RateDate);
                        _db.AddParameter("@ToDate", model.ToDate);
                    }
                    else
                    {
                        _db.AddParameter("@month", month);
                        _db.AddParameter("@Year", year);
                        _db.AddParameter("@updateRate", 1);
                        _db.AddParameter("@CreatedDate", model.RateDate);
                        _db.AddParameter("@ToDate", model.ToDate);
                    }
                }
                else
                {
                    _db.AddParameter("@month", month);
                    _db.AddParameter("@Year", year);
                    _db.AddParameter("@updateRate", 0);
                    _db.AddParameter("@CreatedDate", model.RateDate);
                    _db.AddParameter("@ToDate", model.ToDate);
                }
                _db.ExecuteNonQuery("UpdateStockItem", CommandType.StoredProcedure);

                //if (date == model.CreatedDate)
                //{ _db.AddParameter("@updateRate", 1); }
                //else
                //{ _db.AddParameter("@updateRate", 0); }

            }
            catch (Exception ex)
            {
                throw;

            }
            finally
            {
                _db.Dispose();
            }
        }

        /// <summary>
        /// Update stock Item
        /// </summary>
        /// <param name="model"></param>
        /// <param name="AdminId"></param>
        public void Update(StockItemMasterDataModel model, string AdminId)
        {
            _db = new DbProvider();
            try
            {

                int bmonth = 13;
                int byear = 13;
                if (model.RateDate.HasValue)
                {
                    byear = model.RateDate.Value.Year;
                    bmonth = model.RateDate.Value.Month;
                }

                
                string date = DateTime.Today.ToString("MM/dd/yy");
                _db.AddParameter("@StockItemGasRateId", model.StockItemGasRateId);
                _db.AddParameter("@StockItemId", model.StockItemId);
                _db.AddParameter("@StockItemName", model.StockItemName.Trim());
                _db.AddParameter("@AdminId", AdminId);
                _db.AddParameter("@CompanyId", model.CompanyId);
                _db.AddParameter("@Rate", model.Rate);
                _db.AddParameter("@IsGas", model.IsGas);
                _db.AddParameter("@weight", model.weight);
                _db.AddParameter("@month", bmonth);
                _db.AddParameter("@Year", byear);
                _db.AddParameter("@updateRate", model.IsShow);
                _db.AddParameter("@CreatedDate", model.RateDate);
                _db.AddParameter("@ToDate", model.ToDate);
                _db.ExecuteNonQuery("UpdateStockItemnew", CommandType.StoredProcedure);

            }
            catch (Exception ex)
            {
                throw;

            }
            finally
            {
                _db.Dispose();
            }
        }


        /// <summary>
        /// Get List of stock Item
        /// </summary>
        /// <returns></returns>
        public List<StockItemMasterDataModel> GetAllStockItem(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey, string CityId, string AdminId)
        {
            List<StockItemMasterDataModel> Stock = new List<StockItemMasterDataModel>();
            try
            {
                _db = new DbProvider();
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
                int month = DateTime.Now.Month;
                int year = DateTime.Now.Year;
                int NoEdit = 0;
                DbDataReader sdr = _db.ExecuteDataReader("GetAllStockItem", CommandType.StoredProcedure);
                while (sdr.Read())
                {
                    if (month == Convert.ToInt16(sdr["month"]) && year == Convert.ToInt16(sdr["Year"]))
                    { NoEdit = 1; }
                    else
                    { NoEdit = 0; }


                    Stock.Add(new StockItemMasterDataModel
                    {
                        StockItemId = Convert.ToString(sdr["Id"]),
                        StockItemName = sdr["StockItemName"].ToString(),
                        CompanyId = Convert.ToString(sdr["CompanyId"]),
                        CompanyName = sdr["CompanyName"].ToString(),
                        Rate = Convert.ToDecimal(sdr["Rate"]),
                        StockItemGasRateId = Convert.ToString(sdr["StockItemGasRateId"]),
                        month = Convert.ToInt16(sdr["month"]),
                        Year = Convert.ToInt16(sdr["Year"]),
                        IsGas = Convert.ToBoolean(sdr["typeG"]),
                        NoEdit = NoEdit,
                        weight = Convert.ToDecimal(sdr["weight"]),
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
            return Stock;

        }



        /// <summary>
        /// Get List of stock Item
        /// </summary>
        /// <returns></returns>
        public List<StockItemGasRateDataModel> GetAllStockItemRate(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey, string CityId, string AdminId, string StockItemId)
        {
            List<StockItemGasRateDataModel> Stock = new List<StockItemGasRateDataModel>();
            try
            {
                _db = new DbProvider();
                _db.AddParameter("@PageNo", PageNo);
                if (PageSize != 0)
                    _db.AddParameter("@PageSize", PageSize);
                if (!string.IsNullOrWhiteSpace(q))
                    _db.AddParameter("@q", q);
                _db.AddParameter("@sortkey ", sortby);
                _db.AddParameter("@sortby", sortkey != "desc" ? 1 : 0);
                _db.AddParameter("@CityId ", CityId);
                _db.AddParameter("@AdminId ", AdminId);
                _db.AddParameter("@StockItemId ", StockItemId);

                int TotalRows = 0;
                DbDataReader sdr = _db.ExecuteDataReader("GetAllStockItemRateHistory", CommandType.StoredProcedure);
                while (sdr.Read())
                {
                    Stock.Add(new StockItemGasRateDataModel
                    {
                        Ratemonth = Convert.ToString(sdr["Ratemonth"]),
                        RateYear = sdr["RateYear"].ToString(),
                        createdate = Convert.ToDateTime(sdr["createdate"]).ToString("yyyy-MM-dd"),
                        ToCreatedDate = sdr["ToCreatedDate"] != DBNull.Value ? Convert.ToDateTime(sdr["ToCreatedDate"]).ToString("yyyy-MM-dd"):"",
                        Rate = Convert.ToDecimal(sdr["Rate"]),
                        TotalRows = TotalRows = sdr["TotalRows"] != DBNull.Value ? Convert.ToInt16(sdr["TotalRows"]) : 0,
                    });
                }

            }
            catch (Exception ex )
            {
                throw ex;

            }
            finally
            {
                _db.Dispose();
            }
            return Stock;

        }

        /// <summary>
        /// Get details of Stock Item by Id
        /// </summary>
        /// <param name="StockItemId"></param>
        /// <returns></returns>
        public StockItemMasterDataModel GetStockItemById(string StockItemId)
        {
            StockItemMasterDataModel Stock = new StockItemMasterDataModel();
            _db = new DbProvider();
            try
            {

                _db.AddParameter("@StockItemId", StockItemId);
                DbDataReader sdr = _db.ExecuteDataReader("GetStockItemIdById", CommandType.StoredProcedure);
                while (sdr.Read())
                {
                    Stock.StockItemName = sdr["StockItemName"].ToString();
                    Stock.StockItemId = Convert.ToString(sdr["Id"]);
                    Stock.CompanyId = Convert.ToString(sdr["CompanyId"]);
                    Stock.month = Convert.ToInt16(sdr["month"]);
                    Stock.Year = Convert.ToInt16(sdr["Year"]);
                    Stock.StockItemGasRateId = Convert.ToString(sdr["StockItemGasRateId"]);
                    Stock.Rate = Convert.ToDecimal(sdr["Rate"]);
                    Stock.IsGas = sdr["IsGas"] != DBNull.Value ? Convert.ToBoolean(sdr["IsGas"]) : false;
                    Stock.weight = Convert.ToDecimal(sdr["weight"]);
                    Stock.CreatedDate = sdr["CreatedDate"].ToString();
                    Stock.ToDate = Convert.ToDateTime(sdr["ToCreatedDate"]);
                    Stock.RateDate = Convert.ToDateTime(sdr["CreatedDate"]);
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
            return Stock;
        }

        /// <summary>
        /// Active & InActive Item
        /// </summary>
        /// <param name="StockItemId"></param>
        /// <param name="IsActive"></param>
        public void DeleteStockItem(String StockItemId, Int16 IsActive)
        {
            _db = new DbProvider();
            try
            {

                _db.AddParameter("@StockItemId", StockItemId);
                _db.AddParameter("@IsActive", IsActive);
                _db.ExecuteNonQuery("DeleteStockItem", CommandType.StoredProcedure);

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
        /// Get Item Company Wise
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        public List<StockItemMasterDataModel> GetStockItemByCompany(string CompanyId)
        {
            _db = new DbProvider();
            List<StockItemMasterDataModel> StockItem = new List<StockItemMasterDataModel>();

            try
            {

                _db.AddParameter("@CompanyId", CompanyId);
                DbDataReader sdr = _db.ExecuteDataReader("GetStockItemByCompany", CommandType.StoredProcedure);
                while (sdr.Read())
                {
                    StockItem.Add(new StockItemMasterDataModel
                    {
                        StockItemId = Convert.ToString(sdr["Id"]),
                        StockItemName = sdr["StockItemName"].ToString(),
                        Value = Convert.ToString(sdr["Id"]),
                        Text = sdr["StockItemName"].ToString(),
                        Rate = Convert.ToDecimal(sdr["Rate"]),
                        ReConnectionFee = Convert.ToDecimal(sdr["ReConnectionFee"]),
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
            return StockItem;
        }

        /// <summary>
        /// Get Rate of item company wise
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <param name="StockItemId"></param>
        /// <returns></returns>
        public List<StockItemMasterDataModel> Getstockrate(string CompanyId, string StockItemId)
        {
            List<StockItemMasterDataModel> StockItem = new List<StockItemMasterDataModel>();
            _db = new DbProvider();
            try
            {

                _db.AddParameter("@CompanyId", CompanyId);
                _db.AddParameter("@StockItemId", StockItemId);
                DbDataReader sdr = _db.ExecuteDataReader("GetStockRate", CommandType.StoredProcedure);

                while (sdr.Read())
                {
                    StockItem.Add(new StockItemMasterDataModel
                    {
                        StockItemId = Convert.ToString(sdr["StockItemId"]),
                        month = Convert.ToInt32(sdr["Ratemonth"]),
                        Year = Convert.ToInt32(sdr["RateYear"]),
                        Rate = Convert.ToDecimal(sdr["Rate"]),
                        weight = sdr["weight"] != DBNull.Value ? Convert.ToDecimal(sdr["weight"]) : 0

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
            return StockItem;
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
        /// Save StockItems.
        /// </summary>
        /// <param name="model"></param>
        public void SaveImport(DataTable model)
        {
            _db = new DbProvider();
            try
            {
                _db.AddParameter("@StockItemGasRateRef", model);
                _db.ExecuteNonQuery("ImportStockItemGasRate", CommandType.StoredProcedure);
            }
            catch (Exception ex)
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
