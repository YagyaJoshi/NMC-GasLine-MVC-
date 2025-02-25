using NMCDataAccesslayer.DataModel;
using NMCPipedGasLineAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NMCDataAccesslayer.BL;
using AutoMapper;
using System.Linq.Dynamic;
using NMCPipedGasLineAPI.Properties;

namespace NMCPipedGasLineAPI.Controllers
{
    public class StockItemController : Controller
    {
        StockItemMaster objStock = new StockItemMaster();
        StockItemMasterDataModel objDM = new StockItemMasterDataModel();
        StockItemBL objbl = new StockItemBL();
        AreaBL ObjAreaBL = new AreaBL();
        NMCDataAccesslayer.BL.CompanyBL ObjBL = new NMCDataAccesslayer.BL.CompanyBL();

        // GET: StockItem
        public ActionResult Index()
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                objDM.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<StockItemMaster, StockItemMasterDataModel>();
                });
                objStock = Mapper.Map<StockItemMasterDataModel, StockItemMaster>(objDM);
                if (Session["RoleName"].ToString() != "Super Admin")
                {
                    objStock.CompanyId = objDM.Company.ToList().FirstOrDefault().Id.ToString();
                }
                objStock.RateDate =  DateTime.Now;
                objStock.ToDate = DateTime.Now;

                return View(objStock);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public ActionResult Index(StockItemMaster objStock)
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        AutoMapper.Mapper.Reset();

                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<StockItemMasterDataModel, StockItemMaster>();
                        });

                        objDM = Mapper.Map<StockItemMaster, StockItemMasterDataModel>(objStock);

                        // objDM.UserId = user.Id;i
                        if (objStock.StockItemId == null)
                        {

                            objbl.Save(objDM, Session["user"].ToString());
                            TempData["Msg"] = Resource1.SaveSuccess;
                        }
                        else
                        {
                            objbl.Update(objDM, Session["user"].ToString());
                            TempData["Msg"] = Resource1.UpdateSuccess;
                        }



                        return RedirectToAction("List");
                    }
                    else
                    {
                        objDM.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                        AutoMapper.Mapper.Reset();
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<StockItemMaster, StockItemMasterDataModel>();
                        });
                        objStock = Mapper.Map<StockItemMasterDataModel, StockItemMaster>(objDM);

                        string messages = string.Join(Environment.NewLine, ModelState.Values
                                  .SelectMany(x => x.Errors)
                                  .Select(x => x.Exception));

                        ViewBag.Error = messages;
                        return View(objStock);


                    }
                }
                catch (Exception ex)
                {
                    objDM.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<StockItemMaster, StockItemMasterDataModel>();
                    });
                    objStock = Mapper.Map<StockItemMasterDataModel, StockItemMaster>(objDM);

                    ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                    return View(objStock);

                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }

        public ActionResult List()
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                try
                {
                    if (TempData["Msg"] != null)
                    {
                        ViewBag.Error = TempData["Msg"].ToString();
                    }

                    List<StockItemMaster> objLStock = new List<StockItemMaster>();
                    List<StockItemMasterDataModel> Stockdata = new List<StockItemMasterDataModel>();
                    //  Stockdata = objbl.GetAllStock();
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<StockItemMasterDataModel, StockItemMaster>();
                    });
                    objLStock = Mapper.Map<List<StockItemMasterDataModel>, List<StockItemMaster>>(Stockdata);

                    return View(objLStock);
                }
                catch (Exception ex)
                {
                    ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                    return RedirectToAction("error", "Area", new { Error = ex.Message });
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpGet]
        public ActionResult Edit(string StockItemId)
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                try
                {
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<StockItemMasterDataModel, StockItemMaster>();
                    });

                    objDM = objbl.GetStockById(StockItemId);
                    objDM.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                    objStock = Mapper.Map<StockItemMasterDataModel, StockItemMaster>(objDM);
                    //objStock.RateDate = DateTime.Now;
                    return View("Index", objStock);
                }
                catch (Exception ex)
                {
                    ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                    return RedirectToAction("error", "Area", new { Error = ex.Message });
                }
            }

            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpGet]
        public ActionResult Add(string StockItemId)
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                try
                {
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<StockItemMasterDataModel, StockItemMaster>();
                    });

                    objDM = objbl.GetStockById(StockItemId);
                    objDM.IsShow = 1;
                    objDM.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                    objStock = Mapper.Map<StockItemMasterDataModel, StockItemMaster>(objDM);
                    //objStock.RateDate = DateTime.Now;
                    return View("Index", objStock);
                }
                catch (Exception ex)
                {
                    ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                    return RedirectToAction("error", "Area", new { Error = ex.Message });
                }
            }

            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        [HttpGet]
        public ActionResult Delete(string StockItemId)
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                try
                {
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<AreaDataModel, AreaMaster>();
                });
                    objbl.DeleteStock(StockItemId, 0);
                    AutoMapper.Mapper.Reset();
                    TempData["Msg"] = Resource1.StockItemInactive;
                    return RedirectToAction("List");
                }
                catch (Exception ex)
                {
                    ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                    return RedirectToAction("error", "Area", new { Error = ex.Message });
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public JsonResult AjaxMethod(int pageIndex)
        {
            StockItemMaster model = new StockItemMaster();
            try
            {
                List<StockItemMaster> objLStock = new List<StockItemMaster>();
                List<StockItemMasterDataModel> Stockdata = new List<StockItemMasterDataModel>();
                //Stockdata = objbl.GetAllStock();
                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<StockItemMasterDataModel, StockItemMaster>();
                });
                objLStock = Mapper.Map<List<StockItemMasterDataModel>, List<StockItemMaster>>(Stockdata);

                model.PageIndex = pageIndex;
                model.PageSize = 15;
                model.RecordCount = objLStock.Count();
                int startIndex = (pageIndex - 1) * model.PageSize;
                model.StockItemMasterList = objLStock
                                .OrderBy(customer => customer.StockItemId)
                                .Skip(startIndex)
                                .Take(model.PageSize).ToList();
                return Json(model);
            }
            catch (Exception ex)
            {
                ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                model.PageIndex = 402;
                model.Value = ex.Message;
                return Json(model);
            }
        }


        [HttpPost]
        public ActionResult LoadData()
        {
            //Get parameters
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                // get Start (paging start index) and length (page size for paging)
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                //Get Sort columns value
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                string search = Request.Params["search[value]"];
                if (sortColumn == "StockItemName" && sortColumnDir == "asc") sortColumn = "";
                //string search = Request.Params["search[value]"];
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int totalRecords = 0;


                var sEcho1 = Convert.ToInt32((Convert.ToInt32(start) + Convert.ToInt32(length)) / Convert.ToInt32(length));
                int pageNo = sEcho1; //!string.IsNullOrEmpty(search) ? 1 : sEcho1;
                List<StockItemMaster> objLStock = new List<StockItemMaster>();
                List<StockItemMasterDataModel> Stockdata = new List<StockItemMasterDataModel>();

                Stockdata = objbl.GetAllStock(pageNo, pageSize, search, sortColumn, sortColumnDir, Session["City"].ToString(), Session["RoleName"].ToString());
                if (Stockdata.Count() > 0)
                {
                    totalRecords = Stockdata.LastOrDefault().TotalRows;
                }
                var data = Stockdata;

                return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        [HttpPost]
        public ActionResult StockItemHistory(string StockItemId)
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                //Get parameters
                StockItemId = TempData["StockItemId"].ToString();
                TempData.Keep("StockItemId");
                // get Start (paging start index) and length (page size for paging)
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                //Get Sort columns value
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                string search = Request.Params["search[value]"];
                if (sortColumn == "Rate" && sortColumnDir == "asc") sortColumn = "";
                //string search = Request.Params["search[value]"];
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int totalRecords = 0;


                var sEcho1 = Convert.ToInt32((Convert.ToInt32(start) + Convert.ToInt32(length)) / Convert.ToInt32(length));
                int pageNo = sEcho1; //!string.IsNullOrEmpty(search) ? 1 : sEcho1;
                List<StockItemGasRate> objLStock = new List<StockItemGasRate>();
                List<StockItemGasRateDataModel> Stockdata = new List<StockItemGasRateDataModel>();

                Stockdata = objbl.GetAllStockItemRate(pageNo, pageSize, search, sortColumn, sortColumnDir, Session["City"].ToString(), Session["RoleName"].ToString(), StockItemId);
                if (Stockdata.Count() > 0)
                {
                    totalRecords = Stockdata.LastOrDefault().TotalRows;
                }
                var data = Stockdata;

                return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        public ActionResult HistoryList(string StockItemId)
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                TempData["StockItemId"] = StockItemId;
                try
                {
                    List<StockItemGasRate> objLStock = new List<StockItemGasRate>();
                    List<StockItemGasRateDataModel> Stockdata = new List<StockItemGasRateDataModel>();
                    //Stockdata = objbl.GetAllStock();
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<StockItemGasRateDataModel, StockItemGasRate>();
                    });
                    objLStock = Mapper.Map<List<StockItemGasRateDataModel>, List<StockItemGasRate>>(Stockdata);

                    return View(objLStock);
                }
                catch (Exception ex)
                {
                    ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                    return RedirectToAction("error", "Area", new { Error = ex.Message });
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
    }
}