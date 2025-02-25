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
    public class CustomerLoginDAL
    {
        DbProvider _db;

        /// <summary>
        /// create new Customer Login
        /// </summary>
        /// <param name="model"></param>
        public CustLoginDataModel SaveCustomerLogin(CustLoginDataModel model)
        {
            _db = new DbProvider();
            try
            {
                _db.AddParameter("@EmailId", model.EmailId);
                _db.AddParameter("@Phone", model.Phone);
                _db.AddParameter("@CustomerId", model.UniqueId);
                _db.AddParameter("@CreatedDate", System.DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss"));
                _db.AddParameter("@UpdatedDate", System.DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss"));
                _db.AddParameter("@CreatedAt", System.DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss"));
                _db.AddParameter("@OTP", model.OTP);
                DbDataReader sdr = _db.ExecuteDataReader("sp_InsertCustomerLogin", CommandType.StoredProcedure);
                if (sdr.Read())
                {
                    model.CustomerId = sdr["Id"].ToString();
                    model.Name = sdr["Name"].ToString();
                    model.EmailId = sdr["Email"].ToString();
                }
                else
                { return null; }

                return model;
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

        public CustomerLoginDataModel CustLogin(CustLoginDataModel objCust)
        {
            _db = new DbProvider();
            CustomerLoginDataModel model = new CustomerLoginDataModel();
            try
            {
                _db.AddParameter("@EmailId", objCust.EmailId);
                //_db.AddParameter("@Password", objCust.Password);
                DbDataReader dr = _db.ExecuteDataReader("sp_customerLogin", CommandType.StoredProcedure);
                dr.Read();
                if (dr.HasRows)
                {
                    model.EmailId = Convert.ToString(dr["EmailId"]);
                    model.Password = Convert.ToString(dr["Password"]);
                    model.CustomerId = Convert.ToString(dr["CustomerId"]);
                    model.Phone = Convert.ToString(dr["Phone"]);
                    model.OTP = Convert.ToString(dr["OTP"]);
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

        public string SaveCustomerPayment(CustomerPaymentDataModel model)
        {
            string _res = "";
            _db = new DbProvider();
            try
            {
                _db.AddParameter("@Id", Guid.NewGuid().ToString());
                _db.AddParameter("@MerchantID", model.MerchantID);
                _db.AddParameter("@CustomerID", model.AdditionalInfo5);
                _db.AddParameter("@TxnReferenceNo", model.TxnReferenceNo);
                _db.AddParameter("@BankReferenceNo", model.BankReferenceNo);
                _db.AddParameter("@TxnAmount", model.BankReferenceNo);
                _db.AddParameter("@BankID", model.BankReferenceNo);
                _db.AddParameter("@BankMerchantID", model.BankReferenceNo);
                _db.AddParameter("@TxnType", model.TxnType);
                _db.AddParameter("@CurrencyName", model.CurrencyName);
                _db.AddParameter("@ItemCode", model.ItemCode);
                _db.AddParameter("@SecurityType", model.SecurityType);
                _db.AddParameter("@SecurityID", model.SecurityID);
                _db.AddParameter("@SecurityPassword", model.SecurityPassword);
                _db.AddParameter("@TxnDate", model.TxnDate);
                _db.AddParameter("@AuthStatus", model.AuthStatus);
                _db.AddParameter("@SettlementType", model.SettlementType);
                _db.AddParameter("@AdditionalInfo1", model.AdditionalInfo1);
                _db.AddParameter("@AdditionalInfo2", model.AdditionalInfo2);
                _db.AddParameter("@AdditionalInfo3", model.AdditionalInfo3);
                _db.AddParameter("@AdditionalInfo4", model.AdditionalInfo4);
                _db.AddParameter("@AdditionalInfo5", model.AdditionalInfo5);
                _db.AddParameter("@AdditionalInfo6", model.AdditionalInfo6);
                _db.AddParameter("@AdditionalInfo7", model.AdditionalInfo7);
                _db.AddParameter("@ErrorStatus", model.ErrorStatus);
                _db.AddParameter("@ErrorDescription", model.ErrorDescription);
                _db.AddParameter("@CheckSum", model.CheckSum);
                _db.AddParameter("@CreatedAt", DateTime.Now);

                _db.AddParameter("@ResponseMsg", model.ResponseMsg);
                _res = _db.ExecuteNonQuery("InsertCustomerPayment", CommandType.StoredProcedure).ToString();
                return "Save";
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

        /// <summary>
        /// Create Customer bill reading.
        /// </summary>
        /// <param name="customerReading"></param>
        /// <returns></returns>
        public string SaveCustomerBillRedg(CustomerReading customerReading)
        {
            string _res = "";
            _db = new DbProvider();
            try
            {
                if (customerReading.MeterRedgImage != null)
                {
                    customerReading.MeterRedgImage = customerReading.MeterRedgImage;
                }
                else
                {
                    customerReading.MeterRedgImage = customerReading.UpdateMeterRedgImage;
                }
                _db.AddParameter("@Id", customerReading.Id);
                _db.AddParameter("@CustomerId", customerReading.CustomerId);
                _db.AddParameter("@CurrentRedg", customerReading.CurrentRedg);
                _db.AddParameter("@CurrentRedgDate", DateTime.Now);
                _db.AddParameter("@MeterRedgImage", customerReading.MeterRedgImage);
                _db.AddParameter("@CreatedAt", DateTime.Now);
                _db.AddParameter("@UpdatedAt", DateTime.Now);

                _res = _db.ExecuteNonQuery("InsertCustomerBillReading", CommandType.StoredProcedure).ToString();
                return "Save";
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
        /// Get Customer Bill Reading.
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        public CustomerReading GetCustomerBillReading(string CustomerRedgId)
        {
            CustomerReading customerReadings = new CustomerReading();
            _db = new DbProvider();
            try
            {
                _db.AddParameter("@CustomerRedgId", Convert.ToString(CustomerRedgId));
                DbDataReader sdr = _db.ExecuteDataReader("GetCustomerBillReading", CommandType.StoredProcedure);
                while (sdr.Read())

                {
                    customerReadings.Id = Convert.ToString(sdr["Id"]);
                    customerReadings.CurrentRedg = Convert.ToDecimal(sdr["CurrentRedg"]).ToString();
                    customerReadings.MeterRedgImage = Convert.ToString(sdr["MeterRedgImage"]);
                    customerReadings.BillId = Convert.ToString(sdr["BillId"]);
                    customerReadings.UpdateMeterRedgImage = Convert.ToString(sdr["MeterRedgImage"]);

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
            return customerReadings;
        }

        /// <summary>
        /// Get Customer Bill details.
        /// </summary>
        /// <param name="BillID"></param>
        /// <returns></returns>
        public BillInformationData GetCustomerBillDeatils(string BillID)
        {
            BillInformationData BillInformationData = new BillInformationData();
            _db = new DbProvider();
            try
            {
                DataSet ds = null;
                _db.AddParameter("@BillID", Convert.ToString(BillID));
                ds = _db.ExecuteDataSet("GetCustomerBillDeatils", CommandType.StoredProcedure);
                BillInformationData = (from DataRow dr in ds.Tables[0].Rows
                                       select new BillInformationData
                                       {
                                           TotalPaid = dr["Amount"] != DBNull.Value ? Convert.ToDecimal(dr["Amount"]) : 0,
                                           PartyName = dr["Name"].ToString(),
                                           Address = dr["Address"].ToString(),
                                           ClosingBalance = dr["ClosingBalance"] != DBNull.Value ? Convert.ToDecimal(dr["ClosingBalance"]) : 0,
                                           ClosingRedg = dr["ClosingRedg"] != DBNull.Value ? Math.Round(Convert.ToDecimal(dr["ClosingRedg"]), 3) : 0,
                                           PreviousBalance = dr["PreviousBalance"] != DBNull.Value ? Convert.ToDecimal(dr["PreviousBalance"]) : 0,
                                           PreviousRedg = dr["PreviousRedg"] != DBNull.Value ? Math.Round(Convert.ToDecimal(dr["PreviousRedg"]), 3) : 0,
                                           PreviousDue = dr["PreviousBalance"] != DBNull.Value ? Math.Round(Convert.ToDecimal(dr["PreviousBalance"]), 2) : 0,
                                           PreviousDiposite = dr["PreviousDiposite"] != DBNull.Value ? Math.Round(Convert.ToDecimal(dr["PreviousDiposite"]), 2) : 0,
                                           BillNo = dr["BillNo"].ToString(),
                                           DueDate = Convert.ToDateTime(dr["DueDate1"]).ToString("dd/MM/yyyy"),
                                           ConsumeUnit = dr["ConsumeUnit"] != DBNull.Value ? Math.Round(Convert.ToDecimal(dr["ConsumeUnit"]), 2) : 0,
                                           Rate = dr["Rate"] != DBNull.Value ? Math.Round(Convert.ToDecimal(dr["Rate"]), 2) : 0,
                                           ServiceAmt = dr["ServiceAmt"] != DBNull.Value ? Math.Round(Convert.ToDecimal(dr["ServiceAmt"]), 2) : 0,
                                           TotalAmt = dr["TotalAmt"] != DBNull.Value ? Math.Round(Convert.ToDecimal(dr["TotalAmt"]), 2) : 0,
                                           CGST = dr["CGST"] != DBNull.Value ? Math.Round(Convert.ToDecimal(dr["CGST"]), 2) : 0,
                                           SGST = dr["SGST"] != DBNull.Value ? Math.Round(Convert.ToDecimal(dr["SGST"]), 2) : 0,
                                           CurrentScm = dr["CurrentScm"] != DBNull.Value ? Math.Round(Convert.ToDecimal(dr["CurrentScm"]), 2) : 0,
                                           CurrentKGS = dr["CurrentKGS"] != DBNull.Value ? Math.Round(Convert.ToDecimal(dr["CurrentKGS"]), 2) : 0,
                                           BillPeriod = Convert.ToString(dr["BillPeriod"]),
                                           MinAmt = dr["MinAmt"] != DBNull.Value ? Math.Round(Convert.ToDecimal(dr["MinAmt"]), 2) : 0,
                                           Round = dr["Round"] != DBNull.Value ? Convert.ToDecimal(dr["Round"]) : 0,
                                           Diff = dr["Diff"] != DBNull.Value ? Math.Round(Convert.ToDecimal(dr["Diff"]), 2) : 0,
                                           PaymentDue = dr["PreviousBalance"] != DBNull.Value ? Math.Round(Convert.ToDecimal(dr["PreviousBalance"]), 2) : 0,
                                           LateFee = dr["LateFee"] != DBNull.Value ? Convert.ToDecimal(dr["LateFee"]) : 0,
                                           cloasingdate = Convert.ToDateTime(dr["BillDate1"]).ToString("dd/MM/yyyy"),
                                           BillDate = Convert.ToDateTime(dr["BillDate1"]).ToString("dd/MM/yyyy"),
                                           PreviousBillDate = Convert.ToDateTime(dr["PreviousBillDate1"]).ToString("dd/MM/yyyy"),
                                           LatePaymentFee = dr["LatePaymentFee"] != DBNull.Value ? Convert.ToDecimal(dr["LatePaymentFee"]) : 0,
                                           DelayDays = dr["DelayDays"] != DBNull.Value ? Convert.ToInt32(dr["DelayDays"]) : 0,
                                           DelayMinAmount = dr["DelayMinAmount"] != DBNull.Value ? Convert.ToDecimal(dr["DelayMinAmount"]) : 0,
                                           BillType = Convert.ToString(dr["BillType"]),
                                           Arrears = dr["InstallationAmount"] != DBNull.Value ? Math.Round(Convert.ToDecimal(dr["InstallationAmount"]), 3) : 0,
                                           ReconnectionAmt = dr["ReconnectionAmt"] != DBNull.Value ? Math.Round(Convert.ToDecimal(dr["ReconnectionAmt"]), 3) : 0,
                                           PreviousLateFree = dr["PreviousLateFree"] != DBNull.Value ? Math.Round(Convert.ToDecimal(dr["PreviousLateFree"]), 2) : 0,
                                           Lastdat = dr["lastDate1"] != DBNull.Value ? Convert.ToDateTime(dr["lastDate1"]).ToString("dd/MM/yyyy") : "",
                                       }).FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                _db.Dispose();
            }

            if (BillInformationData.Round != 0)
            {
                BillInformationData.Balcencedue = BillInformationData.Round;
            }
            else
            {
                BillInformationData.Balcencedue = BillInformationData.PaymentDue;

            }
            return BillInformationData;
        }

        /// <summary>
        /// Get Customer Bill Reading.
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        public CustomerReading CheckCustomerBillReading(string CustomerId)
        {
            CustomerReading customerReadings = new CustomerReading();
            _db = new DbProvider();
            try
            {
                _db.AddParameter("@CustomerId", Convert.ToString(CustomerId));
                DbDataReader sdr = _db.ExecuteDataReader("CheckCustomerBillReading", CommandType.StoredProcedure);
                while (sdr.Read())
                {
                    customerReadings.Id = Convert.ToString(sdr["Id"]);


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
            return customerReadings;
        }

        public CustomerReading GetPreviousCustomerBillReading(string CustomerId)
        {
            CustomerReading customerReadings = new CustomerReading();
            _db = new DbProvider();
            try
            {
                _db.AddParameter("@CustomerId", Convert.ToString(CustomerId));
                DbDataReader sdr = _db.ExecuteDataReader("GetPreviousCustomerBillReading", CommandType.StoredProcedure);
                while (sdr.Read())
                {
                    customerReadings.PreviousRedg = sdr["PreviousRedg"] != DBNull.Value ? Convert.ToDecimal(sdr["PreviousRedg"]) : 0;

                    customerReadings.PreviousBillDate = sdr["PreviousBillDate"] != DBNull.Value ? Convert.ToString(Convert.ToDateTime(sdr["PreviousBillDate"]).ToString("dd/MM/yyyy")) : "";
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
            return customerReadings;
        }

        public decimal totalamt(decimal Totalamt, decimal PLateFee)
        {
            decimal totalAmount = Totalamt + PLateFee + (PLateFee * 18 / 100);
            return totalAmount;
        }
        /// <summary>
        /// Get Godown List
        /// </summary>
        /// <returns></returns>
        public List<CustomerBillInformationData> GetBillHistory(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey, string CustomerId)
        {
            _db = new DbProvider();
            List<CustomerBillInformationData> Godown = new List<CustomerBillInformationData>();
            try
            {

                _db.AddParameter("@PageNo", PageNo);
                if (PageSize != 0)
                    _db.AddParameter("@PageSize", PageSize);
                if (!string.IsNullOrWhiteSpace(q))
                    _db.AddParameter("@q", q);
                _db.AddParameter("@sortkey ", sortby);
                _db.AddParameter("@CustomerId ", CustomerId);
                _db.AddParameter("@sortby", sortkey != "desc" ? 1 : 0);
                DbDataReader sdr = _db.ExecuteDataReader("GetBillHistory", CommandType.StoredProcedure);
                int TotalRows = 0;
                string date = "";


                while (sdr.Read())
                {
                    Godown.Add(new CustomerBillInformationData
                    {
                        billid = sdr["billId"].ToString(),
                        customerid = sdr["CustomerId"].ToString(),
                        BillDate = Convert.ToDateTime(sdr["BillDate"]).ToString("dd/MM/yyyy"),
                        BillNo = sdr["BillNo"].ToString(),
                        diposit = Convert.ToDecimal(sdr["PreviousDiposite"]),
                        ConsumeUnit = Convert.ToDecimal(sdr["ConsumeUnit"]),
                        //InvoiceValue = Convert.ToDecimal(sdr["Round"]),
                        Type = Convert.ToString(sdr["Type"]),
                        PLateFee = Convert.ToDecimal(sdr["PreviousLateFree"]),
                        ClosingRedg = Convert.ToDecimal(sdr["ClosingRedg"]),
                        TotalRows = TotalRows = sdr["TotalRows"] != DBNull.Value ? Convert.ToInt16(sdr["TotalRows"]) : 0,
                        InvoiceValue = totalamt(Convert.ToDecimal(sdr["TotalAmt"]), Convert.ToDecimal(sdr["PreviousLateFree"])) + Convert.ToDecimal(sdr["PreviousBalance"])
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

        public List<CustomerReading> GetCustomerRedgHistory(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey, string CustomerId)
        {
            _db = new DbProvider();
            List<CustomerReading> customerReadings = new List<CustomerReading>();
            try
            {

                _db.AddParameter("@PageNo", PageNo);
                if (PageSize != 0)
                    _db.AddParameter("@PageSize", PageSize);
                if (!string.IsNullOrWhiteSpace(q))
                    _db.AddParameter("@q", q);
                _db.AddParameter("@sortkey ", sortby);
                _db.AddParameter("@CustomerId ", CustomerId);
                _db.AddParameter("@sortby", sortkey != "desc" ? 1 : 0);
                DbDataReader sdr = _db.ExecuteDataReader("GetCustomerRedgHistory", CommandType.StoredProcedure);
                int TotalRows = 0;
                string date = "";
                while (sdr.Read())
                {
                    customerReadings.Add(new CustomerReading
                    {
                        Id = sdr["customerRedgId"].ToString(),
                        CustomerId = sdr["CustomerId"].ToString(),
                        BillId = sdr["BillId"].ToString(),
                        BillDate = sdr["BillDate"] != DBNull.Value ? Convert.ToDateTime(sdr["BillDate"]).ToString("dd/MM/yyyy") : "",
                        BillNo = sdr["BillNo"].ToString(),

                        MeterRedgImage = sdr["MeterRedgImage"].ToString(),
                        CurrentRedgDate = Convert.ToDateTime(sdr["CurrentRedgDate"]).ToLocalTime().ToString("dd/MM/yyyy"),
                        CurrentRedg = Convert.ToDecimal(sdr["CurrentRedg"]).ToString(),
                        TotalRows = TotalRows = sdr["TotalRows"] != DBNull.Value ? Convert.ToInt16(sdr["TotalRows"]) : 0,

                        Type = sdr["Type"].ToString(),
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
            return customerReadings;
        }

        public List<CustomerReading> GetAllCustomerRedgHistory(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey, reading objreading)
        {
            _db = new DbProvider();
            List<CustomerReading> customerReadings = new List<CustomerReading>();
            try
            {

                _db.AddParameter("@PageNo", PageNo);
                if (PageSize != 0)
                    _db.AddParameter("@PageSize", PageSize);
                if (!string.IsNullOrWhiteSpace(q))
                    _db.AddParameter("@q", q);
                _db.AddParameter("@sortkey ", sortby);
                _db.AddParameter("@CompanyId ", objreading.CompanyId);
                _db.AddParameter("@GodownId ", objreading.GodownId);
                _db.AddParameter("@AreaId ", objreading.AreaId);
                _db.AddParameter("@ReadingDate ", objreading.ReadingDate);
                _db.AddParameter("@sortby", sortkey != "desc" ? 1 : 0);
                DbDataReader sdr = _db.ExecuteDataReader("GetALLCustomerRedgHistory", CommandType.StoredProcedure);
                int TotalRows = 0;
                string date = "";
                while (sdr.Read())
                {
                    customerReadings.Add(new CustomerReading
                    {
                        Name = Convert.ToString(sdr["Name"]),
                        Id = sdr["customerRedgId"].ToString(),
                        CustomerId = sdr["CustomerId"].ToString(),
                        MeterRedgImage = sdr["MeterRedgImage"].ToString(),
                        CurrentRedgDate = Convert.ToDateTime(sdr["CurrentRedgDate"]).ToLocalTime().ToString("dd/MM/yyyy h:mm:ss tt"),
                        CurrentRedg = Convert.ToDecimal(sdr["CurrentRedg"]).ToString(),
                        TotalRows = TotalRows = sdr["TotalRows"] != DBNull.Value ? Convert.ToInt16(sdr["TotalRows"]) : 0,


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
            return customerReadings;
        }

        public List<CustomerReading> GetCustomerReadingHistoryList(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey, string CustomerId)
        {
            _db = new DbProvider();
            List<CustomerReading> customerReadings = new List<CustomerReading>();
            try
            {

                _db.AddParameter("@PageNo", PageNo);
                if (PageSize != 0)
                    _db.AddParameter("@PageSize", PageSize);
                if (!string.IsNullOrWhiteSpace(q))
                    _db.AddParameter("@q", q);
                _db.AddParameter("@sortkey ", sortby);
                _db.AddParameter("@CustomerId ", CustomerId);
                //_db.AddParameter("@BillId ", BillId);
                _db.AddParameter("@sortby", sortkey != "desc" ? 1 : 0);
                DbDataReader sdr = _db.ExecuteDataReader("GetAllCustomerBillRedgHistory", CommandType.StoredProcedure);
                int TotalRows = 0;
                string date = "";
                while (sdr.Read())
                {
                    customerReadings.Add(new CustomerReading
                    {
                        Id = sdr["customerRedgId"].ToString(),
                        CustomerId = sdr["CustomerId"].ToString(),
                        BillId = sdr["BillId"].ToString(),
                        MeterRedgImage = sdr["MeterRedgImage"].ToString(),
                        BillDate = sdr["BillDate"] != DBNull.Value ? Convert.ToDateTime(sdr["BillDate"]).ToString("dd/MM/yyyy") : "",
                        BillNo = sdr["BillNo"].ToString(),
                        CurrentRedgDate = Convert.ToDateTime(sdr["CurrentRedgDate"]).ToLocalTime().ToString("dd/MM/yyyy h:mm:ss tt"),
                        CurrentRedg = Convert.ToDecimal(sdr["CurrentRedg"]).ToString(),
                        TotalRows = TotalRows = sdr["TotalRows"] != DBNull.Value ? Convert.ToInt16(sdr["TotalRows"]) : 0,
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
            return customerReadings;
        }

        public CustomerBillInformationData GetAmountdetails(string CustomerId)
        {
            _db = new DbProvider();
            CustomerBillInformationData model = new CustomerBillInformationData();
            try
            {
                _db.AddParameter("@CustomerId", CustomerId);

                DbDataReader dr = _db.ExecuteDataReader("GetAmountDetails", CommandType.StoredProcedure);
                dr.Read();
                if (dr.HasRows)
                {
                    model.InvoiceValue = totalamt(Convert.ToDecimal(dr["Totalamount"]), Convert.ToDecimal(dr["PreviousLateFree"])); //Convert.ToDecimal(dr["Totalamount"]);
                    model.ClosingBalance = Convert.ToDecimal(dr["PaymentDue"]);
                    model.PreviousRedg = dr["PaidAmount"] != null ? Convert.ToDecimal(dr["PaidAmount"]) : 0;
                    model.diposit = Convert.ToDecimal(dr["PreviousDiposite"]);
                    model.billid = Convert.ToString(dr["billid"]);
                }
                else
                {
                    dr.Close();
                    _db.AddParameter("@CustomerId", CustomerId);
                    dr = _db.ExecuteDataReader("GetcustomerAmount", CommandType.StoredProcedure);
                    dr.Read();
                    if (dr.HasRows)
                    {
                        model.InvoiceValue = Convert.ToDecimal(dr["PaymentDue"]);
                        model.ClosingBalance = Convert.ToDecimal(dr["PaymentDue"]);
                        model.PreviousRedg = dr["PaidAmount"] != null ? Convert.ToDecimal(dr["PaidAmount"]) : 0;
                        model.diposit = 0;
                        model.billid = "";
                    }
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

        public PaymentDataModel GetBilldetails(string CustomerId)
        {
            _db = new DbProvider();
            PaymentDataModel model = new PaymentDataModel();
            try
            {
                _db.AddParameter("@CustomerId", CustomerId);
                DbDataReader dr = _db.ExecuteDataReader("GetAmountDetails", CommandType.StoredProcedure);
                dr.Read();
                if (dr.HasRows)
                {
                    model.TotalAmount = Convert.ToDecimal(dr["Totalamount"]);
                    model.PaymentDue = Convert.ToDecimal(dr["PaymentDue"]);
                    model.PaidAmount = dr["PaidAmount"] != null ? Convert.ToDecimal(dr["PaidAmount"]) : 0;
                    model.UniqueReceiptNo = Convert.ToInt32(dr["UniqueReceiptNo"]);
                    model.LatePaymentFee = Convert.ToDecimal(dr["LatePaymentFee"]);
                    model.DelayDays = Convert.ToInt32(dr["DelayDays"]);
                    model.DelayMinAmount = Convert.ToDecimal(dr["DelayMinAmount"]);
                    model.Address = Convert.ToString(dr["address"]);
                    model.Society = Convert.ToString(dr["sname"]);
                    model.CutomerName = Convert.ToString(dr["Name"]);
                    model.shortname = Convert.ToString(dr["shortname"]);
                    model.YEAR = Convert.ToString(dr["YEAR"]);
                    model.MONTH = Convert.ToString(dr["MONTH"]);
                    model.latefee = Convert.ToDecimal(dr["latefee"]);
                    model.closingBalance = Convert.ToDecimal(dr["closingBalance"]);
                    //model.BillDate1 = Convert.ToDateTime(dr["Billdate"]);
                    model.BillDate = Convert.ToString(dr["Billdate"]);
                    model.BillNo = Convert.ToString(dr["billno"]);
                    model.BillId = Convert.ToString(dr["BillId"]);
                    model.CustomerID = Convert.ToString(dr["Id"]);
                    model.GodownId = Convert.ToString(dr["GodownId"]);
                    model.ReceiptNo = Convert.ToString(dr["ReceiptNo1"]);
                    model.CutomerEmail = Convert.ToString(dr["CutomerEmail"]);
                    model.CutomerName = Convert.ToString(dr["Name"]);
                    model.CustNumber = Convert.ToString(dr["CustomerNumber"]);
                    model.PreviousLateFree = Convert.ToDecimal(dr["PreviousLateFree"]);
                }
                else
                {
                    dr.Close();
                    _db.AddParameter("@CustomerId", CustomerId);
                    dr = _db.ExecuteDataReader("GetcustomerAmount", CommandType.StoredProcedure);
                    dr.Read();
                    if (dr.HasRows)
                    {
                        model.TotalAmount = Convert.ToDecimal(dr["PaymentDue"]);
                        model.PaymentDue = Convert.ToDecimal(dr["PaymentDue"]);
                        model.PaidAmount = dr["PaidAmount"] != null ? Convert.ToDecimal(dr["PaidAmount"]) : 0;

                        model.BillId = "";
                        model.CustomerID = Convert.ToString(dr["Id"]);
                        model.GodownId = Convert.ToString(dr["GodownId"]);
                        model.ReceiptNo = Convert.ToString(dr["ReceiptNo1"]);
                        model.UniqueReceiptNo = Convert.ToInt32(dr["UniqueReceiptNo"]);
                        model.Society = Convert.ToString(dr["sname"]);
                        model.CutomerName = Convert.ToString(dr["Name"]);
                        model.shortname = Convert.ToString(dr["shortname"]);
                        model.YEAR = Convert.ToString(dr["YEAR"]);
                        model.MONTH = Convert.ToString(dr["MONTH"]);
                        model.CustNumber = Convert.ToString(dr["CustomerNumber"]);
                    }
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

        public PaymentDataModel GetAmountDetailsbyBillId(string CustomerId, string billid)
        {
            _db = new DbProvider();
            PaymentDataModel model = new PaymentDataModel();
            try
            {
                _db.AddParameter("@CustomerId", CustomerId);
                _db.AddParameter("@billid", billid);


                DbDataReader dr = _db.ExecuteDataReader("GetAmountDetailsbyBillId", CommandType.StoredProcedure);
                dr.Read();
                if (dr.HasRows)
                {
                    model.TotalAmount = Convert.ToDecimal(dr["Totalamount"]);
                    model.PaymentDue = Convert.ToDecimal(dr["PaymentDue"]);
                    model.PaidAmount = dr["PaidAmount"] != null ? Convert.ToDecimal(dr["PaidAmount"]) : 0;
                    model.UniqueReceiptNo = Convert.ToInt32(dr["UniqueReceiptNo"]);
                    model.LatePaymentFee = Convert.ToDecimal(dr["LatePaymentFee"]);
                    model.DelayDays = Convert.ToInt32(dr["DelayDays"]);
                    model.DelayMinAmount = Convert.ToDecimal(dr["DelayMinAmount"]);
                    model.Address = Convert.ToString(dr["address"]);
                    model.Society = Convert.ToString(dr["sname"]);
                    model.CutomerName = Convert.ToString(dr["Name"]);
                    model.shortname = Convert.ToString(dr["shortname"]);
                    model.YEAR = Convert.ToString(dr["YEAR"]);
                    model.MONTH = Convert.ToString(dr["MONTH"]);
                    model.latefee = Convert.ToDecimal(dr["latefee"]);
                    model.closingBalance = Convert.ToDecimal(dr["closingBalance"]);
                    //model.BillDate1 = Convert.ToDateTime(dr["Billdate"]);
                    model.BillDate = Convert.ToString(dr["Billdate"]);
                    model.BillNo = Convert.ToString(dr["billno"]);
                    model.BillId = Convert.ToString(dr["BillId"]);
                    model.GodownId = Convert.ToString(dr["GodownId"]);
                    model.ReceiptNo = Convert.ToString(dr["ReceiptNo1"]);
                    model.CustomerID = Convert.ToString(dr["Id"]);
                    model.CutomerEmail = Convert.ToString(dr["CutomerEmail"]);
                    //model.PreviousLateFree = Convert.ToDecimal(dr["PreviousLateFree"]);

                }
                else
                {
                    dr.Close();
                    _db.AddParameter("@CustomerId", CustomerId);
                    dr = _db.ExecuteDataReader("GetcustomerAmount", CommandType.StoredProcedure);
                    dr.Read();
                    if (dr.HasRows)
                    {
                        model.TotalAmount = Convert.ToDecimal(dr["PaymentDue"]);
                        model.PaymentDue = Convert.ToDecimal(dr["PaymentDue"]);
                        model.PaidAmount = dr["PaidAmount"] != null ? Convert.ToDecimal(dr["PaidAmount"]) : 0;

                        model.BillId = "";
                        model.CustomerID = Convert.ToString(dr["Id"]);
                        model.GodownId = Convert.ToString(dr["GodownId"]);
                        model.ReceiptNo = Convert.ToString(dr["ReceiptNo1"]);
                        model.UniqueReceiptNo = Convert.ToInt32(dr["UniqueReceiptNo"]);
                        model.Society = Convert.ToString(dr["sname"]);
                        model.CutomerName = Convert.ToString(dr["Name"]);
                        model.shortname = Convert.ToString(dr["shortname"]);
                        model.YEAR = Convert.ToString(dr["YEAR"]);
                        model.MONTH = Convert.ToString(dr["MONTH"]);
                    }
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

        public List<PaymentDataModel> GetPaymentHistory(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey, string CustomerId)
        {
            _db = new DbProvider();
            List<PaymentDataModel> Godown = new List<PaymentDataModel>();
            try
            {

                _db.AddParameter("@PageNo", PageNo);
                if (PageSize != 0)
                    _db.AddParameter("@PageSize", PageSize);
                if (!string.IsNullOrWhiteSpace(q))
                    _db.AddParameter("@q", q);
                _db.AddParameter("@sortkey ", sortby);
                _db.AddParameter("@billId ", CustomerId);
                _db.AddParameter("@sortby", sortkey != "desc" ? 1 : 0);
                DbDataReader sdr = _db.ExecuteDataReader("GetReceiptDetails", CommandType.StoredProcedure);
                int TotalRows = 0;
                string date = "";
                while (sdr.Read())
                {
                    Godown.Add(new PaymentDataModel
                    {

                        BillDate = sdr["BillDate"] !=DBNull.Value? Convert.ToDateTime(sdr["BillDate"]).ToString("dd/MM/yyyy"):"",
                        PaymentDate = sdr["PaymentDate"]!= DBNull.Value ? Convert.ToDateTime(sdr["PaymentDate"]).ToString("dd/MM/yyyy"):"",

                        BillNo = sdr["BillNo"].ToString(),
                        CheckNo = Convert.ToString(sdr["ChequeNo"]),
                        BankName = Convert.ToString(sdr["BankName"]),
                        BankDetail = Convert.ToString(sdr["BankDetail"]),
                        AccountNo = Convert.ToString(sdr["AccountNo"]),
                        ReceiptNo = Convert.ToString(sdr["ReceiptNo"]),
                        PaymentType = Convert.ToString(sdr["PaymentType"]),
                        Amount = Convert.ToDecimal(sdr["Amount"]),
                        TotalRows = TotalRows = sdr["TotalRows"] != DBNull.Value ? Convert.ToInt16(sdr["TotalRows"]) : 0,
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

        public List<PaymentDataModel> GetReceiptDetailsbyCustomerId(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey, string CustomerId)
        {
            _db = new DbProvider();
            List<PaymentDataModel> Godown = new List<PaymentDataModel>();
            try
            {

                _db.AddParameter("@PageNo", PageNo);
                if (PageSize != 0)
                    _db.AddParameter("@PageSize", PageSize);
                if (!string.IsNullOrWhiteSpace(q))
                    _db.AddParameter("@q", q);
                _db.AddParameter("@sortkey ", sortby);
                _db.AddParameter("@CustomerId ", CustomerId);
                _db.AddParameter("@sortby", sortkey != "desc" ? 1 : 0);
                DbDataReader sdr = _db.ExecuteDataReader("GetReceiptDetailsbyCustomer", CommandType.StoredProcedure);
                int TotalRows = 0;
                string date = "";
                while (sdr.Read())
                {
                    Godown.Add(new PaymentDataModel
                    {

                        BillDate = sdr["BillDate"]!=DBNull.Value? Convert.ToDateTime(sdr["BillDate"]).ToString("dd/MM/yyyy"):"",
                        PaymentDate = Convert.ToDateTime(sdr["PaymentDate"]).ToString("dd/MM/yyyy"),

                        BillNo = sdr["BillNo"].ToString(),
                        ChequeNo = Convert.ToString(sdr["ChequeNo"]),
                        BankName = Convert.ToString(sdr["BankName"]),
                        BankDetail = Convert.ToString(sdr["BankDetail"]),
                        AccountNo = Convert.ToString(sdr["AccountNo"]),
                        ReceiptNo = Convert.ToString(sdr["ReceiptNo"]),
                        PaymentType = Convert.ToString(sdr["PaymentType"]),
                        Amount = Convert.ToDecimal(sdr["Amount"]),
                        TotalRows = TotalRows = sdr["TotalRows"] != DBNull.Value ? Convert.ToInt16(sdr["TotalRows"]) : 0,
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

        public string UpdateTransactionFree(PaymentDataModel model)
        {
            string _res = "";
            _db = new DbProvider();
            try
            {
                _db.AddParameter("@BillId", model.BillId);
                _db.AddParameter("@TransactionFree", model.TransactionFree);
                _res = _db.ExecuteNonQuery("UpdateTransactionFree", CommandType.StoredProcedure).ToString();
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

        public string SavePayment(PaymentDataModel model)
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
                _db.AddParameter("@Id", Guid.NewGuid().ToString());
                _db.AddParameter("@PaymentTypeId", model.PaymentTypeId);
                _db.AddParameter("@CustomerID", model.CustomerID);
                _db.AddParameter("@BillId", model.BillId);
                _db.AddParameter("@ReceiptNo", model.ReceiptNo);
                _db.AddParameter("@Amount", model.Amount);
                _db.AddParameter("@PaymentType", model.PaymentType);
                _db.AddParameter("@GodownId", model.GodownId);
                _db.AddParameter("@UniqueReceiptNo", model.UniqueReceiptNo);
                _db.AddParameter("@PaymentDue", model.PaymentDue);
                _db.AddParameter("@ClosingBalance", model.closingBalance);
                _db.AddParameter("@CustomerclosingBalance", model.CustomerclosingBalance);
                _db.AddParameter("@PreviousDue", model.PreviousDue);
                _db.AddParameter("@TransactionFree", model.TransactionFree);
                _res = _db.ExecuteScalar("InsertPaymentDetails", CommandType.StoredProcedure).ToString();
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

        public string SavePaymentWithoutBill(PaymentDataModel model)
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
                _db.AddParameter("@Id", Guid.NewGuid().ToString());
                _db.AddParameter("@PaymentTypeId", model.PaymentTypeId);
                _db.AddParameter("@CustomerID", model.CustomerID);
                _db.AddParameter("@BillId", model.BillId);
                _db.AddParameter("@ReceiptNo", model.ReceiptNo);
                _db.AddParameter("@Amount", model.Amount);
                _db.AddParameter("@PaymentType", model.PaymentType);
                _db.AddParameter("@GodownId", model.GodownId);
                _db.AddParameter("@UniqueReceiptNo", model.UniqueReceiptNo);
                _db.AddParameter("@PaymentDue", model.PaymentDue);
                _db.AddParameter("@ClosingBalance", model.closingBalance);
                _db.AddParameter("@CustomerclosingBalance", model.CustomerclosingBalance);
                _db.AddParameter("@PreviousDue", model.PreviousDue);
                _db.AddParameter("@TransactionFree", model.TransactionFree);
                _res = _db.ExecuteScalar("UpdatePaymentDetailswithoutbill", CommandType.StoredProcedure).ToString();
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


        public ChangPwDataModel EmailCheck(ChangPwDataModel forgotPasswordDataModel)
        {
            ChangPwDataModel ChangPwDataModel = new ChangPwDataModel();
            //string _res = "";
            _db = new DbProvider();
            try
            {
                _db.AddParameter("@CustomerNumber", forgotPasswordDataModel.EmailId);
                DbDataReader dr = _db.ExecuteDataReader("sp_EmailExist", CommandType.StoredProcedure);
                dr.Read();
                if (dr.HasRows)
                {
                    ChangPwDataModel.Id = Convert.ToString(dr["Id"]);
                    ChangPwDataModel.EmailId = Convert.ToString(dr["Email"]);
                    ChangPwDataModel.Name = Convert.ToString(dr["Name"]);


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
            return ChangPwDataModel;
        }

        public string changePwd(ChangPwDataModel model)
        {
            string _res = "";
            _db = new DbProvider();
            try
            {
                _db.AddParameter("@Id", model.Id);
                _db.AddParameter("@Password", model.Password);
                _res = _db.ExecuteNonQuery("sp_ChangePassword", CommandType.StoredProcedure).ToString();
                return "Save";
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

        public string UpdateEmail(ChangPwDataModel model)
        {
            string _res = "";
            _db = new DbProvider();
            try
            {
                _db.AddParameter("@Id", model.Id);
                _db.AddParameter("@Password", model.Password);
                _db.AddParameter("@Email", model.EmailId);
                _res = _db.ExecuteNonQuery("UpdateCustomerEmail", CommandType.StoredProcedure).ToString();
                return "Save";
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

        public CustomerLoginDataModel searchCustomer(CustomerLoginDataModel objcustomerLogin)
        {
            _db = new DbProvider();
            CustomerLoginDataModel model = new CustomerLoginDataModel();
            try
            {
                _db.AddParameter("@CompanyId", objcustomerLogin.CompanyId);
                _db.AddParameter("@GodownId", objcustomerLogin.GodownId);
                _db.AddParameter("@Email", objcustomerLogin.EmailId);
                _db.AddParameter("@Phone", objcustomerLogin.Phone);
                _db.AddParameter("@Id", objcustomerLogin.CustomerId);
                DbDataReader dr = _db.ExecuteDataReader("sp_searchcustomer", CommandType.StoredProcedure);
                dr.Read();
                if (dr.HasRows)
                {
                    model.Name = Convert.ToString(dr["Name"]);
                    model.EmailId = Convert.ToString(dr["Email"]);
                    model.Address = Convert.ToString(dr["Address"]);
                    model.Phone = Convert.ToString(dr["Phone"]);
                    model.CustomerId = Convert.ToString(dr["Id"]);
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

        public string GetPassword(string email)
        {
            string result = "";
            _db = new DbProvider();
            try
            {
                _db.AddParameter("@EmailId", email);
                DbDataReader dr = _db.ExecuteDataReader("sp_GetPassword", CommandType.StoredProcedure);
                dr.Read();
                if (dr.HasRows)
                {
                    result = Convert.ToString(dr["Password"]);
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
            return result;
        }

        public CustLoginDataModel OTPCheck(CustLoginDataModel objCust)
        {
            _db = new DbProvider();
            CustLoginDataModel model = new CustLoginDataModel();
            try
            {
                _db.AddParameter("@OTP", objCust.OTP);
                _db.AddParameter("@Id", objCust.Id);
                DbDataReader dr = _db.ExecuteDataReader("sp_OTPCheck", CommandType.StoredProcedure);
                dr.Read();
                if (dr.HasRows)
                {
                    model.EmailId = Convert.ToString(dr["EmailId"]);
                    model.UniqueId = Convert.ToString(dr["CustomerId"]);
                    model.Phone = Convert.ToString(dr["Phone"]);
                    model.OTP = Convert.ToString(dr["OTP"]);
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

        public string GetPasswordId(string CustomerId)
        {
            string pwa = "";
            _db = new DbProvider();

            try
            {
                _db.AddParameter("@CustomerId", CustomerId);
                DbDataReader dr = _db.ExecuteDataReader("GetPasswordofcustomerbyId", CommandType.StoredProcedure);
                dr.Read();
                if (dr.HasRows)
                {
                    pwa = Convert.ToString(dr["PassWord"]);

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
            return pwa;
        }

        public CustomerBillInformationData GetBillById(string BillId)
        {
            _db = new DbProvider();
            CustomerBillInformationData model = new CustomerBillInformationData();
            try
            {
                _db.AddParameter("@BillId", BillId);
                DbDataReader dr = _db.ExecuteDataReader("GetBillById", CommandType.StoredProcedure);
                dr.Read();
                if (dr.HasRows)
                {
                    model.Month = Convert.ToString(dr["BillPeriod"]);
                    model.StockItemName = Convert.ToString(dr["Address"]);
                    model.PartyName = Convert.ToString(dr["Name"]);
                    model.ClosingBalance = Convert.ToDecimal(dr["PaymentDue"]);
                    model.billid = Convert.ToString(dr["Id"]);
                    model.BillNo = Convert.ToString(dr["BillNo"]);
                    model.BillDate = dr["Billdate"].ToString();
                    model.DueDate = dr["DueDate"].ToString();
                    model.PreviousBillDate = dr["PreviousBillDate"].ToString();
                    model.ClosingRedg = Convert.ToDecimal(dr["ClosingRedg"]);
                    model.PreviousRedg = dr["PreviousRedg"] != null ? Convert.ToDecimal(dr["PreviousRedg"]) : 0;
                    model.GASCOMSUMPTIONLed = Convert.ToDecimal(dr["CurrentScm"]);
                    model.ConsumptioninKG = dr["CurrentKGS"] != null ? Convert.ToDecimal(dr["CurrentKGS"]) : 0;
                    model.Rate = Convert.ToDecimal(dr["Rate"]);
                    model.ConsumeUnit = Convert.ToDecimal(dr["ConsumeUnit"]);
                    model.MinAmt = dr["MinAmt"] != null ? Convert.ToDecimal(dr["MinAmt"]) : 0;
                    model.SGST = Convert.ToDecimal(dr["SGST"]);
                    model.CGST = Convert.ToDecimal(dr["CGST"]);
                    model.ServiceAmt = Convert.ToDecimal(dr["ServiceAmt"]);
                    model.ReconnectionAmt = Convert.ToDecimal(dr["ReconnectionAmt"]);

                    model.InstallationCharges = Convert.ToDecimal(dr["InstallationCharges"]);

                    model.InvoiceValue = Convert.ToDecimal(dr["TotalAmt"]);
                    model.diposit = dr["PreviousDiposite"] != null ? Convert.ToDecimal(dr["PreviousDiposite"]) : 0;
                    model.PreviousBalance = dr["PreviousBalance"] != null ? Convert.ToDecimal(dr["PreviousBalance"]) : 0;
                    model.Arrears = Convert.ToDecimal(dr["PaymentDue"]);
                    model.LateFee = Convert.ToDecimal(dr["LateFee"]);
                    model.Type = dr["Type"].ToString();
                    model.Amount = Convert.ToDecimal(dr["Amount"]);
                    model.PaymentDue = Convert.ToDecimal(dr["PaymentDue"]);
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

        public CompaniesPaymentModel GetCompaniesPaymentSetup(string CustomerId)
        {
            _db = new DbProvider();
            CompaniesPaymentModel Company = new CompaniesPaymentModel();
            try
            {

                _db.AddParameter("@CustomerId", CustomerId);
                DbDataReader sdr = _db.ExecuteDataReader("GetCompanyPaymentSetup", CommandType.StoredProcedure);
                while (sdr.Read())
                {

                    //Company.Id = Convert.ToString(sdr["Id"]);
                    //Company.CompanyId = Convert.ToString(sdr["CompanyId"]);
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


        public CustLoginDataModel CustomerLogin(CustLoginDataModel objCust)
        {
            _db = new DbProvider();
            CustLoginDataModel model = new CustLoginDataModel();
            try
            {
                _db.AddParameter("@Username", objCust.CustomerNumber);
                _db.AddParameter("@Password", objCust.password);
                DbDataReader dr = _db.ExecuteDataReader("CustomerLogin", CommandType.StoredProcedure);
                dr.Read();
                if (dr.HasRows)
                {
                    model.CustomerNumber = Convert.ToString(dr["CustomerNumber"]);
                    model.Id = Convert.ToString(dr["Id"]);
                    model.Name = Convert.ToString(dr["Name"]);
                    model.EmailId = Convert.ToString(dr["AliasName"]);
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

        /// <summary>
        /// Get  CustomerDeatils.
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        public DataSet GetCustomerDeatils(string CustomerId, string GodownId, string CompanyId)
        {
            DataSet ds = null;
            _db = new DbProvider();
            try
            {
                _db.AddParameter("@CustomerId", Convert.ToString(CustomerId));
                _db.AddParameter("@GodownId", Convert.ToString(GodownId));
                _db.AddParameter("@CompanyId", Convert.ToString(CompanyId));
                ds = _db.ExecuteDataSet("GetCustomerDeatils", CommandType.StoredProcedure);

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                _db.Dispose();
            }
            return ds;
        }

        public string Insertbill(BillInformationData objBillInformationData, DateTime DueDate, DateTime PreviousBillDate, string MeterRedgImage)
        {
            string _res = "";
            _db = new DbProvider();
            try
            {
                _db.AddParameter("@PreviosBillId", objBillInformationData.BillId);
                _db.AddParameter("@CustomerId", objBillInformationData.CustomerId);
                _db.AddParameter("@StockItemId", objBillInformationData.StockItemId);
                _db.AddParameter("@GodownId", objBillInformationData.GodownId);
                _db.AddParameter("@UserId", objBillInformationData.UserId);
                _db.AddParameter("@BillNo", objBillInformationData.BillNo);
                _db.AddParameter("@BillDate", DateTime.Now);
                _db.AddParameter("@ClosingBalance", objBillInformationData.ClosingBalance);
                _db.AddParameter("@ClosingRedg", objBillInformationData.ClosingRedg);
                _db.AddParameter("@PreviousBalance", objBillInformationData.PreviousBalance);
                _db.AddParameter("@PreviousRedg", objBillInformationData.PreviousRedg);
                _db.AddParameter("@DueDate", DueDate);
                _db.AddParameter("@ConsumeUnit", objBillInformationData.ConsumeUnit);
                _db.AddParameter("@Rate", objBillInformationData.Rate);
                _db.AddParameter("@BillMonth", objBillInformationData.BillMonth);
                _db.AddParameter("@ServiceAmt", objBillInformationData.ServiceAmt);
                _db.AddParameter("@isPaid", objBillInformationData.isPaid);
                _db.AddParameter("@lastDate", PreviousBillDate);
                _db.AddParameter("@TotalAmt", objBillInformationData.Stotal);
                _db.AddParameter("@CGST", objBillInformationData.CGST);
                _db.AddParameter("@SGST", objBillInformationData.SGST);
                _db.AddParameter("@BillType", objBillInformationData.BillType);
                _db.AddParameter("@CurrentScm", objBillInformationData.SCurrentScm);
                _db.AddParameter("@CurrentKGS", objBillInformationData.SCurrentKGS);
                _db.AddParameter("@PaymentDue", objBillInformationData.Balcencedue);
                _db.AddParameter("@BillPeriod", objBillInformationData.BillPeriod);
                if (objBillInformationData.ConsumeUnit == 0)
                {
                    _db.AddParameter("@MinAmt", objBillInformationData.MinAmt);
                }
                else
                {
                    _db.AddParameter("@MinAmt", 0);
                }
                _db.AddParameter("@CreatedDate", DateTime.Now);
                _db.AddParameter("@PreviousDiposite", objBillInformationData.PreviousDiposite);
                _db.AddParameter("@BillCount", objBillInformationData.BillCount);
                _db.AddParameter("@Round", objBillInformationData.Round);
                _db.AddParameter("@Diff", objBillInformationData.Diff);
                _db.AddParameter("@MeterRedgImage", MeterRedgImage);
                _db.AddParameter("@PBillId", objBillInformationData.PBillId);
                _db.AddParameter("@PLateFee", objBillInformationData.PLateFee);
                _db.AddParameter("@PreviousLateFree", objBillInformationData.PreviousLateFree);
                _res = _db.ExecuteNonQuery("InsertCustomerBill", CommandType.StoredProcedure).ToString();
                return "Save";
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
    }
}
       