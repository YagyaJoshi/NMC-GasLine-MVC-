using AutoMapper;
using GemBox.Spreadsheet;
using NMCDataAccesslayer.BL;
using NMCDataAccesslayer.DataModel;
using NMCPipedGasLineAPI.Models;
using NMCPipedGasLineAPI.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NMCPipedGasLineAPI.Controllers
{
    public class CustomerController : Controller
    {
        CustomerLoginBL BObjBL = new CustomerLoginBL();
        CustomerBillInformationData objcDMB = new CustomerBillInformationData();
        Customer objCustomer = new Customer();
        CustomerDataModel objDM = new CustomerDataModel();
        CompanyBL ObjBL = new CompanyBL();
        AreaBL objabl = new AreaBL();
        GodownBL objGbl = new GodownBL();
        StockItemBL objstock = new StockItemBL();
        CustomerBL ObjCustomerBL = new CustomerBL();
        AreaBL ObjAreaBL = new AreaBL();
        CustomerLoginBL ObjBL1 = new CustomerLoginBL();
        // GET: Company
        public ActionResult Index()
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                AutoMapper.Mapper.Reset();
                objDM.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Customer, CustomerDataModel>();
                });
                objCustomer = Mapper.Map<CustomerDataModel, Customer>(objDM);
                AutoMapper.Mapper.Reset();
                objCustomer.DateofInstallation = DateTime.Now.Date;
                objCustomer.CustomerType = "Tenant";
                objCustomer.PaymentTypeId = "251B4F15-7DB8-4033-8796-9F41605C7A62";
                return View(objCustomer);

            }
            else
            { return RedirectToAction("Login", "Account"); }
        }

        [HttpPost]
        public async Task<ActionResult> Index(Customer objCustomer)
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                try
                {
                    AutoMapper.Mapper.Reset();
                    if (ModelState.IsValid)
                    {
                        //  AutoMapper.Mapper.Reset();
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<CustomerDataModel, Customer>();
                        });

                        objDM = Mapper.Map<Customer, CustomerDataModel>(objCustomer);

                        // objDM.UserId = user.Id;i
                        if (objCustomer.CustomerID == null)
                        {
                            string msg = ObjCustomerBL.SaveCustomerByAdmin(objDM, Session["user"].ToString());
                            if (msg == "1")
                            { }
                            else
                            {
                                ViewBag.Error = msg;
                                objDM.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                                objDM.Area = objabl.GetArea(objDM.CompanyId);
                                objDM.GoDown = await objGbl.GetGoDownAreawise(objDM.AreaId);
                                objDM.StockItem = objstock.GetStockItemByCompany(objDM.CompanyId);
                                objCustomer = Mapper.Map<CustomerDataModel, Customer>(objDM);

                                return View(objCustomer);

                            }
                        }
                        else
                        {
                            string msg = ObjCustomerBL.UpdateCustomerByAdmin(objDM, Session["user"].ToString());

                            if (msg == "1")
                            { }
                            else
                            {
                                AutoMapper.Mapper.Reset();
                                ObjAreaBL.ErrorSave("Message:-" + msg + "InnerException:-" + msg, "", "", Session["user"].ToString());
                                ViewBag.Error = msg;
                                objDM.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                                objDM.Area = objabl.GetArea(objDM.CompanyId);
                                objDM.GoDown = await objGbl.GetGoDownAreawise(objDM.AreaId);
                                objDM.StockItem = objstock.GetStockItemByCompany(objDM.CompanyId);
                                Mapper.Initialize(cfg =>
                                {
                                    cfg.CreateMap<Customer, CustomerDataModel>();
                                });
                                objCustomer = Mapper.Map<CustomerDataModel, Customer>(objDM);

                                return View(objCustomer);
                            }
                        }

                        AutoMapper.Mapper.Reset();

                        return RedirectToAction("List");

                    }
                    else
                    {
                        objDM.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                        AutoMapper.Mapper.Reset();
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Customer, CustomerDataModel>();
                        });
                        objCustomer = Mapper.Map<CustomerDataModel, Customer>(objDM);

                        string messages = string.Join(Environment.NewLine, ModelState.Values
                                .SelectMany(x => x.Errors)
                                .Select(x => x.Exception));

                        ViewBag.Error = messages;
                        return View(objCustomer);
                    }
                }
                catch (Exception ex)
                {
                    ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                    objDM.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Customer, CustomerDataModel>();
                    });
                    objCustomer = Mapper.Map<CustomerDataModel, Customer>(objDM);
                    AutoMapper.Mapper.Reset();
                    ViewBag.Error = ex.Message;
                    return View(objCustomer);
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
                    Customer objLCustomer = new Customer();
                    CustomerDataModel Customerdata = new CustomerDataModel();
                    //Customerdata = ObjCustomerBL.GetAllCustomer();
                    Customerdata.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<CustomerDataModel, Customer>();
                    });
                    objLCustomer = Mapper.Map<CustomerDataModel, Customer>(Customerdata);
                    if (Session["RoleName"].ToString() != "Super Admin")
                    {
                        objLCustomer.CompanyId = objLCustomer.Company.ToList().FirstOrDefault().Id.ToString();
                        objLCustomer.CompanyName = objLCustomer.Company.ToList().FirstOrDefault().CompanyName.ToString();
                    }
                    return View(objLCustomer);
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
        public async Task<ActionResult> Edit(string CustomerId)
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                AutoMapper.Mapper.Reset();
                try
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<CustomerDataModel, Customer>();
                    });
                    objDM = ObjCustomerBL.GetCustomerById(CustomerId, "BFBC4C62-B98B-4C18-96E7-2C8327886BCB");
                    objDM.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                    objDM.Area = objabl.GetArea(objDM.CompanyId);
                    objDM.GoDown = await objGbl.GetGoDownAreawise(objDM.AreaId);
                    objDM.StockItem = objstock.GetStockItemByCompany(objDM.CompanyId);
                    objDM.CustomerType = objDM.CustomerType;
                    objDM.PaymentTypeId = objDM.PaymentTypeId;
                    objCustomer = Mapper.Map<CustomerDataModel, Customer>(objDM);

                    return View("Index", objCustomer);
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
        public ActionResult Delete(string CustomerId)
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                try
                {
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<CustomerDataModel, Customer>();
                });
                    ObjCustomerBL.DeleteCustomer(CustomerId, 0);

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
            Customer model = new Customer();
            try
            {
                AutoMapper.Mapper.Reset();
                List<Customer> objLCustomer = new List<Customer>();
                List<CustomerDataModel> Godowndata = new List<CustomerDataModel>();
                //Godowndata = ObjCustomerBL.GetAllCustomer();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<CustomerDataModel, Customer>();
                });
                objLCustomer = Mapper.Map<List<CustomerDataModel>, List<Customer>>(Godowndata);
                AutoMapper.Mapper.Reset();


                //NorthwindEntities entities = new NorthwindEntities();

                //CustomerModel model = new CustomerModel();
                model.PageIndex = pageIndex;
                model.PageSize = 15;
                model.RecordCount = objLCustomer.Count();
                int startIndex = (pageIndex - 1) * model.PageSize;
                model.CustomerList = objLCustomer
                                .OrderBy(customer => customer.CustomerID)
                                .Skip(startIndex)
                                .Take(model.PageSize).ToList();
                return Json(model);
            }
            catch (Exception ex)
            {
                ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                model.PageIndex = 402;
                model.BankName = ex.Message;
                return Json(model);
            }
        }

        [HttpGet]
        public ActionResult NewBill(string CustomerId)
        {
            if (Session["user"] != null)
            {
                AutoMapper.Mapper.Reset();
                try
                {
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<CustomerDataModel, Customer>();
                    });
                    objDM = ObjCustomerBL.GetCustomerById(CustomerId, "BFBC4C62-B98B-4C18-96E7-2C8327886BCB");
                    Random generator = new Random();

                    objDM.BillNo = generator.Next(0, 999999).ToString("D6");
                    objDM.BillDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
                    objDM.DueDate = DateTime.Now.Date.AddDays(5).ToString("yyyy-MM-dd");
                    objDM.ClosingRedg = 0;
                    objDM.CGST = 0;
                    objDM.SGST = 0;
                    objDM.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                    objDM.Amount = 0;
                    objDM.ClosingBalance = 0;
                    objDM.ChequeNo = "";
                    objDM.BankName = "";
                    objDM.AccountNo = "";
                    objDM.StockItem = objstock.GetStockItemByCompany(objDM.CompanyId);
                    objCustomer = Mapper.Map<CustomerDataModel, Customer>(objDM);

                    return View("NewBill", objCustomer);
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
        public JsonResult getrate(List<StockRate> model)
        {
            Customer cust = new Customer();
            try
            {
                AutoMapper.Mapper.Reset();
                StockItemBL objbl = new StockItemBL();
                List<StockItemMaster> objLStock = new List<StockItemMaster>();
                List<StockItemMasterDataModel> Stockdata = new List<StockItemMasterDataModel>();
                Stockdata = objbl.Getstockrate(model.FirstOrDefault().CompanyId, model.FirstOrDefault().StockItemId);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<StockItemMasterDataModel, StockItemMaster>();
                });
                objLStock = Mapper.Map<List<StockItemMasterDataModel>, List<StockItemMaster>>(Stockdata);


                double daytotal = model.Sum(item => Convert.ToDouble(item.day));
                decimal scm = (model.FirstOrDefault().ClosingRedg - model.FirstOrDefault().PreviousRedg);
                decimal kgs = scm * model.FirstOrDefault().InputRate;
                decimal perdayconsume = kgs / Convert.ToDecimal(daytotal);

                var result1 = objLStock
            .Where(sid => model.Any(si => si.month == Convert.ToString(sid.month) && si.year == Convert.ToString(sid.Year)))
            .Select(StockItemMaster => new StockItemMaster
            {
                month = StockItemMaster.month,
                Year = StockItemMaster.Year,
                Rate = StockItemMaster.weight != 0 ? Convert.ToDecimal(StockItemMaster.Rate / StockItemMaster.weight) : StockItemMaster.Rate,
                day = model.Where(si => si.month == Convert.ToString(StockItemMaster.month) && si.year == Convert.ToString(StockItemMaster.Year)).Select(si => si.day).FirstOrDefault(),
                weight = Convert.ToDecimal(StockItemMaster.Rate) * Convert.ToDecimal(model.Where(si => si.month == Convert.ToString(StockItemMaster.month) && si.year == Convert.ToString(StockItemMaster.Year)).Select(si => si.day).FirstOrDefault())

            })
          .ToList();

                decimal gasc = result1.Sum(item => Convert.ToDecimal(item.weight));
                decimal rate = result1.Sum(item => Convert.ToDecimal(item.Rate)) / 2;

                cust.CurrentScm = scm;
                cust.CurrentKGS = kgs;
                cust.ConsumeUnit = gasc;
                cust.Rate = rate;
                cust.BankName = "";
            }
            catch (Exception ex)
            {
                ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                cust.BankName = ex.Message;
            }
            return Json(cust);
        }


        [HttpPost]
        public ActionResult NewBill(Customer objCustomer)
        {
            if (Session["user"] != null)
            {
                try
                {
                    AutoMapper.Mapper.Reset();
                    if (ModelState.IsValid)
                    {
                        AutoMapper.Mapper.Reset();
                        Mapper.Initialize(cfg =>
                     {
                         cfg.CreateMap<CustomerDataModel, Customer>();
                     });

                        objDM = Mapper.Map<Customer, CustomerDataModel>(objCustomer);

                        string msg = ObjCustomerBL.SaveBillCustomer(objDM, Session["user"].ToString());
                        if (msg == "1")
                        {

                        }
                        else
                        {
                            ViewBag.Error = msg;
                            objDM.StockItem = objstock.GetStockItemByCompany(objDM.CompanyId);
                            objCustomer = Mapper.Map<CustomerDataModel, Customer>(objDM);
                            AutoMapper.Mapper.Reset();
                            ObjAreaBL.ErrorSave("Message:-" + msg + "InnerException:-" + msg, "", "", Session["user"].ToString());
                            return View(objCustomer);
                        }


                        return RedirectToAction("List");

                    }
                    else
                    {
                        objDM.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                        AutoMapper.Mapper.Reset();
                        Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Customer, CustomerDataModel>();
                    });
                        objCustomer = Mapper.Map<CustomerDataModel, Customer>(objDM);


                        string messages = string.Join(Environment.NewLine, ModelState.Values
                                      .SelectMany(x => x.Errors)
                                      .Select(x => x.ErrorMessage));
                        ViewBag.Error = messages;
                        ObjAreaBL.ErrorSave("Message:-" + messages + "InnerException:-" + messages, "", "", Session["user"].ToString());
                        return View(objCustomer);
                    }
                }
                catch (Exception ex)
                {
                    objDM.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Customer, CustomerDataModel>();
                    });
                    objCustomer = Mapper.Map<CustomerDataModel, Customer>(objDM);


                    ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.Message, "", "", Session["user"].ToString());
                    return View(objCustomer);
                }
            }
            else
            { return RedirectToAction("Login", "Account"); }
        }

        [HttpPost]
        public ActionResult LoadData(CustomerList cust)
        {
            //Get parameters
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                CustomerDataModel modleDATA = new CustomerDataModel();
                // get Start (paging start index) and length (page size for paging)
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                //Get Sort columns value
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                string search = Request.Params["search[value]"];//Search;

                //string search = Request.Params["search[value]"];
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int totalRecords = 0;

                var sEcho1 = Convert.ToInt32((Convert.ToInt32(start) + Convert.ToInt32(length)) / Convert.ToInt32(length));
                int pageNo = sEcho1;//!string.IsNullOrEmpty(search) ? 1 : sEcho1;
                List<Customer> objLCustomer = new List<Customer>();
                List<CustomerDataModel> Godowndata = new List<CustomerDataModel>();

                Godowndata = ObjCustomerBL.GetAllCustomer(pageNo, pageSize, search, sortColumn, sortColumnDir, Session["City"].ToString(), Session["RoleName"].ToString(), cust);
                if (Godowndata.Count() > 0)
                {
                    totalRecords = Godowndata.LastOrDefault().TotalRows;
                }
                var data = Godowndata;
                return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            { return RedirectToAction("Login", "Account"); }
        }

        [HttpPost]
        public ActionResult LoadCustomerData(CustomerList cust)
        {
            //Get parameters
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                CustomerDataModel modleDATA = new CustomerDataModel();
                // get Start (paging start index) and length (page size for paging)
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                //Get Sort columns value
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                string search = Request.Params["search[value]"];//Search;

                //string search = Request.Params["search[value]"];
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int totalRecords = 0;

                var sEcho1 = Convert.ToInt32((Convert.ToInt32(start) + Convert.ToInt32(length)) / Convert.ToInt32(length));
                int pageNo = sEcho1;//!string.IsNullOrEmpty(search) ? 1 : sEcho1;
                List<Customer> objLCustomer = new List<Customer>();
                List<CustomerDataModel> Godowndata = new List<CustomerDataModel>();

                Godowndata = ObjCustomerBL.GetAllCustomerforMail(pageNo, pageSize, search, sortColumn, sortColumnDir, Session["City"].ToString(), Session["RoleName"].ToString(), cust);
                if (Godowndata.Count() > 0)
                {
                    //totalRecords = Godowndata.LastOrDefault().TotalRows;
                    totalRecords = Godowndata.LastOrDefault().TotalRows;
                }
                var data = Godowndata;
                return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            { return RedirectToAction("Login", "Account"); }
        }

        public ActionResult CreditNodeList()
        {
            if (TempData["Msg"] != null)
            {
                ViewBag.Error = TempData["Msg"].ToString();
            }
            if (Session["user"] != null)
            {
                try
                {
                    Customer objLCustomer = new Customer();

                    //Customerdata = ObjCustomerBL.GetAllCustomer();

                    return View(objLCustomer);
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
        public ActionResult BillLoadData()
        {
            //Get parameters
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                // get Start (paging start index) and length (page size for paging)
                //var draw = Request.Form.GetValues("draw").FirstOrDefault();
                //var start = Request.Form.GetValues("start").FirstOrDefault();
                //var length = Request.Form.GetValues("length").FirstOrDefault();
                ////Get Sort columns value
                //var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                //var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                //string search = Request.Params["search[value]"];
                //if (sortColumn == "BillNo" && sortColumnDir == "asc") sortColumn = "";

                ////string search = Request.Params["search[value]"];
                //int pageSize = length != null ? Convert.ToInt32(length) : 0;
                //int skip = start != null ? Convert.ToInt32(start) : 0;
                int totalRecords = 0;
                //var sEcho1 = Convert.ToInt32((Convert.ToInt32(start) + Convert.ToInt32(length)) / Convert.ToInt32(length));
                //int pageNo = !string.IsNullOrEmpty(search) ? 1 : sEcho1;
                List<CustomerBillInformation> objLbill = new List<CustomerBillInformation>();
                List<CustomerBillInformationData> billdata = new List<CustomerBillInformationData>();
                billdata = new List<CustomerBillInformationData>();// ObjBL1.GetBillHistory(pageNo, pageSize, search, sortColumn, sortColumnDir, Session["user"].ToString());
                if (billdata.Count() > 0)
                {
                    totalRecords = billdata.LastOrDefault().TotalRows;
                }
                var data = billdata;
                return Json(new { draw = 0, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login", "Payment");
            }
        }


        public ActionResult BillList(string CustomerId)
        {
            if (TempData["Msg"] != null)
            {
                ViewBag.Error = TempData["Msg"].ToString();
            }
            if (Session["user"] != null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(CustomerId))
                        Session["Customer"] = CustomerId;

                    CustomerBillInformation CustomerBillInformation = new CustomerBillInformation();
                    AutoMapper.Mapper.Reset();

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<CustomerBillInformation, CustomerBillInformationData>();
                    });
                    CustomerBillInformation = Mapper.Map<CustomerBillInformationData, CustomerBillInformation>(objcDMB);


                    return View(CustomerBillInformation);
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
        public ActionResult BillData()
        {
            //Get parameters
            if (Session["user"] != null && Session["Customer"] != null)
            {
                // get Start (paging start index) and length (page size for paging)
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                //Get Sort columns value
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                string search = Request.Params["search[value]"];
                if (sortColumn == "BillNo" && sortColumnDir == "asc") sortColumn = "";

                //string search = Request.Params["search[value]"];
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int totalRecords = 0;
                var sEcho1 = Convert.ToInt32((Convert.ToInt32(start) + Convert.ToInt32(length)) / Convert.ToInt32(length));
                int pageNo = !string.IsNullOrEmpty(search) ? 1 : sEcho1;
                List<CustomerBillInformation> objLbill = new List<CustomerBillInformation>();
                List<CustomerBillInformationData> billdata = new List<CustomerBillInformationData>();
                billdata = BObjBL.GetBillHistory(pageNo, pageSize, search, sortColumn, sortColumnDir, Session["Customer"].ToString());
                if (billdata.Count() > 0)
                {
                    totalRecords = billdata.LastOrDefault().TotalRows;
                }
                var data = billdata;
                return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        public ActionResult PaymentHistory(string CustomerId)
        {
            if (Session["user"] != null)
            {
                CustomerBillInformation CustomerBillInformation = new CustomerBillInformation();
                AutoMapper.Mapper.Reset();
                if (!string.IsNullOrEmpty(CustomerId))
                    //Session["Customer"] = CustomerId;
                    Session["billid"] = CustomerId;
                objcDMB = BObjBL.GetAmountdetails(CustomerId);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<CustomerBillInformation, CustomerBillInformationData>();
                });
                CustomerBillInformation = Mapper.Map<CustomerBillInformationData, CustomerBillInformation>(objcDMB);
                return View(CustomerBillInformation);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        [HttpPost]
        public ActionResult LoadPaymentData()
        {
            if (Session["user"] != null && Session["Customer"] != null)
            {
                //Get parameters

                // get Start (paging start index) and length (page size for paging)
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                //Get Sort columns value
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                string search = Request.Params["search[value]"];
                if (sortColumn == "BillNo" && sortColumnDir == "asc") sortColumn = "";
                //string search = Request.Params["search[value]"];
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int totalRecords = 0;
                var sEcho1 = Convert.ToInt32((Convert.ToInt32(start) + Convert.ToInt32(length)) / Convert.ToInt32(length));
                int pageNo = !string.IsNullOrEmpty(search) ? 1 : sEcho1;
                List<Payment> objLbill = new List<Payment>();
                List<PaymentDataModel> billdata = new List<PaymentDataModel>();

                billdata = BObjBL.GetPaymentHistory(pageNo, pageSize, search, sortColumn, sortColumnDir, Session["Customer"].ToString());
                if (billdata.Count() > 0)
                {
                    totalRecords = billdata.LastOrDefault().TotalRows;
                }
                var data = billdata;
                return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        public ActionResult ViewBill(string BillId)
        {
            if (Session["user"] != null)
            {
                //string root = Server.MapPath("~/img/nmc_logo.png");
                AutoMapper.Mapper.Reset();
                //obj.Logimg = root;
                CustomerBillInformation CustomerBillInformation = new CustomerBillInformation();
                objcDMB = BObjBL.GetBillById(BillId);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<CustomerBillInformation, CustomerBillInformationData>();
                });
                CustomerBillInformation = Mapper.Map<CustomerBillInformationData, CustomerBillInformation>(objcDMB);
                return View("ViewBill", CustomerBillInformation);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult SendWelcomeMail()
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                CustomerDataModel modleDATA = new CustomerDataModel();
                modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<CustomerDataModel, Customer>();
                });
                objCustomer = Mapper.Map<CustomerDataModel, Customer>(modleDATA);
                if (Session["RoleName"].ToString() != "Super Admin")
                {
                    objCustomer.CompanyId = objCustomer.Company.ToList().FirstOrDefault().Id.ToString();
                    objCustomer.CompanyName = objCustomer.Company.ToList().FirstOrDefault().CompanyName.ToString();
                }
                return View(objCustomer);

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }



        public ActionResult SendMailToAllCustomer(Customer ObjCustomer)
        {
            string Msgbody, CustomerSms = "";
            CustomerDataModel modleDATA = new CustomerDataModel();
            try
            {

                if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
                {

                    DataTable data = ObjCustomerBL.GetSocietyWiseCustomer(ObjCustomer.CompanyId, ObjCustomer.GodownId, ObjCustomer.AreaId);

                    foreach (DataRow item in data.Rows)
                    {
                        if (item["PreviousBillDate"] != DBNull.Value && item["PreviousRedg"].ToString() != "0.00")
                        {
                            if (Convert.ToInt32(item["IsActive"]) != 2)
                            {
                                if (item["email"].ToString() != "")
                                {

                                    modleDATA.PassWord = BObjBL.GetDecrypted(item["password"].ToString());
                                    if (string.IsNullOrEmpty(modleDATA.PassWord))
                                    {
                                        modleDATA.PassWord = ObjBL1.GenratePass();
                                        ChangPwDataModel changep = new ChangPwDataModel();
                                        changep.Id = item["Id"].ToString();
                                        changep.Password = ObjBL1.GetEncrypted(modleDATA.PassWord);
                                        ObjBL1.changePwd(changep);
                                    }
                                    Msgbody = Resource1.CustomerEmail.Replace("@Fname", item["Name"].ToString()).Replace("@CustomerNumber", item["CustomerNumber"].ToString()).Replace("@password", modleDATA.PassWord);
                                    //CustomerSms = Resource1.CustomerSms.Replace("@Fname", item["Name"].ToString()).Replace("@CustomerNumber", item["CustomerNumber"].ToString()).Replace("@password", modleDATA.PassWord).Replace("@n", Environment.NewLine);

                                    BObjBL.SendSmsEmail(item["CustomerNumber"].ToString(), item["Name"].ToString(), item["Phone"].ToString(), item["email"].ToString(), Msgbody, CustomerSms, "", modleDATA.PassWord);


                                }
                                else if (item["Phone"].ToString() != "")
                                {


                                    CustomerSms = Resource1.CustomerSms.Replace("@Fname", item["Name"].ToString()).Replace("@CustomerNumber", item["CustomerNumber"].ToString()).Replace("@password", modleDATA.PassWord).Replace("@n", Environment.NewLine);
                                    BObjBL.SendSmsEmail(item["Id"].ToString(), item["Name"].ToString(), item["Phone"].ToString(), "", "", CustomerSms, "", modleDATA.PassWord);

                                }
                            }

                        }
                    }

                    //ViewBag.Status = "Welcome mails send to customer successfully!";
                    TempData["Success"] = "Welcome mails send to customer successfully!";

                    return RedirectToAction("SendWelcomeMail", "Customer");

                }
                else
                {
                    return RedirectToAction("Login", "Account");

                }
            }
            catch (Exception ex)
            {
                modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<CustomerDataModel, Customer>();
                });
                ObjCustomer = Mapper.Map<CustomerDataModel, Customer>(modleDATA);

                ViewBag.Error = ex.Message;
                return RedirectToAction("SendWelcomeMail");
                //return View(ObjCustomer);
            }
        }


        public JsonResult SendEmail(string CustomerId)
        {
            string Msgbody, CustomerSms = "";

            CustomerDataModel modleDATA = new CustomerDataModel();

            AutoMapper.Mapper.Reset();
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<CustomerDataModel, Customer>();
            });
            objCustomer = Mapper.Map<CustomerDataModel, Customer>(modleDATA);

            try
            {

                if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
                {

                    DataTable data = ObjCustomerBL.GetCustomerByIdForMail(CustomerId);

                    foreach (DataRow item in data.Rows)
                    {
                        if (item["PreviousBillDate"] != DBNull.Value && item["PreviousRedg"].ToString() != "0.00")
                        {
                            if (Convert.ToInt32(item["IsActive"]) != 2)
                            {
                                if (item["email"].ToString() != "")
                                {

                                    modleDATA.PassWord = BObjBL.GetDecrypted(item["password"].ToString());
                                    if (string.IsNullOrEmpty(modleDATA.PassWord))
                                    {
                                        modleDATA.PassWord = ObjBL1.GenratePass();
                                        ChangPwDataModel changep = new ChangPwDataModel();
                                        changep.Id = CustomerId;
                                        changep.Password = ObjBL1.GetEncrypted(modleDATA.PassWord);
                                        ObjBL1.changePwd(changep);
                                    }
                                    Msgbody = Resource1.CustomerEmail.Replace("@Fname", item["Name"].ToString()).Replace("@CustomerNumber", item["CustomerNumber"].ToString()).Replace("@password", modleDATA.PassWord);
                                    //CustomerSms = Resource1.CustomerSms.Replace("@Fname", item["Name"].ToString()).Replace("@CustomerNumber", item["CustomerNumber"].ToString()).Replace("@password", modleDATA.PassWord).Replace("@n", Environment.NewLine);

                                    BObjBL.SendSmsEmail(item["CustomerNumber"].ToString(), item["Name"].ToString(), item["Phone"].ToString(), item["email"].ToString(), Msgbody, CustomerSms, "", modleDATA.PassWord);


                                }
                                else if (item["Phone"].ToString() != "")
                                {


                                    CustomerSms = Resource1.CustomerSms.Replace("@Fname", item["Name"].ToString()).Replace("@CustomerNumber", item["CustomerNumber"].ToString()).Replace("@password", modleDATA.PassWord).Replace("@n", Environment.NewLine);
                                    BObjBL.SendSmsEmail(item["Id"].ToString(), item["Name"].ToString(), item["Phone"].ToString(), "", "", CustomerSms, "", modleDATA.PassWord);

                                }
                            }
                        }
                    }
                    ViewBag.Message = "Welcome mails send to customer successfully!";
                    //TempData["Success"] = "Welcome mails send to customer successfully!";

                    return Json(new { test = "Welcome mails send to customer successfully!" }, JsonRequestBehavior.AllowGet);


                }
                else
                {
                    RedirectToAction("Login", "Account");
                    return Json(null);
                }
            }
            catch (Exception ex)
            {
                modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<CustomerDataModel, Customer>();
                });
                objCustomer = Mapper.Map<CustomerDataModel, Customer>(modleDATA);

                ViewBag.Error = ex.Message;
                RedirectToAction("SendWelcomeMail");
                return Json(null);
            }
        }

        [HttpPost]
        public JsonResult LastBill(string GodownId)
        {
            CustomerDataModel modleDATA = new CustomerDataModel();
            Customer cust = new Customer();
            CustomerBL objBl = new CustomerBL();
            modleDATA = objBl.GetMaxBillDate(GodownId);
            return Json(modleDATA);
        }

        public ActionResult CustomerWithoutBill()
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                CustomerDataModel modleDATA = new CustomerDataModel();
                modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<CustomerDataModel, Customer>();
                });
                objCustomer = Mapper.Map<CustomerDataModel, Customer>(modleDATA);
                if (Session["RoleName"].ToString() != "Super Admin")
                {
                    objCustomer.CompanyId = objCustomer.Company.ToList().FirstOrDefault().Id.ToString();
                    objCustomer.CompanyName = objCustomer.Company.ToList().FirstOrDefault().CompanyName.ToString();
                }
                return View(objCustomer);

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }

        [HttpPost]
        public ActionResult CustomerWithoutBill(ExcelExportCustomerWithoutBill ObjCustomer)
        {
            CustomerDataModel modleDATA = new CustomerDataModel();
            if (Session["user"] != null)
            {
                if (ModelState.IsValid)
                {
                    List<ExportCustomerWithoutBill> customerWithoutBills = new List<ExportCustomerWithoutBill>();
                    customerWithoutBills = ObjCustomerBL.GetCustomerList(ObjCustomer.GodownId);
                    SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
                    ExcelFile ef = new ExcelFile();
                    ExcelWorksheet ws = ef.Worksheets.Add("Sheet1");
                    if (customerWithoutBills != null && customerWithoutBills.Count > 0)
                    {
                        int i = 1;
                        foreach (ExportCustomerWithoutBill item in customerWithoutBills)
                        {
                            if (i == 1)
                            {
                                ws.Cells[0, 0].Value = "SR No";
                                ws.Cells[0, 1].Value = "TallyName";
                                ws.Cells[0, 2].Value = "AliasName";
                                ws.Cells[0, 3].Value = "Address";
                                ws.Cells[0, 4].Value = "Email";
                                ws.Cells[0, 5].Value = "Phone";
                                ws.Cells[0, 6].Value = "FlatNo";


                            }

                            ws.Cells[i, 0].Value = i;
                            ws.Cells[i, 1].Value = item.TallyName;
                            ws.Cells[i, 2].Value = item.AliasName;
                            ws.Cells[i, 3].Value = item.Address;
                            ws.Cells[i, 4].Value = item.Email;
                            ws.Cells[i, 5].Value = item.Phone;
                            ws.Cells[i, 6].Value = item.FlatNo;

                            i++;
                        }
                        string root = Server.MapPath("~/Bill");
                        if (!Directory.Exists(root)) { Directory.CreateDirectory(root); }

                        string sFileName = "CustomerWithoutBill.xlsx";
                        ef.Save(root + "\\" + sFileName);
                        Response.ContentType = "application/ms-excel";
                        Response.AppendHeader("Content-Disposition", "attachment; filename=" + sFileName + "");
                        Response.TransmitFile(root + "\\" + sFileName);
                        Response.End();
                        if (System.IO.File.Exists(root + "\\" + sFileName))
                        {
                            System.IO.File.Delete(root + "\\" + sFileName);
                        }

                    }
                    AutoMapper.Mapper.Reset();
                    modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<CustomerDataModel, Customer>();
                    });
                    objCustomer = Mapper.Map<CustomerDataModel, Customer>(modleDATA);
                    AutoMapper.Mapper.Reset();
                    if (customerWithoutBills.Count <= 0)
                    {
                        TempData["Message"] = "No Data Available";
                    }
                    //ViewBag.Message = "No Data Available";

                    return Redirect("CustomerWithoutBill");

                }

                else
                {
                    modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<CustomerDataModel, Customer>();
                    });
                    objCustomer = Mapper.Map<CustomerDataModel, Customer>(modleDATA);

                    return View("CustomerWithoutBill", objCustomer);
                }

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }

        [HttpGet] 
        public ActionResult EditEmail(string CustomerId) 
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                try
                {                  
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<CustomerDataModel, Customer>();
                    });
                    DataTable dt = ObjCustomerBL.GetCustomerByIdForMail(CustomerId);
                    objDM.CustomerID = dt.Rows[0]["CustomerID"].ToString();
                    objDM.Email=dt.Rows[0]["Email"].ToString();
                    objCustomer = Mapper.Map<CustomerDataModel, Customer>(objDM);

                    return PartialView("_EditEmail", objCustomer);
                }
                catch (Exception ex)
                {
                    ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                    return RedirectToAction("error", "Area", new { Error = ex.Message });
                }


            }
            else
            { return RedirectToAction("Login", "Account"); }

           // return View();
        }

        [HttpPost]
        public ActionResult EditEmail(string Id, string Email)   
        {
            Customer objcust = new Customer();
            objcust.Email = Email;
            objcust.CustomerID = Id;
            if(Session["user"] != null)
            {
                try
                {
                    if (objcust.Email!= null)
                    {
                        AutoMapper.Mapper.Reset();
                        Mapper.Initialize(cfg =>
                       {
                           cfg.CreateMap<Customer,CustomerDataModel>();
                       });
                        objDM= Mapper.Map<Customer, CustomerDataModel>(objcust);

                        CustomerDataModel dataModel = new CustomerDataModel();
                        string passw = BObjBL.GenratePass();
                        objcust.PassWord = BObjBL.GetEncrypted(passw);
                        ObjCustomerBL.EditEmail(objcust.Email, objcust.CustomerID, objcust.PassWord);
                    }

                }
                catch(Exception ex) 
                {
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Customer, CustomerDataModel>();
                    });
                    objCustomer = Mapper.Map<CustomerDataModel, Customer>(objDM);


                    ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.Message, "", "", Session["user"].ToString());
                    return View(objCustomer);
                }
            }
            var msg= Resource1.UpdateEmail;
            return Json(new { Msg = msg, }, JsonRequestBehavior.AllowGet );          
            
        }
       
    }
}
