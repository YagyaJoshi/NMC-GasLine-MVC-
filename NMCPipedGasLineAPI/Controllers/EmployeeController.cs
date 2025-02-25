using NMCPipedGasLineAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NMCDataAccesslayer.BL;
using NMCDataAccesslayer.DataModel;
using AutoMapper;
using System.Linq.Dynamic;
using NMCPipedGasLineAPI.Properties;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using GemBox.Spreadsheet;

namespace NMCPipedGasLineAPI.Controllers
{
    public class EmployeeController : Controller
    {
        User objUser = new User();
        UserDataModel objDM = new UserDataModel();
        UserBL objUBL = new UserBL();
        CompanyBL ObjBL = new CompanyBL();
        AreaBL ObjAreaBL = new AreaBL();

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
                    List<User> objuser = new List<User>();
                    List<UserDataModel> user = new List<UserDataModel>();
                    //user = objUBL.GetAllUser();
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<UserDataModel, User>();
                    });
                    objuser = Mapper.Map<List<UserDataModel>, List<User>>(user);

                    return View(objuser);
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
        public ActionResult Edit(string Id)
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                try
                {
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
               {
                   cfg.CreateMap<UserDataModel, User>();
               });
                    objDM = objUBL.GetUser(Id);
                    objDM.Country = objUBL.GetCountry();
                    objDM.State = objUBL.GetState(objDM.CountryId, Session["City"].ToString());
                    objDM.City = objUBL.GetCity(objDM.StateId, Session["City"].ToString());
                    objDM.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), objDM.CityId);
                    objDM.Area = ObjAreaBL.GetArea(objDM.CompanyId);
                    objDM.Role = objUBL.GetRole();
                    objUser = Mapper.Map<UserDataModel, User>(objDM);

                    //if (Session["RoleName"].ToString() != "Super Admin")
                    //{
                    //    objDM.RoleId = "f8f58731-7404-4e8e-87af-1175f1520a98";
                    //}
                    return View("~/Views/Account/Register.cshtml", objUser);
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
        public ActionResult Delete(string Id)
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                try
                {
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<UserDataModel, User>();
                    });
                    objUBL.DeleteUser(Id, 0);
                    TempData["Msg"] = Resource1.EmployeeInactive;
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
            User model = new User();
            try
            {
                List<User> objuser = new List<User>();
                List<UserDataModel> user = new List<UserDataModel>();
                //user = objUBL.GetAllUser();
                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<UserDataModel, User>();
                });
                objuser = Mapper.Map<List<UserDataModel>, List<User>>(user);
                model.PageIndex = pageIndex;
                model.PageSize = 15;
                model.RecordCount = user.Count();
                int startIndex = (pageIndex - 1) * model.PageSize;
                model.UserList = objuser
                                .OrderBy(customer => customer.Id)
                                .Skip(startIndex)
                                .Take(model.PageSize).ToList();
                return Json(model);
            }
            catch (Exception ex)
            {
                ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                model.PageIndex = 402;
                model.CityName = ex.Message;
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
                int pageNo = sEcho1;//    !string.IsNullOrEmpty(search) ? 1 : sEcho1;
                List<User> objuser = new List<User>();
                List<UserDataModel> user = new List<UserDataModel>();

                user = objUBL.GetAllUser(pageNo, pageSize, search, sortColumn, sortColumnDir, Session["City"].ToString(), Session["RoleName"].ToString());
                if (search != "")
                { }
                if (user.Count() > 0)
                {
                    totalRecords = user.FirstOrDefault().TotalRows;

                }
                var data = user;

                return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        public ActionResult DataFileList()
        {
            if (Session["user"] != null)
            {
                return View();

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        [HttpPost]
        public ActionResult LoadDataFile()
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
                int pageNo = sEcho1;//    !string.IsNullOrEmpty(search) ? 1 : sEcho1;
                List<User> objuser = new List<User>();
                List<UserDataModel> user = new List<UserDataModel>();

                user = objUBL.GetEmployeeFile(pageNo, pageSize, search, sortColumn, sortColumnDir);
                if (search != "")
                { }
                if (user.Count() > 0)
                {
                    totalRecords = user.FirstOrDefault().TotalRows;

                }
                var data = user;

                return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        public static DataTable ConvertCSVtoDataTable(string strFilePath)
        {
            StreamReader sr = new StreamReader(strFilePath);
            string[] headers = sr.ReadLine().Split(',');
            DataTable dt = new DataTable();
            foreach (string header in headers)
            {
                dt.Columns.Add(header);
            }
            while (!sr.EndOfStream)
            {
                string[] rows = Regex.Split(sr.ReadLine(), ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                DataRow dr = dt.NewRow();
                for (int i = 0; i < headers.Length; i++)
                {
                    dr[i] = rows[i];
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public decimal totalamt(decimal Totalamt, decimal PLateFee)
        {
            decimal totalAmount = Totalamt + PLateFee + (PLateFee * 18 / 100);
            return totalAmount;
        }


        [HttpGet]
        public ActionResult BillDownload(string FileName)
        {
            string name = "";
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                try
                {
                    string root = HttpContext.Server.MapPath("~/DataFiles");
                    string filepath = root + "/" + FileName;
                    DataTable dt3 = ConvertCSVtoDataTable(filepath);
                   

                    List<CustomerBillInformationData> Bill = new List<CustomerBillInformationData>();
                    foreach (DataRow item in dt3.Rows)
                    {
                        name = Convert.ToString(item["PartyName"]);
                        if (name != "")
                        {
                            Bill.Add(new CustomerBillInformationData
                            {
                                BillNo = item["BillNo"] != null ? Convert.ToString(item["BillNo"]) : "",
                                PartyName = item["PartyName"] != null ? Convert.ToString(item["PartyName"]) : "",
                                ClosingBalance = item["ClosingBalance"] != null ? Convert.ToDecimal(item["ClosingBalance"]) : 0,
                                BillDate = item["BillDate"] != null ? Convert.ToString(item["BillDate"]) : "",
                                ClosingRedg = item["ClosingRedg"] != null ? Convert.ToDecimal(item["ClosingRedg"]) : 0,
                                PreviousBillDate = item["PreviousBillDate"] != null ? Convert.ToString(item["PreviousBillDate"]) : "",
                                PreviousRedg = item["PreviousRedg"] != null ? Convert.ToDecimal(item["PreviousRedg"]) : 0,
                                DueDate = item["DueDate"] != null ? Convert.ToString(item["DueDate"]) : "",
                                ConsumeUnit = item["ConsumeUnit"] != null ? Convert.ToDecimal(item["ConsumeUnit"]) : 0,
                                InputRate = item["InputRate"] != null ? Convert.ToDecimal(item["InputRate"]) : Convert.ToDecimal(2.6),
                                StockItemName = item["StockItemName"] != null ? Convert.ToString(item["StockItemName"]) : "",
                                ConsumptioninKG = item["CurrentKGS"] != null ? Convert.ToDecimal(item["CurrentKGS"]) : 0,
                                Rate = item["Rate"] != null ? Convert.ToDecimal(item["Rate"]) : 0,
                                Month = item["BillMonth"] != null ? Convert.ToString(Convert.ToInt32(item["BillMonth"])) : "",
                                ServiceAmt = item["ServiceAmt"] != null ? Convert.ToDecimal(item["ServiceAmt"]) : 0,
                                Arrears = item["Arrears"] != null ? Convert.ToDecimal(item["Arrears"]) : 0,
                                MinAmt = item["MinAmt"] != null ? Convert.ToDecimal(item["MinAmt"]) : 0,
                                LateFee = item["LateFee"] != null ? Convert.ToDecimal(item["LateFee"]) : 0,
                                GodownName = item["GodownName"] != null ? Convert.ToString(item["GodownName"]) : "",
                                ReconnectionAmt = item["ReconnectionAmt"] != null ? Convert.ToDecimal(item["ReconnectionAmt"]) : 0,
                                InvoiceValue = item["TotalAmt"] != null ? Convert.ToDecimal(item["TotalAmt"]) : 0,
                                GASCOMSUMPTIONLed = item["Led"] != null ? Convert.ToDecimal(item["Led"]) : 0,
                                diposit = item["PreviousDiposite"] != null ? Convert.ToDecimal(item["PreviousDiposite"]) : 0,
                                PreviousBalance = item["PreviousBalance"] != null ? Convert.ToDecimal(item["PreviousBalance"]) : 0,
                                Round = item["Round"] != null ? Convert.ToDecimal(item["Round"]) : 0,
                                Diff = item["Diff"] != null ? Convert.ToDecimal(item["Diff"]) : 0,
                                SGST = item["SGST"] != null ? Convert.ToDecimal(item["SGST"]) : 0,
                                CGST = item["CGST"] != null ? Convert.ToDecimal(item["CGST"]) : 0,
                                PLateFee = item["PreviousLateFree"] != null ? Convert.ToDecimal(item["PreviousLateFree"]) : 0,
                                AliasName = item["AliasName"] != null ? Convert.ToString(item["AliasName"]) : "",
                            });
                        }
                    }


                    var grid = new GridView();
                    List<CustomerBillInformationData> LstcustomerBillInforData = new List<CustomerBillInformationData>();
                    LstcustomerBillInforData = Bill;

                    SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
                    ExcelFile ef = new ExcelFile();
                    ExcelWorksheet ws = ef.Worksheets.Add("Sheet1");
                    if (LstcustomerBillInforData != null)
                    {
                        ws.Cells[0, 0].Value = "SR No";
                        ws.Cells[0, 1].Value = "Party Name";
                        ws.Cells[0, 2].Value = "Clg.Bal.";
                        ws.Cells[0, 3].Value = "BillNo";
                        ws.Cells[0, 4].Value = "Bill Date";
                        ws.Cells[0, 5].Value = "Cl.Read.";
                        ws.Cells[0, 6].Value = "Pre.BillDt.";
                        ws.Cells[0, 7].Value = "Pre.Read";
                        ws.Cells[0, 8].Value = "Due Date";
                        ws.Cells[0, 9].Value = "Cons.";
                        ws.Cells[0, 10].Value = "Input Rate";
                        ws.Cells[0, 11].Value = "Stock ItemName";
                        ws.Cells[0, 12].Value = "Cons.inKG";
                        ws.Cells[0, 13].Value = "Rate";
                        ws.Cells[0, 14].Value = "Month";
                        ws.Cells[0, 15].Value = "Srv.Amt.";
                        ws.Cells[0, 16].Value = "Arrears";
                        ws.Cells[0, 17].Value = "Min.Amt.";
                        ws.Cells[0, 18].Value = "Late Fees";
                        ws.Cells[0, 19].Value = "Godown Name";
                        ws.Cells[0, 20].Value = "Recont.";
                        ws.Cells[0, 21].Value = "Invoice Value [=ROUND(ROUND(((L2*M2)+R2+T2+O2+Q2),1),0)] ";//[=ROUND(ROUND(((L2*M2)+R2+T2+O2+Q2),1),0)]
                        ws.Cells[0, 22].Value = "GAS COMSUMPTION Led [=ROUND((L2*M2),1)]";//[=ROUND((L2*M2),1)]
                        ws.Cells[0, 23].Value = "Round";
                        ws.Cells[0, 24].Value = "Diff [=U2-W2]";// [=U2-W2]
                        ws.Cells[0, 25].Value = "Cgst Amt";
                        ws.Cells[0, 26].Value = "Sgst Amt";
                        ws.Cells[0, 27].Value = "Alias Name";
                        int i = 1;
                        foreach (CustomerBillInformationData item in LstcustomerBillInforData)
                        {
                            decimal InvoiceValue = totalamt(item.InvoiceValue, item.PLateFee);
                            decimal Round = Convert.ToInt32(InvoiceValue);
                            decimal differ = Round - InvoiceValue;
                            string formate = @"""""0.00";
                            string formate2 = @"""""0.000";
                            string formate1 = @"""""0";
                            string formate3 = @"""""0.00"" Cr""";
                            string formate4 = @"""""0.00"" Dr""";
                            decimal ConsumptioninKG = (item.ClosingRedg - item.PreviousRedg) * item.InputRate;
                            ws.Cells[i, 0].Value = i;
                            ws.Cells[i, 0].Style.NumberFormat = "@";
                            ws.Cells[i, 1].Value = item.PartyName;
                            ws.Cells[i, 1].Style.NumberFormat = "@";
                            if (item.Arrears < 0)
                            {
                                ws.Cells[i, 2].Value = item.ClosingBalance;
                                ws.Cells[i, 2].Style.NumberFormat = formate3; //negative
                            }
                            else
                            {
                                ws.Cells[i, 2].Value = item.ClosingBalance;
                                ws.Cells[i, 2].Style.NumberFormat = formate4;
                            }

                            ws.Cells[i, 3].Value = item.BillNo;
                            ws.Cells[i, 4].Value = item.BillDate;  //item.BillDate;"25-Jun-2019";
                            ws.Cells[i, 4].Style.NumberFormat = "@";
                            ws.Cells[i, 5].Value = item.ClosingRedg;
                            ws.Cells[i, 5].Style.NumberFormat = formate;
                            ws.Cells[i, 6].Value = item.PreviousBillDate;//"26-May-2019";
                            ws.Cells[i, 6].Style.NumberFormat = "@";
                            ws.Cells[i, 7].Value = item.PreviousRedg;
                            ws.Cells[i, 7].Style.NumberFormat = formate2;
                            ws.Cells[i, 8].Value = item.DueDate;// "07/07/2019";
                            ws.Cells[i, 8].Style.NumberFormat = "dd-mm-yyyy";
                            ws.Cells[i, 9].Value = item.ConsumeUnit; //Cons. Consumption
                            ws.Cells[i, 9].Style.NumberFormat = formate2;
                            ws.Cells[i, 10].Value = item.InputRate;
                            ws.Cells[i, 10].Style.NumberFormat = formate;
                            ws.Cells[i, 11].Value = item.StockItemName; //"GAS CONSUM  (PUNE)";
                            ws.Cells[i, 11].Style.NumberFormat = "@";
                            ws.Cells[i, 12].Value = item.ConsumptioninKG; //Cons.inKG
                            ws.Cells[i, 12].Style.NumberFormat = formate2;
                            ws.Cells[i, 13].Value = item.Rate;
                            ws.Cells[i, 13].Style.NumberFormat = formate;
                            ws.Cells[i, 14].Value = item.Month;
                            ws.Cells[i, 14].Style.NumberFormat = formate1;
                            ws.Cells[i, 15].Value = item.ServiceAmt;
                            ws.Cells[i, 15].Style.NumberFormat = formate;
                            ws.Cells[i, 16].Value = item.Arrears;
                            ws.Cells[i, 16].Style.NumberFormat = formate;
                            ws.Cells[i, 17].Value = item.MinAmt;   //(item.ClosingRedg == item.PreviousRedg) ? item.MinAmt : (Decimal?)null; //""; Min.Amt.
                            ws.Cells[i, 17].Style.NumberFormat = formate1;
                            ws.Cells[i, 18].Value = item.LateFee;
                            ws.Cells[i, 18].Style.NumberFormat = formate1;
                            ws.Cells[i, 19].Value = item.GodownName; //"Lunawat Cosmos Socitey";
                            ws.Cells[i, 19].Style.NumberFormat = "@";
                            ws.Cells[i, 20].Value = item.ReconnectionAmt;
                            ws.Cells[i, 20].Style.NumberFormat = formate1;
                            ws.Cells[i, 21].Value = InvoiceValue;//Convert.ToInt16(item.InvoiceValue);//Math.Round(Math.Round(((ConsumptioninKG * item.Rate) + item.LateFee + item.ReconnectionAmt + item.ServiceAmt + item.MinAmt), 1), 0); //Invoice Value [=ROUND(ROUND(((L2*M2)+R2+T2+O2+Q2),1),0)]
                            ws.Cells[i, 21].Style.NumberFormat = formate;
                            ws.Cells[i, 22].Value = item.GASCOMSUMPTIONLed;//GASCOMSUMPTIONLed [=ROUND((L2*M2),1)]
                            ws.Cells[i, 22].Style.NumberFormat = formate;
                            ws.Cells[i, 23].Value = Round; //item.Round;////(item.InvoiceValue - Math.Round(item.InvoiceValue, 0));
                            ws.Cells[i, 23].Style.NumberFormat = formate;
                            ws.Cells[i, 24].Value = differ;  //item.Diff;////(item.InvoiceValue - Math.Round(item.InvoiceValue, 0));   //item.Diff; Diff [=U2-W2]
                            ws.Cells[i, 24].Style.NumberFormat = formate;
                            ws.Cells[i, 25].Value = item.CGST; //Cgst Amt
                            ws.Cells[i, 25].Style.NumberFormat = formate;
                            ws.Cells[i, 26].Value = item.SGST; //Sgst Amt
                            ws.Cells[i, 26].Style.NumberFormat = formate;
                            ws.Cells[i, 27].Value = item.AliasName;
                            ws.Cells[i, 27].Style.NumberFormat = "@";
                            i++;
                        }

                        root = Server.MapPath("~/Bill");
                        if (!Directory.Exists(root)) { Directory.CreateDirectory(root); }
                        //ws.Cells.GetSubrangeAbsolute(4, 0, 4, 7).Merged = true;

                        string sFileName = "CustomerBillDetails.xlsx";
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
                    else
                    {
                        ViewBag.Error = Resource1.DataNotExist;
                    }

                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message + "Erorr:Line  PartyName:"+name;
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            return View("DataFileList");
        }

        [HttpGet]
        public ActionResult PaymentDownload(string FileName)
        {
            string name = "";
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                try
                {
                    string root = HttpContext.Server.MapPath("~/DataFiles");
                    string filepath = root + "/" + FileName;
                    DataTable dt3 = ConvertCSVtoDataTable(filepath);

                    List<ExportCustomerData> Bill = new List<ExportCustomerData>();
                    foreach (DataRow item in dt3.Rows)
                    {
                        name = Convert.ToString(item["Name"]);
                        Bill.Add(new ExportCustomerData
                        {
                            Name = item["Name"] != null ? Convert.ToString(item["Name"]):"",
                            ALIAS = item["ALIAS"] != null ? Convert.ToString(item["ALIAS"]) : "",
                            BILLWISEDETAILS = item["BILLWISEDETAILS"] != null ? Convert.ToString(item["BILLWISEDETAILS"]) : "",
                            DUEDATE = item["DUEDATE"] != null ? Convert.ToString(item["DUEDATE"]) : "",
                            Address = item["Address"] != null ? Convert.ToString(item["Address"]) : "",
                            PINCODE = item["PINCODE"] != null ? Convert.ToString(item["PINCODE"]) : "",
                            CONTACTPERSON = item["CONTACTPERSON"] != null ? item["CONTACTPERSON"].ToString() : "",
                            Phone = item["Phone"] != null ? Convert.ToString(item["Phone"]) : "",
                            MOBILE = item["MOBILE"] != null ? Convert.ToString(item["MOBILE"]) : "",
                            EMAIL = item["EMAIL"] != null ? Convert.ToString(item["EMAIL"]) : "",
                            ClosingBalance = item["ClosingBalance"] != null ? Convert.ToDecimal(item["ClosingBalance"]) : 0,
                            BillType = item["BillType"] != null ? Convert.ToString(item["BillType"]) : "",
                            GodownMastername = item["GodownMastername"] != null ? Convert.ToString(item["GodownMastername"]) : "",
                            ReceiptNo = item["ReceiptNo"] != null ? Convert.ToString(item["ReceiptNo"]) : "",
                            BankName = item["BankName"] != null ? Convert.ToString(item["BankName"]) : "",
                            AccountNo = item["AccountNo"] != null ? Convert.ToString(item["AccountNo"]) : "",
                            ChequeNo = item["ChequeNo"] != null ? Convert.ToString(item["ChequeNo"]) : "",
                            PaymentType = item["PaymentType"] != null ? Convert.ToString(item["PaymentType"]) : "",
                            TallyName = item["TallyName"] != null ? Convert.ToString(item["TallyName"]) : "",
                            InitialMeterReading = item["ClosingRedg"] != null ? Convert.ToDecimal(item["ClosingRedg"]) : 0,
                            DepositeAccountNo = item["DepositeAccountNo"] != null ? Convert.ToString(item["DepositeAccountNo"]) : "",
                            DepositeBankName = item["DepositeBankName"] != null ? Convert.ToString(item["DepositeBankName"]) : "",
                            Voucherdate = item["Voucherdate"] != null ? Convert.ToString(item["Voucherdate"]) : "",
                            CustomerNumber = item["CustomerNumber"] != null ? Convert.ToString(item["CustomerNumber"]) : "",

                            Type = "GasConsume"
                        });
                    }

                    List<ExportCustomerData> LstcustomerBillInforData = new List<ExportCustomerData>();
                    LstcustomerBillInforData = Bill;
                    SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
                    ExcelFile ef = new ExcelFile();
                    ExcelWorksheet ws = ef.Worksheets.Add("Sheet1");
                    if (LstcustomerBillInforData != null)
                    {
                        int i = 1;
                        foreach (ExportCustomerData item in LstcustomerBillInforData)
                        {
                            if (i == 1)
                            {
                                ws.Cells[0, 0].Value = "SR No";
                                ws.Cells[0, 1].Value = "Name";
                                ws.Cells[0, 2].Value = "SOCIETY";
                                ws.Cells[0, 3].Value = "ALIAS";
                                ws.Cells[0, 4].Value = "Phone";
                                ws.Cells[0, 5].Value = "EMAIL";
                                ws.Cells[0, 6].Value = "Address";
                                ws.Cells[0, 7].Value = "Voucher date";
                                ws.Cells[0, 8].Value = "Amount";
                                ws.Cells[0, 9].Value = "deposit Type";
                                ws.Cells[0, 10].Value = "Narration";
                                ws.Cells[0, 11].Value = "ReceiptNo";
                                ws.Cells[0, 12].Value = "AccountNo";
                                ws.Cells[0, 13].Value = "ChequeNo";
                                ws.Cells[0, 14].Value = "BankName";
                                ws.Cells[0, 15].Value = "Deposite Bank";
                                ws.Cells[0, 16].Value = "Deposite Bank AccountNo.";

                            }

                            ws.Cells[i, 0].Value = i;
                            ws.Cells[i, 1].Value = item.Name;
                            ws.Cells[i, 2].Value = item.GodownMastername;
                            ws.Cells[i, 3].Value = item.ALIAS;
                            ws.Cells[i, 4].Value = item.Phone.TrimEnd(',').TrimStart(',');
                            ws.Cells[i, 5].Value = item.EMAIL.TrimEnd(',').TrimStart(',');
                            ws.Cells[i, 6].Value = item.Address;
                            ws.Cells[i, 7].Value = item.Voucherdate;
                            ws.Cells[i, 8].Value = item.ClosingBalance;
                            ws.Cells[i, 9].Value = item.PaymentType;
                            ws.Cells[i, 10].Value = item.Type;
                            ws.Cells[i, 11].Value = item.ReceiptNo;
                            ws.Cells[i, 12].Value = item.AccountNo;
                            ws.Cells[i, 13].Value = item.ChequeNo;
                            ws.Cells[i, 14].Value = item.BankName;
                            ws.Cells[i, 15].Value = item.DepositeBankName;
                            ws.Cells[i, 16].Value = item.DepositeAccountNo;

                            i++;
                        }



                        root = Server.MapPath("~/Bill");
                        if (!Directory.Exists(root)) { Directory.CreateDirectory(root); }

                        string sFileName = "CustomerReceiptDetails.xlsx";
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
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message + "Erorr:Line  PartyName:" + name;
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            return View("DataFileList");
        }

        [HttpGet]
        public ActionResult DownloadFile(string bill,string payment)
        {
           
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
               string    root = Server.MapPath("~/DataFiles");
                if (!Directory.Exists(root)) { Directory.CreateDirectory(root); }
                if (bill != "")
                {
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + bill + "");
                    Response.TransmitFile(root + "\\" + bill);
                    Response.End();
                    if (System.IO.File.Exists(root + "\\" + bill))
                    {
                        System.IO.File.Delete(root + "\\" + bill);
                    }
                }
                else
                {
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + payment + "");
                    Response.TransmitFile(root + "\\" + payment);
                    Response.End();
                    if (System.IO.File.Exists(root + "\\" + payment))
                    {
                        System.IO.File.Delete(root + "\\" + payment);
                    }
                }

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            return View("DataFileList");
        }
    }
}