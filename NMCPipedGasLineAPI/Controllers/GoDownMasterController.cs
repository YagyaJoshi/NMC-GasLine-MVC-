using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NMCDataAccesslayer.BL;
using NMCDataAccesslayer.DataModel;
using AutoMapper;
using NMCPipedGasLineAPI.Models;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Linq.Dynamic;
using NMCPipedGasLineAPI.Properties;

namespace NMCPipedGasLineAPI.Controllers
{
    public class GoDownMasterController : Controller
    {
        GodownMaster objGodown = new GodownMaster();
        GodownDataModel objDM = new GodownDataModel();
        CompanyBL ObjBL = new CompanyBL();
        UserBL ObjCBL = new UserBL();
        AreaBL ObjAreaBL = new AreaBL();
        GodownBL ObjGodownBL = new GodownBL();
        string result;
        // GET: Company
        public ActionResult Index()
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                //objDM.Area = ObjAreaBL.GetArea(1);
                objDM.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<GodownMaster, GodownDataModel>();
                            });
                objGodown = Mapper.Map<GodownDataModel, GodownMaster>(objDM);
                if (Session["RoleName"].ToString() != "Super Admin")
                {
                    objGodown.CompanyId = objDM.Company.ToList().FirstOrDefault().Id.ToString();
                    objGodown.CompanyName = objDM.Company.ToList().FirstOrDefault().CompanyName.ToString();
                }
                return View(objGodown);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public ActionResult Index(GodownMaster objGodown)
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
                              cfg.CreateMap<GodownDataModel, GodownMaster>();
                          });

                        objDM = Mapper.Map<GodownMaster, GodownDataModel>(objGodown);

                        // objDM.UserId = user.Id;i
                        if (objGodown.Id == null)
                        {

                            //var str = String.Join(",", objGodown.AreaId);
                            objDM.AdminId = Session["user"].ToString();
                            result = ObjGodownBL.Save(objDM);
                            if (result != "1")
                            {
                                objDM.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                                AutoMapper.Mapper.Reset();
                                Mapper.Initialize(cfg =>
                                {
                                    cfg.CreateMap<GodownMaster, GodownDataModel>();
                                });
                                objDM.Area = ObjAreaBL.GetArea(objGodown.CompanyId);
                                objGodown = Mapper.Map<GodownDataModel, GodownMaster>(objDM);
                                ViewBag.Error = result;

                                return View(objGodown);
                            }
                            TempData["Msg"] = Resource1.SaveSuccess;

                        }
                        else
                        {
                            result = ObjGodownBL.Update(objDM);
                            if (result != "1")
                            {
                                objDM.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                                AutoMapper.Mapper.Reset();
                                Mapper.Initialize(cfg =>
                                {
                                    cfg.CreateMap<GodownMaster, GodownDataModel>();
                                });
                                objDM.Area = ObjAreaBL.GetArea(objGodown.CompanyId);
                                objGodown = Mapper.Map<GodownDataModel, GodownMaster>(objDM);
                                ViewBag.Error = result;
                                return View(objGodown);
                            }
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
                            cfg.CreateMap<GodownMaster, GodownDataModel>();
                        });
                        objDM.Area = ObjAreaBL.GetArea(string.IsNullOrEmpty(objGodown.CompanyId) ? "" : objGodown.CompanyId);
                        GodownMaster objGodown1 = Mapper.Map<GodownDataModel, GodownMaster>(objDM);
                        objGodown1.AreaId = objGodown.AreaId;
                        if (objGodown.UAreadId != null)
                        {
                            objGodown1.UAreadId = objGodown.UAreadId;
                        }

                        string messages = string.Join(Environment.NewLine, ModelState.Values
                               .SelectMany(x => x.Errors)
                               .Select(x => x.Exception));

                        ViewBag.Error = messages;
                        return View(objGodown1);

                    }

                }
                catch (Exception ex)
                {
                    ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                    objDM.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<GodownMaster, GodownDataModel>();
                    });
                    objDM.Area = ObjAreaBL.GetArea(string.IsNullOrEmpty(objGodown.CompanyId) ? "" : objGodown.CompanyId);
                    objGodown = Mapper.Map<GodownDataModel, GodownMaster>(objDM);
                    ViewBag.Error = ex.Message;
                    return View(objGodown);
                }

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult List()
        {          
            if (TempData["Msg"] != null)
            {
                ViewBag.Error = TempData["Msg"].ToString();
            }
            if (Session["user"] != null)
            {
                try
                {
                    List<GodownMaster> objLGodown = new List<GodownMaster>();
                    List<GodownDataModel> Godowndata = new List<GodownDataModel>();
                    //Godowndata = ObjGodownBL.GetAllGodown();
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<GodownDataModel, GodownMaster>();
                    });
                    objLGodown = Mapper.Map<List<GodownDataModel>, List<GodownMaster>>(Godowndata);

                    return View(objLGodown);
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
        public ActionResult Edit(string GodownId)
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                try
                {
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<GodownDataModel, GodownMaster>();
                });
                    objDM = ObjGodownBL.GetGodownById(GodownId);
                    objDM.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                    objDM.Area = ObjAreaBL.GetArea(objDM.CompanyId);
                    objGodown = Mapper.Map<GodownDataModel, GodownMaster>(objDM);

                    return View("Index", objGodown);
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
        public ActionResult Delete(string GodownId)
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                try
                {
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<GodownDataModel, GodownMaster>();
                });
                    ObjGodownBL.DeleteGodown(GodownId, 0);
                    TempData["Msg"] = Resource1.GoDownInactive;
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
            GodownMaster model = new GodownMaster();
            try
            {
                List<GodownMaster> objLGodown = new List<GodownMaster>();
                List<GodownDataModel> Godowndata = new List<GodownDataModel>();
                //Godowndata = ObjGodownBL.GetAllGodown();
                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<GodownDataModel, GodownMaster>();
                });
                objLGodown = Mapper.Map<List<GodownDataModel>, List<GodownMaster>>(Godowndata);



                //NorthwindEntities entities = new NorthwindEntities();

                //CustomerModel model = new CustomerModel();
                model.PageIndex = pageIndex;
                model.PageSize = 15;
                model.RecordCount = objLGodown.Count();
                int startIndex = (pageIndex - 1) * model.PageSize;
                model.GodownMasterList = objLGodown
                                .OrderBy(customer => customer.Id)
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

                //string search = Request.Params["search[value]"];
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int totalRecords = 0;

                var sEcho1 = Convert.ToInt32((Convert.ToInt32(start) + Convert.ToInt32(length)) / Convert.ToInt32(length));
                int pageNo = sEcho1;//!string.IsNullOrEmpty(search) ? 1 : sEcho1;
                List<GodownMaster> objLGodown = new List<GodownMaster>();
                List<GodownDataModel> Godowndata = new List<GodownDataModel>();

                Godowndata = ObjGodownBL.GetAllGodown(pageNo, pageSize, search, sortColumn, sortColumnDir, Session["City"].ToString(), Session["RoleName"].ToString());
                if (Godowndata.Count() > 0)
                {
                    totalRecords = Godowndata.LastOrDefault().TotalRows;
                }
                var data = Godowndata;
                //AutoMapper.Mapper.Reset();
                //Mapper.Initialize(cfg =>
                //{
                //    cfg.CreateMap<GodownDataModel, GodownMaster>();
                //});
                //objLGodown = Mapper.Map<List<GodownDataModel>, List<GodownMaster>>(Godowndata);

                //var v = (from a in Godowndata select a);
                ////Sorting
                //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                //{
                //    v = v.OrderBy(sortColumn + " " + sortColumnDir);
                //}

                //if (!string.IsNullOrEmpty(search))
                //{
                //    v = v.Where(c => c.CompanyName.ToLower().Contains(search.ToLower())
                //                         ||
                //              c.Name.ToLower().Contains(search.ToLower())
                //                       ||
                //             Convert.ToString(c.InputRate).Contains(search.ToLower())
                //                         ||
                //                         Convert.ToString(c.NewServiceCharge).Contains(search.ToLower())
                //                           );
                //}
                //totalRecords = v.Count();
                //var data = v.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpGet]
        public ActionResult DeleteSociety(string godownId)
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                try
                {
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<GodownDataModel, GodownMaster>();
                    });
                    objGodown.Id = godownId;
                    return PartialView("_DeleteSociety", objGodown);
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
        public ActionResult DeleteSociety(string godownId, string password)
        {
            var msg = "";
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                try
                {
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<GodownDataModel, GodownMaster>();
                    });
                    if (password == "Admin@123")
                    {
                        ObjGodownBL.DeleteGodownSociety(godownId);
                        msg = Resource1.DeleteSociety;
                    }
                    else
                    {
                        msg = "Incorrect Password";
                    }
                    return Json(new { test = msg }, JsonRequestBehavior.AllowGet);              
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