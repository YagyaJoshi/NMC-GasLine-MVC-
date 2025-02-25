using NMCDataAccesslayer.DAL;
using System;
using System.Collections.Generic;
using NMCDataAccesslayer.DataModel;
using System.Data;

namespace NMCDataAccesslayer.BL
{
    public class StockItemBL
    {
        StockItemDAL obj = new StockItemDAL();

        public void DeleteStock(String StockId, Int16 IsActive)
        {
            obj.DeleteStockItem(StockId, IsActive);
        }

        public void Save(StockItemMasterDataModel objModel,string AdminId)
        {
            obj.Save(objModel, AdminId);
        }

        public void Update(StockItemMasterDataModel objModel, string AdminId)
        {
            obj.Update(objModel, AdminId);
        }

        public List<StockItemMasterDataModel> GetAllStock(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey, string CityId, string AdminId)
        {
            List<StockItemMasterDataModel> Stock = new List<StockItemMasterDataModel>();
            return Stock = obj.GetAllStockItem(PageNo, PageSize,  q, sortby, sortkey, CityId, AdminId);
        }


        public List<StockItemGasRateDataModel> GetAllStockItemRate(Int32 PageNo, Int32 PageSize, string q, string sortby, string sortkey, string CityId, string AdminId,string StockItemId)
        {
            List<StockItemGasRateDataModel> Stock = new List<StockItemGasRateDataModel>();
            return Stock = obj.GetAllStockItemRate(PageNo, PageSize, q, sortby, sortkey, CityId, AdminId, StockItemId);
        }

        public StockItemMasterDataModel GetStockById(string StockId)
        {
            StockItemMasterDataModel Stock = new StockItemMasterDataModel();
            return Stock = obj.GetStockItemById(StockId);
        }

        public List<StockItemMasterDataModel> GetStockItemByCompany(string CompanyId)
        {
            List<StockItemMasterDataModel> Stock = new List<StockItemMasterDataModel>();
            return Stock = obj.GetStockItemByCompany(CompanyId);
        }

        public List<StockItemMasterDataModel> Getstockrate(string CompanyId,string StockItemId)
        {
            List<StockItemMasterDataModel> Stock = new List<StockItemMasterDataModel>();
            return Stock = obj.Getstockrate(CompanyId, StockItemId);
        }

        public void SaveImport(DataTable model)
        {
            obj.SaveImport(model);
        }

    }
}
