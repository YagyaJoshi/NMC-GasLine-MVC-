using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NMCDataAccesslayer.BL;
using NMCDataAccesslayer.DataModel;
using AutoMapper;
using NMCPipedGasLineAPI.Models;
using System.Linq.Dynamic;
using NMCPipedGasLineAPI.Properties;

namespace NMCPipedGasLineAPI.Controllers
{
    public class AreaController : Controller
    {
        string result;
        AreaMaster objArea = new AreaMaster();
        AreaDataModel objDM = new AreaDataModel();
        UserBL objUBL = new UserBL();
        CompanyBL ObjBL = new CompanyBL();
        AreaBL ObjAreaBL = new AreaBL();
        // GET: Company
        public ActionResult Index()
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {

                objDM.Company = ObjBL.GetCompany(Convert.ToString(Session["RoleName"]), Convert.ToString(Session["City"]));
                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                   {
                       cfg.CreateMap<AreaMaster, AreaDataModel>();
                   });
                objArea = Mapper.Map<AreaDataModel, AreaMaster>(objDM);
                if (Convert.ToString(Session["RoleName"]) != "Super Admin")
                {
                    objArea.CompanyId = objDM.Company.ToList().FirstOrDefault().Id.ToString();
                }
                return View(objArea);
            }
            else
            { return RedirectToAction("Login", "Account"); }
        }

        [HttpPost]
        public ActionResult Index(AreaMaster objArea)
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
                            cfg.CreateMap<AreaDataModel, AreaMaster>();
                        });

                        objDM = Mapper.Map<AreaMaster, AreaDataModel>(objArea);

                        // objDM.UserId = user.Id;i
                        if (string.IsNullOrEmpty(objArea.AreaId))
                        {
                            objDM.AdminId = Convert.ToString(Session["user"]);
                            result = ObjAreaBL.Save(objDM);
                            TempData["Msg"] = Resource1.SaveSuccess;

                        }
                        else
                        {
                            result = ObjAreaBL.Update(objDM);
                            TempData["Msg"] = Resource1.UpdateSuccess;

                        }

                        if (result != "1")
                        {
                            AutoMapper.Mapper.Reset();
                            objDM.Company = ObjBL.GetCompany(Convert.ToString(Session["RoleName"]), Convert.ToString(Session["City"]));
                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<AreaMaster, AreaDataModel>();
                            });

                            objArea = Mapper.Map<AreaDataModel, AreaMaster>(objDM);

                            //objArea = Mapper.Map<AreaDataModel, AreaMaster>(objDM);
                            ViewBag.Error = Resource1.AreaAlreadyExist;
                            return View(objArea);
                        }
                        else
                        {
                            return RedirectToAction("List");
                        }

                    }
                    else
                    {
                        AutoMapper.Mapper.Reset();
                        objDM.Company = ObjBL.GetCompany(Convert.ToString(Session["RoleName"]), Convert.ToString(Session["City"]));
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<AreaMaster, AreaDataModel>();
                        });

                        objArea = Mapper.Map<AreaDataModel, AreaMaster>(objDM);



                        string messages = string.Join(Environment.NewLine, ModelState.Values
                                    .SelectMany(x => x.Errors)
                                    .Select(x => x.Exception));

                        ViewBag.Error = messages;
                        return View(objArea);

                    }
                }
                catch (Exception ex)
                {

                    ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Convert.ToString(Session["user"]));
                    objDM.Company = ObjBL.GetCompany(Convert.ToString(Session["RoleName"]), Convert.ToString(Session["City"]));
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<AreaMaster, AreaDataModel>();
                    });
                    objArea = Mapper.Map<AreaDataModel, AreaMaster>(objDM);

                    ViewBag.Error = ex.Message;
                    return View(objArea);
                }
            }
            else
            { return RedirectToAction("Login", "Account"); }

        }

        public ActionResult List()
        {
            if (TempData["Msg"] != null)
            {
                ViewBag.Error = TempData["Msg"].ToString();
            }
            List<AreaMaster> objLArea = new List<AreaMaster>();
            if (Session["user"] != null)
            {
                try
                {

                    List<AreaDataModel> Areadata = new List<AreaDataModel>();
                    //Areadata = ObjAreaBL.GetAllArea();
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<AreaDataModel, AreaMaster>();
                    });
                    objLArea = Mapper.Map<List<AreaDataModel>, List<AreaMaster>>(Areadata);

                    return View(objLArea);
                }
                catch (Exception ex)
                {
                    ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                    ViewBag.Error = ex.Message;
                    return RedirectToAction("error", "Area", new { Error = ex.Message });
                }

            }
            else
            { return RedirectToAction("Login", "Account"); }
        }

        [HttpPost]
        public JsonResult AjaxMethod(int pageIndex)
        {
            AreaMaster model = new AreaMaster();
            try
            {
                List<AreaMaster> objLArea = new List<AreaMaster>();
                List<AreaDataModel> Areadata = new List<AreaDataModel>();
                // Areadata = ObjAreaBL.GetAllArea();
                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<AreaDataModel, AreaMaster>();
                });
                objLArea = Mapper.Map<List<AreaDataModel>, List<AreaMaster>>(Areadata);

                //NorthwindEntities entities = new NorthwindEntities();
                //CustomerModel model = new CustomerModel();
                model.PageIndex = pageIndex;
                model.PageSize = 15;
                model.RecordCount = objLArea.Count();
                int startIndex = (pageIndex - 1) * model.PageSize;
                model.AreaMasterList = objLArea
                                .OrderBy(customer => customer.AreaId)
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


        [HttpGet]
        public ActionResult Edit(string AreaId)
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
                    objDM = ObjAreaBL.GetAreaById(AreaId);
                    objDM.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                    objArea = Mapper.Map<AreaDataModel, AreaMaster>(objDM);

                    return View("Index", objArea);
                }
                catch (Exception ex)
                {
                    ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                    ViewBag.Error = ex.Message;
                    return RedirectToAction("error", "Area", new { Error = ex.Message });
                }


            }
            else
            { return RedirectToAction("Login", "Account"); }
        }

        [HttpGet]
        public ActionResult Delete(string AreaId)
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                try
                {
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg => { cfg.CreateMap<AreaDataModel, AreaMaster>(); });
                    ObjAreaBL.DeleteArea(AreaId, 0);
                    TempData["Msg"] = Resource1.AresInactive;
                    return RedirectToAction("List");
                }
                catch (Exception ex)
                {
                    ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                    ViewBag.Error = ex.Message;
                    return RedirectToAction("error", "Area", new { Error = ex.Message });
                }

            }
            else
            { return RedirectToAction("Login", "Account"); }
        }

        public ActionResult error(string Error)
        {
            ViewBag.Error = Error;
            return View("error");
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
                //if (sortColumn == "AreaName" && sortColumnDir == "asc") sortColumn = "";
                //string search = Request.Params["search[value]"];
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int totalRecords = 0;
                var sEcho1 = Convert.ToInt32((Convert.ToInt32(start) + Convert.ToInt32(length)) / Convert.ToInt32(length));
                int pageNo = sEcho1;   //!string.IsNullOrEmpty(search) ? 1 : sEcho1;
                List<AreaMaster> objLArea = new List<AreaMaster>();
                List<AreaDataModel> Areadata = new List<AreaDataModel>();
                Areadata = ObjAreaBL.GetAllArea(pageNo, pageSize, search, sortColumn, sortColumnDir, Session["City"].ToString(), Session["RoleName"].ToString());
                if (Areadata.Count() > 0)
                {
                    totalRecords = Areadata.LastOrDefault().TotalRows;
                }
                var data = Areadata;
                return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);

            }
            else
            { return RedirectToAction("Login", "Account"); }
        }
    }
}