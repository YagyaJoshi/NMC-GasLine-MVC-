using NMCDataAccesslayer.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NMCDataAccesslayer.DataModel;

namespace NMCDataAccesslayer.BL
{
    public class BankBL
    {
        string result;
        BankDAL obj = new BankDAL();


        public void DeleteBank(string UserId, Int16 IsActive)
        {
            obj.DeleteBank(UserId, IsActive);
        }
        public string Save(BankData objModel)
        {
            return result = obj.Save(objModel);
        }

        public string Update(BankData objModel)
        {
            return result = obj.Update(objModel);
        }

        public List<BankData> GetAllbank(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey)
        {
            List<BankData> Area = new List<BankData>();
            return Area = obj.GetAllBank(PageNo, PageSize, q, sortby, sortkey);
        }

        public BankData GetbankById(string BankId)
        {
            BankData Area = new BankData();
            return Area = obj.GetBankById(BankId);
        }



        public void ErrorSave(string ErrorDescription, string ErrorScope, string Action, string AdminId)
        {
            obj.ErrorSave(ErrorDescription, ErrorScope, Action, AdminId);
        }

    }
}
