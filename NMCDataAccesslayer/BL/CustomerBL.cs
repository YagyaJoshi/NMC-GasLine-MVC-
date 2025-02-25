using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NMCDataAccesslayer.DAL;
using System.Threading.Tasks;
using NMCDataAccesslayer.DataModel;
using System.Data;

namespace NMCDataAccesslayer.BL
{
    public class CustomerBL
    {
        CustomerDAL objDl;

        public DataTable Save(List<ImportCustomerData> obj, string CompanyId, string AdminId,string GodownId)
        {
            objDl = new CustomerDAL();
            DataTable dt= objDl.Save(obj, CompanyId, AdminId, GodownId);
            return dt;
            
        }
        public DataTable PaymentSave(DataTable tdt)
        {
            objDl = new CustomerDAL();
            DataTable dt = objDl.PaymentSave(tdt);
            return dt;

        }

        public void InsertEmployeeFile(string UserId, string Type, string BillFileName, string PaymentFileName, string FileName)
        {
            objDl = new CustomerDAL();
            objDl.InsertEmployeeFile(UserId, Type, BillFileName, PaymentFileName, FileName);
        }

        public async Task<List<CustomerDataModel>> GetCustomerAreawise(String AreaId)
        {
            objDl = new CustomerDAL();
            return await objDl.GetCustomerAreawise(AreaId);
        }

        public List<CustomerBillInformationData> GetAllBill(string CompanyId,string GodownId,string AreaId,string BillTypeId)
        {
            objDl = new CustomerDAL();
            List<CustomerBillInformationData> Bill = new List<CustomerBillInformationData>();
            return Bill = objDl.GetBill(CompanyId, GodownId, AreaId, BillTypeId);
        }

        public List<CustomerBillInformationData> GetBillList(string CompanyId, string GodownId, string AreaId, string BillTypeId)
        {
            objDl = new CustomerDAL();
            List<CustomerBillInformationData> Bill = new List<CustomerBillInformationData>();
            return Bill = objDl.GetAllBill(CompanyId, GodownId, AreaId, BillTypeId);
        }

        

        public List<ExportCustomerData> GetCustomerForExport(string CompanyId)
        {
            objDl = new CustomerDAL();
            List<ExportCustomerData> Bill = new List<ExportCustomerData>();
            return Bill = objDl.GetCustomerForExport(CompanyId);
        }
        
        public string SaveCustomerByAdmin(CustomerDataModel ObjcustomerData, string userId)
        {
            objDl = new CustomerDAL();
            string cust = objDl.SaveCustomerByAdmin(ObjcustomerData, userId);
            return cust;
        }

        public string SaveBillCustomer(CustomerDataModel ObjcustomerData, string userId)
        {
            objDl = new CustomerDAL();
            string cust = objDl.SaveBillCustomer(ObjcustomerData, userId);
            return cust;
        }


        public string UpdateCustomerByAdmin(CustomerDataModel ObjcustomerData, string userId)
        {
            objDl = new CustomerDAL();
            string cust = objDl.UpdateCustomerByAdmin(ObjcustomerData, userId);
            return cust;
        }


        public CustomerDataModel GetCustomerById(string CustomerId, string BillType)
        {
            objDl = new CustomerDAL();
            return objDl.GetCustomerById(CustomerId, BillType);
        }

        public List<CustomerDataModel> GetAllCustomer(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey, string CityId, string AdminId, CustomerList cust)
        {
            objDl = new CustomerDAL();
            return objDl.GetAllCustomer(PageNo, PageSize, q, sortby, sortkey, CityId,AdminId, cust);
        }

        public List<CustomerDataModel> GetAllCustomerforMail(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey, string CityId, string AdminId, CustomerList cust)
        {
            objDl = new CustomerDAL();
            return objDl.GetAllCustomerforMail(PageNo, PageSize, q, sortby, sortkey, CityId, AdminId, cust);
        }

        public void DeleteCustomer(string CustomerId, Int16 IsActive)
        {
            objDl = new CustomerDAL();
            objDl.DeleteCustomer(CustomerId, IsActive);
        }

        public string GetGodownName(string GodownId)
        {
            objDl = new CustomerDAL();
            return  objDl.GetGodownName(GodownId);
        }


        public string GetCustomerunique(string GodownId)
        {
            objDl = new CustomerDAL();
            return objDl.GetCustomerunique(GodownId);
        }


        public List<ExportCustomerData> GetCustomerForExportReceipt(string CompanyId)
        {
            objDl = new CustomerDAL();
            List<ExportCustomerData> Bill = new List<ExportCustomerData>();
            return Bill = objDl.GetCustomerForExportReceipt(CompanyId);
        }

        public List<ExportWithoutBillData> GetPaymentsWithoutBill(string CompanyId)
        {
            objDl = new CustomerDAL();
            List<ExportWithoutBillData> Payment = new List<ExportWithoutBillData>();
            return Payment = objDl.GetPaymentsWithoutBill(CompanyId);

        }

        public DataTable GetSocietyWiseCustomer(string CompanyId, string GodownId, string AreaId)
        {
            objDl = new CustomerDAL();
            return  objDl.GetSocietyWiseCustomer(CompanyId, GodownId, AreaId);

        }
        public CustomerDataModel GetMaxBillDate(string GodownId)
        {
            objDl = new CustomerDAL();
            return objDl.GetMaxBillDate(GodownId);
        }
        public List<ExportCustomerWithoutBill> GetCustomerList(string GodownId)
        {
            objDl = new CustomerDAL();
            List<ExportCustomerWithoutBill> list = new List<ExportCustomerWithoutBill>();
            return list = objDl.GetCustomerList(GodownId);
        }


        public CustomerBillData GetCustomerBillData(string CustomerId)
        {
            objDl = new CustomerDAL();
           
            return  objDl.GetCustomerBillData(CustomerId);
        }
        public string SavePayment(CustomerPaymentData objModel)
        {
            string result;
            objDl = new CustomerDAL();
            return result = objDl.SavePayment(objModel);
        }
        

        public DataTable GetCustomerByIdForMail(string CustomerId)
        {
            objDl = new CustomerDAL();
            return objDl.GetCustomerByIdForMail(CustomerId);

        }

        public void EditEmail(string Email, string CustomerId,string Password)
        {
            objDl = new CustomerDAL();
            objDl.EditEmail(Email, CustomerId, Password);           
        }


    }
}

