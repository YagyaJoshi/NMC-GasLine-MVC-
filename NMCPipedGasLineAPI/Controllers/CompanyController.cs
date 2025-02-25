using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NMCDataAccesslayer.BL;
using NMCDataAccesslayer.DataModel;
using AutoMapper;
using NMCPipedGasLineAPI.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq.Dynamic;
using NMCPipedGasLineAPI.Properties;

namespace NMCPipedGasLineAPI.Controllers
{
    public class CompanyController : Controller
    {
        string result;
        CompanyMaster objcompany = new CompanyMaster();
        CompanyDataModel objDM = new CompanyDataModel();
        UserBL objUBL = new UserBL();
        CompanyBL ObjBL = new CompanyBL();
        AreaBL ObjAreaBL = new AreaBL();
        ImCustomer objDIM = new ImCustomer();
        CompaniesPaymentModel objDM1 = new CompaniesPaymentModel();
        CompanyPayment company = new CompanyPayment();

        // GET: Company
        public ActionResult Index()
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                objDM.Country = objUBL.GetCountry();
                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                  {
                      cfg.CreateMap<CompanyDataModel, CompanyMaster>();
                  });
                objcompany = Mapper.Map<CompanyDataModel, CompanyMaster>(objDM);
                objcompany.CompanyName = "NMC";
                return View(objcompany);
            }
            else
            {
                Session.Abandon();
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public ActionResult Index(CompanyMaster objcompany)
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        CompanyDataModel objDM = new CompanyDataModel();
                        AutoMapper.Mapper.Reset();
                        Mapper.Initialize(cfg =>
                                       {
                                           cfg.CreateMap<CompanyDataModel, CompanyMaster>();
                                       });

                        objDM = Mapper.Map<CompanyMaster, CompanyDataModel>(objcompany);

                        // objDM.UserId = user.Id;i
                        if (objcompany.Id == null)
                        {
                            objDM.AdminId = Session["user"].ToString();
                            result = ObjBL.SaveCompany(objDM);
                            TempData["Msg"] = Resource1.SaveSuccess;
                        }
                        else
                        {
                            result = ObjBL.UpdateCompany(objDM);
                            TempData["Msg"] = Resource1.UpdateSuccess;
                        }

                        if (result != "1")
                        {
                            objDM.Country = objUBL.GetCountry();
                            objDM.State = objUBL.AllGetState();
                            objDM.City = objUBL.AllGetCity();
                            AutoMapper.Mapper.Reset();
                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<CompanyDataModel, CompanyMaster>();
                            });
                            objcompany = Mapper.Map<CompanyDataModel, CompanyMaster>(objDM);

                            ViewBag.Error = Resource1.CompanyAlreadyExist;

                            return View(objcompany);


                        }
                        else
                        {
                            return RedirectToAction("List");
                        }
                    }
                    else
                    {
                        objDM.Country = objUBL.GetCountry();
                        objDM.State = objUBL.AllGetState();
                        objDM.City = objUBL.AllGetCity();
                        AutoMapper.Mapper.Reset();
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<CompanyDataModel, CompanyMaster>();
                        });
                        objcompany = Mapper.Map<CompanyDataModel, CompanyMaster>(objDM);


                        string messages = string.Join(Environment.NewLine, ModelState.Values
                                    .SelectMany(x => x.Errors)
                                    .Select(x => x.Exception));
                        ViewBag.Error = messages;
                        return View(objcompany);

                    }
                }
                catch (Exception ex)
                {
                    ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                    objDM.Country = objUBL.GetCountry();
                    objDM.State = objUBL.AllGetState();
                    objDM.City = objUBL.AllGetCity();
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<CompanyDataModel, CompanyMaster>();
                    });
                    objcompany = Mapper.Map<CompanyDataModel, CompanyMaster>(objDM);

                    ViewBag.Error = ex.Message;
                    return View(objcompany);
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
            if (Session["user"] != null)
            {
                try
                {
                    List<CompanyMaster> objcompany = new List<CompanyMaster>();
                    List<CompanyDataModel> companydata = new List<CompanyDataModel>();
                    // companydata = ObjBL.GetAllCompany();
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<CompanyDataModel, CompanyMaster>();
                    });
                    objcompany = Mapper.Map<List<CompanyDataModel>, List<CompanyMaster>>(companydata);

                    return View(objcompany);
                }
                catch (Exception ex)
                {
                    ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                    return RedirectToAction("error", "Area", new { Error = ex.Message });
                }

            }
            else
            { return RedirectToAction("Login", "Account"); }
        }

        [HttpGet]
        public ActionResult Edit(string CompanyId)
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                try
                {
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                      {
                          cfg.CreateMap<CompanyDataModel, CompanyMaster>();
                      });
                    objDM = ObjBL.GetCompanyById(CompanyId);
                    objDM.Country = objUBL.GetCountry();
                    objDM.State = objUBL.GetState(objDM.CountryId, Session["City"].ToString());
                    objDM.City = objUBL.GetCity(objDM.StateId, Session["City"].ToString());
                    objcompany = Mapper.Map<CompanyDataModel, CompanyMaster>(objDM);

                    return View("Index", objcompany);
                }
                catch (Exception ex)
                {
                    ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                    return RedirectToAction("error", "Area", new { Error = ex.Message });
                }

            }
            else
            { return RedirectToAction("Login", "Account"); }
        }

        [HttpGet]
        public ActionResult Delete(string CompanyId)
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                try
                {
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                   {
                       cfg.CreateMap<CompanyDataModel, CompanyMaster>();
                   });
                    ObjBL.DeleteCompany(CompanyId, 0);
                    TempData["Msg"] = Resource1.CompanyInactive;
                    return RedirectToAction("List");
                }
                catch (Exception ex)
                {
                    ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                    return RedirectToAction("error", "Area", new { Error = ex.Message });
                }
            }
            else
            { return RedirectToAction("Login", "Account"); }
        }


        [HttpPost]
        public JsonResult AjaxMethod(int pageIndex)
        {
            CompanyMaster model = new CompanyMaster();
            try
            {
                List<CompanyMaster> objcompany = new List<CompanyMaster>();
                List<CompanyDataModel> companydata = new List<CompanyDataModel>();
                // companydata = ObjBL.GetAllCompany();
                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<CompanyDataModel, CompanyMaster>();
                });
                objcompany = Mapper.Map<List<CompanyDataModel>, List<CompanyMaster>>(companydata);



                //NorthwindEntities entities = new NorthwindEntities();
                //CustomerModel model = new CustomerModel();
                model.PageIndex = pageIndex;
                model.PageSize = 10;
                model.RecordCount = objcompany.Count();
                int startIndex = (pageIndex - 1) * model.PageSize;
                model.CompanyMasterList = objcompany
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
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                var reqst = System.Web.HttpContext.Current.Request.InputStream;
                System.IO.StreamReader reader = new System.IO.StreamReader(reqst);
                String xmlData = reader.ReadToEnd();
                string decode = HttpUtility.UrlDecode(Uri.UnescapeDataString(xmlData));
                //Get parameters
                // get Start (paging start index) and length (page size for paging)
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                //Get Sort columns value

                

                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                string search = Request.Params["search[value]"];
               
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int totalRecords = 0;

                var sEcho1 = Convert.ToInt32((Convert.ToInt32(start) + Convert.ToInt32(length)) / Convert.ToInt32(length));
                int pageNo = sEcho1;//!string.IsNullOrEmpty(search) ? 1 : sEcho1;
                List<CompanyMaster> objcompany = new List<CompanyMaster>();
                List<CompanyDataModel> companydata = new List<CompanyDataModel>();

                companydata = ObjBL.GetAllCompany(pageNo, pageSize, search, sortColumn, sortColumnDir, Session["City"].ToString(), Session["RoleName"].ToString());
                if (companydata.Count() > 0)
                {
                    totalRecords = companydata.LastOrDefault().TotalRows;
                }
                var data = companydata;
                return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpGet]
        public ActionResult CompaniesPaymentSetup()
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                CompaniesPaymentModel modleDATA = new CompaniesPaymentModel();
                CompanyPayment objDIM =new  CompanyPayment();
                modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<CompaniesPaymentModel, CompanyPayment>();
                });
                objDIM = Mapper.Map<CompaniesPaymentModel, CompanyPayment>(modleDATA);
                if (Session["RoleName"].ToString() != "Super Admin")
                {
                    objDIM.CompanyId = objDIM.Company.ToList().FirstOrDefault().Id.ToString();
                    objDIM.CompanyName = objDIM.Company.ToList().FirstOrDefault().CompanyName.ToString();
                }
                return View(objDIM);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        [HttpPost]
        public ActionResult CompaniesPaymentSetup(CompanyPayment objcompany)
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
                            cfg.CreateMap<CompaniesPaymentModel, CompanyPayment>();
                        });

                        objDM1 = Mapper.Map<CompanyPayment, CompaniesPaymentModel>(objcompany);
                        if (objcompany.Id == null)
                        {

                            result = ObjBL.InsertCompaniesPaymentSetup(objDM1);
                            TempData["Msg"] = Resource1.SaveSuccess;
                        }
                        else
                        {
                            result = ObjBL.UpdateCompaniesPaymentSetup(objDM1);
                            TempData["Msg"] = Resource1.UpdateSuccess;
                        }

                        if (result != "1")
                        {
                            objDM1.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                            AutoMapper.Mapper.Reset();
                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<CompaniesPaymentModel, CompanyPayment>();
                            });
                            objcompany = Mapper.Map<CompaniesPaymentModel, CompanyPayment>(objDM1);

                            ViewBag.Error = Resource1.CompanyAlreadyExist;

                            return View(objcompany);
                        }

                        else
                        {
                            return RedirectToAction("CompaniesPaymentSetupList");
                        }
                    }
                    else
                    {
                        AutoMapper.Mapper.Reset();
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<CompaniesPaymentModel, CompanyPayment>();
                        });
                        objcompany = Mapper.Map<CompaniesPaymentModel, CompanyPayment>(objDM1);
                        string messages = string.Join(Environment.NewLine, ModelState.Values
                                    .SelectMany(x => x.Errors)
                                    .Select(x => x.Exception));
                        ViewBag.Error = messages;
                        return View(objcompany);
                    }
                }


                catch (Exception ex)
                {
                    ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                    objDM1.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<CompaniesPaymentModel, CompanyPayment>();
                    });
                    objcompany = Mapper.Map<CompaniesPaymentModel, CompanyPayment>(objDM1);

                    ViewBag.Error = ex.Message;
                    return View(objcompany);
                }
            }
        
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult CompaniesPaymentSetupList()
        {
            if (TempData["Msg"] != null)
            {
                ViewBag.Error = TempData["Msg"].ToString();
            }
            if (Session["user"] != null)
            {
                try
                {
                  
                    List<CompanyPayment> objcompany = new List<CompanyPayment>();
                    List<CompaniesPaymentModel> companydata = new List<CompaniesPaymentModel>();
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<CompaniesPaymentModel, CompanyPayment>();
                    });
                    objcompany = Mapper.Map<List<CompaniesPaymentModel>, List<CompanyPayment>>(companydata);

                    return View(objcompany);
                }
                catch (Exception ex)
                {
                    ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                    return RedirectToAction("error", "Area", new { Error = ex.Message });
                }

            }
            else
            { return RedirectToAction("Login", "Account"); }
        }

        [HttpPost]
        public ActionResult LoadCompaniesPaymentSetupData()
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                var reqst = System.Web.HttpContext.Current.Request.InputStream;
                System.IO.StreamReader reader = new System.IO.StreamReader(reqst);
                String xmlData = reader.ReadToEnd();
                string decode = HttpUtility.UrlDecode(Uri.UnescapeDataString(xmlData));
                //Get parameters
                // get Start (paging start index) and length (page size for paging)
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                //Get Sort columns value
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                string search = Request.Params["search[value]"];

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int totalRecords = 0;

                var sEcho1 = Convert.ToInt32((Convert.ToInt32(start) + Convert.ToInt32(length)) / Convert.ToInt32(length));
                int pageNo = sEcho1;//!string.IsNullOrEmpty(search) ? 1 : sEcho1;
                List<CompanyPayment> objcompany = new List<CompanyPayment>();
                List<CompaniesPaymentModel> companydata = new List<CompaniesPaymentModel>();

                companydata = ObjBL.GetAllCompaniesPaymentSetup(pageNo, pageSize, search, sortColumn, sortColumnDir, Session["City"].ToString(), Session["RoleName"].ToString());
                if (companydata.Count() > 0)
                {
                    totalRecords = companydata.LastOrDefault().TotalRows;
                }
                var data = companydata;
                return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpGet]
        public ActionResult EditCompaniesPaymentSetup(string Id)
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                try
                {

                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<CompaniesPaymentModel, CompanyPayment>();
                    });
                    objDM1 = ObjBL.GetCompaniesPaymentById(Id);
                    objDM1.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), objDM1.CityId);
                    company = Mapper.Map<CompaniesPaymentModel, CompanyPayment>(objDM1);
                    return View("CompaniesPaymentSetup", company);
                }
                catch (Exception ex)
                {
                    ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                    return RedirectToAction("error", "Area", new { Error = ex.Message });
                }

            }
            else
            { return RedirectToAction("Login", "Account"); }
        }


    }



}
