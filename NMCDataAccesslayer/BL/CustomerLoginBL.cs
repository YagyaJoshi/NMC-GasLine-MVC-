using NMCDataAccesslayer.DAL;
using NMCDataAccesslayer.DataModel;
using NMCDataAccesslayer.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace NMCDataAccesslayer.BL
{
    public class CustomerLoginBL
    {
        CustomerLoginDAL obj = new CustomerLoginDAL();
        public CustLoginDataModel SaveCustomerLogin(DataModel.CustLoginDataModel objModel)
        {
            return obj.SaveCustomerLogin(objModel);
        }
        public CustomerLoginDataModel CustLogin(CustLoginDataModel objCust)
        {
            CustomerLoginDataModel custLoginDataModel = new CustomerLoginDataModel();
            custLoginDataModel = obj.CustLogin(objCust);
            return custLoginDataModel;
        }

        public string SaveCustomerPayment(CustomerPaymentDataModel model)
        {
            string res = obj.SaveCustomerPayment(model);
            return res;
        }

        public string SaveCustomerBillRedg(CustomerReading customerReading)
        {
            string res = obj.SaveCustomerBillRedg(customerReading);
            return res;
        }

        public CustomerReading GetCustomerBillReading(string CustomerRedgId)
        {
            CustomerReading customerReadings = new CustomerReading();
            return customerReadings = obj.GetCustomerBillReading(CustomerRedgId);
        }

        public BillInformationData GetCustomerBillDeatils(string BillID)
        {
            BillInformationData customerReadings = new BillInformationData();
            return customerReadings = obj.GetCustomerBillDeatils(BillID);
        }

        public CustomerReading CheckCustomerBillReading(string CustomerRedgId)
        {
            CustomerReading customerReadings = new CustomerReading();
            return customerReadings = obj.CheckCustomerBillReading(CustomerRedgId);
        }

        public CustomerReading GetPreviousCustomerBillReading(string CustomerRedgId)
        {
            CustomerReading customerReadings = new CustomerReading();
            return customerReadings = obj.GetPreviousCustomerBillReading(CustomerRedgId);
        }

        public List<CustomerBillInformationData> GetBillHistory(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey, string CustomerId)
        {
            List<CustomerBillInformationData> custLoginDataModel = new List<CustomerBillInformationData>();
            custLoginDataModel = obj.GetBillHistory(PageNo, PageSize, q, sortby, sortkey, CustomerId);
            return custLoginDataModel;
        }
        public List<CustomerReading> GetCustomerRedgHistory(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey, string CustomerId)
        {
            List<CustomerReading> objCustomerReading = new List<CustomerReading>();
            objCustomerReading = obj.GetCustomerRedgHistory(PageNo, PageSize, q, sortby, sortkey, CustomerId);
            return objCustomerReading;
        }

        public List<CustomerReading> GetAllCustomerRedgHistory(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey, reading objreading)
        {
            List<CustomerReading> objCustomerReading = new List<CustomerReading>();
            objCustomerReading = obj.GetAllCustomerRedgHistory(PageNo, PageSize, q, sortby, sortkey, objreading);
            return objCustomerReading;
        }

        public List<CustomerReading> GetCustomerReadingHistoryList(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey, string CustomerId)
        {
            List<CustomerReading> objCustomerReading = new List<CustomerReading>();
            objCustomerReading = obj.GetCustomerReadingHistoryList(PageNo, PageSize, q, sortby, sortkey, CustomerId);
            return objCustomerReading;
        }

        public CustomerBillInformationData GetAmountdetails(string CustomerId)
        {
            CustomerBillInformationData custLoginDataModel = new CustomerBillInformationData();
            custLoginDataModel = obj.GetAmountdetails(CustomerId);
            return custLoginDataModel;
        }


        public PaymentDataModel GetBilldetails(string CustomerId)
        {
            PaymentDataModel custLoginDataModel = new PaymentDataModel();
            custLoginDataModel = obj.GetBilldetails(CustomerId);
            return custLoginDataModel;
        }
  

        public PaymentDataModel GetAmountDetailsbyBillId(string CustomerId, string billid)
        {
            PaymentDataModel custLoginDataModel = new PaymentDataModel();
            custLoginDataModel = obj.GetAmountDetailsbyBillId(CustomerId, billid);
            return custLoginDataModel;
        }


        public string SavePayment(PaymentDataModel objPaymentDataModel)
        {
            return obj.SavePayment(objPaymentDataModel);
        }

        public string SavePaymentWithoutBill(PaymentDataModel objPaymentDataModel)
        {
            return obj.SavePaymentWithoutBill(objPaymentDataModel);
        }

        

        public string UpdateTransactionFree(PaymentDataModel objPaymentDataModel)
        {
            return obj.UpdateTransactionFree(objPaymentDataModel);
        }

        public List<PaymentDataModel> GetPaymentHistory(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey, string CustomerId)
        {
            List<PaymentDataModel> custLoginDataModel = new List<PaymentDataModel>();
            custLoginDataModel = obj.GetPaymentHistory(PageNo, PageSize, q, sortby, sortkey, @CustomerId);
            return custLoginDataModel;
        }

        public List<PaymentDataModel> GetReceiptDetailsbyCustomerId(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey, string CustomerId)
        {
            List<PaymentDataModel> custLoginDataModel = new List<PaymentDataModel>();
            custLoginDataModel = obj.GetReceiptDetailsbyCustomerId(PageNo, PageSize, q, sortby, sortkey, @CustomerId);
            return custLoginDataModel;
        }


        public string GetReceiptno(Int32 UniqueReceiptNo, string code, string year, string month, string ReceiptNo)
        {
            Int32 recno = Convert.ToInt32(ReceiptNo) + 1;
            string UniqueReceipt = "";
            Int32 no = UniqueReceiptNo;
            UniqueReceipt = Convert.ToString(UniqueReceiptNo);
            if (UniqueReceipt.Length == 1)
            {
                UniqueReceipt = "000" + (1 + Convert.ToInt32(UniqueReceipt)); //no = 1+ Convert.ToInt32(no);
            }
            else if (UniqueReceipt.Length == 2)
            {
                UniqueReceipt = "00" + UniqueReceiptNo; //no = 1 + Convert.ToInt32(no);
            }
            else if (UniqueReceipt.Length == 3)
            {
                UniqueReceipt = "0" + UniqueReceiptNo; //no = 1 + Convert.ToInt32(no);
            }
            UniqueReceiptNo = Convert.ToInt32(UniqueReceipt);
            UniqueReceipt = "R" + code + "" + year + "" + month + "" + "-" + UniqueReceipt + "-" + recno + "W";
            return UniqueReceipt;

        }
        public decimal calculateDelayCharges(DateTime billDateTime, DateTime currentDateTime, int delayDays, decimal companyDelayCharges)
        {
            Int32 DUE_DAYS_SLOT = 3;
            Int32 delayDaysLimit = delayDays * DUE_DAYS_SLOT;
            DateTime date1 = billDateTime;
            DateTime date2 = currentDateTime;
            // getting the difference
            TimeSpan t = date2.Subtract(date1);
            double daysDifference = t.TotalDays;
            if (daysDifference > delayDaysLimit)
            {
                daysDifference = delayDaysLimit;
            }

            int modulo;
            try
            {
                modulo = (int)(daysDifference / delayDays);
            }
            catch (Exception e)
            {
                modulo = 0;
            }

            decimal delayCharge = companyDelayCharges * modulo;
            return delayCharge;
        }
        public decimal calculateTaxOnDelayAmount(decimal totalAmount)
        {
            return (totalAmount * 18) / 100;
        }
        decimal billletfee;
        public Tuple<decimal, decimal, decimal, decimal, decimal, Int32, Int32> calculateDelayCharges(Int32 UniqueReceiptNo, string BillDate, decimal CLatePaymentFee, Int32 DelayDays, decimal DelayMinAmount, decimal PaymentDue, decimal bLateFee, decimal bClosingBalance, decimal PreviousLateFree)
        {
            int delayDays = DelayDays;
            decimal delayMinAmount = DelayMinAmount;
            decimal taxableDelayAmount; // delaychg.
            decimal ClosingBalance;  //ClosingBalance
            decimal Total;
            Int32 Unumber = UniqueReceiptNo;
            UniqueReceiptNo = UniqueReceiptNo + 1;
            //comment on 8 march
            //if (PaymentDue < delayMinAmount)
            //{
            //    taxableDelayAmount = 0;
            //}
            //else
            //{
            //    AreaBL ObjAreaBL = new AreaBL();
            //    string date1 = BillDate;
            //    string date = DateTime.Now.ToString("dd/MM/yyyy");

            //    var dateFrom = DateTime.ParseExact(date1, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //    var dateTo = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //    decimal delayCharge = calculateDelayCharges(dateFrom, dateTo, delayDays, Convert.ToDecimal(CLatePaymentFee));
            //    decimal gst = calculateTaxOnDelayAmount(delayCharge);
            //    if (bLateFee == 0)
            //    {
            //        taxableDelayAmount = delayCharge + gst;
            //        // update late fee to billing table
            //        billletfee = taxableDelayAmount;
            //    }
            //    else
            //    {
            //        taxableDelayAmount = (delayCharge + gst) - bLateFee;
            //        if ((delayCharge + gst) > bLateFee)
            //        {
            //            // update late fee to billing table
            //            billletfee = taxableDelayAmount + bLateFee;
            //        }
            //    }
            //}
            //new change for letfree 8 march2021 
            decimal delayCharge = PreviousLateFree;
            decimal gst = calculateTaxOnDelayAmount(delayCharge);
            taxableDelayAmount = delayCharge + gst;
            // update closing balance to bean with taxable amount
            ClosingBalance = bClosingBalance + taxableDelayAmount;
            decimal totalBillingAmount = taxableDelayAmount + PaymentDue;
            //comment on 8 march2021
            // update payment due to bean with taxable amount
            //PaymentDue = PaymentDue + taxableDelayAmount;
            Total = totalBillingAmount;
            //comment on 8 march2021
            //  var author = new Tuple<decimal, decimal, decimal, decimal, decimal, Int32, Int32>(
            //taxableDelayAmount, billletfee, ClosingBalance, Total, PaymentDue, UniqueReceiptNo, Unumber);
            //  return author;
            //new change for letfree 8 march
            var author = new Tuple<decimal, decimal, decimal, decimal, decimal, Int32, Int32>(
           taxableDelayAmount, delayCharge, ClosingBalance, Total, PaymentDue, UniqueReceiptNo, Unumber);
            return author;
        }

        public ChangPwDataModel EmailCheck(ChangPwDataModel forgotPasswordDataModel)
        {
            ChangPwDataModel ChangPwDataModel = obj.EmailCheck(forgotPasswordDataModel);
            return ChangPwDataModel;
        }

        public string changePwd(ChangPwDataModel changepwd)
        {
            string res = obj.changePwd(changepwd);
            return res;
        }
        public string UpdateEmail(ChangPwDataModel changepwd)
        {
            string res = obj.UpdateEmail(changepwd);
            return res;
        }

        public CustomerLoginDataModel searchCustomer(CustomerLoginDataModel customerLogin)
        {
            return obj.searchCustomer(customerLogin);
        }
        public string GetPassword(string email)
        {
            return obj.GetPassword(email);
        }
        public CustLoginDataModel OTPCheck(CustLoginDataModel objCust)
        {
            return obj.OTPCheck(objCust);
        }


        public CustomerBillInformationData GetBillById(string BillId)
        {
            return obj.GetBillById(BillId);
        }

        public CompaniesPaymentModel GetCompaniesPaymentSetup(string CustomerId)
        {
            CompaniesPaymentModel Company = new CompaniesPaymentModel();
            return Company = obj.GetCompaniesPaymentSetup(CustomerId);
        }

        public string GetPasswordID(string CustomerId)
        {
            return obj.GetPasswordId(CustomerId);
        }


        public CustLoginDataModel CustomerLogin(CustLoginDataModel objCust)
        {
            return obj.CustomerLogin(objCust);
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
        public string GetEncrypted(string p_PlainText)
        {
            string l_GetEncrypted = string.Empty;
            if (p_PlainText.Trim().Length == 0) { return l_GetEncrypted; }
            BMSecurity3DES objCrypto = new BMSecurity3DES();
            try
            { l_GetEncrypted = objCrypto.Encrypt_(p_PlainText); }
            catch (System.Exception ex) { throw new Exception("-GE: " + ex.Message); }
            finally { objCrypto.Dispose(); objCrypto = null; }
            return l_GetEncrypted;
        }

        public string GenratePass()
        {
            var chars = ConfigurationManager.AppSettings["ForgotKey"].ToString();
            var stringChars = new char[6];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            String ReferralCode = new String(stringChars);
            return ReferralCode;

        }


        public string SendEMailSms(string Id, string CustomerName, string contact, string Emailid, string EmailMsg, string SMSMsg, string Type, string password)
        {
            dynamic sendemail;
            dynamic sendsms;
            string res;
            ChangPwDataModel changep = new ChangPwDataModel();
            changep.Id = Id;
            //changep.Password = GenratePass();
            changep.Password = password;
            changep.EmailId = Emailid;
            //Parallel.Invoke(() => sendemail = Email.SendMailtoUser(Emailid, "Welcome to NMC", EmailMsg), () => sendsms = Email.SendSMStoUser(contact, SMSMsg));
            changep.Password = GetEncrypted(changep.Password);

            if (Type == "API")
            {
                res = obj.UpdateEmail(changep);
            }
            else
            {
                res = obj.changePwd(changep);
            }

            return res;

        }
        public void SendSmsEmail(string Id, string CustomerName, string contact, string Emailid, string EmailMsg, string SMSMsg, string Type, string password)
        {
            dynamic sendemail;
            dynamic sendsms;
         
     
                Parallel.Invoke(() => sendemail = Email.SendMailtoUser(Emailid, "Welcome to NMC", EmailMsg), () => sendsms = Email.SendSMStoUser(contact, SMSMsg));
          
        }
        

        public DataSet GetCustomerDeatils(string CustomerId, string GodownId, string CompanyId)
        {
            DataSet ds = new DataSet();
            return ds = obj.GetCustomerDeatils(CustomerId, GodownId, CompanyId);
        }






        public string Insertbill(BillInformationData objBillInformationData, DateTime DueDate, DateTime PreviousBillDate, string MeterRedgImage)
        {

            return obj.Insertbill(objBillInformationData, DueDate, PreviousBillDate, MeterRedgImage);

        }
    }
}
