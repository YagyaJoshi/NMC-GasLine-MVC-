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
    public class BankMasterController : Controller
    {
        string result;
        BankMaster objBank = new BankMaster();
        BankData objDM = new BankData();
        BankBL objBankBL = new BankBL();


        // GET: Company
        public ActionResult Index()
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {


                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<BankMaster, BankData>();
                });
                objBank = Mapper.Map<BankData, BankMaster>(objDM);

                return View(objBank);
            }
            else
            { return RedirectToAction("Login", "Account"); }
        }

        [HttpPost]
        public ActionResult Index(BankMaster objBank)
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
                            cfg.CreateMap<BankData, BankMaster>();
                        });

                        objDM = Mapper.Map<BankMaster, BankData>(objBank);

                        // objDM.UserId = user.Id;i
                        if (string.IsNullOrEmpty(objBank.Id))
                        {
                            objDM.AdminId = Convert.ToString(Session["user"]);
                            result = objBankBL.Save(objDM);
                            TempData["Msg"] = Resource1.SaveSuccess;

                        }
                        else
                        {
                            result = objBankBL.Update(objDM);
                            TempData["Msg"] = Resource1.UpdateSuccess;

                        }

                        if (result != "1")
                        {
                            AutoMapper.Mapper.Reset();

                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<BankMaster, BankData>();
                            });

                            objBank = Mapper.Map<BankData, BankMaster>(objDM);

                            //objArea = Mapper.Map<BankData, BankMaster>(objDM);
                            ViewBag.Error = Resource1.BankExist;
                            return View(objBank);
                        }
                        else
                        {
                            return RedirectToAction("List");
                        }

                    }
                    else
                    {
                        AutoMapper.Mapper.Reset();

                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<BankMaster, BankData>();
                        });

                        objBank = Mapper.Map<BankData, BankMaster>(objDM);



                        string messages = string.Join(Environment.NewLine, ModelState.Values
                                    .SelectMany(x => x.Errors)
                                    .Select(x => x.Exception));

                        ViewBag.Error = messages;
                        return View(objBank);

                    }
                }
                catch (Exception ex)
                {

                    objBankBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Convert.ToString(Session["user"]));

                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<BankMaster, BankData>();
                    });
                    objBank = Mapper.Map<BankData, BankMaster>(objDM);

                    ViewBag.Error = ex.Message;
                    return View(objBank);
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
            List<BankMaster> objbank = new List<BankMaster>();
            if (Session["user"] != null)
            {
                try
                {

                    List<BankData> Areadata = new List<BankData>();
                    //Areadata = ObjAreaBL.GetAllArea();
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<BankData, BankMaster>();
                    });
                    objbank = Mapper.Map<List<BankData>, List<BankMaster>>(Areadata);

                    return View(objbank);
                }
                catch (Exception ex)
                {
                    objBankBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                    ViewBag.Error = ex.Message;
                    return RedirectToAction("error", "Area", new { Error = ex.Message });
                }

            }
            else
            { return RedirectToAction("Login", "Account"); }
        }

     
        [HttpGet]
        public ActionResult Edit(string BankId)
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                try
                {
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<BankData, BankMaster>();
                    });
                    objDM = objBankBL.GetbankById(BankId);

                    objBank = Mapper.Map<BankData, BankMaster>(objDM);

                    return View("Index", objBank);
                }
                catch (Exception ex)
                {
                    objBankBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                    ViewBag.Error = ex.Message;
                    return RedirectToAction("error", "Area", new { Error = ex.Message });
                }


            }
            else
            { return RedirectToAction("Login", "Account"); }
        }

        [HttpGet]
        public ActionResult Delete(string BankId)
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                try
                {
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg => { cfg.CreateMap<BankData, BankMaster>(); });
                    objBankBL.DeleteBank(BankId, 0);
                    TempData["Msg"] = Resource1.AresInactive;
                    return RedirectToAction("List");
                }
                catch (Exception ex)
                {
                    objBankBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
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
                if (sortColumn == "BankName" && sortColumnDir == "asc") sortColumn = "";
                //string search = Request.Params["search[value]"];
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int totalRecords = 0;
                var sEcho1 = Convert.ToInt32((Convert.ToInt32(start) + Convert.ToInt32(length)) / Convert.ToInt32(length));
                int pageNo = sEcho1;   //!string.IsNullOrEmpty(search) ? 1 : sEcho1;
                List<BankMaster> objLBank = new List<BankMaster>();
                List<BankData> bankdata = new List<BankData>();


                bankdata = objBankBL.GetAllbank(pageNo, pageSize, search, sortColumn, sortColumnDir);
                if (bankdata.Count() > 0)
                {
                    totalRecords = bankdata.LastOrDefault().TotalRows;
                }
                var data = bankdata;
                return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            { return RedirectToAction("Login", "Account"); }
        }
    }
}