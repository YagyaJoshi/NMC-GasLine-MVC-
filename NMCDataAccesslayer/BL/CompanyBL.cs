using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NMCDataAccesslayer.DAL;
using NMCDataAccesslayer.DataModel;

namespace NMCDataAccesslayer.BL
{
   
    public class CompanyBL
    {
        string result;
        CompanyDAL obj = new CompanyDAL();
        public List<CompanyDataModel> GetCompany(string adminId,string CityId)
        {
            List<CompanyDataModel> Company = new List<CompanyDataModel>();
            return Company = obj.GetCompany(adminId, CityId);
        }

        public List<BillTypeData> GetBillType()
        {
            List<BillTypeData> BillTypeData = new List<BillTypeData>();
            return BillTypeData = obj.GetBillType();
        }


        public void DeleteCompany(string CompanyId, Int16 IsActive)
        {
            obj.DeleteCompany(CompanyId, IsActive);
        }

        public string SaveCompany(CompanyDataModel objModel)
        {
            return result = obj.SaveCompany(objModel);
        }

        public string UpdateCompany(CompanyDataModel objModel)
        {
            return result = obj.UpdateCompany(objModel);
        }

        public List<CompanyDataModel> GetAllCompany(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey, string CityId, string AdminId)
        {
            List<CompanyDataModel> Company = new List<CompanyDataModel>();
            return Company = obj.GetCompany(PageNo,PageSize, q, sortby, sortkey,  CityId,  AdminId);
        }

        public List<CompaniesPaymentModel> GetAllCompaniesPaymentSetup(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey, string CityId, string AdminId)
        {
            List<CompaniesPaymentModel> Company = new List<CompaniesPaymentModel>();
            return Company = obj.GetAllCompaniesPaymentSetup(PageNo, PageSize, q, sortby, sortkey, CityId, AdminId);
        }


        public CompanyDataModel GetCompanyById(string CompanyId)
        {
            CompanyDataModel Company = new CompanyDataModel();
            return Company = obj.GetCompanyById(CompanyId);
        }

        public string InsertCompaniesPaymentSetup(CompaniesPaymentModel objModel)
        {
            return result = obj.InsertCompaniesPaymentSetup(objModel);
        }

        public string UpdateCompaniesPaymentSetup(CompaniesPaymentModel objModel)
        {
            return result = obj.UpdateCompaniesPaymentSetup(objModel);
        } 

        public CompaniesPaymentModel GetCompaniesPaymentById(string Id)
        {
            CompaniesPaymentModel Company = new CompaniesPaymentModel();
            return Company = obj.GetCompaniesPaymentById(Id);
        }
    }
}
