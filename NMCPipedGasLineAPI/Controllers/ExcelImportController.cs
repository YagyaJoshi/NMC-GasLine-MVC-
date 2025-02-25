using AutoMapper;
using ClosedXML.Excel;
using ExcelDataReader;
using GemBox.Spreadsheet;
using NMCDataAccesslayer.BL;
using NMCDataAccesslayer.DataModel;
using NMCPipedGasLineAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NMCDataAccesslayer.Helper;
using NMCPipedGasLineAPI.Properties;
using System.Configuration;
using System.Web;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using OfficeOpenXml;

namespace NMCPipedGasLineAPI.Controllers
{
    public class ExcelImportController : Controller
    {
        CustomerLoginBL ObjchL = new CustomerLoginBL();
        Customer objDM = new Customer();
        ImCustomer objDIM = new ImCustomer();
        UserBL objUBL = new UserBL();
        CompanyBL ObjBL = new CompanyBL();
        AreaBL areaBL = new AreaBL();
        GodownBL godownBL = new GodownBL();
        CustomerBL objCustomer = new CustomerBL();
        string Password, Msgbody, CustomerSms = "";
        // GET: ExcelImport
        public ActionResult ImportExcel()
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                CustomerDataModel modleDATA = new CustomerDataModel();
                modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<CustomerDataModel, ImCustomer>();
                });
                objDIM = Mapper.Map<CustomerDataModel, ImCustomer>(modleDATA);
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


        [ActionName("Importexcel")]
        [HttpPost]
        public async Task<ActionResult> ImportExcel1(ImCustomer ObjCustomer)
        {

            CustomerDataModel modleDATA = new CustomerDataModel();
            List<ImportCustomer> lst = new List<ImportCustomer>();
            try
            {
                AutoMapper.Mapper.Reset();
                if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
                {
                    if (ModelState.IsValid)
                    {

                        string name3 = Request.Files["FileUpload1"].FileName;
                        string path3 = Server.MapPath("~/") + "ExcelFile/";
                        string extension1 = Path.GetExtension(Request.Files["FileUpload1"].FileName).ToLower();

                        if (extension1.ToLower() == ".xls" || extension1.ToLower() == ".xlsx")
                        {

                            string path4 = Server.MapPath("~/") + "ExcelFile/" + Request.Files["FileUpload1"].FileName;
                            if (!Directory.Exists(Server.MapPath("~/") + "ExcelFile/")) { Directory.CreateDirectory(Server.MapPath("~/") + "ExcelFile/"); }
                            if (System.IO.File.Exists(path4))
                                System.IO.File.Delete(path4);
                            Request.Files["FileUpload1"].SaveAs(path4);
                            FileStream stream5 = System.IO.File.Open(path4, FileMode.Open, FileAccess.Read);
                            ISheet sheet;
                            DataTable dt2 = new DataTable();
                            if (extension1 == ".xls")
                            {
                                HSSFWorkbook hssfwb = new HSSFWorkbook(stream5); //HSSWorkBook object will read the Excel 97-2000 formats  
                                sheet = hssfwb.GetSheetAt(0); //get first Excel sheet from workbook  
                            }
                            else
                            {
                                XSSFWorkbook hssfwb = new XSSFWorkbook(stream5); //XSSFWorkBook will read 2007 Excel format  
                                sheet = hssfwb.GetSheetAt(0); //get first Excel sheet from workbook   
                            }

                            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

                            IRow headerRow = sheet.GetRow(0);
                            int cellCount = headerRow.LastCellNum;
                            DataFormatter formatter = new DataFormatter();
                            for (int j = 0; j < cellCount; j++)
                            {
                                ICell cell = headerRow.GetCell(j);
                                dt2.Columns.Add(cell.ToString());
                            }

                            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                            {
                                IRow row = sheet.GetRow(i);
                                DataRow dataRow = dt2.NewRow();
                                if (row == null)
                                {
                                    break;
                                }
                                for (int j = row.FirstCellNum; j < cellCount; j++)
                                {
                                    if (row.GetCell(j) != null)

                                        if (headerRow.GetCell(j).ToString().Trim() == "Closing Balance".Trim())
                                        {
                                            decimal data = 0;
                                            string coldata = formatter.FormatCellValue(row.GetCell(j));
                                            string c = coldata.ToUpper();
                                            if (coldata.ToUpper().Contains("DR"))
                                            {
                                                coldata = coldata.ToUpper().Replace("DR", "");
                                                data = Convert.ToDecimal(coldata) * -1;
                                            }
                                            else if (coldata.ToUpper().Contains("CR"))
                                            {
                                                coldata = coldata.ToUpper().Replace("CR", "");
                                                data = Convert.ToDecimal(coldata) * 1;
                                            }
                                            dataRow[j] = data;

                                        }
                                        else
                                        {
                                            dataRow[j] = formatter.FormatCellValue(row.GetCell(j));
                                        }
                                }

                                dt2.Rows.Add(dataRow);
                            }
                            DataTable dt3 = dt2;

                            string GetCustomerunique = objCustomer.GetCustomerunique(ObjCustomer.GodownId);
                            string shortName = objCustomer.GetGodownName(ObjCustomer.GodownId);
                            int n = Convert.ToInt32(GetCustomerunique) + 1;
                            foreach (DataRow item in dt3.Rows)
                            {

                                ImportCustomer obj = new ImportCustomer();
                                if (item["Party Name"].ToString() != "")
                                {
                                    obj.TallyName = item["Party Name"].ToString();
                                    if (item["Closing Balance"].ToString() != "")
                                    {
                                        if (item["Closing Balance"].ToString().Contains('-'))
                                        {
                                            obj.PreviousDue = item["Closing Balance"].ToString() != "" ? Convert.ToDecimal(item["Closing Balance"].ToString().Replace("-", "")) : 0;
                                            obj.ClosingBalance = 0;
                                        }
                                        else
                                        {
                                            obj.PreviousDue = 0;
                                            obj.ClosingBalance = item["Closing Balance"].ToString() != "" ? Convert.ToDecimal(item["Closing Balance"]) : 0;
                                        }
                                    }
                                    string CustomerNumber = "NMC-" + shortName + '-' + "000" + Convert.ToString(n);
                                    obj.PrevBillDate = item["Prev. Bill Date"] == null ? (DateTime?)null : (item["Prev. Bill Date"].ToString().Trim() != "" ? Convert.ToDateTime(item["Prev. Bill Date"]) : (DateTime?)null);
                       
                                    obj.PrevReading = item["Prev.Reading"]==null?0:( item["Prev.Reading"].ToString().Trim() != "" ? Convert.ToDecimal(item["Prev.Reading"]) : 0);
                                    obj.ContactNos = item["Contact Nos."].ToString();
                                    obj.EmailId = item["Email Id"].ToString().Replace("/", ",");
                                    obj.AreaId = ObjCustomer.AreaId;
                                    obj.GodownId = ObjCustomer.GodownId;
                                    obj.CustomerNumber = CustomerNumber;
                                    obj.Customerunique = n;
                                    if (obj.TallyName.EndsWith("(D)"))
                                    { obj.IsActive = 2; }
                                    else
                                    { obj.IsActive = 1; }
                                    obj.FlatNo = item["FLAT NO"].ToString();
                                    obj.AliasName = item["ALIAS NAME"].ToString();
                                    lst.Add(obj);
                                    n++;
                                }
                            }
                        }
                        else
                        {
                            modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                            modleDATA.Area = areaBL.GetArea(ObjCustomer.CompanyId);
                            modleDATA.GoDown = await godownBL.GetGoDownAreawise(ObjCustomer.AreaId);
                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<CustomerDataModel, ImCustomer>();
                            });
                            ObjCustomer = Mapper.Map<CustomerDataModel, ImCustomer>(modleDATA);

                            ViewBag.Error = Resource1.FileFormate;
                            return View(ObjCustomer);
                        }

                        List<ImportCustomerData> customerDataModel = new List<ImportCustomerData>();
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<List<ImportCustomerData>, List<ImportCustomer>>();
                        });
                        customerDataModel = Mapper.Map<List<ImportCustomer>, List<ImportCustomerData>>(lst);
                        AutoMapper.Mapper.Reset();

                        DataTable datadt = objCustomer.Save(customerDataModel, ObjCustomer.CompanyId, Session["user"].ToString(), ObjCustomer.GodownId);

                        foreach (DataRow item in datadt.Rows)
                        {  //item.MailEMAIL = "jagratisyn@gmail.com";
                            if (item["PreviousBillDate"] != DBNull.Value && item["PreviousRedg"].ToString() != "0.00")
                            {
                                if (Convert.ToInt32(item["IsActive"]) != 2)
                                {
                                    if (item["email"].ToString() != "")
                                    {
                                        Password = ObjchL.GenratePass();
                                        Msgbody = Resource1.CustomerEmail.Replace("@Fname", item["Name"].ToString()).Replace("@CustomerNumber", item["CustomerNumber"].ToString()).Replace("@password", Password);
                                        CustomerSms = Resource1.CustomerSms.Replace("@Fname", item["Name"].ToString()).Replace("@CustomerNumber", item["CustomerNumber"].ToString()).Replace("@password", Password).Replace("@n", Environment.NewLine);
                                        foreach (var email in item["email"].ToString().Split(','))
                                        {
                                            var res = ObjchL.SendEMailSms(item["Id"].ToString(), item["Name"].ToString(), item["Phone"].ToString(), email, Msgbody, CustomerSms, "", Password);
                                            if (res != "Save")
                                            {
                                                ViewBag.Error = "Error in Password saving";
                                                return View(ObjCustomer);
                                            }
                                        }
                                    }
                                    else if (item["Phone"].ToString() != "")
                                    {
                                        Password = ObjchL.GenratePass();
                                        CustomerSms = Resource1.CustomerSms.Replace("@Fname", item["Name"].ToString()).Replace("@CustomerNumber", item["CustomerNumber"].ToString()).Replace("@password", Password).Replace("@n", Environment.NewLine);
                                        var res = ObjchL.SendEMailSms(item["Id"].ToString(), item["Name"].ToString(), item["Phone"].ToString(), "", "", CustomerSms, "", Password);
                                        if (res != "Save")
                                        {
                                            ViewBag.Error = "Error in Password saving";
                                            return View(ObjCustomer);
                                        }
                                    }
                                }
                            }
                        }


                        modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());

                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<CustomerDataModel, Customer>();
                        });

                        modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                        modleDATA.Area = areaBL.GetArea(ObjCustomer.CompanyId);
                        modleDATA.GoDown = await godownBL.GetGoDownAreawise(ObjCustomer.AreaId);

                        ObjCustomer = Mapper.Map<CustomerDataModel, ImCustomer>(modleDATA);

                        ViewBag.Error = Resource1.CustomerImported;
                        TempData["Msg"] = Resource1.CustomerImported;
                        return RedirectToAction("List", "Customer");

                    }
                    else
                    {
                        modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                        if (ObjCustomer.CompanyId != null)
                        {
                            modleDATA.Area = areaBL.GetArea(ObjCustomer.CompanyId);
                        }

                        if (ObjCustomer.AreaId != null)
                        { modleDATA.GoDown = await godownBL.GetGoDownAreawise(ObjCustomer.AreaId); }

                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<CustomerDataModel, Customer>();
                        });
                        ObjCustomer = Mapper.Map<CustomerDataModel, ImCustomer>(modleDATA);
                        string messages = string.Join(Environment.NewLine, ModelState.Values
                                 .SelectMany(x => x.Errors)
                                 .Select(x => x.Exception));
                        ViewBag.Error = messages;
                    }
                    return View(ObjCustomer);
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                modleDATA.Area = areaBL.GetArea(ObjCustomer.CompanyId);
                modleDATA.GoDown = await godownBL.GetGoDownAreawise(ObjCustomer.AreaId);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<CustomerDataModel, Customer>();
                });
                ObjCustomer = Mapper.Map<CustomerDataModel, ImCustomer>(modleDATA);

                ViewBag.Error = ex.Message;
                return View(ObjCustomer);
            }
        }


        public ActionResult ExportToExcel()
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                CustomerDataModel modleDATA = new CustomerDataModel();
                modleDATA.Company = ObjBL.GetCompany(Convert.ToString(Session["RoleName"]), Convert.ToString(Session["City"]));
                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<CustomerDataModel, Customer>();
                });
                objDM = Mapper.Map<CustomerDataModel, Customer>(modleDATA);
                if (Convert.ToString(Session["RoleName"]) != "Super Admin")
                {
                    objDM.CompanyId = objDM.Company.ToList().FirstOrDefault().Id.ToString();
                    objDM.CompanyName = objDM.Company.ToList().FirstOrDefault().CompanyName.ToString();
                }
                return View(objDM);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }

        public string GenratePass()
        {
            var chars = ConfigurationManager.AppSettings["ForgotKey"].ToString();
            var stringChars = new char[6];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            String ReferralCode = new String(stringChars);
            return ReferralCode;

        }

        public decimal totalamt(decimal Totalamt, decimal PLateFee)
        {
            decimal totalAmount = Totalamt + PLateFee + (PLateFee * 18 / 100);
            return totalAmount;
        }

        //export bills
        //[ActionName("ExportToExcel")]
        //[HttpPost]
        //public async Task<ActionResult> ExportToExcel(Customer objmodelCustomer)
        //{
        //    CustomerDataModel modleDATA = new CustomerDataModel();
        //    if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var grid = new GridView();
        //            List<CustomerBillInformationData> LstcustomerBillInforData = new List<CustomerBillInformationData>();
        //            LstcustomerBillInforData = objCustomer.GetAllBill(objmodelCustomer.CompanyId, objmodelCustomer.GodownId, objmodelCustomer.AreaId, objmodelCustomer.BillTypeId);

        //            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
        //            ExcelFile ef = new ExcelFile();
        //            GemBox.Spreadsheet.ExcelWorksheet ws = ef.Worksheets.Add("Sheet1");
        //            if (LstcustomerBillInforData != null)
        //            {
        //                ws.Cells[0, 0].Value = "SR No";
        //                ws.Cells[0, 1].Value = "Party Name";
        //                ws.Cells[0, 2].Value = "Clg.Bal.";
        //                ws.Cells[0, 3].Value = "BillNo";
        //                ws.Cells[0, 4].Value = "Bill Date";
        //                ws.Cells[0, 5].Value = "Cl.Read.";
        //                ws.Cells[0, 6].Value = "Pre.BillDt.";
        //                ws.Cells[0, 7].Value = "Pre.Read";
        //                ws.Cells[0, 8].Value = "Due Date";
        //                ws.Cells[0, 9].Value = "Cons.";
        //                ws.Cells[0, 10].Value = "Input Rate";
        //                ws.Cells[0, 11].Value = "Stock ItemName";
        //                ws.Cells[0, 12].Value = "Cons.inKG";
        //                ws.Cells[0, 13].Value = "Rate";
        //                ws.Cells[0, 14].Value = "Month";
        //                ws.Cells[0, 15].Value = "Srv.Amt.";
        //                ws.Cells[0, 16].Value = "Arrears";
        //                ws.Cells[0, 17].Value = "Min.Amt.";
        //                ws.Cells[0, 18].Value = "Late Fees";
        //                ws.Cells[0, 19].Value = "Godown Name";
        //                ws.Cells[0, 20].Value = "Recont.";
        //                ws.Cells[0, 21].Value = "Invoice Value [=ROUND(ROUND(((L2*M2)+R2+T2+O2+Q2),1),0)] ";//[=ROUND(ROUND(((L2*M2)+R2+T2+O2+Q2),1),0)]
        //                ws.Cells[0, 22].Value = "GAS COMSUMPTION Led [=ROUND((L2*M2),1)]";//[=ROUND((L2*M2),1)]
        //                ws.Cells[0, 23].Value = "Round";
        //                ws.Cells[0, 24].Value = "Diff [=U2-W2]";// [=U2-W2]
        //                ws.Cells[0, 25].Value = "Cgst Amt";
        //                ws.Cells[0, 26].Value = "Sgst Amt";
        //                ws.Cells[0, 27].Value = "Alias Name";
        //                int i = 1;
        //                foreach (CustomerBillInformationData item in LstcustomerBillInforData)
        //                {
        //                    decimal InvoiceValue = totalamt(item.InvoiceValue, item.PLateFee);
        //                    decimal Round = Convert.ToInt32(InvoiceValue);
        //                    decimal differ = Round - InvoiceValue;
        //                    string formate = @"""""0.00";
        //                    string formate2 = @"""""0.000";
        //                    string formate1 = @"""""0";
        //                    string formate3 = @"""""0.00"" Cr""";
        //                    string formate4 = @"""""0.00"" Dr""";
        //                    decimal ConsumptioninKG = (item.ClosingRedg - item.PreviousRedg) * item.InputRate;
        //                    ws.Cells[i, 0].Value = i;
        //                    ws.Cells[i, 0].Style.NumberFormat = "@";
        //                    ws.Cells[i, 1].Value = item.PartyName;
        //                    ws.Cells[i, 1].Style.NumberFormat = "@";
        //                    if (item.Arrears < 0)
        //                    {
        //                        ws.Cells[i, 2].Value = item.ClosingBalance;
        //                        ws.Cells[i, 2].Style.NumberFormat = formate3; //negative
        //                    }
        //                    else
        //                    {
        //                        ws.Cells[i, 2].Value = item.ClosingBalance;
        //                        ws.Cells[i, 2].Style.NumberFormat = formate4;
        //                    }

        //                    ws.Cells[i, 3].Value = item.BillNo;
        //                    ws.Cells[i, 4].Value = item.BillDate;  //item.BillDate;"25-Jun-2019";
        //                    ws.Cells[i, 4].Style.NumberFormat = "@";
        //                    ws.Cells[i, 5].Value = item.ClosingRedg;
        //                    ws.Cells[i, 5].Style.NumberFormat = formate;
        //                    ws.Cells[i, 6].Value = item.PreviousBillDate;//"26-May-2019";
        //                    ws.Cells[i, 6].Style.NumberFormat = "@";
        //                    ws.Cells[i, 7].Value = item.PreviousRedg;
        //                    ws.Cells[i, 7].Style.NumberFormat = formate2;
        //                    ws.Cells[i, 8].Value = item.DueDate;// "07/07/2019";
        //                    ws.Cells[i, 8].Style.NumberFormat = "dd-mm-yyyy";
        //                    ws.Cells[i, 9].Value = item.ConsumeUnit; //Cons. Consumption
        //                    ws.Cells[i, 9].Style.NumberFormat = formate2;
        //                    ws.Cells[i, 10].Value = item.InputRate;
        //                    ws.Cells[i, 10].Style.NumberFormat = formate;
        //                    ws.Cells[i, 11].Value = item.StockItemName; //"GAS CONSUM  (PUNE)";
        //                    ws.Cells[i, 11].Style.NumberFormat = "@";
        //                    ws.Cells[i, 12].Value = item.ConsumptioninKG; //Cons.inKG
        //                    ws.Cells[i, 12].Style.NumberFormat = formate2;
        //                    ws.Cells[i, 13].Value = item.Rate;
        //                    ws.Cells[i, 13].Style.NumberFormat = formate;
        //                    ws.Cells[i, 14].Value = item.Month;
        //                    ws.Cells[i, 14].Style.NumberFormat = formate1;
        //                    ws.Cells[i, 15].Value = item.ServiceAmt;
        //                    ws.Cells[i, 15].Style.NumberFormat = formate;
        //                    ws.Cells[i, 16].Value = item.Arrears;
        //                    ws.Cells[i, 16].Style.NumberFormat = formate;
        //                    ws.Cells[i, 17].Value = item.MinAmt;   //(item.ClosingRedg == item.PreviousRedg) ? item.MinAmt : (Decimal?)null; //""; Min.Amt.
        //                    ws.Cells[i, 17].Style.NumberFormat = formate1;
        //                    ws.Cells[i, 18].Value = item.LateFee;
        //                    ws.Cells[i, 18].Style.NumberFormat = formate1;
        //                    ws.Cells[i, 19].Value = item.GodownName; //"Lunawat Cosmos Socitey";
        //                    ws.Cells[i, 19].Style.NumberFormat = "@";
        //                    ws.Cells[i, 20].Value = item.ReconnectionAmt;
        //                    ws.Cells[i, 20].Style.NumberFormat = formate1;
        //                    ws.Cells[i, 21].Value = InvoiceValue;//Convert.ToInt16(item.InvoiceValue);//Math.Round(Math.Round(((ConsumptioninKG * item.Rate) + item.LateFee + item.ReconnectionAmt + item.ServiceAmt + item.MinAmt), 1), 0); //Invoice Value [=ROUND(ROUND(((L2*M2)+R2+T2+O2+Q2),1),0)]
        //                    ws.Cells[i, 21].Style.NumberFormat = formate;
        //                    ws.Cells[i, 22].Value = item.GASCOMSUMPTIONLed;//GASCOMSUMPTIONLed [=ROUND((L2*M2),1)]
        //                    ws.Cells[i, 22].Style.NumberFormat = formate;
        //                    ws.Cells[i, 23].Value = Round; //item.Round;////(item.InvoiceValue - Math.Round(item.InvoiceValue, 0));
        //                    ws.Cells[i, 23].Style.NumberFormat = formate;
        //                    ws.Cells[i, 24].Value = differ;  //item.Diff;////(item.InvoiceValue - Math.Round(item.InvoiceValue, 0));   //item.Diff; Diff [=U2-W2]
        //                    ws.Cells[i, 24].Style.NumberFormat = formate;
        //                    ws.Cells[i, 25].Value = item.CGST; //Cgst Amt
        //                    ws.Cells[i, 25].Style.NumberFormat = formate;
        //                    ws.Cells[i, 26].Value = item.SGST; //Sgst Amt
        //                    ws.Cells[i, 26].Style.NumberFormat = formate;
        //                    ws.Cells[i, 27].Value = item.AliasName;
        //                    ws.Cells[i, 27].Style.NumberFormat = "@";
        //                    i++;


        //                    //ws.Cells[1, 0].Value = 1;
        //                    //ws.Cells[1, 1].Value = "AnkitKumar,L1,Lunawatcosmossocitey";
        //                    //ws.Cells[1, 2].Value = "27.63";
        //                    //ws.Cells[1, 3].Value = "26-Jun-2019";
        //                    //ws.Cells[1, 4].Value = "2";
        //                    //ws.Cells[1, 5].Value = "26-May-2019";
        //                    //ws.Cells[1, 6].Value = "1";
        //                    //ws.Cells[1, 7].Value = "07/07/2019";
        //                    //ws.Cells[1, 8].Value = "1";
        //                    //ws.Cells[1, 9].Value = "0";
        //                    //ws.Cells[1, 10].Value = "GAS CONSUM  (PUNE)";
        //                    //ws.Cells[1, 11].Value = "0";
        //                    //ws.Cells[1, 12].Value = "0";
        //                    //ws.Cells[1, 13].Value = "1";
        //                    //ws.Cells[1, 14].Value = "50";
        //                    //ws.Cells[1, 15].Value = "-27.63";
        //                    //ws.Cells[1, 16].Value = "";
        //                    //ws.Cells[1, 17].Value = "";
        //                    //ws.Cells[1, 18].Value = "Lunawat Cosmos Socitey";
        //                    //ws.Cells[1, 19].Value = "";
        //                    //ws.Cells[1, 20].Value = "59";
        //                    //ws.Cells[1, 21].Value = "";
        //                    //ws.Cells[1, 22].Value = "59";
        //                    //ws.Cells[1, 23].Value = "";
        //                    //ws.Cells[1, 24].Value = "4.50";
        //                    //ws.Cells[1, 25].Value = "4.50";
        //                }

        //                string root = Server.MapPath("~/Bill");
        //                if (!Directory.Exists(root)) { Directory.CreateDirectory(root); }
        //                //ws.Cells.GetSubrangeAbsolute(4, 0, 4, 7).Merged = true;

        //                string sFileName = "CustomerBillDetails.xlsx";
        //                ef.Save(root + "\\" + sFileName);
        //                Response.ContentType = "application/ms-excel";
        //                Response.AppendHeader("Content-Disposition", "attachment; filename=" + sFileName + "");
        //                Response.TransmitFile(root + "\\" + sFileName);
        //                Response.End();
        //                if (System.IO.File.Exists(root + "\\" + sFileName))
        //                {
        //                    System.IO.File.Delete(root + "\\" + sFileName);
        //                }

        //                //ef.Save(sFileName);



        //                //    Response.Clear();
        //                //    Response.Buffer = true;
        //                //    Response.Charset = "";
        //                //Response.ContentType = "application/ms-excel";
        //                //Response.AddHeader("content-disposition", "attachment;filename=" + sFileName + "");
        //                //    using (MemoryStream MyMemoryStream = new MemoryStream())
        //                //    {
        //                //    //ef.Save(MyMemoryStream, sFileName);
        //                //    //ef.SaveAs(MyMemoryStream);
        //                //        MyMemoryStream.WriteTo(Response.OutputStream);
        //                //        Response.Flush();
        //                //        Response.End();
        //                //    }


        //            }
        //            else
        //            {
        //                ViewBag.Error = Resource1.DataNotExist;


        //            }
        //            AutoMapper.Mapper.Reset();
        //            modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
        //            Mapper.Initialize(cfg =>
        //            {
        //                cfg.CreateMap<CustomerDataModel, Customer>();
        //            });
        //            objDM = Mapper.Map<CustomerDataModel, Customer>(modleDATA);
        //            AutoMapper.Mapper.Reset();


        //            return View("ExportToExcel", objDM);

        //        }
        //        else
        //        {
        //            modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
        //            if (objmodelCustomer.CompanyId != null)
        //                modleDATA.Area = areaBL.GetArea(objmodelCustomer.CompanyId);
        //            if (objmodelCustomer.AreaId != null)
        //                modleDATA.GoDown = await godownBL.GetGoDownAreawise(objmodelCustomer.AreaId);
        //            AutoMapper.Mapper.Reset();
        //            Mapper.Initialize(cfg =>
        //            {
        //                cfg.CreateMap<CustomerDataModel, Customer>();
        //            });
        //            objDM = Mapper.Map<CustomerDataModel, Customer>(modleDATA);

        //            return View(objDM);
        //        }

        //    }
        //    else
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }
        //}

        [ActionName("ExportToExcel")]
        [HttpPost]
        public async Task<ActionResult> ExportToExcel(Customer objmodelCustomer)
        {
            CustomerDataModel modleDATA = new CustomerDataModel();
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                if (ModelState.IsValid)
                {
                    var grid = new GridView();
                    List<CustomerBillInformationData> LstcustomerBillInforData = new List<CustomerBillInformationData>();
                    LstcustomerBillInforData = objCustomer.GetAllBill(objmodelCustomer.CompanyId, objmodelCustomer.GodownId, objmodelCustomer.AreaId, objmodelCustomer.BillTypeId);

                    //SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
                    //ExcelFile ef = new ExcelFile();
                    //GemBox.Spreadsheet.ExcelWorksheet ws = ef.Worksheets.Add("Sheet1");
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    ExcelPackage excel = new ExcelPackage();
                    var ws = excel.Workbook.Worksheets.Add("Sheet1");
                    if (LstcustomerBillInforData != null)
                    {
                        ws.Cells[1, 1].Value = "SR No";
                        ws.Cells[1, 2].Value = "Party Name";
                        ws.Cells[1, 3].Value = "Clg.Bal.";
                        ws.Cells[1, 4].Value = "BillNo";
                        ws.Cells[1, 5].Value = "Bill Date";
                        ws.Cells[1, 6].Value = "Cl.Read.";
                        ws.Cells[1, 7].Value = "Pre.BillDt.";
                        ws.Cells[1, 8].Value = "Pre.Read";
                        ws.Cells[1, 9].Value = "Due Date";
                        ws.Cells[1, 10].Value = "Cons.";
                        ws.Cells[1, 11].Value = "Input Rate";
                        ws.Cells[1, 12].Value = "Stock ItemName";
                        ws.Cells[1, 13].Value = "Cons.inKG";
                        ws.Cells[1, 14].Value = "Rate";
                        ws.Cells[1, 15].Value = "Month";
                        ws.Cells[1, 16].Value = "Srv.Amt.";
                        ws.Cells[1, 17].Value = "Arrears";
                        ws.Cells[1, 18].Value = "Min.Amt.";
                        ws.Cells[1, 19].Value = "Late Fees";
                        ws.Cells[1, 20].Value = "Godown Name";
                        ws.Cells[1, 21].Value = "Recont.";
                        ws.Cells[1, 22].Value = "Invoice Value [=ROUND(ROUND(((L2*M2)+R2+T2+O2+Q2),1),0)] ";//[=ROUND(ROUND(((L2*M2)+R2+T2+O2+Q2),1),0)]
                        ws.Cells[1, 23].Value = "GAS COMSUMPTION Led [=ROUND((L2*M2),1)]";//[=ROUND((L2*M2),1)]
                        ws.Cells[1, 24].Value = "Round";
                        ws.Cells[1, 25].Value = "Diff [=U2-W2]";// [=U2-W2]
                        ws.Cells[1, 26].Value = "Cgst Amt";
                        ws.Cells[1, 27].Value = "Sgst Amt";
                        ws.Cells[1, 28].Value = "Alias Name";
                        int i = 2;
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
                            ws.Cells[i, 1].Value = (i - 1);
                            ws.Cells[i, 1].Style.Numberformat.Format = "@";
                            ws.Cells[i, 2].Value = item.PartyName;
                            ws.Cells[i, 2].Style.Numberformat.Format = "@";
                            if (item.Arrears < 0)
                            {
                                ws.Cells[i, 3].Value = item.ClosingBalance;
                                ws.Cells[i, 3].Style.Numberformat.Format = formate3; //negative
                            }
                            else
                            {
                                ws.Cells[i, 3].Value = item.ClosingBalance;
                                ws.Cells[i, 3].Style.Numberformat.Format = formate4;
                            }

                            ws.Cells[i, 4].Value = item.BillNo;
                            ws.Cells[i, 5].Value = item.BillDate;  //item.BillDate;"25-Jun-2019";
                            ws.Cells[i, 5].Style.Numberformat.Format = "@";
                            ws.Cells[i, 6].Value = item.ClosingRedg;
                            ws.Cells[i, 6].Style.Numberformat.Format = formate;
                            ws.Cells[i, 7].Value = item.PreviousBillDate;//"26-May-2019";
                            ws.Cells[i, 7].Style.Numberformat.Format = "@";
                            ws.Cells[i, 8].Value = item.PreviousRedg;
                            ws.Cells[i, 8].Style.Numberformat.Format = formate2;
                            ws.Cells[i, 9].Value = item.DueDate;// "07/07/2019";
                            ws.Cells[i, 9].Style.Numberformat.Format = "dd-mm-yyyy";
                            ws.Cells[i, 10].Value = item.ConsumeUnit; //Cons. Consumption
                            ws.Cells[i, 10].Style.Numberformat.Format = formate2;
                            ws.Cells[i, 11].Value = item.InputRate;
                            ws.Cells[i, 11].Style.Numberformat.Format = formate;
                            ws.Cells[i, 12].Value = item.StockItemName; //"GAS CONSUM  (PUNE)";
                            ws.Cells[i, 12].Style.Numberformat.Format = "@";
                            ws.Cells[i, 13].Value = item.ConsumptioninKG; //Cons.inKG
                            ws.Cells[i, 13].Style.Numberformat.Format = formate2;
                            ws.Cells[i, 14].Value = item.Rate;
                            ws.Cells[i, 14].Style.Numberformat.Format = formate;
                            ws.Cells[i, 15].Value = item.Month;
                            ws.Cells[i, 15].Style.Numberformat.Format = formate1;
                            ws.Cells[i, 16].Value = item.ServiceAmt;
                            ws.Cells[i, 16].Style.Numberformat.Format = formate;
                            ws.Cells[i, 17].Value = item.Arrears;
                            ws.Cells[i, 17].Style.Numberformat.Format = formate;
                            ws.Cells[i, 18].Value = item.MinAmt;   //(item.ClosingRedg == item.PreviousRedg) ? item.MinAmt : (Decimal?)null; //""; Min.Amt.
                            ws.Cells[i, 18].Style.Numberformat.Format = formate1;
                            ws.Cells[i, 19].Value = item.LateFee;
                            ws.Cells[i, 19].Style.Numberformat.Format = formate1;
                            ws.Cells[i, 20].Value = item.GodownName; //"Lunawat Cosmos Socitey";
                            ws.Cells[i, 20].Style.Numberformat.Format = "@";
                            ws.Cells[i, 21].Value = item.ReconnectionAmt;
                            ws.Cells[i, 21].Style.Numberformat.Format = formate1;
                            ws.Cells[i, 22].Value = InvoiceValue;//Convert.ToInt16(item.InvoiceValue);//Math.Round(Math.Round(((ConsumptioninKG * item.Rate) + item.LateFee + item.ReconnectionAmt + item.ServiceAmt + item.MinAmt), 1), 0); //Invoice Value [=ROUND(ROUND(((L2*M2)+R2+T2+O2+Q2),1),0)]
                            ws.Cells[i, 22].Style.Numberformat.Format = formate;
                            ws.Cells[i, 23].Value = item.GASCOMSUMPTIONLed;//GASCOMSUMPTIONLed [=ROUND((L2*M2),1)]
                            ws.Cells[i, 23].Style.Numberformat.Format = formate;
                            ws.Cells[i, 24].Value = Round; //item.Round;////(item.InvoiceValue - Math.Round(item.InvoiceValue, 0));
                            ws.Cells[i, 24].Style.Numberformat.Format = formate;
                            ws.Cells[i, 25].Value = differ;  //item.Diff;////(item.InvoiceValue - Math.Round(item.InvoiceValue, 0));   //item.Diff; Diff [=U2-W2]
                            ws.Cells[i, 25].Style.Numberformat.Format = formate;
                            ws.Cells[i, 26].Value = item.CGST; //Cgst Amt
                            ws.Cells[i, 26].Style.Numberformat.Format = formate;
                            ws.Cells[i, 27].Value = item.SGST; //Sgst Amt
                            ws.Cells[i, 27].Style.Numberformat.Format = formate;
                            ws.Cells[i, 28].Value = item.AliasName;
                            ws.Cells[i, 28].Style.Numberformat.Format = "@";
                            i++;


                                                 }

                        string excelName = "CustomerBillDetails";
                        using (var memoryStream = new MemoryStream())
                        {
                            Response.ContentType = "application/ms-excel";
                            Response.AddHeader("content-disposition", "attachment; filename=" + excelName + ".xlsx");
                            excel.SaveAs(memoryStream);
                            memoryStream.WriteTo(Response.OutputStream);
                            Response.Flush();
                            Response.End();
                        }
                      



                        //    Response.Clear();
                        //    Response.Buffer = true;
                        //    Response.Charset = "";
                        //Response.ContentType = "application/ms-excel";
                        //Response.AddHeader("content-disposition", "attachment;filename=" + sFileName + "");
                        //    using (MemoryStream MyMemoryStream = new MemoryStream())
                        //    {
                        //    //ef.Save(MyMemoryStream, sFileName);
                        //    //ef.SaveAs(MyMemoryStream);
                        //        MyMemoryStream.WriteTo(Response.OutputStream);
                        //        Response.Flush();
                        //        Response.End();
                        //    }


                    }
                    else
                    {
                        ViewBag.Error = Resource1.DataNotExist;


                    }
                    AutoMapper.Mapper.Reset();
                    modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<CustomerDataModel, Customer>();
                    });
                    objDM = Mapper.Map<CustomerDataModel, Customer>(modleDATA);
                    AutoMapper.Mapper.Reset();


                    return View("ExportToExcel", objDM);

                }
                else
                {
                    modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                    if (objmodelCustomer.CompanyId != null)
                        modleDATA.Area = areaBL.GetArea(objmodelCustomer.CompanyId);
                    if (objmodelCustomer.AreaId != null)
                        modleDATA.GoDown = await godownBL.GetGoDownAreawise(objmodelCustomer.AreaId);
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<CustomerDataModel, Customer>();
                    });
                    objDM = Mapper.Map<CustomerDataModel, Customer>(modleDATA);

                    return View(objDM);
                }

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetSocietyBillWise(string type, string value, string pagename)
        {
            UserDataModel modleDATA = new UserDataModel();
            UserBL objBL = new UserBL();
            AreaBL objarea = new AreaBL();
            CompanyBL objcompany = new CompanyBL();
            GodownBL objGodown = new GodownBL();
            StockItemBL objstock = new StockItemBL();
            switch (type)
            {
                case "CompanyId":
                    modleDATA.Area = objarea.GetAreaBillWise(value);
                    if (pagename == "customer")
                    {
                        modleDATA.StockItemList = objstock.GetStockItemByCompany(value);
                    }
                    break;
                case "AreaId":
                    modleDATA.GoDown = await objGodown.GetGoDownBillWise(value);
                    break;
            }
            return Json(modleDATA);
        }

        //public ActionResult ExportToExcel(Customer objmodelCustomer)
        //{
        //    CustomerDataModel modleDATA = new CustomerDataModel();
        //    if (Session["user"] != null)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var grid = new GridView();
        //            grid.DataSource = objCustomer.GetAllBill(objmodelCustomer.CompanyId, objmodelCustomer.GodownId, objmodelCustomer.AreaId);
        //            grid.DataBind();

        //            Response.ClearContent();
        //            Response.Buffer = true;
        //            Response.AddHeader("content-disposition", "attachment; filename=CustomerBillDetails.xlsx");
        //            Response.ContentType = "application/ms-excel";

        //            Response.Charset = "";
        //            StringWriter sw = new StringWriter();
        //            System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);

        //            grid.RenderControl(htw);

        //            Response.Output.Write(sw.ToString());
        //            Response.Flush();
        //            Response.End();

        //            AutoMapper.Mapper.Reset();
        //            modleDATA.Company = ObjBL.GetCompany("");
        //            Mapper.Initialize(cfg =>
        //            {
        //                cfg.CreateMap<CustomerDataModel, Customer>();
        //            });
        //            objDM = Mapper.Map<CustomerDataModel, Customer>(modleDATA);
        //            AutoMapper.Mapper.Reset();


        //            return View("ExportToExcel", objDM);

        //        }
        //        else
        //        {
        //            modleDATA.Company = ObjBL.GetCompany("");
        //            AutoMapper.Mapper.Reset();
        //            Mapper.Initialize(cfg =>
        //            {
        //                cfg.CreateMap<CustomerDataModel, Customer>();
        //            });
        //            objDM = Mapper.Map<CustomerDataModel, Customer>(modleDATA);

        //            return View(objDM);
        //        }

        //    }
        //    else
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }
        //}


        //public double CreditDebit(Range myCell)
        // {
        //     string strVal;
        //     int myMult;
        //     strVal = myCell.Text;
        //     if ((strVal.Substring((strVal.Length - 2)) == "Cr"))
        //     {
        //         myMult = -1;
        //     }
        //     else
        //     {
        //         myMult = 1;
        //     }

        // }
        public ActionResult CustomerExport()
        {
            if (Session["user"] != null)
            {
                CustomerDataModel modleDATA = new CustomerDataModel();
                modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                //modleDATA.BillTypeList = ObjBL.GetBillType();
                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<CustomerDataModel, Customer>();
                });
                objDM = Mapper.Map<CustomerDataModel, Customer>(modleDATA);
                if (Session["RoleName"].ToString() != "Super Admin")
                {
                    objDM.CompanyId = objDM.Company.ToList().FirstOrDefault().Id.ToString();
                    objDM.CompanyName = objDM.Company.ToList().FirstOrDefault().CompanyName.ToString();
                }
                return View(objDM);

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }

        [ActionName("CustomerExport")]
        [HttpPost]
        public ActionResult CustomerExport(ExcelImportCustomer objmodelCustomer)
        {
            CustomerDataModel modleDATA = new CustomerDataModel();
            if (Session["user"] != null)
            {
                if (ModelState.IsValid)
                {

                    List<ExportCustomerData> LstcustomerBillInforData = new List<ExportCustomerData>();
                    List<ExportCustomerData> LstcustomerBillInforData1 = new List<ExportCustomerData>();
                    LstcustomerBillInforData = objCustomer.GetCustomerForExport(objmodelCustomer.CompanyId);
                    LstcustomerBillInforData1 = LstcustomerBillInforData;

                    SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
                    ExcelFile ef = new ExcelFile();
                    GemBox.Spreadsheet.ExcelWorksheet ws = ef.Worksheets.Add("Sheet1");
                    if (LstcustomerBillInforData != null)
                    {
                        LstcustomerBillInforData = LstcustomerBillInforData.Where(car => car.BillType == "BFBC4C62-B98B-4C18-96E7-2C8327886BCB").GroupBy(car => car.TallyName).Select(g => g.First()).ToList();
                        int i = 1;
                        foreach (ExportCustomerData item in LstcustomerBillInforData)
                        {
                            if (item.BillType == "BFBC4C62-B98B-4C18-96E7-2C8327886BCB")
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
                                    ws.Cells[0, 8].Value = "ClosingBalance";
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
                                ws.Cells[i, 10].Value = "deposite";
                                ws.Cells[i, 11].Value = item.ReceiptNo;
                                ws.Cells[i, 12].Value = item.AccountNo;
                                ws.Cells[i, 13].Value = item.ChequeNo;
                                ws.Cells[i, 14].Value = item.BankName;
                                ws.Cells[i, 15].Value = item.DepositeBankName;
                                ws.Cells[i, 16].Value = item.DepositeAccountNo;

                                i++;
                            }


                        }

                        GemBox.Spreadsheet.ExcelWorksheet ws1 = ef.Worksheets.Add("BILLING");

                        int i1 = 1;


                        LstcustomerBillInforData1 = LstcustomerBillInforData1.Where(car => car.BillType == "F2653C96-46D8-4609-A5ED-8568C129BAA3").GroupBy(car => car.TallyName).Select(g => g.First()).ToList();

                        //var j = LstcustomerBillInforData.Distinct().GroupBy(f => f.ReceiptNo).ToList();
                        foreach (ExportCustomerData item in LstcustomerBillInforData1)
                        {
                            if (item.BillType == "F2653C96-46D8-4609-A5ED-8568C129BAA3")
                            {

                                if (i1 == 1)
                                {

                                    ws1.Cells[0, 0].Value = "Sr. No.";
                                    ws1.Cells[0, 1].Value = "Name";
                                    ws1.Cells[0, 2].Value = "Alias";
                                    ws1.Cells[0, 3].Value = "Phone";
                                    ws1.Cells[0, 4].Value = "Email";
                                    ws1.Cells[0, 5].Value = "Address";
                                    ws1.Cells[0, 6].Value = "Initial Meter Reading";

                                }

                                ws1.Cells[i1, 0].Value = i1;
                                ws1.Cells[i1, 1].Value = item.Name;
                                ws1.Cells[i1, 2].Value = item.ALIAS;
                                ws1.Cells[i1, 3].Value = item.Phone.TrimEnd(',').TrimStart(',');
                                ws1.Cells[i1, 4].Value = item.EMAIL.TrimEnd(',').TrimStart(',');
                                ws1.Cells[i1, 5].Value = item.Address;
                                ws1.Cells[i1, 6].Value = item.InitialMeterReading;

                                i1++;
                            }

                        }
                        string root = Server.MapPath("~/Bill");
                        if (!Directory.Exists(root)) { Directory.CreateDirectory(root); }
                        //ws.Cells.GetSubrangeAbsolute(4, 0, 4, 7).Merged = true;

                        string sFileName = "CustomerDetails.xlsx";
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
                    modleDATA.BillTypeList = ObjBL.GetBillType();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<CustomerDataModel, Customer>();
                    });
                    objDM = Mapper.Map<CustomerDataModel, Customer>(modleDATA);
                    AutoMapper.Mapper.Reset();

                    try
                    {

                        foreach (ExportCustomerData item in LstcustomerBillInforData)
                        {
                            if (item.EmailSend != "True")
                            {
                                //item.MailEMAIL = "jagratisyn@gmail.com";
                                if (item.MailEMAIL != "")
                                {
                                    Password = ObjchL.GenratePass();
                                    Msgbody = Resource1.CustomerEmail.Replace("@Fname", item.Name).Replace("@CustomerNumber", item.CustomerNumber).Replace("@password", Password); ;
                                    CustomerSms = Resource1.CustomerSms.Replace("@Fname", item.Name).Replace("@CustomerNumber", item.CustomerNumber).Replace("@password", Password).Replace("@n", Environment.NewLine);

                                    foreach (var email in item.MailEMAIL.Split(','))
                                    {
                                        var res = ObjchL.SendEMailSms(item.Id, item.Name, item.Phone, email, Msgbody, CustomerSms, "", Password);
                                        if (res != "Save")
                                        {
                                            ViewBag.Error = "Error in Password saving";
                                            return View("CustomerExport", objDM);
                                        }

                                    }
                                }
                                else if (item.Phone != "")
                                {
                                    var res = ObjchL.SendEMailSms(item.Id, item.Name, item.Phone, "", "", CustomerSms, "", Password);
                                    if (res != "Save")
                                    {
                                        ViewBag.Error = "Error in Password saving";
                                        return View("CustomerExport", objDM);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var t = ex;
                    }

                    try
                    {

                        foreach (ExportCustomerData item in LstcustomerBillInforData1)
                        {
                            if (item.EmailSend != "True")
                            {
                                //item.MailEMAIL = "jagratisyn@gmail.com";
                                if (item.MailEMAIL != "")
                                {
                                    Password = ObjchL.GenratePass();
                                    Msgbody = Resource1.CustomerEmail.Replace("@Fname", item.Name).Replace("@CustomerNumber", item.CustomerNumber).Replace("@password", Password); ;
                                    CustomerSms = Resource1.CustomerSms.Replace("@Fname", item.Name).Replace("@CustomerNumber", item.CustomerNumber).Replace("@password", Password).Replace("@n", Environment.NewLine);
                                    foreach (var email in item.MailEMAIL.Split(','))
                                    {

                                        var res = ObjchL.SendEMailSms(item.Id, item.Name, item.Phone, email, Msgbody, CustomerSms, "", Password);
                                        if (res != "Save")
                                        {
                                            ViewBag.Error = "Error in Password saving";
                                            return View("CustomerExport", objDM);
                                        }

                                    }
                                }
                                else if (item.Phone != "")
                                {
                                    Password = ObjchL.GenratePass();
                                    CustomerSms = Resource1.CustomerSms.Replace("@Fname", item.Name).Replace("@CustomerNumber", item.CustomerNumber).Replace("@password", Password).Replace("@n", Environment.NewLine);
                                    var res = ObjchL.SendEMailSms(item.Id, item.Name, item.Phone, "", "", CustomerSms, "", Password);
                                    if (res != "Save")
                                    {
                                        ViewBag.Error = "Error in Password saving";
                                        return View("CustomerExport", objDM);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var t = ex;
                    }


                    return View("CustomerExport", objDM);

                }
                else
                {
                    modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                    modleDATA.BillTypeList = ObjBL.GetBillType();
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<CustomerDataModel, Customer>();
                    });
                    objDM = Mapper.Map<CustomerDataModel, Customer>(modleDATA);

                    return View(objDM);
                }

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        //public ActionResult ExportToExcel(Customer objmodelCustomer)
        //{
        //    CustomerDataModel modleDATA = new CustomerDataModel();
        //    if (Session["user"] != null)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var grid = new GridView();
        //            grid.DataSource = objCustomer.GetAllBill(objmodelCustomer.CompanyId, objmodelCustomer.GodownId, objmodelCustomer.AreaId);
        //            grid.DataBind();

        //            Response.ClearContent();
        //            Response.Buffer = true;
        //            Response.AddHeader("content-disposition", "attachment; filename=CustomerBillDetails.xlsx");
        //            Response.ContentType = "application/ms-excel";

        //            Response.Charset = "";
        //            StringWriter sw = new StringWriter();
        //            System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);

        //            grid.RenderControl(htw);

        //            Response.Output.Write(sw.ToString());
        //            Response.Flush();
        //            Response.End();

        //            AutoMapper.Mapper.Reset();
        //            modleDATA.Company = ObjBL.GetCompany("");
        //            Mapper.Initialize(cfg =>
        //            {
        //                cfg.CreateMap<CustomerDataModel, Customer>();
        //            });
        //            objDM = Mapper.Map<CustomerDataModel, Customer>(modleDATA);
        //            AutoMapper.Mapper.Reset();


        //            return View("ExportToExcel", objDM);

        //        }
        //        else
        //        {
        //            modleDATA.Company = ObjBL.GetCompany("");
        //            AutoMapper.Mapper.Reset();
        //            Mapper.Initialize(cfg =>
        //            {
        //                cfg.CreateMap<CustomerDataModel, Customer>();
        //            });
        //            objDM = Mapper.Map<CustomerDataModel, Customer>(modleDATA);

        //            return View(objDM);
        //        }

        //    }
        //    else
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }
        //}


        //public double CreditDebit(Range myCell)
        // {
        //     string strVal;
        //     int myMult;
        //     strVal = myCell.Text;
        //     if ((strVal.Substring((strVal.Length - 2)) == "Cr"))
        //     {
        //         myMult = -1;
        //     }
        //     else
        //     {
        //         myMult = 1;
        //     }
            
        // }
        public ActionResult CustomerReceiptExport()
        {
            if (Session["user"] != null)
            {
                CustomerDataModel modleDATA = new CustomerDataModel();
                modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                //modleDATA.BillTypeList = ObjBL.GetBillType();
                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<CustomerDataModel, Customer>();
                });
                objDM = Mapper.Map<CustomerDataModel, Customer>(modleDATA);
                if (Session["RoleName"].ToString() != "Super Admin")
                {
                    objDM.CompanyId = objDM.Company.ToList().FirstOrDefault().Id.ToString();
                    objDM.CompanyName = objDM.Company.ToList().FirstOrDefault().CompanyName.ToString();
                }
                return View(objDM);

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }

        [ActionName("ExportPaymentWithoutBill")]
        [HttpPost]
        
        public ActionResult ExportPaymentWithoutBill(ExcelImportCustomer objmodelCustomer)
        {
            CustomerDataModel modleDATA = new CustomerDataModel();
            if (Session["user"] != null)
            {
                if (ModelState.IsValid)
                {
                    List<ExportWithoutBillData> lstPaymentData = new List<ExportWithoutBillData>();
                    lstPaymentData = objCustomer.GetPaymentsWithoutBill(objmodelCustomer.CompanyId); SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
                    ExcelFile ef = new ExcelFile();
                    GemBox.Spreadsheet.ExcelWorksheet ws = ef.Worksheets.Add("Sheet1");
                    if (lstPaymentData != null && lstPaymentData.Count>0)
                    {
                        int i = 1;
                        foreach (ExportWithoutBillData item in lstPaymentData)
                        {
                            if (i == 1)
                            {
                                ws.Cells[0, 0].Value = "SR No";
                                ws.Cells[0, 1].Value = "Name";
                                ws.Cells[0, 2].Value = "AliasName";
                                ws.Cells[0, 3].Value = "PaymentDate";
                                ws.Cells[0, 4].Value = "Narration";
                                ws.Cells[0, 5].Value = "Amount";
                            }

                            ws.Cells[i, 0].Value = i;
                            ws.Cells[i, 1].Value = item.Name;
                            ws.Cells[i, 2].Value = item.AliasName;
                            ws.Cells[i, 3].Value = item.PaymentDate;
                            ws.Cells[i, 4].Value = item.Narration;
                            ws.Cells[i, 5].Value = item.Amount;

                            i++;
                        }
                        string root = Server.MapPath("~/Bill");
                        if (!Directory.Exists(root)) { Directory.CreateDirectory(root); }

                        string sFileName = "PaymentDataWithoutBill.xlsx";
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
                    modleDATA.BillTypeList = ObjBL.GetBillType();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<CustomerDataModel, Customer>();
                    });
                    objDM = Mapper.Map<CustomerDataModel, Customer>(modleDATA);
                    AutoMapper.Mapper.Reset();



                    return View("CustomerReceiptExport", objDM);
                }
                else
                {
                    modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                    modleDATA.BillTypeList = ObjBL.GetBillType();
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<CustomerDataModel, Customer>();
                    });
                    objDM = Mapper.Map<CustomerDataModel, Customer>(modleDATA);

                    return View("CustomerReceiptExport",objDM);
                }

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        //[ActionName("CustomerReceiptExport")]
        //[HttpPost]
        //public ActionResult CustomerReceiptExport(ExcelImportCustomer objmodelCustomer)
        //{
        //    CustomerDataModel modleDATA = new CustomerDataModel();
        //    if (Session["user"] != null)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            List<ExportCustomerData> LstcustomerBillInforData = new List<ExportCustomerData>();
        //            LstcustomerBillInforData = objCustomer.GetCustomerForExportReceipt(objmodelCustomer.CompanyId);


        //            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
        //            ExcelFile ef = new ExcelFile();
        //            GemBox.Spreadsheet.ExcelWorksheet ws = ef.Worksheets.Add("Sheet1");
        //            if (LstcustomerBillInforData != null)
        //            {

        //                int i = 1;
        //                foreach (ExportCustomerData item in LstcustomerBillInforData)
        //                {
        //                    if (i == 1)
        //                    {
        //                        ws.Cells[0, 0].Value = "SR No";
        //                        ws.Cells[0, 1].Value = "Name";
        //                        ws.Cells[0, 2].Value = "SOCIETY";
        //                        ws.Cells[0, 3].Value = "ALIAS";
        //                        ws.Cells[0, 4].Value = "Phone";
        //                        ws.Cells[0, 5].Value = "EMAIL";
        //                        ws.Cells[0, 6].Value = "Address";
        //                        ws.Cells[0, 7].Value = "Voucher date";
        //                        ws.Cells[0, 8].Value = "Amount";
        //                        ws.Cells[0, 9].Value = "deposit Type";
        //                        ws.Cells[0, 10].Value = "Narration";
        //                        ws.Cells[0, 11].Value = "ReceiptNo";
        //                        ws.Cells[0, 12].Value = "AccountNo";
        //                        ws.Cells[0, 13].Value = "ChequeNo";
        //                        ws.Cells[0, 14].Value = "BankName";
        //                        ws.Cells[0, 15].Value = "Deposite Bank";
        //                        ws.Cells[0, 16].Value = "Deposite Bank AccountNo.";

        //                    }

        //                    ws.Cells[i, 0].Value = i;
        //                    ws.Cells[i, 1].Value = item.Name;
        //                    ws.Cells[i, 2].Value = item.GodownMastername;
        //                    ws.Cells[i, 3].Value = item.ALIAS;
        //                    ws.Cells[i, 4].Value = item.Phone.TrimEnd(',').TrimStart(',');
        //                    ws.Cells[i, 5].Value = item.EMAIL.TrimEnd(',').TrimStart(',');
        //                    ws.Cells[i, 6].Value = item.Address;
        //                    ws.Cells[i, 7].Value = item.Voucherdate;
        //                    ws.Cells[i, 8].Value = item.ClosingBalance;
        //                    ws.Cells[i, 9].Value = item.PaymentType;
        //                    ws.Cells[i, 10].Value = item.Type;
        //                    ws.Cells[i, 11].Value = item.ReceiptNo;
        //                    ws.Cells[i, 12].Value = item.AccountNo;
        //                    ws.Cells[i, 13].Value = item.ChequeNo;
        //                    ws.Cells[i, 14].Value = item.BankName;
        //                    ws.Cells[i, 15].Value = item.DepositeBankName;
        //                    ws.Cells[i, 16].Value = item.DepositeAccountNo;

        //                    i++;
        //                }



        //                string root = Server.MapPath("~/Bill");
        //                if (!Directory.Exists(root)) { Directory.CreateDirectory(root); }

        //                string sFileName = "CustomerReceiptDetails.xlsx";
        //                ef.Save(root + "\\" + sFileName);
        //                Response.ContentType = "application/ms-excel";
        //                Response.AppendHeader("Content-Disposition", "attachment; filename=" + sFileName + "");
        //                Response.TransmitFile(root + "\\" + sFileName);
        //                Response.End();
        //                if (System.IO.File.Exists(root + "\\" + sFileName))
        //                {
        //                    System.IO.File.Delete(root + "\\" + sFileName);
        //                }
        //            }

        //            AutoMapper.Mapper.Reset();
        //            modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
        //            modleDATA.BillTypeList = ObjBL.GetBillType();
        //            Mapper.Initialize(cfg =>
        //            {
        //                cfg.CreateMap<CustomerDataModel, Customer>();
        //            });
        //            objDM = Mapper.Map<CustomerDataModel, Customer>(modleDATA);
        //            AutoMapper.Mapper.Reset();



        //            return View("CustomerExport", objDM);

        //        }
        //        else
        //        {
        //            modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
        //            modleDATA.BillTypeList = ObjBL.GetBillType();
        //            AutoMapper.Mapper.Reset();
        //            Mapper.Initialize(cfg =>
        //            {
        //                cfg.CreateMap<CustomerDataModel, Customer>();
        //            });
        //            objDM = Mapper.Map<CustomerDataModel, Customer>(modleDATA);

        //            return View(objDM);
        //        }

        //    }
        //    else
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }
        //}

        [ActionName("CustomerReceiptExport")]
        [HttpPost]
        public ActionResult CustomerReceiptExport(ExcelImportCustomer objmodelCustomer)
        {
            CustomerDataModel modleDATA = new CustomerDataModel();
            if (Session["user"] != null)
            {
                if (ModelState.IsValid)
                {
                    List<ExportCustomerData> LstcustomerBillInforData = new List<ExportCustomerData>();
                    LstcustomerBillInforData = objCustomer.GetCustomerForExportReceipt(objmodelCustomer.CompanyId);


                    //SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
                    //ExcelFile ef = new ExcelFile();
                    //ExcelWorksheet ws = ef.Worksheets.Add("Sheet1");
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    ExcelPackage excel = new ExcelPackage();
                    var ws = excel.Workbook.Worksheets.Add("Sheet1");

                    if (LstcustomerBillInforData != null)
                    {

                               ws.Cells[1, 1].Value = "SR No";
                                ws.Cells[1, 2].Value = "Name";
                                ws.Cells[1, 3].Value = "SOCIETY";
                                ws.Cells[1, 4].Value = "ALIAS";
                                ws.Cells[1, 5].Value = "Phone";
                                ws.Cells[1, 6].Value = "EMAIL";
                                ws.Cells[1, 7].Value = "Address";
                                ws.Cells[1, 8].Value = "Voucher date";
                                ws.Cells[1, 9].Value = "Amount";
                                ws.Cells[1, 10].Value = "deposit Type";
                                ws.Cells[1, 11].Value = "Narration";
                                ws.Cells[1, 12].Value = "ReceiptNo";
                                ws.Cells[1, 13].Value = "AccountNo";
                                ws.Cells[1, 14].Value = "ChequeNo";
                                ws.Cells[1, 15].Value = "BankName";
                                ws.Cells[1, 16].Value = "Deposite Bank";
                                ws.Cells[1, 17].Value = "Deposite Bank AccountNo.";
                        int i = 2;

                        foreach (ExportCustomerData item in LstcustomerBillInforData)
                        {

                            ws.Cells[i, 1].Value = (i - 1);
                            ws.Cells[i, 2].Value = item.Name;
                            ws.Cells[i, 3].Value = item.GodownMastername;
                            ws.Cells[i, 4].Value = item.ALIAS;
                            ws.Cells[i, 5].Value = item.Phone.TrimEnd(',').TrimStart(',');
                            ws.Cells[i, 6].Value = item.EMAIL.TrimEnd(',').TrimStart(',');
                            ws.Cells[i, 7].Value = item.Address;
                            ws.Cells[i, 8].Value = item.Voucherdate;
                            ws.Cells[i, 9].Value = item.ClosingBalance;
                            ws.Cells[i, 10].Value = item.PaymentType;
                            ws.Cells[i, 11].Value = item.Type;
                            ws.Cells[i, 12].Value = item.ReceiptNo;
                            ws.Cells[i, 13].Value = item.AccountNo;
                            ws.Cells[i, 14].Value = item.ChequeNo;
                            ws.Cells[i, 15].Value = item.BankName;
                            ws.Cells[i, 16].Value = item.DepositeBankName;
                            ws.Cells[i, 17].Value = item.DepositeAccountNo;

                            i++;
                        }

                        string excelName = "CustomerReceiptDetails";
                        using (var memoryStream = new MemoryStream())
                        {
                            Response.ContentType = "application/ms-excel";
                            Response.AddHeader("content-disposition", "attachment; filename=" + excelName + ".xlsx");
                            excel.SaveAs(memoryStream);
                            memoryStream.WriteTo(Response.OutputStream);
                            Response.Flush();
                            Response.End();
                        }

                    }

                    AutoMapper.Mapper.Reset();
                    modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                    modleDATA.BillTypeList = ObjBL.GetBillType();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<CustomerDataModel, Customer>();
                    });
                    objDM = Mapper.Map<CustomerDataModel, Customer>(modleDATA);
                    AutoMapper.Mapper.Reset();



                    return View("CustomerExport", objDM);

                }
                else
                {
                    modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                    modleDATA.BillTypeList = ObjBL.GetBillType();
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<CustomerDataModel, Customer>();
                    });
                    objDM = Mapper.Map<CustomerDataModel, Customer>(modleDATA);

                    return View(objDM);
                }

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult ExportBillList()
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                CustomerDataModel modleDATA = new CustomerDataModel();
                modleDATA.Company = ObjBL.GetCompany(Convert.ToString(Session["RoleName"]), Convert.ToString(Session["City"]));
                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<CustomerDataModel, Customer>();
                });
                objDM = Mapper.Map<CustomerDataModel, Customer>(modleDATA);
                if (Convert.ToString(Session["RoleName"]) != "Super Admin")
                {
                    objDM.CompanyId = objDM.Company.ToList().FirstOrDefault().Id.ToString();
                    objDM.CompanyName = objDM.Company.ToList().FirstOrDefault().CompanyName.ToString();
                }
                return View(objDM);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }

        [ActionName("ExportBillList")]
        [HttpPost]
        public async Task<ActionResult> ExportBillList(Customer objmodelCustomer)
        {
            CustomerDataModel modleDATA = new CustomerDataModel();
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                if (ModelState.IsValid)
                {
                    var grid = new GridView();
                    List<CustomerBillInformationData> LstcustomerBillInforData = new List<CustomerBillInformationData>();
                    LstcustomerBillInforData = objCustomer.GetBillList(objmodelCustomer.CompanyId, objmodelCustomer.GodownId, objmodelCustomer.AreaId, objmodelCustomer.BillTypeId);

                    SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
                    ExcelFile ef = new ExcelFile();
                    GemBox.Spreadsheet.ExcelWorksheet ws = ef.Worksheets.Add("Sheet1");
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
                        ws.Cells[0, 27].Value = "Export Status";
                        ws.Cells[0, 28].Value = "Payment Status";

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
                            ws.Cells[i, 23].Value = Round; //item.Round;//(item.InvoiceValue - Math.Round(item.InvoiceValue, 0));
                            ws.Cells[i, 23].Style.NumberFormat = formate;
                            ws.Cells[i, 24].Value = differ; //item.Diff;//(item.InvoiceValue - Math.Round(item.InvoiceValue, 0));   //item.Diff; Diff [=U2-W2]
                            ws.Cells[i, 24].Style.NumberFormat = formate;
                            ws.Cells[i, 25].Value = item.CGST; //Cgst Amt
                            ws.Cells[i, 25].Style.NumberFormat = formate;
                            ws.Cells[i, 26].Value = item.SGST; //Sgst Amt
                            ws.Cells[i, 26].Style.NumberFormat = formate;
                            ws.Cells[i, 27].Value = item.ExportStatus;
                            ws.Cells[i, 28].Value = item.isPaid;
                            i++;
                        }

                        string root = Server.MapPath("~/Bill");
                        if (!Directory.Exists(root)) { Directory.CreateDirectory(root); }
                        //ws.Cells.GetSubrangeAbsolute(4, 0, 4, 7).Merged = true;

                        string sFileName = "CustomerBillListDetails.xlsx";
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
                    AutoMapper.Mapper.Reset();
                    modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<CustomerDataModel, Customer>();
                    });
                    objDM = Mapper.Map<CustomerDataModel, Customer>(modleDATA);
                    AutoMapper.Mapper.Reset();


                    return View("ExportToExcel", objDM);

                }
                else
                {
                    modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                    if (objmodelCustomer.CompanyId != null)
                        modleDATA.Area = areaBL.GetArea(objmodelCustomer.CompanyId);
                    if (objmodelCustomer.AreaId != null)
                        modleDATA.GoDown = await godownBL.GetGoDownAreawise(objmodelCustomer.AreaId);
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<CustomerDataModel, Customer>();
                    });
                    objDM = Mapper.Map<CustomerDataModel, Customer>(modleDATA);

                    return View(objDM);
                }

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        public ActionResult ReadingList()
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                CustomerDataModel modleDATA = new CustomerDataModel();
                modleDATA.Company = ObjBL.GetCompany(Convert.ToString(Session["RoleName"]), Convert.ToString(Session["City"]));
                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<CustomerDataModel, Customer>();
                });
                objDM = Mapper.Map<CustomerDataModel, Customer>(modleDATA);
                if (Convert.ToString(Session["RoleName"]) != "Super Admin")
                {
                    objDM.CompanyId = objDM.Company.ToList().FirstOrDefault().Id.ToString();
                    objDM.CompanyName = objDM.Company.ToList().FirstOrDefault().CompanyName.ToString();
                }
                objDM.ReadingDate = DateTime.Now.Date.ToString("MM/dd/yyyy");
                return View(objDM);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }


        public ActionResult ImportPayment()
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                return View(objDIM);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        [ActionName("ImportPayment")]
        [HttpPost]
        public ActionResult ImportPayment(ImCustomer ObjCustomer)
        {

            CustomerDataModel modleDATA = new CustomerDataModel();
            List<ImportCustomer> lst = new List<ImportCustomer>();
            try
            {
                AutoMapper.Mapper.Reset();
                if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
                {
                    string name3 = Request.Files["FileUpload1"].FileName;
                    string path3 = Server.MapPath("~/") + "ExcelFile/";
                    string CashPayment = string.Empty, BankPayment = string.Empty;
                    string extension1 = Path.GetExtension(Request.Files["FileUpload1"].FileName).ToLower();

                    if (extension1.ToLower() == ".xls" || extension1.ToLower() == ".xlsx")
                    {

                        string path4 = Server.MapPath("~/") + "ExcelFile/" + Request.Files["FileUpload1"].FileName;
                        if (!Directory.Exists(Server.MapPath("~/") + "ExcelFile/")) { Directory.CreateDirectory(Server.MapPath("~/") + "ExcelFile/"); }
                        if (System.IO.File.Exists(path4))
                            System.IO.File.Delete(path4);
                        Request.Files["FileUpload1"].SaveAs(path4);
                        FileStream stream5 = System.IO.File.Open(path4, FileMode.Open, FileAccess.Read);
                        ISheet sheet;
                        DataTable dt2 = new DataTable();
                        if (extension1 == ".xls")
                        {
                            HSSFWorkbook hssfwb = new HSSFWorkbook(stream5); //HSSWorkBook object will read the Excel 97-2000 formats  
                            sheet = hssfwb.GetSheetAt(0); //get first Excel sheet from workbook  
                        }
                        else
                        {
                            XSSFWorkbook hssfwb = new XSSFWorkbook(stream5); //XSSFWorkBook will read 2007 Excel format  
                            sheet = hssfwb.GetSheetAt(0); //get first Excel sheet from workbook   
                        }

                        System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

                        IRow headerRow = sheet.GetRow(0);
                        int cellCount = headerRow.LastCellNum;
                        DataFormatter formatter = new DataFormatter();
                        //for (int j = 0; j < cellCount; j++)
                        //{
                        //     //date
                        //    //partyname
                        //    //aliasname
                        //    //narration
                        //    //amount
                        //    //paymenttype
                        //    ICell cell = headerRow.GetCell(j);
                        //    dt2.Columns.Add(cell.ToString());
                        //}
                      
                        dt2.Columns.Add("Date");
                        dt2.Columns.Add("PartyName"); 
                        dt2.Columns.Add("AliasName");
                        dt2.Columns.Add("Narration");
                        dt2.Columns.Add("Amount");
                        dt2.Columns.Add("PaymentType");


                        for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                        {
                            IRow row = sheet.GetRow(i);
                            DataRow dataRow = dt2.NewRow();
                            if (row == null)
                            {
                                break;
                            }
                            CashPayment = formatter.FormatCellValue(row.GetCell(4));
                            BankPayment = formatter.FormatCellValue(row.GetCell(5));
                            CashPayment = !string.IsNullOrEmpty(CashPayment) ? CashPayment.Trim() : CashPayment;
                            BankPayment = !string.IsNullOrEmpty(BankPayment) ? BankPayment.Trim() : BankPayment;
                            dataRow["Date"] = formatter.FormatCellValue(row.GetCell(0));
                            dataRow["PartyName"] = formatter.FormatCellValue(row.GetCell(1));
                            dataRow["AliasName"] = formatter.FormatCellValue(row.GetCell(2));
                            dataRow["Narration"] = formatter.FormatCellValue(row.GetCell(3));
                            dataRow["Amount"] = string.IsNullOrEmpty(CashPayment) ? BankPayment : CashPayment;
                            dataRow["PaymentType"] = string.IsNullOrEmpty(CashPayment) ? "Bank" : "Cash";

                           
                            //for (int j = row.FirstCellNum; j < cellCount; j++)
                            //{
                            //    dataRow[j] = formatter.FormatCellValue(row.GetCell(j));

                            //}

                            dt2.Rows.Add(dataRow);
                        }
                        DataTable dt3 = dt2;
                        DataTable datadt = objCustomer.PaymentSave(dt3);

                       ViewBag.customer = datadt != null ? datadt.AsEnumerable() : null;
                        ViewBag.sucess = Resource1.PaymentImported;
                        TempData["Msg"] = Resource1.PaymentImported;
                        return View();
                    }
                    else
                    {
                        ViewBag.Error = Resource1.FileFormate;
                        return View();
                    }

                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;

            }

            return View();
        }



        /// <summary>
        /// Convert Datatable into list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public class CreateDataTable
        {
            public DataTable CreateTable(DataSet dataSet)
            {
                try
                {
                    //Create a new Table
                    DataTable dataTable = new DataTable();
                    dataTable.Clear();
                    int count = 1;
                    //Insert the Names of Column in Table. Name will be Same as Datatable name. 
                    dataTable.Columns.Add("StockItemId");
                    dataTable.Columns.Add("Rate");
                    dataTable.Columns.Add("Ratemonth");
                    dataTable.Columns.Add("RateYear");
                    dataTable.Columns.Add("CreatedDate");
                    dataTable.Columns.Add("weight");
                    dataTable.Columns.Add("ToCreatedDate");

                    //Loop to Read all Rows from data table
                    for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                    {
                        //Add a new row in dataTable.
                        DataRow _dataRow = dataTable.NewRow();

                        //Add data in rows
                        _dataRow["StockItemId"] = dataSet.Tables[0].Rows[i]["StockItemId"].ToString();
                        _dataRow["Rate"] = dataSet.Tables[0].Rows[i]["Rate"] != DBNull.Value ? Convert.ToDecimal(dataSet.Tables[0].Rows[i]["Rate"]) : 0;
                        _dataRow["Ratemonth"] = dataSet.Tables[0].Rows[i]["Ratemonth"] != DBNull.Value ? Convert.ToInt32(dataSet.Tables[0].Rows[i]["Ratemonth"]) : 0;
                        _dataRow["RateYear"] = dataSet.Tables[0].Rows[i]["RateYear"] != DBNull.Value ? Convert.ToInt32(dataSet.Tables[0].Rows[i]["RateYear"]) : 0;
                        _dataRow["CreatedDate"] = dataSet.Tables[0].Rows[i]["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreatedDate"]).ToString() : "";
                        _dataRow["weight"] = dataSet.Tables[0].Rows[i]["weight"] != DBNull.Value ? Convert.ToDecimal(dataSet.Tables[0].Rows[i]["weight"]) : 0;
                        _dataRow["ToCreatedDate"] = dataSet.Tables[0].Rows[i]["ToCreatedDate"] != DBNull.Value ? Convert.ToDateTime(dataSet.Tables[0].Rows[i]["ToCreatedDate"]).ToString() : "";
                        dataTable.Rows.Add(_dataRow);
                    }

                    //Return The new created table.
                    return dataTable;
                }
                catch (Exception ex)
                {
                    throw;
                }

            }
        }

        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }



        [HttpGet]
        public ActionResult Import()
        {
            return View();
        }



        [ActionName("Import")]
        [HttpPost]
        public async System.Threading.Tasks.Task<string> ImportExceL(RateImport objImport)
        {
            FileStream stream = null;

            try
            {

                string name3 = Request.Files["FileUpload"].FileName;
                string path3 = Server.MapPath("~/") + "ExcelSheet/";
                string extension1 = Path.GetExtension(Request.Files["FileUpload"].FileName).ToLower();

                if (extension1.ToLower() == ".xls" || extension1.ToLower() == ".xlsx")
                {
                    string path4 = Server.MapPath("~/") + "ExcelSheet/" + Request.Files["FileUpload"].FileName;
                    if (!Directory.Exists(Server.MapPath("~/") + "ExcelSheet/")) { Directory.CreateDirectory(Server.MapPath("~/") + "ExcelSheet/"); }
                    if (System.IO.File.Exists(path4))
                        System.IO.File.Delete(path4);
                    Request.Files["FileUpload"].SaveAs(path4);
                    CreateDataTable createDataTableObj = new CreateDataTable();

                    DataSet dataset; string filePath = path4;    //stream will open and read file
                    stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read);

                    //Will read Data from Excel
                    using (var excelReader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var configuration = new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                            {
                                //set 1 row of table as the 1 row in excel.
                                UseHeaderRow = true
                            }
                        };

                        //store all tables in dataset.
                        dataset = excelReader.AsDataSet(configuration);

                        //New created table as per DB table
                        DataTable CreatedTable = createDataTableObj.CreateTable(dataset);
                        StockItemBL objbl = new StockItemBL();
                        // bulk insert data into DB.
                        objbl.SaveImport(CreatedTable);
                    }

                }

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                stream.Close();
            }
            return "";
        }



        public ActionResult ImportBillPaymentExcel()
        {
            if (Session["user"] != null)
            {
                CustomerDataModel modleDATA = new CustomerDataModel();
                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<CustomerDataModel, ImCustomer>();
                });
                objDIM = Mapper.Map<CustomerDataModel, ImCustomer>(modleDATA);
                return View(objDIM);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        [ActionName("ImportBillPaymentExcel")]
        [HttpPost]
        public ActionResult ImportBillPaymentExcel(ImCustomer ObjCustomer)
        {

            CustomerDataModel modleDATA = new CustomerDataModel();
            List<ImportCustomer> lst = new List<ImportCustomer>();
            try
            {
                AutoMapper.Mapper.Reset();
                if (Session["user"] != null)
                {

                    string name3 = Request.Files["FileUpload1"].FileName;
                    string path3 = Server.MapPath("~/") + "ExcelFile/";
                    string extension1 = Path.GetExtension(Request.Files["FileUpload1"].FileName).ToLower();

                    if (extension1.ToLower() == ".xls" || extension1.ToLower() == ".xlsx")
                    {

                        string path4 = Server.MapPath("~/") + "ExcelFile/" + Request.Files["FileUpload1"].FileName;
                        if (!Directory.Exists(Server.MapPath("~/") + "ExcelFile/")) { Directory.CreateDirectory(Server.MapPath("~/") + "ExcelFile/"); }
                        if (System.IO.File.Exists(path4))
                            System.IO.File.Delete(path4);
                        Request.Files["FileUpload1"].SaveAs(path4);
                        FileStream stream5 = System.IO.File.Open(path4, FileMode.Open, FileAccess.Read);
                        ISheet sheet;
                        DataTable dt2 = new DataTable();
                        if (extension1 == ".xls")
                        {
                            HSSFWorkbook hssfwb = new HSSFWorkbook(stream5); //HSSWorkBook object will read the Excel 97-2000 formats  
                            sheet = hssfwb.GetSheetAt(0); //get first Excel sheet from workbook  
                        }
                        else
                        {
                            XSSFWorkbook hssfwb = new XSSFWorkbook(stream5); //XSSFWorkBook will read 2007 Excel format  
                            sheet = hssfwb.GetSheetAt(0); //get first Excel sheet from workbook   
                        }

                        System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

                        IRow headerRow = sheet.GetRow(0);
                        int cellCount = headerRow.LastCellNum;
                        DataFormatter formatter = new DataFormatter();
                        for (int j = 0; j < cellCount; j++)
                        {
                            ICell cell = headerRow.GetCell(j);
                            dt2.Columns.Add(cell.ToString());
                        }

                        for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                        {
                            IRow row = sheet.GetRow(i);
                            DataRow dataRow = dt2.NewRow();
                            if (row == null)
                            {
                                break;
                            }
                            for (int j = row.FirstCellNum; j < cellCount; j++)
                            {
                                if (row.GetCell(j) != null)
                                    dataRow[j] = formatter.FormatCellValue(row.GetCell(j));

                            }

                            dt2.Rows.Add(dataRow);
                        }
                        DataTable dt3 = dt2;
                        List<CustomerBillInformationData> Bill = new List<CustomerBillInformationData>();
                        foreach (DataRow item in dt3.Rows)
                        {
                            Bill.Add(new CustomerBillInformationData

                            {
                                BillNo = item["BillNo"] != DBNull.Value ? Convert.ToString(item["BillNo"]) : "",
                                PartyName = item["PartyName"] != DBNull.Value ? Convert.ToString(item["PartyName"]) : "",
                                ClosingBalance = item["ClosingBalance"] != DBNull.Value ? Convert.ToDecimal(item["ClosingBalance"]) : 0,
                                BillDate = item["BillDate"] != DBNull.Value ? Convert.ToString(item["BillDate"]) : "",
                                ClosingRedg = item["ClosingRedg"] != DBNull.Value ? Convert.ToDecimal(item["ClosingRedg"]) : 0,
                                PreviousBillDate = item["PreviousBillDate"] != DBNull.Value ? Convert.ToString(item["PreviousBillDate"]) : "",
                                PreviousRedg = item["PreviousRedg"] != DBNull.Value ? Convert.ToDecimal(item["PreviousRedg"]) : 0,
                                DueDate = item["DueDate"] != DBNull.Value ? Convert.ToString(item["DueDate"]) : "",
                                ConsumeUnit = item["ConsumeUnit"] != DBNull.Value ? Convert.ToDecimal(item["ConsumeUnit"]) : 0,
                                InputRate = item["InputRate"] != DBNull.Value ? Convert.ToDecimal(item["InputRate"]) : Convert.ToDecimal(2.6),
                                StockItemName = item["StockItemName"] != DBNull.Value ? Convert.ToString(item["StockItemName"]) : "",
                                ConsumptioninKG = item["CurrentKGS"] != DBNull.Value ? Convert.ToDecimal(item["CurrentKGS"]) : 0,
                                Rate = item["Rate"] != DBNull.Value ? Convert.ToDecimal(item["Rate"]) : 0,
                                Month = item["BillMonth"] != DBNull.Value ? Convert.ToString(Convert.ToInt32(item["BillMonth"])) : "",
                                ServiceAmt = item["ServiceAmt"] != DBNull.Value ? Convert.ToDecimal(item["ServiceAmt"]) : 0,
                                Arrears = item["Arrears"] != DBNull.Value ? Convert.ToDecimal(item["Arrears"]) : 0,
                                MinAmt = item["MinAmt"] != DBNull.Value ? Convert.ToDecimal(item["MinAmt"]) : 0,
                                LateFee = item["LateFee"] != DBNull.Value ? Convert.ToDecimal(item["LateFee"]) : 0,
                                GodownName = item["GodownName"] != DBNull.Value ? Convert.ToString(item["GodownName"]) : "",
                                ReconnectionAmt = item["ReconnectionAmt"] != DBNull.Value ? Convert.ToDecimal(item["ReconnectionAmt"]) : 0,
                                InvoiceValue = item["TotalAmt"] != DBNull.Value ? Convert.ToDecimal(item["TotalAmt"]) : 0,
                                GASCOMSUMPTIONLed = item["Led"] != DBNull.Value ? Convert.ToDecimal(item["Led"]) : 0,
                                diposit = item["PreviousDiposite"] != DBNull.Value ? Convert.ToDecimal(item["PreviousDiposite"]) : 0,
                                PreviousBalance = item["PreviousBalance"] != DBNull.Value ? Convert.ToDecimal(item["PreviousBalance"]) : 0,
                                Round = item["Round"] != DBNull.Value ? Convert.ToDecimal(item["Round"]) : 0,
                                Diff = item["Diff"] != DBNull.Value ? Convert.ToDecimal(item["Diff"]) : 0,
                                SGST = item["SGST"] != DBNull.Value ? Convert.ToDecimal(item["SGST"]) : 0,
                                CGST = item["CGST"] != DBNull.Value ? Convert.ToDecimal(item["CGST"]) : 0,
                                PLateFee = item["PreviousLateFree"] != DBNull.Value ? Convert.ToDecimal(item["PreviousLateFree"]) : 0,
                                AliasName = item["AliasName"] != DBNull.Value ? Convert.ToString(item["AliasName"]) : "",
                            });

                        }


                        var grid = new GridView();
                        List<CustomerBillInformationData> LstcustomerBillInforData = new List<CustomerBillInformationData>();
                        LstcustomerBillInforData = Bill;

                        SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
                        ExcelFile ef = new ExcelFile();
                        GemBox.Spreadsheet.ExcelWorksheet ws = ef.Worksheets.Add("Sheet1");
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

                            string root = Server.MapPath("~/Bill");
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
                    else
                    {
                        ViewBag.Error = Resource1.FileFormate;
                        return View(ObjCustomer);
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {

            }

            return View();
        }

        public ActionResult ImportPaymentExcel()
        {
            if (Session["user"] != null)
            {
                CustomerDataModel modleDATA = new CustomerDataModel();
                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<CustomerDataModel, ImCustomer>();
                });
                objDIM = Mapper.Map<CustomerDataModel, ImCustomer>(modleDATA);
                return View(objDIM);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        [ActionName("ImportPaymentExcel")]
        [HttpPost]
        public ActionResult ImportPaymentExcel(ImCustomer ObjCustomer)
        {

            CustomerDataModel modleDATA = new CustomerDataModel();
            List<ImportCustomer> lst = new List<ImportCustomer>();
            try
            {
                AutoMapper.Mapper.Reset();
                if (Session["user"] != null)
                {

                    string name3 = Request.Files["FileUpload1"].FileName;
                    string path3 = Server.MapPath("~/") + "ExcelFile/";
                    string extension1 = Path.GetExtension(Request.Files["FileUpload1"].FileName).ToLower();

                    if (extension1.ToLower() == ".xls" || extension1.ToLower() == ".xlsx")
                    {

                        string path4 = Server.MapPath("~/") + "ExcelFile/" + Request.Files["FileUpload1"].FileName;
                        if (!Directory.Exists(Server.MapPath("~/") + "ExcelFile/")) { Directory.CreateDirectory(Server.MapPath("~/") + "ExcelFile/"); }
                        if (System.IO.File.Exists(path4))
                            System.IO.File.Delete(path4);
                        Request.Files["FileUpload1"].SaveAs(path4);
                        FileStream stream5 = System.IO.File.Open(path4, FileMode.Open, FileAccess.Read);
                        ISheet sheet;
                        DataTable dt2 = new DataTable();
                        if (extension1 == ".xls")
                        {
                            HSSFWorkbook hssfwb = new HSSFWorkbook(stream5); //HSSWorkBook object will read the Excel 97-2000 formats  
                            sheet = hssfwb.GetSheetAt(0); //get first Excel sheet from workbook  
                        }
                        else
                        {
                            XSSFWorkbook hssfwb = new XSSFWorkbook(stream5); //XSSFWorkBook will read 2007 Excel format  
                            sheet = hssfwb.GetSheetAt(0); //get first Excel sheet from workbook   
                        }

                        System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

                        IRow headerRow = sheet.GetRow(0);
                        int cellCount = headerRow.LastCellNum;
                        DataFormatter formatter = new DataFormatter();
                        for (int j = 0; j < cellCount; j++)
                        {
                            ICell cell = headerRow.GetCell(j);
                            dt2.Columns.Add(cell.ToString());
                        }

                        for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                        {
                            IRow row = sheet.GetRow(i);
                            DataRow dataRow = dt2.NewRow();
                            if (row == null)
                            {
                                break;
                            }
                            for (int j = row.FirstCellNum; j < cellCount; j++)
                            {
                                if (row.GetCell(j) != null)
                                    dataRow[j] = formatter.FormatCellValue(row.GetCell(j));

                            }

                            dt2.Rows.Add(dataRow);
                        }
                        DataTable dt3 = dt2;
                        List<ExportCustomerData> Bill = new List<ExportCustomerData>();
                        foreach (DataRow item in dt3.Rows)
                        {
                            Bill.Add(new ExportCustomerData
                            {
                                Name = Convert.ToString(item["Name"]),
                                ALIAS = Convert.ToString(item["ALIAS"]),
                                BILLWISEDETAILS = Convert.ToString(item["BILLWISEDETAILS"]),
                                DUEDATE = Convert.ToString(item["DUEDATE"]),
                                Address = Convert.ToString(item["Address"]),
                                PINCODE = Convert.ToString(item["PINCODE"]),
                                CONTACTPERSON = item["CONTACTPERSON"].ToString(),
                                Phone = Convert.ToString(item["Phone"]),
                                MOBILE = Convert.ToString(item["MOBILE"]),
                                EMAIL = Convert.ToString(item["EMAIL"]),
                                ClosingBalance = Convert.ToDecimal(item["ClosingBalance"]),
                                BillType = Convert.ToString(item["BillType"]),
                                GodownMastername = Convert.ToString(item["GodownMastername"]),
                                ReceiptNo = Convert.ToString(item["ReceiptNo"]),
                                BankName = Convert.ToString(item["BankName"]),
                                AccountNo = Convert.ToString(item["AccountNo"]),
                                ChequeNo = Convert.ToString(item["ChequeNo"]),
                                PaymentType = Convert.ToString(item["PaymentType"]),
                                TallyName = Convert.ToString(item["TallyName"]),
                                InitialMeterReading = Convert.ToDecimal(item["ClosingRedg"]),
                                DepositeAccountNo = Convert.ToString(item["DepositeAccountNo"]),
                                DepositeBankName = Convert.ToString(item["DepositeBankName"]),
                                Voucherdate = Convert.ToString(item["Voucherdate"]),
                                CustomerNumber = Convert.ToString(item["CustomerNumber"]),

                                Type = "GasConsume"
                            });
                        }

                        List<ExportCustomerData> LstcustomerBillInforData = new List<ExportCustomerData>();
                        LstcustomerBillInforData = Bill;
                        SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
                        ExcelFile ef = new ExcelFile();
                        GemBox.Spreadsheet.ExcelWorksheet ws = ef.Worksheets.Add("Sheet1");
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
                            string root = Server.MapPath("~/Bill");
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
                    else
                    {
                        ViewBag.Error = Resource1.FileFormate;
                        return View(ObjCustomer);
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {

            }

            return View();
        }


        //[HttpPost]
        //public ActionResult ReadingLoadData(reading objreading)
        //{
        //    if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
        //    {
        //        var reqst = System.Web.HttpContext.Current.Request.InputStream;
        //        System.IO.StreamReader reader = new System.IO.StreamReader(reqst);
        //        String xmlData = reader.ReadToEnd();
        //        string decode = HttpUtility.UrlDecode(Uri.UnescapeDataString(xmlData));
        //        //Get parameters
        //        // get Start (paging start index) and length (page size for paging)
        //        var draw = Request.Form.GetValues("draw").FirstOrDefault();
        //        var start = Request.Form.GetValues("start").FirstOrDefault();
        //        var length = Request.Form.GetValues("length").FirstOrDefault();
        //        //Get Sort columns value

        //        var h = Request.Form.GetValues("order[0][column]").FirstOrDefault();

        //        var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
        //        var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
        //        string search = Request.Params["search[value]"];
        //        if (sortColumn == "CompanyName" && sortColumnDir == "asc") sortColumn = "";
        //        int pageSize = length != null ? Convert.ToInt32(length) : 0;
        //        int skip = start != null ? Convert.ToInt32(start) : 0;
        //        int totalRecords = 0;

        //        var sEcho1 = Convert.ToInt32((Convert.ToInt32(start) + Convert.ToInt32(length)) / Convert.ToInt32(length));
        //        int pageNo = sEcho1;//!string.IsNullOrEmpty(search) ? 1 : sEcho1;
        //        List<CompanyMaster> objcompany = new List<CompanyMaster>();
        //        List<CompanyDataModel> companydata = new List<CompanyDataModel>();

        //        companydata = ObjBL.GetAllReading(pageNo, pageSize, search, sortColumn, sortColumnDir, Session["City"].ToString(), Session["RoleName"].ToString());
        //        if (companydata.Count() > 0)
        //        {
        //            totalRecords = companydata.LastOrDefault().TotalRows;
        //        }
        //        var data = companydata;
        //        return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }
        //}
    }
}


// //using excel

//            //Open the Excel file using ClosedXML.
//            using (XLWorkbook workBook = new XLWorkbook(path4))
//            {
//                //Read the first Sheet from Excel file.
//                IXLWorksheet workSheet = workBook.Worksheet(1);

////Loop through the Worksheet rows.
//bool firstRow = true;
//                foreach (IXLRow row in workSheet.Rows())
//                {
//                    //Use the first row to add columns to DataTable.
//                    if (firstRow)
//                    {
//                        foreach (IXLCell cell in row.Cells())
//                        {
//                            dt2.Columns.Add(cell.Value.ToString());
//                        }
//                        firstRow = false;
//                    }
//                    else
//                    {
//                        //Add rows to DataTable.
//                        dt2.Rows.Add();
//                        int i = 0;
//                        foreach (IXLCell cell in row.Cells())
//                        {
//                            dt2.Rows[dt2.Rows.Count - 1][i] = cell.Value.ToString();

//                            dt2.Rows[dt2.Rows.Count - 1][i] = cell.Value.ToString();
//                            string t = cell.RichText.Text.ToString();

//i++;
//                        }
//                    }

//                }




//            }

//try
//{
//    AutoMapper.Mapper.Reset();
//    if (Session["user"] != null)
//    {
//        if (ModelState.IsValid)
//        {
//            if (Request.Files["FileUpload1"].ContentLength > 0)
//            {
//                string extension = Path.GetExtension(Request.Files["FileUpload1"].FileName).ToLower();
//                string name = Request.Files["FileUpload1"].FileName;
//                string path = Server.MapPath("~/") + "ExcelFile/";
//                if (extension.ToLower() == ".xls" || extension.ToLower() == ".xlsx")
//                {


//                    string query = null;
//                    string connString = "";
//                    List<ImportCustomer> lst = new List<ImportCustomer>();
//                    string[] validFileTypes = { ".xls", ".xlsx", ".csv" };

//                    string path1 = Server.MapPath("~/") + "ExcelFile/" + Request.Files["FileUpload1"].FileName;
//                    if (!Directory.Exists(Server.MapPath("~/") + "ExcelFile/")) { Directory.CreateDirectory(Server.MapPath("~/") + "ExcelFile/"); }
//                    if (System.IO.File.Exists(path1))
//                        System.IO.File.Delete(path1);
//                    Request.Files["FileUpload1"].SaveAs(path1);


//                    FileStream stream = null;
//                    DataSet dataset;
//                    //stream will open and read file
//                    stream = System.IO.File.Open(path1, FileMode.Open, FileAccess.Read);

//                    //Will read Data from Excel
//                    using (var excelReader = ExcelReaderFactory.CreateReader(stream))
//                    {
//                        var configuration = new ExcelDataSetConfiguration()
//                        {
//                            ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
//                            {
//                                //set 1 row of table as the 1 row in excel.
//                                UseHeaderRow = true
//                            }
//                        };

//                        //store all tables in dataset.
//                        dataset = excelReader.AsDataSet(configuration); //THIS IS WHERE IT IS USED
//                    }

//                    DataTable dt = dataset.Tables[0];

//                    if (extension.Trim() == ".xls")
//                    {
//                        //connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path1 + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
//                        //DataTable dt1 = Utility.ConvertXSLXtoDataTable(path1, connString);


//                        foreach (DataRow item in dt.Rows)
//                        {
//                            if (item["Closing Balance"].ToString().Contains("Dr"))
//                            {
//                                // string h = item["Closing Balance"].ToString().Remove('Dr');

//                            }


//                            ImportCustomer obj = new ImportCustomer();
//                            if (item["Party Name"].ToString() != "")
//                            {
//                                obj.TallyName = item["Party Name"].ToString();
//                                obj.ClosingBalance = item["Closing Balance"].ToString() != "" ? Convert.ToDecimal(item["Closing Balance"]) : 0;
//                                obj.PrevBillDate = item["Prev. Bill Date"].ToString() != "" ? Convert.ToDateTime(item["Prev. Bill Date"]) : (DateTime?)null;
//                                obj.PrevReading = item["Prev.Reading"].ToString() != "" ? Convert.ToDecimal(item["Prev.Reading"]) : 0;
//                                obj.ContactNos = item["Contact Nos."].ToString();
//                                obj.EmailId = item["Email Id"].ToString();
//                                obj.AreaId = ObjCustomer.AreaId;
//                                obj.GodownId = ObjCustomer.GodownId;
//                                lst.Add(obj);
//                            }
//                        }
//                    }

//                    else if (extension.Trim() == ".xlsx")
//                    {


//                        foreach (DataRow item in dt.Rows)
//                        {
//                            if (item["Closing Balance"].ToString().Contains("Dr"))
//                            {
//                                // string h = item["Closing Balance"].ToString().Remove('Dr');

//                            }
//                            ImportCustomer obj = new ImportCustomer();
//                            if (item["Party Name"].ToString() != "")
//                            {
//                                obj.TallyName = item["Party Name"].ToString();
//                                obj.ClosingBalance = item["Closing Balance"] != DBNull.Value ? Convert.ToDecimal(item["Closing Balance"]) : 0;
//                                obj.PrevBillDate = item["Prev. Bill Date"].ToString() != "" ? Convert.ToDateTime(item["Prev. Bill Date"]) : (DateTime?)null;
//                                obj.PrevReading = item["Prev.Reading"].ToString() != "" ? Convert.ToDecimal(item["Prev.Reading"]) : 0;
//                                obj.ContactNos = item["Contact Nos."].ToString();
//                                obj.EmailId = item["Email Id"].ToString();
//                                obj.AreaId = ObjCustomer.AreaId;
//                                obj.GodownId = ObjCustomer.GodownId;
//                                lst.Add(obj);
//                            }

//                        }

//                    }
//                    else
//                    {

//                        modleDATA.Company = ObjBL.GetCompany("");
//                        modleDATA.Area = areaBL.GetArea(ObjCustomer.CompanyId);
//                        modleDATA.GoDown = await godownBL.GetGoDownAreawise(ObjCustomer.AreaId);
//                        Mapper.Initialize(cfg =>
//                        {
//                            cfg.CreateMap<CustomerDataModel, Customer>();
//                        });
//                        ObjCustomer = Mapper.Map<CustomerDataModel, Customer>(modleDATA);

//                        ViewBag.Error = "Please Upload Files in .xls, .xlsx or .csv format";
//                        return View(ObjCustomer);
//                    }

//                    List<ImportCustomerData> customerDataModel = new List<ImportCustomerData>();
//                    Mapper.Initialize(cfg =>
//                    {
//                        cfg.CreateMap<List<ImportCustomerData>, List<ImportCustomer>>();
//                    });
//                    customerDataModel = Mapper.Map<List<ImportCustomer>, List<ImportCustomerData>>(lst);
//                    AutoMapper.Mapper.Reset();

//                    objCustomer.Save(customerDataModel, ObjCustomer.CompanyId, Session["user"].ToString());


//                    modleDATA.Company = ObjBL.GetCompany("");

//                    Mapper.Initialize(cfg =>
//                    {
//                        cfg.CreateMap<CustomerDataModel, Customer>();
//                    });

//                    modleDATA.Company = ObjBL.GetCompany("");
//                    modleDATA.Area = areaBL.GetArea(ObjCustomer.CompanyId);
//                    modleDATA.GoDown = await godownBL.GetGoDownAreawise(ObjCustomer.AreaId);

//                    ObjCustomer = Mapper.Map<CustomerDataModel, Customer>(modleDATA);

//                    ViewBag.Error = "Customer Imported Successfully";
//                    TempData["Msg"] = "Customer Imported Successfully";
//                    return RedirectToAction("List", "Customer");

//                }
//                else
//                {

//                    modleDATA.Company = ObjBL.GetCompany("");
//                    if (ObjCustomer.CompanyId != null)
//                    {
//                        modleDATA.Area = areaBL.GetArea(ObjCustomer.CompanyId);
//                    }
//                    if (ObjCustomer.AreaId != null)
//                    { modleDATA.GoDown = await godownBL.GetGoDownAreawise(ObjCustomer.AreaId); }

//                    Mapper.Initialize(cfg =>
//                    {
//                        cfg.CreateMap<CustomerDataModel, Customer>();
//                    });
//                    ObjCustomer = Mapper.Map<CustomerDataModel, Customer>(modleDATA);

//                    ViewBag.Error = "Only .xls , .xlsx file allowed.";


//                }
//            }
//            else
//            {
//                modleDATA.Company = ObjBL.GetCompany("");
//                modleDATA.Area = areaBL.GetArea(ObjCustomer.CompanyId);
//                modleDATA.GoDown = await godownBL.GetGoDownAreawise(ObjCustomer.AreaId);

//                Mapper.Initialize(cfg =>
//                {
//                    cfg.CreateMap<CustomerDataModel, Customer>();
//                });
//                ObjCustomer = Mapper.Map<CustomerDataModel, Customer>(modleDATA);

//                ViewBag.Error = "Please Select File";
//            }
//        }
//        else
//        {
//            modleDATA.Company = ObjBL.GetCompany("");
//            if (ObjCustomer.CompanyId != null)
//            {
//                modleDATA.Area = areaBL.GetArea(ObjCustomer.CompanyId);
//            }
//            if (ObjCustomer.AreaId != null)
//            { modleDATA.GoDown = await godownBL.GetGoDownAreawise(ObjCustomer.AreaId); }

//            Mapper.Initialize(cfg =>
//            {
//                cfg.CreateMap<CustomerDataModel, Customer>();
//            });
//            ObjCustomer = Mapper.Map<CustomerDataModel, Customer>(modleDATA);
//            string messages = string.Join(Environment.NewLine, ModelState.Values
//                     .SelectMany(x => x.Errors)
//                     .Select(x => x.Exception));
//            ViewBag.Error = messages;
//        }
//        return View(ObjCustomer);
//    }
//    else
//    {
//        return RedirectToAction("Login", "Account");
//    }
//}
//catch (Exception ex)
//{
//    modleDATA.Company = ObjBL.GetCompany("");
//    modleDATA.Area = areaBL.GetArea(ObjCustomer.CompanyId);
//    modleDATA.GoDown = await godownBL.GetGoDownAreawise(ObjCustomer.AreaId);

//    Mapper.Initialize(cfg =>
//    {
//        cfg.CreateMap<CustomerDataModel, Customer>();
//    });
//    ObjCustomer = Mapper.Map<CustomerDataModel, Customer>(modleDATA);

//    ViewBag.Error = ex.Message;
//    return View(ObjCustomer);
//}