using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NMCDataAccesslayer.DataModel;
using System.Threading.Tasks;
using DbProviderFactorie;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data;
using System.Reflection;
using NMCDataAccesslayer.Helper;

namespace NMCDataAccesslayer.DAL
{
    public class CustomerDAL
    {
        DbProvider _db;
        object Result = "";
        /// <summary>
        /// Save Customer by excel  imported from website . 
        /// </summary>
        /// <param name="obj">List of customer</param>
        /// <param name="CompanyId"></param>
        /// <param name="AdminId"></param>
        public DataTable Save(List<ImportCustomerData> obj, string CompanyId, string AdminId, string GodownId)
        {
            DataTable dt = null;
            _db = new DbProvider();
            try
            {

                _db.AddParameter("@CustomerRef", ToDataTable(obj));
                _db.AddParameter("@CompanyId", CompanyId);
                _db.AddParameter("@GodownId", GodownId);
                _db.AddParameter("@isupdate", 0);
                _db.AddParameter("@LastImportedBy", AdminId);
                _db.AddParameter("@LastImportdate", DateTime.Now);

                DataSet ds = _db.ExecuteDataSet("ImportCustomer", CommandType.StoredProcedure);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    int a = ds.Tables[0].Rows[0]["EmailId"].ToString().Length;
                    _db.AddParameter("@CustomerRef", ds.Tables[0]);
                    _db.AddParameter("@CompanyId", CompanyId);
                    _db.AddParameter("@isupdate", 1);
                    _db.AddParameter("@LastImportedBy", AdminId);
                    _db.AddParameter("@LastImportdate", DateTime.Now);
                    // _db.ExecuteNonQuery("ImportCustomer", CommandType.StoredProcedure);
                    ds = _db.ExecuteDataSet("ImportCustomer", CommandType.StoredProcedure);

                }
                return dt = ds.Tables[0];
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
        /// Get Customer by area id using at mobile app for employee
        /// </summary>
        /// <param name="AreaId"></param>
        /// <returns></returns>
        public async Task<List<CustomerDataModel>> GetCustomerAreawise(string AreaId)
        {
            List<CustomerDataModel> listCustomer = new List<CustomerDataModel>();
            _db = new DbProvider();
            try

            {
                _db.AddParameter("@AreaId", AreaId);
                DbDataReader dr = await _db.ExecuteDataReaderAsync("getCutomerListAreawise", CommandType.StoredProcedure);
                while (dr.Read())
                {
                    CustomerDataModel model = new CustomerDataModel();
                    model.CustomerID = Convert.ToString(dr["CustomerId"]);
                    model.Name = Convert.ToString(dr["Name"]);
                    model.Address = Convert.ToString(dr["Address"]);
                    model.Email = Convert.ToString(dr["Email"]);
                    model.Phone = Convert.ToString(dr["Phone"]);
                    model.MeterNo = Convert.ToString(dr["MeterNo"]);
                    model.FlatNo = Convert.ToString(dr["FlatNo"]);
                    model.ClosingBalance = Convert.ToDecimal(dr["ClosingBalance"]);
                    if (dr["PreviousBillDate"] != DBNull.Value)
                    { model.PreviousBillDate = Convert.ToDateTime(dr["PreviousBillDate"]); }
                    model.PreviousRedg = Convert.ToInt32(dr["PreviousRedg"]);
                    model.GodownId = Convert.ToString(dr["GodownId"]);
                    model.TallyName = Convert.ToString(dr["TallyName"]);
                    listCustomer.Add(model);
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
            return listCustomer;
        }
        /// <summary>
        /// Get Bill of Customer Wise and Export
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        public List<CustomerBillInformationData> GetBill(string CompanyId, string GodownId, string AreaId, string BillTypeId)
        {
            _db = new DbProvider();
            List<CustomerBillInformationData> Bill = new List<CustomerBillInformationData>();
            try
            {
                _db.AddParameter("@CompanyId", CompanyId);
                _db.AddParameter("@GodownId", GodownId);
                _db.AddParameter("@AreaId", AreaId);
                _db.AddParameter("@BillTypeId", BillTypeId);
                DbDataReader sdr = _db.ExecuteDataReader("GetAllBill", CommandType.StoredProcedure);

                while (sdr.Read())
                {
                    Bill.Add(new CustomerBillInformationData
                    {
                        BillNo = Convert.ToString(sdr["BillNo"]),
                        PartyName = Convert.ToString(sdr["PartyName"]),
                        ClosingBalance = Convert.ToDecimal(sdr["ClosingBalance"]),
                        BillDate = sdr["BillDate"] != DBNull.Value ? Convert.ToDateTime(sdr["BillDate"]).ToString("d-MMM-yyyy") : "",
                        ClosingRedg = Convert.ToDecimal(sdr["ClosingRedg"]),
                        PreviousBillDate = sdr["PreviousBillDate"] != DBNull.Value ? Convert.ToDateTime(sdr["PreviousBillDate"]).ToString("d-MMM-yyyy") : "",
                        PreviousRedg = Convert.ToDecimal(sdr["PreviousRedg"]),
                        DueDate = sdr["DueDate"] != DBNull.Value ? Convert.ToDateTime(sdr["DueDate"]).ToString("dd-MM-yyyy") : "",
                        ConsumeUnit = Convert.ToDecimal(sdr["ConsumeUnit"]),
                        InputRate = Convert.ToDecimal(sdr["InputRate"]),
                        StockItemName = Convert.ToString(sdr["StockItemName"]),
                        ConsumptioninKG = Convert.ToDecimal(sdr["CurrentKGS"]),
                        Rate = Convert.ToDecimal(sdr["Rate"]),
                        Month = Convert.ToString(Convert.ToInt32(sdr["BillMonth"])),
                        ServiceAmt = Convert.ToDecimal(sdr["ServiceAmt"]),
                        Arrears = Convert.ToDecimal(sdr["Arrears"]),
                        MinAmt = Convert.ToDecimal(sdr["MinAmt"]),
                        LateFee = Convert.ToDecimal(sdr["LateFee"]),
                        GodownName = Convert.ToString(sdr["GodownName"]),
                        ReconnectionAmt = Convert.ToDecimal(sdr["ReconnectionAmt"]),
                        InvoiceValue = Convert.ToDecimal(sdr["TotalAmt"]),
                        GASCOMSUMPTIONLed = Convert.ToDecimal(sdr["Led"]),
                        diposit = Convert.ToDecimal(sdr["PreviousDiposite"]),
                        PreviousBalance = Convert.ToDecimal(sdr["PreviousBalance"]),
                        Round = Convert.ToDecimal(sdr["Round"]),
                        Diff = Convert.ToDecimal(sdr["Diff"]),
                        SGST = Convert.ToDecimal(sdr["SGST"]),
                        CGST = Convert.ToDecimal(sdr["CGST"]),
                        PLateFee = Convert.ToDecimal(sdr["PreviousLateFree"]),
                        AliasName = Convert.ToString(sdr["AliasName"]),
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
            return Bill;
        }

        /// <summary>
        /// Get all bill list
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <param name="GodownId"></param>
        /// <param name="AreaId"></param>
        /// <param name="BillTypeId"></param>
        /// <returns></returns>
        public List<CustomerBillInformationData> GetAllBill(string CompanyId, string GodownId, string AreaId, string BillTypeId)
        {
            _db = new DbProvider();
            List<CustomerBillInformationData> Bill = new List<CustomerBillInformationData>();
            try
            {
                _db.AddParameter("@CompanyId", CompanyId);
                _db.AddParameter("@GodownId", GodownId);
                _db.AddParameter("@AreaId", AreaId);
                _db.AddParameter("@BillTypeId", BillTypeId);
                DbDataReader sdr = _db.ExecuteDataReader("GetAllBillList", CommandType.StoredProcedure);

                while (sdr.Read())
                {
                    Bill.Add(new CustomerBillInformationData

                    {
                        BillNo = Convert.ToString(sdr["BillNo"]),
                        PartyName = Convert.ToString(sdr["PartyName"]),
                        ClosingBalance = Convert.ToDecimal(sdr["ClosingBalance"]),
                        BillDate = sdr["BillDate"] != DBNull.Value ? Convert.ToDateTime(sdr["BillDate"]).ToString("d-MMM-yyyy") : "",
                        ClosingRedg = Convert.ToDecimal(sdr["ClosingRedg"]),
                        PreviousBillDate = sdr["PreviousBillDate"] != DBNull.Value ? Convert.ToDateTime(sdr["PreviousBillDate"]).ToString("d-MMM-yyyy") : "",
                        PreviousRedg = Convert.ToDecimal(sdr["PreviousRedg"]),
                        DueDate = sdr["DueDate"] != DBNull.Value ? Convert.ToDateTime(sdr["DueDate"]).ToString("dd-MM-yyyy") : "",
                        ConsumeUnit = Convert.ToDecimal(sdr["ConsumeUnit"]),
                        InputRate = Convert.ToDecimal(sdr["InputRate"]),
                        StockItemName = Convert.ToString(sdr["StockItemName"]),
                        ConsumptioninKG = Convert.ToDecimal(sdr["CurrentKGS"]),
                        Rate = Convert.ToDecimal(sdr["Rate"]),
                        Month = Convert.ToString(sdr["BillMonth"]),
                        ServiceAmt = Convert.ToDecimal(sdr["ServiceAmt"]),
                        Arrears = Convert.ToDecimal(sdr["Arrears"]),
                        MinAmt = Convert.ToDecimal(sdr["MinAmt"]),
                        LateFee = Convert.ToDecimal(sdr["LateFee"]),
                        GodownName = Convert.ToString(sdr["GodownName"]),
                        ReconnectionAmt = Convert.ToDecimal(sdr["ReconnectionAmt"]),
                        InvoiceValue = Convert.ToDecimal(sdr["TotalAmt"]),
                        GASCOMSUMPTIONLed = Convert.ToDecimal(sdr["Led"]),
                        diposit = Convert.ToDecimal(sdr["PreviousDiposite"]),
                        PreviousBalance = Convert.ToDecimal(sdr["PreviousBalance"]),
                        SGST = Convert.ToDecimal(sdr["SGST"]),
                        CGST = Convert.ToDecimal(sdr["CGST"]),
                        ExportStatus = Convert.ToInt16(sdr["ExportStatus"]) != 0 ? "YES" : "NO",
                        isPaid = Convert.ToInt16(sdr["isPaid"]) != 0 ? "YES" : "NO",
                        PLateFee = Convert.ToDecimal(sdr["PreviousLateFree"]),

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
            return Bill;
        }



        /// <summary>
        /// Get List of Customer For Export company Wise
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        public List<ExportCustomerData> GetCustomerForExport(string CompanyId)
        {
            _db = new DbProvider();
            List<ExportCustomerData> Bill = new List<ExportCustomerData>();
            try
            {
                _db.AddParameter("@CompanyId", CompanyId);
                //_db.AddParameter("@BillTypeId", BillTypeId);
                _db.AddParameter("@ExportDate", DateTime.Now);

                DbDataReader sdr = _db.ExecuteDataReader("GetCustomerForExport", CommandType.StoredProcedure);

                while (sdr.Read())
                {
                    Bill.Add(new ExportCustomerData
                    {
                        Id = Convert.ToString(sdr["customerid"]),
                        Name = Convert.ToString(sdr["Name"]),
                        ALIAS = Convert.ToString(sdr["ALIAS"]),
                        BILLWISEDETAILS = Convert.ToString(sdr["BILLWISEDETAILS"]),
                        DUEDATE = Convert.ToString(sdr["DUEDATE"]),
                        Address = Convert.ToString(sdr["Address"]),
                        COUNTRY = Convert.ToString(sdr["COUNTRY"]),
                        State = Convert.ToString(sdr["State"]),
                        PINCODE = Convert.ToString(sdr["PINCODE"]),
                        CONTACTPERSON = sdr["CONTACTPERSON"].ToString(),
                        Phone = Convert.ToString(sdr["Phone"]),
                        MOBILE = Convert.ToString(sdr["MOBILE"]),
                        EMAIL = Convert.ToString(sdr["EMAIL"]),
                        ClosingBalance = Convert.ToDecimal(sdr["ClosingBalance"]),
                        BillType = Convert.ToString(sdr["BillType"]),
                        GodownMastername = Convert.ToString(sdr["GodownMastername"]),
                        ReceiptNo = Convert.ToString(sdr["ReceiptNo"]),
                        BankName = Convert.ToString(sdr["BankName"]),
                        AccountNo = Convert.ToString(sdr["AccountNo"]),
                        ChequeNo = Convert.ToString(sdr["ChequeNo"]),
                        PaymentType = Convert.ToString(sdr["PaymentType"]),
                        TallyName = Convert.ToString(sdr["TallyName"]),
                        InitialMeterReading = Convert.ToDecimal(sdr["ClosingRedg"]),
                        DepositeAccountNo = Convert.ToString(sdr["DepositeAccountNo"]),
                        DepositeBankName = Convert.ToString(sdr["DepositeBankName"]),
                        Voucherdate = Convert.ToString(sdr["Voucherdate"]),
                        CustomerNumber = Convert.ToString(sdr["CustomerNumber"]),
                        MailEMAIL = Convert.ToString(sdr["MailEMAIL"]),
                        EmailSend = Convert.ToString(sdr["EmailSend"])
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
            return Bill;
        }
        /// <summary>
        /// Create customer by admin 
        /// </summary>
        /// <param name="ObjcustomerData"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string SaveCustomerByAdmin(CustomerDataModel ObjcustomerData, string userId)
        {

            _db = new DbProvider();
            try
            {
                ObjcustomerData.TallyName = ObjcustomerData.Name + "," + ObjcustomerData.FlatNo + "," + ObjcustomerData.GodownName;
                string customerId = Guid.NewGuid().ToString();
                _db.AddParameter("@Id", customerId);
                _db.AddParameter("@AreaId", ObjcustomerData.AreaId);
                _db.AddParameter("@Name", ObjcustomerData.TallyName.Trim());
                _db.AddParameter("@Email", ObjcustomerData.Email);
                _db.AddParameter("@CompanyId", ObjcustomerData.CompanyId);
                _db.AddParameter("@GodownId", ObjcustomerData.GodownId);
                _db.AddParameter("@Phone", ObjcustomerData.Phone);
                _db.AddParameter("@NewMeterReading", ObjcustomerData.NewMeterReading);
                _db.AddParameter("@DateofInstallation", ObjcustomerData.DateofInstallation);
                _db.AddParameter("@TallyName", ObjcustomerData.TallyName.Trim().Replace(" ", ""));
                _db.AddParameter("@FlatNo", ObjcustomerData.FlatNo);
                _db.AddParameter("@InstallationAmount", ObjcustomerData.InstallationAmount);
                _db.AddParameter("@MeterNo", ObjcustomerData.MeterNo);
                _db.AddParameter("@OwnerName", ObjcustomerData.OwnerName);
                _db.AddParameter("@OwnerEmail", ObjcustomerData.OwnerEmail);
                _db.AddParameter("@OwnerPhone", ObjcustomerData.OwnerPhone);
                _db.AddParameter("@Address", ObjcustomerData.Address);
                _db.AddParameter("@OwnerAddress", ObjcustomerData.OwnerAddress);
                _db.AddParameter("@CustomerType", ObjcustomerData.CustomerType);
                _db.AddParameter("@ClosingBalance", ObjcustomerData.Amount);
                _db.AddParameter("@CreatedAt", DateTime.Now);
                _db.AddParameter("@UserId", userId);
                Result = _db.ExecuteScalar("sp_InsertCustomer", CommandType.StoredProcedure);

                if (Convert.ToString(Result) == "1")
                {
                    DbProvider _db1 = new DbProvider();
                    _db1.AddParameter("@Id", Guid.NewGuid().ToString());
                    _db1.AddParameter("@CustomerId", customerId);
                    _db1.AddParameter("@StockItemId", ObjcustomerData.StockItemId);
                    _db1.AddParameter("@GodownId", ObjcustomerData.GodownId);
                    _db1.AddParameter("@UserId", userId);
                    _db1.AddParameter("@BillNo", ObjcustomerData.BillNo);
                    _db1.AddParameter("@BillDate", DateTime.Now);
                    _db1.AddParameter("@ClosingBalance", ObjcustomerData.Amount);
                    _db1.AddParameter("@ClosingRedg", ObjcustomerData.NewMeterReading);
                    _db1.AddParameter("@Month", DateTime.Now.Month);
                    _db1.AddParameter("@isPaid", ObjcustomerData.isPaid);
                    _db1.AddParameter("@ReconnectionAmt", ObjcustomerData.ReconnectionAmt);
                    _db1.AddParameter("@SGST", ObjcustomerData.SGST);
                    _db1.AddParameter("@CGST", ObjcustomerData.CGST);
                    _db1.AddParameter("@TotalAmt", ObjcustomerData.Amount);
                    _db1.AddParameter("@PaymentTypeId", ObjcustomerData.PaymentTypeId);
                    _db1.AddParameter("@AccountNo", ObjcustomerData.AccountNo);
                    _db1.AddParameter("@ChequeNo", ObjcustomerData.ChequeNo);
                    _db1.AddParameter("@MinAmt", ObjcustomerData.MinAmt);
                    _db1.AddParameter("@BankName", ObjcustomerData.BankName);
                    _db1.AddParameter("@PreviousBalance", DBNull.Value);
                    _db1.AddParameter("@PreviousRedg", DBNull.Value);
                    //1- deposit 2-GasConsume
                    _db1.AddParameter("@BillType", "BFBC4C62-B98B-4C18-96E7-2C8327886BCB");
                    _db1.AddParameter("@ServiceAmt", DBNull.Value);
                    _db1.AddParameter("@ConsumeUnit", DBNull.Value);
                    _db1.AddParameter("@Rate", DBNull.Value);
                    _db1.AddParameter("@CurrentScm", DBNull.Value);
                    _db1.AddParameter("@CurrentKGS", DBNull.Value);
                    _db1.AddParameter("@PaymentDue", DBNull.Value);
                    _db1.AddParameter("@BillPeriod", DBNull.Value);
                    _db1.AddParameter("@CreatedAt", DateTime.Now);
                    Result = _db1.ExecuteScalar("sp_InsertCustomerBillInformation", CommandType.StoredProcedure);
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
            return Convert.ToString(Result);

        }
        /// <summary>
        /// 
        /// New Bill against customer 
        /// </summary>
        /// <param name="ObjcustomerData"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string SaveBillCustomer(CustomerDataModel ObjcustomerData, string userId)
        {

            DbProvider _db1 = new DbProvider();
            try
            {
                string PaymentType = ObjcustomerData.ClosingBalance - ObjcustomerData.Amount != 0 ? "Partial" : "Full";
                _db1.AddParameter("@Id", Guid.NewGuid().ToString());
                _db1.AddParameter("@CustomerId", ObjcustomerData.CustomerID);
                _db1.AddParameter("@StockItemId", ObjcustomerData.StockItemId);
                _db1.AddParameter("@GodownId", ObjcustomerData.GodownId);
                _db1.AddParameter("@UserId", userId);
                _db1.AddParameter("@BillNo", ObjcustomerData.BillNo);
                _db1.AddParameter("@BillDate", DateTime.Now);
                _db1.AddParameter("@ClosingBalance", ObjcustomerData.Amount);
                _db1.AddParameter("@ClosingRedg", ObjcustomerData.ClosingRedg);
                _db1.AddParameter("@Month", DateTime.Now.Month);
                _db1.AddParameter("@isPaid", ObjcustomerData.isPaid);
                _db1.AddParameter("@ReconnectionAmt", 0);
                _db1.AddParameter("@SGST", ObjcustomerData.SGST);
                _db1.AddParameter("@CGST", ObjcustomerData.CGST);
                _db1.AddParameter("@TotalAmt", ObjcustomerData.ClosingBalance);
                _db1.AddParameter("@PaymentTypeId", ObjcustomerData.PaymentTypeId);
                _db1.AddParameter("@AccountNo", ObjcustomerData.AccountNo);
                _db1.AddParameter("@ChequeNo", ObjcustomerData.ChequeNo);
                _db1.AddParameter("@MinAmt", ObjcustomerData.MinAmt);
                _db1.AddParameter("@BankName", ObjcustomerData.BankName);
                _db1.AddParameter("@PreviousBalance", DBNull.Value);
                _db1.AddParameter("@PreviousRedg", DBNull.Value);
                //1- deposit 2-GasConsume
                _db1.AddParameter("@BillType", "F2653C96-46D8-4609-A5ED-8568C129BAA3");
                _db1.AddParameter("@ServiceAmt", ObjcustomerData.ServiceAmt);
                _db1.AddParameter("@ConsumeUnit", ObjcustomerData.ConsumeUnit);
                _db1.AddParameter("@Rate", ObjcustomerData.Rate);
                _db1.AddParameter("@CurrentScm", ObjcustomerData.CurrentScm);
                _db1.AddParameter("@CurrentKGS", ObjcustomerData.CurrentKGS);
                _db1.AddParameter("@PaymentDue", ObjcustomerData.ClosingBalance - ObjcustomerData.Amount);
                _db1.AddParameter("@BillPeriod", ObjcustomerData.BillMonth);
                _db1.AddParameter("@PaymentType", PaymentType);
                _db1.AddParameter("@CreatedAt", DateTime.Now);
                Result = _db1.ExecuteScalar("sp_InsertCustomerBillInformation", CommandType.StoredProcedure);


            }
            catch (Exception)
            {
                throw;

            }
            finally
            {
                _db.Dispose();
            }
            return Convert.ToString(Result);
        }
        /// <summary>
        /// Update Customer 
        /// </summary>
        /// <param name="ObjcustomerData"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string UpdateCustomerByAdmin(CustomerDataModel ObjcustomerData, string userId)
        {
            _db = new DbProvider();
            try
            {
                ObjcustomerData.TallyName = ObjcustomerData.Name + "," + ObjcustomerData.FlatNo + "," + ObjcustomerData.GodownName;
                _db.AddParameter("@Id", ObjcustomerData.CustomerID);
                _db.AddParameter("@AreaId", ObjcustomerData.AreaId);
                _db.AddParameter("@Name", ObjcustomerData.TallyName);
                _db.AddParameter("@Email", ObjcustomerData.Email);
                _db.AddParameter("@CompanyId", ObjcustomerData.CompanyId);
                _db.AddParameter("@GodownId", ObjcustomerData.GodownId);
                _db.AddParameter("@Phone", ObjcustomerData.Phone);
                _db.AddParameter("@NewMeterReading", ObjcustomerData.NewMeterReading);
                _db.AddParameter("@DateofInstallation", ObjcustomerData.DateofInstallation);
                _db.AddParameter("@TallyName", ObjcustomerData.TallyName.Replace(" ", ""));
                _db.AddParameter("@FlatNo", ObjcustomerData.FlatNo);
                _db.AddParameter("@InstallationAmount", ObjcustomerData.InstallationAmount);
                _db.AddParameter("@MeterNo", ObjcustomerData.MeterNo);
                _db.AddParameter("@OwnerName", ObjcustomerData.OwnerName);
                _db.AddParameter("@OwnerEmail", ObjcustomerData.OwnerEmail);
                _db.AddParameter("@OwnerPhone", ObjcustomerData.OwnerPhone);
                _db.AddParameter("@Address", ObjcustomerData.Address);
                _db.AddParameter("@OwnerAddress", ObjcustomerData.OwnerAddress);
                _db.AddParameter("@CustomerType", ObjcustomerData.CustomerType);
                _db.AddParameter("@ClosingBalance", ObjcustomerData.ClosingBalance);
                _db.AddParameter("@ClosingRedg ", ObjcustomerData.ClosingRedg);
                _db.AddParameter("@OwnerId", ObjcustomerData.OwnerId);
                // _db.AddParameter("@UpdatedAt", DateTime.Now);
                Result = _db.ExecuteScalar("sp_UpdateCustomer", CommandType.StoredProcedure);

                //if (Convert.ToString(h) == "1")
                //{
                //    if (ObjcustomerData.NewMeterReading != 0)
                //    {
                //        if (ObjcustomerData.StockItemId != null)
                //        {
                //            if (ObjcustomerData.Amount != 0)
                //            {
                //                 DbProvider _db1 = new DbProvider();
                //                _db1.AddParameter("@Id", Guid.NewGuid().ToString());
                //                _db1.AddParameter("@CustomerId", ObjcustomerData.CustomerID);
                //                _db1.AddParameter("@StockItemId", ObjcustomerData.StockItemId);
                //                _db1.AddParameter("@GodownId", ObjcustomerData.GodownId);
                //                _db1.AddParameter("@UserId", userId);
                //                _db1.AddParameter("@BillNo", ObjcustomerData.BillNo);
                //                _db1.AddParameter("@BillDate", DateTime.Now.ToShortDateString());
                //                _db1.AddParameter("@ClosingBalance", ObjcustomerData.Amount);
                //                _db1.AddParameter("@ClosingRedg", ObjcustomerData.NewMeterReading);
                //                _db1.AddParameter("@Month", DateTime.Now.Month);
                //                _db1.AddParameter("@isPaid", ObjcustomerData.isPaid);
                //                _db1.AddParameter("@ReconnectionAmt", ObjcustomerData.ReconnectionAmt);
                //                _db1.AddParameter("@SGST", ObjcustomerData.SGST);
                //                _db1.AddParameter("@CGST", ObjcustomerData.CGST);
                //                _db1.AddParameter("@TotalAmt", ObjcustomerData.Amount);
                //                _db1.AddParameter("@PaymentTypeId", ObjcustomerData.PaymentTypeId);
                //                _db1.AddParameter("@AccountNo", ObjcustomerData.AccountNo);
                //                _db1.AddParameter("@ChequeNo", ObjcustomerData.ChequeNo);
                //                _db1.AddParameter("@MinAmt", ObjcustomerData.MinAmt);
                //                _db1.AddParameter("@BankName", ObjcustomerData.BankName);
                //                _db1.AddParameter("@PreviousBalance", ObjcustomerData.ClosingBalance);
                //                //1- deposit 2-GasConsume
                //                _db1.AddParameter("@PreviousRedg", ObjcustomerData.ClosingRedg);
                //                _db1.AddParameter("@BillType", "BFBC4C62-B98B-4C18-96E7-2C8327886BCB");
                //                h = _db1.ExecuteScalar("sp_InsertCustomerBillInformation", CommandType.StoredProcedure);
                //            }
                //            else
                //            { h = "Please Enter Amount"; }
                //        }
                //        else
                //        { h = "Need to fill Deposite Details"; }
                //    }

                //}
                return Convert.ToString(Result);

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                _db.Dispose();
            }
        }
        /// <summary>
        /// Get Customer By Id
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <param name="BillType"></param>
        /// <returns></returns>
        public CustomerDataModel GetCustomerById(string CustomerId, string BillType)
        {
            CustomerDataModel model = new CustomerDataModel();
            _db = new DbProvider();
            try
            {


                _db.AddParameter("@CustomerId", CustomerId);
                _db.AddParameter("@BillType", BillType);
                DbDataReader dr = _db.ExecuteDataReader("GetCustomerById", CommandType.StoredProcedure);
                while (dr.Read())
                {

                    model.CustomerID = Convert.ToString(dr["Id"]);
                    model.Name = Convert.ToString(dr["Name"]);
                    model.Address = Convert.ToString(dr["Address"]);
                    model.Email = Convert.ToString(dr["Email"]);
                    model.Phone = Convert.ToString(dr["Phone"]);
                    model.FlatNo = Convert.ToString(dr["FlatNo"]);
                    model.GodownId = Convert.ToString(dr["GodownId"]);
                    model.TallyName = Convert.ToString(dr["TallyName"]);
                    if (dr["DateofInstallation"] != DBNull.Value)
                    { model.DateofInstallation = DateTime.Parse(dr["DateofInstallation"].ToString()); }
                    model.AreaId = dr["AreaId"] != DBNull.Value ? Convert.ToString(dr["AreaId"]) : "";
                    model.CompanyId = dr["CompanyId"] != DBNull.Value ? Convert.ToString(dr["CompanyId"]) : "";
                    model.GodownName = Convert.ToString(dr["GodownName"]);
                    model.InstallationAmount = Convert.ToDecimal(dr["InstallationAmount"]);
                    model.MeterNo = Convert.ToString(dr["MeterNo"]);
                    model.OwnerName = Convert.ToString(dr["ownername"]);
                    model.OwnerAddress = Convert.ToString(dr["Owneraddress"]);
                    model.OwnerEmail = Convert.ToString(dr["owneremail"]);
                    model.OwnerPhone = Convert.ToString(dr["Ownerphone"]);
                    model.OwnerId = Convert.ToString(dr["ownerid"]);
                    model.BillNo = Convert.ToString(dr["BillNo"]);
                    model.ClosingBalance = Convert.ToDecimal(dr["ClosingBalance"]);
                    model.ClosingRedg = Convert.ToDecimal(dr["ClosingRedg"]);
                    model.PreviousRedg = Convert.ToDecimal(dr["PreviousRedg"]);
                    model.StockItemId = Convert.ToString(dr["StockItemId"]);
                    model.CustomerType = Convert.ToString(dr["CustomerType"]);
                    if (dr["BillDate"] != DBNull.Value)
                    { model.BillDate = dr["BillDate"].ToString(); }
                    model.AccountNo = Convert.ToString(dr["AccountNo"]);
                    model.Amount = Convert.ToDecimal(dr["Amount"]);
                    model.BankName = Convert.ToString(dr["BankName"]);
                    model.ChequeNo = Convert.ToString(dr["ChequeNo"]);
                    model.PaymentTypeId = Convert.ToString(dr["PaymentTypeId"]);
                    model.StockItemId = Convert.ToString(dr["StockItemId"]);
                    model.InstallationAmount = Convert.ToDecimal(dr["InstallationAmount"]);
                    model.isPaid = Convert.ToBoolean(dr["isPaid"]);
                    model.CGST = Convert.ToDecimal(dr["CGST"]);
                    model.SGST = Convert.ToDecimal(dr["SGST"]);
                    model.NewMeterReading = Convert.ToDecimal(dr["InitialMeterReading"]);
                    model.ReconnectionAmt = Convert.ToDecimal(dr["ReconnectionAmt"]);
                    model.LateFee = Convert.ToDecimal(dr["LatePaymentFee"]);
                    model.ServiceAmt = Convert.ToDecimal(dr["ServiceCharges"]);
                    model.MinAmt = Convert.ToDecimal(dr["MinimumGasCharges"]);
                    model.CompanyId = Convert.ToString(dr["CompanyId"]);
                    model.PaymentDue = Convert.ToDecimal(dr["PreviousDue"]);
                    if (dr["PreviousBillDate"] != DBNull.Value)
                    { model.PreviousBillDate = DateTime.Parse(dr["PreviousBillDate"].ToString()); }
                    model.InputRate = Convert.ToDecimal(dr["InputRate"]);
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
        public string GetDecrypted(string p_EncryptedText)
        {
            string l_GetDecrypted = string.Empty;
            if (p_EncryptedText.Trim().Length == 0) { return l_GetDecrypted; }
            BMSecurity3DES objCrypto = new BMSecurity3DES();
            try
            { l_GetDecrypted = objCrypto.Decrypt_(p_EncryptedText); }
            catch (System.Exception ex) { throw new Exception("-GD: " + ex.Message); }
            finally { objCrypto.Dispose(); objCrypto = null; }
            return l_GetDecrypted;
        }

        /// <summary>
        /// Get List of Customer
        /// </summary>
        /// <returns></returns>
        public List<CustomerDataModel> GetAllCustomer(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey, string CityId, string AdminId, CustomerList cust)
        {
            _db = new DbProvider();
            List<CustomerDataModel> Customer = new List<CustomerDataModel>();
            try
            {
                _db.AddParameter("@PageNo", PageNo);
                if (PageSize != 0)
                    _db.AddParameter("@PageSize", PageSize);
                if (!string.IsNullOrEmpty(q))
                    _db.AddParameter("@q", q);
                _db.AddParameter("@sortkey ", sortby);
                _db.AddParameter("@sortby", sortkey != "desc" ? 1 : 0);
                _db.AddParameter("@CityId ", CityId);
                _db.AddParameter("@AdminId ", AdminId);
                _db.AddParameter("@CompanyId ", cust.CompanyId);
                _db.AddParameter("@AreaId", cust.AreaId);
                _db.AddParameter("@GodownId ", cust.GodownId);
                _db.AddParameter("@AliasName ", cust.AliasName);
                int TotalRows = 0;
                using (DbDataReader dr = _db.ExecuteDataReader("GetAllCustomer1", CommandType.StoredProcedure))
                {
                    while (dr.Read())
                    {
                        Customer.Add(new CustomerDataModel
                        {
                            CustomerID = Convert.ToString(dr["Id"]),
                            TallyName = Convert.ToString(dr["Name"]),
                            Address = Convert.ToString(dr["Address"]),
                            Email = Convert.ToString(dr["Email"]),
                            Phone = Convert.ToString(dr["Phone"]),
                            CustomerNumber = Convert.ToString(dr["CustomerNumber"]),
                            PassWord = dr["PassWord"] != DBNull.Value ? GetDecrypted(Convert.ToString(dr["PassWord"])) : "",
                            // FlatNo = Convert.ToString(dr["FlatNo"]),
                            //InstallationAmount = Convert.ToDecimal(dr["InstallationAmount"]),
                            //GodownId = Convert.ToString(dr["GodownId"]),
                            //AreaId = Convert.ToString(dr["AreaId"]),
                            //CompanyId = Convert.ToString(dr["CompanyId"]),
                            CompanyName = Convert.ToString(dr["CompanyName"]),
                            AreaName = Convert.ToString(dr["AreaName"]),
                            GodownName = Convert.ToString(dr["GodownName"]),
                            TotalRows = TotalRows = dr["TotalRows"] != DBNull.Value ? Convert.ToInt16(dr["TotalRows"]) : 0,
                            ConnectionType = Convert.ToString(dr["ConnectionType"]),
                            CreatedAt = Convert.ToDateTime(dr["CreatedAt"]),
                            AliasName = Convert.ToString(dr["AliasName"])
                        });
                    }
                    dr.Close();
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
            return Customer;
        }
        /// <summary>
        /// Active  & InActive Customer
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <param name="IsActive"></param>
        /// 
        public List<CustomerDataModel> GetAllCustomerforMail(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey, string CityId, string AdminId, CustomerList cust)
        {
            _db = new DbProvider();
            List<CustomerDataModel> Customer = new List<CustomerDataModel>();
            try
            {
                _db.AddParameter("@PageNo", PageNo);
                if (PageSize != 0)
                    _db.AddParameter("@PageSize", PageSize);
                if (!string.IsNullOrEmpty(q))
                    _db.AddParameter("@q", q);
                _db.AddParameter("@sortkey ", sortby);
                _db.AddParameter("@sortby", sortkey != "desc" ? 1 : 0);
                _db.AddParameter("@CityId ", CityId);
                _db.AddParameter("@AdminId ", AdminId);
                _db.AddParameter("@CompanyId ", cust.CompanyId);
                _db.AddParameter("@AreaId", cust.AreaId);
                _db.AddParameter("@GodownId ", cust.GodownId);

                int TotalRows = 0;
                using (DbDataReader dr = _db.ExecuteDataReader("GetAllCustomersForMail", CommandType.StoredProcedure))
                {
                    while (dr.Read())
                    {
                        Customer.Add(new CustomerDataModel
                        {
                            CustomerID = Convert.ToString(dr["Id"]),
                            TallyName = Convert.ToString(dr["Name"]),
                            Email = Convert.ToString(dr["Email"]),
                            Phone = Convert.ToString(dr["Phone"]),
                            CreatedAt = Convert.ToDateTime(dr["CreatedAt"]),
                            AliasName = Convert.ToString(dr["AliasName"]),
                            TotalRows = TotalRows = dr["TotalRows"] != DBNull.Value ? Convert.ToInt16(dr["TotalRows"]) : 0,
                        });
                    }
                    dr.Close();
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
            return Customer;
        }

        public void DeleteCustomer(string CustomerId, Int16 IsActive)
        {
            _db = new DbProvider();
            try
            {
                _db.AddParameter("@CustomerId", CustomerId);
                _db.AddParameter("@IsActive", IsActive);
                _db.ExecuteNonQuery("DeleteCustomer", CommandType.StoredProcedure);

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
        public string GetGodownName(string GodownId)
        {
            string Name = "";
            _db = new DbProvider();
            try
            {
                _db.AddParameter("@GodownId", GodownId);
                DbDataReader sdr = _db.ExecuteDataReader("GetGodownName", CommandType.StoredProcedure);
                if (sdr.Read())
                {
                    Name = Convert.ToString(sdr["ShortName"]);
                }
                return Name;
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

        public string GetCustomerunique(string GodownId)
        {
            string Customerunique = "";
            _db = new DbProvider();
            try
            {
                _db.AddParameter("@GodownId", GodownId);
                DbDataReader sdr = _db.ExecuteDataReader("GetCustomerunique", CommandType.StoredProcedure);
                if (sdr.Read())
                {
                    Customerunique = Convert.ToString(sdr["Customerunique"]);
                }
                return Customerunique;
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


        public List<ExportCustomerData> GetCustomerForExportReceipt(string CompanyId)
        {
            _db = new DbProvider();
            List<ExportCustomerData> Bill = new List<ExportCustomerData>();
            try
            {
                _db.AddParameter("@CompanyId", CompanyId);
                //_db.AddParameter("@BillTypeId", BillTypeId);
                _db.AddParameter("@ExportDate", DateTime.Now);

                DbDataReader sdr = _db.ExecuteDataReader("GetCustomerForExportReceipt", CommandType.StoredProcedure);

                while (sdr.Read())
                {
                    Bill.Add(new ExportCustomerData
                    {
                        Name = Convert.ToString(sdr["Name"]),
                        GodownMastername = Convert.ToString(sdr["GodownMastername"]),
                        ALIAS = Convert.ToString(sdr["ALIAS"]),
                        Phone = Convert.ToString(sdr["Phone"]),
                        EMAIL = Convert.ToString(sdr["EMAIL"]),
                        Address = Convert.ToString(sdr["Address"]),
                        Voucherdate = Convert.ToString(sdr["Voucherdate"]),
                        ClosingBalance = Convert.ToDecimal(sdr["ClosingBalance"]),
                        PaymentType = Convert.ToString(sdr["PaymentType"]),
                        Type = Convert.ToString(sdr["type"]),
                        ReceiptNo = Convert.ToString(sdr["ReceiptNo"]),
                        AccountNo = Convert.ToString(sdr["AccountNo"]),
                        ChequeNo = Convert.ToString(sdr["ChequeNo"]),
                        BankName = Convert.ToString(sdr["BankName"]),
                        DepositeAccountNo = Convert.ToString(sdr["DepositeAccountNo"]),
                        DepositeBankName = Convert.ToString(sdr["DepositeBankName"]),

                        BILLWISEDETAILS = Convert.ToString(sdr["BILLWISEDETAILS"]),
                        DUEDATE = Convert.ToString(sdr["DUEDATE"]),
                        COUNTRY = Convert.ToString(sdr["COUNTRY"]),
                        State = Convert.ToString(sdr["State"]),
                        PINCODE = Convert.ToString(sdr["PINCODE"]),
                        CONTACTPERSON = sdr["CONTACTPERSON"].ToString(),
                        MOBILE = Convert.ToString(sdr["MOBILE"]),
                        BillType = Convert.ToString(sdr["BillType"]),
                        TallyName = Convert.ToString(sdr["TallyName"]),
                        InitialMeterReading = Convert.ToDecimal(sdr["ClosingRedg"]),
                        CustomerNumber = Convert.ToString(sdr["CustomerNumber"]),
                        MailEMAIL = Convert.ToString(sdr["MailEMAIL"]),
                        EmailSend = Convert.ToString(sdr["EmailSend"]),

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
            return Bill;
        }


        public List<ExportWithoutBillData> GetPaymentsWithoutBill(string CompanyId)
        {
            _db = new DbProvider();
            List<ExportWithoutBillData> payment = new List<ExportWithoutBillData>();
            try
            {
                _db.AddParameter("@CompanyId", CompanyId);

                DbDataReader sdr = _db.ExecuteDataReader("ExportPaymentWithoutBill", CommandType.StoredProcedure);

                while (sdr.Read())
                {
                    payment.Add(new ExportWithoutBillData
                    {
                        Name = Convert.ToString(sdr["Name"]),
                        AliasName = Convert.ToString(sdr["AliasName"]),
                        Amount = Convert.ToDecimal(sdr["Amount"]),
                        Narration = Convert.ToString(sdr["Narration"]),
                        PaymentDate = Convert.ToString(sdr["PaymentDate"])
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
            return payment;

        }


        public DataTable PaymentSave(DataTable tdt)
        {
            DataTable dt = null;
            _db = new DbProvider();
            try
            {
                _db.AddParameter("@Receipt", tdt);
                DataSet ds = _db.ExecuteDataSet("PaymentImport_v3", CommandType.StoredProcedure);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dt = ds.Tables[0];
                }
                return dt;
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



        public void InsertEmployeeFile(string UserId, string Type, string BillFileName, string PaymentFileName, string FileName)
        {

            _db = new DbProvider();
            try
            {
                _db.AddParameter("@UserId", UserId);
                _db.AddParameter("@Type", Type);
                _db.AddParameter("@BillFileName", BillFileName);
                _db.AddParameter("@PaymentFileName", PaymentFileName);
                _db.AddParameter("@FileName", FileName);
                _db.ExecuteNonQuery("InsertEmployeeFile", CommandType.StoredProcedure);
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

        public DataTable GetSocietyWiseCustomer(string CompanyId, string GodownId, string AreaId)
        {
            DataTable dt = null;

            _db = new DbProvider();
            try

            {

                _db.AddParameter("@CompanyId", CompanyId);
                _db.AddParameter("@GodownId", GodownId);
                _db.AddParameter("@AreaId", AreaId);
                DataSet ds = _db.ExecuteDataSet("GetSocietyWiseCustomer", CommandType.StoredProcedure);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    dt = ds.Tables[0];
                }
                return dt;
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

        public CustomerDataModel GetMaxBillDate(string GodownId)
        {
            CustomerDataModel customer = new CustomerDataModel();
            _db = new DbProvider();
            try
            {
                _db.AddParameter("@GodownId", GodownId);
                DbDataReader sdr = _db.ExecuteDataReader("GetMaxBillDate", CommandType.StoredProcedure);
                while (sdr.Read())
                {

                    customer.MaxBillDate = Convert.ToString(sdr["MaxBillDate"]);


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
            return customer;
        }

        public List<ExportCustomerWithoutBill> GetCustomerList(string GodownId)
        {
            _db = new DbProvider();
            List<ExportCustomerWithoutBill> cust = new List<ExportCustomerWithoutBill>();
            try
            {
                _db.AddParameter("@GodownId", GodownId);

                DbDataReader sdr = _db.ExecuteDataReader("GetCustomerWithoutBill", CommandType.StoredProcedure);

                while (sdr.Read())
                {
                    cust.Add(new ExportCustomerWithoutBill
                    {
                        TallyName = Convert.ToString(sdr["TallyName"]),
                        AliasName = Convert.ToString(sdr["AliasName"]),
                        Email = Convert.ToString(sdr["Email"]),
                        Address = Convert.ToString(sdr["Address"]),
                        Phone = Convert.ToString(sdr["Phone"]),
                        FlatNo = Convert.ToString(sdr["FlatNo"])
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
            return cust;

        }


        public CustomerBillData GetCustomerBillData(string CustomerId)
        {
            CustomerBillData billData = new CustomerBillData();
            _db = new DbProvider();
            try
            {
                DataSet ds = null;
                _db.AddParameter("@CustomerId", CustomerId);
                ds = _db.ExecuteDataSet("GetCustomerBillDataById", CommandType.StoredProcedure);

                var Error = ds.Tables[0].Rows[0]["Error"].ToString();
                if (!string.IsNullOrEmpty(Error))
                {
                    throw new Exception(Error);
                }

                billData = (from DataRow dr in ds.Tables[0].Rows
                            select new CustomerBillData
                            {

                                CustomerId = Convert.ToString(dr["CustomerId"]),
                                Category = Convert.ToString(dr["Category"]),
                                CustomerName = Convert.ToString(dr["CustomerName"]),
                                InvoiceNo = Convert.ToString(dr["InvoiceNo"]),
                                BillDate = Convert.ToString(dr["BillDate"]),
                                DueDate = Convert.ToString(dr["DueDate"]),
                                BillAmount = Convert.ToString(dr["BillAmount"]),


                            }).FirstOrDefault();
                return billData;

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



        public string SavePayment(CustomerPaymentData model)
        {
            string _res = "";
            _db = new DbProvider();
            try
            {
                if (model.Amount < model.TotalAmount)
                {
                    model.PreviousDue = model.TotalAmount - model.Amount;
                    model.PaymentType = "Partial";
                    model.closingBalance = model.PreviousDue;
                    model.CustomerclosingBalance = 0;
                    model.PaymentDue = model.TotalAmount - model.Amount;
                    model.UniqueReceiptNo = Convert.ToInt32(model.Number);
                }
                else
                {
                    model.CustomerclosingBalance = model.Amount - model.TotalAmount;
                    model.closingBalance = model.Amount - model.TotalAmount;
                    model.PreviousDue = 0;
                    model.PaymentDue = 0;
                    model.PaymentType = "Full";
                    model.UniqueReceiptNo = model.UniqueReceiptNo;

                }

                model.PaymentTypeId = "D2FE28D0-7DC0-4BD7-9850-9190EC60FEDF";
                _db.AddParameter("@PaymentId", Guid.NewGuid().ToString());
                _db.AddParameter("@MerchantID", model.MerchantID);
                _db.AddParameter("@TxnReferenceNo", model.TxnReferenceNo);
                _db.AddParameter("@TxnAmount", model.TxnAmount);
                _db.AddParameter("@VendorId", model.VendorId);
                _db.AddParameter("@ReceiptNo", model.ReceiptNo);
                _db.AddParameter("@TxnDate", model.TxnDate);
                _db.AddParameter("@AuthStatus", model.AuthStatus);
                _db.AddParameter("@PaymentTypeId", model.PaymentTypeId);
                _db.AddParameter("@PaymentType", model.PaymentType);
                _db.AddParameter("@UniqueReceiptNo", model.UniqueReceiptNo);
                _db.AddParameter("@PaymentDue", model.PaymentDue);
                _db.AddParameter("@ClosingBalance", model.closingBalance);
                _db.AddParameter("@CustomerclosingBalance", model.CustomerclosingBalance);
                _db.AddParameter("@PreviousDue", model.PreviousDue);
                _db.AddParameter("@TransactionFree", model.TransactionFree);
                _db.AddParameter("@Auth1", model.Auth1);
                _db.AddParameter("@Auth2", model.Auth2);

                _res = _db.ExecuteScalar("InsertPayment", CommandType.StoredProcedure).ToString();


                return _res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _db.Dispose();
            }
        }

        //public string SavePaymentDetails(CustomerPaymentData model)
        //{
        //    string _res = "";
        //    _db = new DbProvider();
        //    try
        //    {
        //        //_db.AddParameter("@Id", Guid.NewGuid().ToString());
        //        _db.AddParameter("@MerchantID", model.MerchantID);
        //        _db.AddParameter("@TxnReferenceNo", model.TxnReferenceNo);
        //        _db.AddParameter("@TxnAmount", model.TxnAmount);
        //        _db.AddParameter("@VendorId", model.VendorId);
        //        _db.AddParameter("@TxnDate", model.TxnDate);
        //        _db.AddParameter("@AuthStatus", model.AuthStatus);
        //        _db.AddParameter("@CustomerID", model.CustomerID);
        //        _db.AddParameter("@Auth1", model.CustomerID);
        //        _db.AddParameter("@Auth2", model.BillId);
        //        _db.AddParameter("@Auth3", model.Auth3);
        //        _db.AddParameter("@Auth4", model.Auth4);
        //        _db.AddParameter("@Auth5", model.Auth5);
        //       _db.AddParameter("@CreatedAt", DateTime.Now);
        //        _res = _db.ExecuteNonQuery("InsertPayment", CommandType.StoredProcedure).ToString();
        //        return "Save";
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _db.Dispose();
        //    }
        //}

        public DataTable GetCustomerByIdForMail(string CustomerId)
        {
            DataTable dt = null;

            _db = new DbProvider();
            try

            {

                _db.AddParameter("@CustomerId", CustomerId);


                DataSet ds = _db.ExecuteDataSet("GetCustomerByIdForMail", CommandType.StoredProcedure);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    dt = ds.Tables[0];
                }
                return dt;
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

        public void EditEmail(string Email, string CustomerId, string Password) 
        { 
            CustomerDataModel model = new CustomerDataModel();
         //   ChangPwDataModel changepassDM= new ChangPwDataModel(); 
            _db = new DbProvider();
            try
            {
                _db.AddParameter("@Id",CustomerId);
                _db.AddParameter("@Email",Email);   
                _db.AddParameter("@Password",Password);
                _db.ExecuteScalar("UpdateCustomerEmailId", CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally { 
                
                _db.Dispose(); 
            }
          
        }
    }
    }
    


