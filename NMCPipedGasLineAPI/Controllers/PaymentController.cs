using NMCPipedGasLineAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Data.SqlClient;
using NMCDataAccesslayer.BL;
using NMCDataAccesslayer.DataModel;
using AutoMapper;
using NMCDataAccesslayer.Helper;
using System.Web;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using NMCPipedGasLineAPI.Controllers;
using System.Web.Routing;
using System.Net.Mail;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.Net;
using HiQPdf;
using iTextSharp.tool.xml.pipeline.html;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.pipeline.end;
using System.Net.Mime;
using NMCPipedGasLineAPI.Properties;
using System.Text.RegularExpressions;
using System.Data;
using Newtonsoft.Json;
using RestSharp;
using System.Web.UI;

namespace NMCPipedGasLineAPI.Controllers
{
    [RoutePrefix("Customer")]
    public class PaymentController : Controller
    {
        CustomerPayment CustomerPayment = new CustomerPayment();
        CustomerPaymentDataModel CustomerPaymentDataModel = new CustomerPaymentDataModel();
        AreaBL ObjAreaBL = new AreaBL();
        CustomerLoginDataModel objDM = new CustomerLoginDataModel();
        CustLoginDataModel objcDM = new CustLoginDataModel();
        CustomerLoginBL ObjBL = new CustomerLoginBL();
        CustomerBillInformationData objcDMB = new CustomerBillInformationData();
        CustomerBillReading objcustomerReading = new CustomerBillReading();
        PaymentDataModel objPDMB = new PaymentDataModel();
        CompanyBL ObjCompanyBL = new CompanyBL();
        GodownBL objGodownBL = new GodownBL();
        ChangPwDataModel objchang = new ChangPwDataModel();
        CustomerReading objCR = new CustomerReading();
        List<CustomerBillReading> objBCRList = new List<CustomerBillReading>();
        List<CustomerReading> objCRList = new List<CustomerReading>();
        static float billCount = 0;
        static decimal godowServiceCharge, miniPayableAmount, godowServiceChargeMaster, miniPayableAmountMaster;
        // GET: Payment
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CustomerPay()
        {
            Session["CustPaymentTest"] = null;
            CustPaymentModel custPaymentModel = new CustPaymentModel();
            custPaymentModel.MerchantID = "abcd";
            custPaymentModel.CustomerID = "2eb5a964-b6bc-42e2-b240-50f00006fc1b";
            custPaymentModel.Amount = 1;
            custPaymentModel.CurrencyType = "INR";
            custPaymentModel.TypeField1 = "R";
            custPaymentModel.SecurityID = "abcd";
            custPaymentModel.TypeField2 = "F";
            custPaymentModel.AdditionalInfo1 = "7415966605";
            custPaymentModel.AdditionalInfo2 = "rishabh@synsoftglobal.com";
            custPaymentModel.RU = "";
            custPaymentModel.ActionURL = "Payment/PostCustomerPay";
            return View(custPaymentModel);
        }

        public string GetHMACSHA256(string text, string key)
        {
            string hex = "";
            try
            {
                UTF8Encoding encoder = new UTF8Encoding();

                byte[] hashValue;
                byte[] keybyt = encoder.GetBytes(key);
                byte[] message = encoder.GetBytes(text);

                HMACSHA256 hashString = new HMACSHA256(keybyt);


                hashValue = hashString.ComputeHash(message);
                foreach (byte x in hashValue)
                {
                    hex += String.Format("{0:x2}", x);
                }
            }
            catch (Exception ex)
            {
                hex = ex.Message;
            }
            return hex;
        }

        [HttpPost]
        public void PostCustomerPay()
        {

            Dictionary<String, String> paytmParams = new Dictionary<String, String>();
            String merchantMid = ConfigurationManager.AppSettings["merchantMid"];
            String custId = "Ab123457";
            String txnAmount = "2.00";
            String callbackUrl = ConfigurationManager.AppSettings["callbackUrl"];
            string paytmChecksum = ConfigurationManager.AppSettings["paytmChecksum"];
            string TypeField1 = ConfigurationManager.AppSettings["TypeField1"];
            string TypeField2 = ConfigurationManager.AppSettings["TypeField2"];
            string SecurityId = ConfigurationManager.AppSettings["SecurityId"];
            string Redirecturl = ConfigurationManager.AppSettings["Redirecturl"];
            string billid = "1234567";
            //callbackUrl = "http://nmcadmin.azurewebsites.net/payment/ThankYou?custId='" + custId + "'&billid=123654";

            string data = "" + merchantMid + "|" + custId + "|NA|" + txnAmount + "|NA|NA|NA|INR|NA|" + TypeField1 + "|" + SecurityId + "|NA|NA|" + TypeField2 + "|" + billid + "|NA|NA|NA|NA|NA|NA|" + callbackUrl + "";
            String commonkey = paytmChecksum;

            //String data1 = "NMCGAS|000000001|NA|2.00|NA|NA|NA|INR|NA|R|nmcgas|NA|NA|F|NA|NA|NA|NA|NA|NA|NA|http://nmcadmin.azurewebsites.net/payment/postdata";
            String hash = String.Empty;
            hash = GetHMACSHA256(data, commonkey);
            string msg = "" + merchantMid + "|" + custId + "|NA|" + txnAmount + "|NA|NA|NA|INR|NA|" + TypeField1 + "|" + SecurityId + "|NA|NA|" + TypeField2 + "|" + billid + "|NA|NA|NA|NA|NA|NA|" + callbackUrl + "|" + hash.ToUpper() + "";
            //Response.Redirect("https://pgi.billdesk.com/pgidsk/PGIMerchantPayment?msg=" + msg);
            Response.Redirect(Redirecturl + "?msg=" + msg);
        }

        public ActionResult postdata()
        {
            // String merchantKey = "gKpu7IKaLSbkchFS";
            List<string> newListLatam = new List<string>();
            List<string> newListLatam2 = new List<string>();
            Dictionary<String, String> paytmParams = new Dictionary<String, String>();
            string paytmChecksum = "";

            foreach (string key in Request.Form.Keys)
            {
                try
                { //save(key.Trim(), Request.Form[key].Trim());
                }
                catch
                {

                    paytmParams.Add(key.Trim(), Request.Form[key].Trim());
                    newListLatam.Add(key.Trim());
                    newListLatam2.Add(Request.Form[key].Trim());
                }



            }
            if (paytmParams.ContainsKey("CHECKSUMHASH"))
            {
                paytmChecksum = paytmParams["CHECKSUMHASH"];
                paytmParams.Remove("CHECKSUMHASH");
            }
            Session["paytmParams"] = newListLatam;
            Session["paytmParams1"] = newListLatam2;


            return View();
        }


        public ActionResult ConvertAboutPageToPdf()
        {
            Payment obj = new Payment();
            string root = Server.MapPath("~/img/nmc_logo.png");
            ObjAreaBL.ErrorSave(root, "", "", "");

            obj.Logimg = root;
            obj.CutomerEmail = "lavish01@yopmail.com";
            obj.CutomerName = "jagrati";
            obj.ReceiptNo = "er3434";
            obj.BillDate = "2019-06-18";
            obj.BillNo = "sdfsdf";
            obj.Amount = 3;
            string htmlToConvert = RenderPartialViewToString("ReceiptPDF", obj);

            // get the About view HTML code
            //string htmlToConvert = RenderViewAsString("About", null);

            // the base URL to resolve relative images and css
            String thisPageUrl = this.ControllerContext.HttpContext.Request.Url.AbsoluteUri;
            String baseUrl = thisPageUrl.Substring(0, thisPageUrl.Length - "Payment/ConvertAboutPageToPdf".Length);

            // instantiate the HiQPdf HTML to PDF converter
            HtmlToPdf htmlToPdfConverter = new HtmlToPdf();

            // render the HTML code as PDF in memory
            byte[] pdfBuffer = htmlToPdfConverter.ConvertHtmlToMemory(htmlToConvert, baseUrl);

            // send the PDF file to browser
            FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "AboutMvcViewToPdf.pdf";

            return fileResult;
        }


        public ActionResult ThankYou(string code)
        {

            // String merchantKey = "gKpu7IKaLSbkchFS";0
            List<string> newListLatam = new List<string>();
            List<string> newListLatam2 = new List<string>();
            Dictionary<String, String> paytmParams = new Dictionary<String, String>();
            //string paytmChecksum = "";
            CustomerPayment = new CustomerPayment();
            foreach (string key in Request.Form.Keys)
            {
                paytmParams.Add(key.Trim(), Request.Form[key].Trim());
                newListLatam.Add(key.Trim());
                newListLatam2.Add(Request.Form[key].Trim());
            }
            try
            {
                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<CustomerPaymentDataModel, CustomerPayment>();
                });
                string result = "";
                if (paytmParams.ContainsKey("msg"))
                {
                    string msg = ""; //"NMCGAS|000000001|NA|NA|2.00|CIT|NA|NA|INR|DIRECT|NA|NA|NA|30-05-2019 17:27:37|NA|NA|NA|NA|NA|NA|NA|NA|NA|ERR113|Sorry. We were unable to process your transaction. We apologise for the inconvenience and request you to try again later.|E9EA0C393AEE35C3475B358B52BFFD2FE0A94DEDD2EECA1D778EF950A5632A26";
                    paytmParams.TryGetValue("msg", out msg);
                    List<string> array = msg.Split('|').ToList();
                    CustomerPayment.ResponseMsg = msg;
                    CustomerPayment.MerchantID = array.Count() > 0 ? Convert.ToString(array[0]) : "";
                    CustomerPayment.CustomerID = array.Count() > 1 ? Convert.ToString(array[1].ToString()) : "";
                    CustomerPayment.TxnReferenceNo = array.Count() > 2 ? Convert.ToString(array[2].ToString()) : "";
                    CustomerPayment.BankReferenceNo = array.Count() > 3 ? Convert.ToString(array[3].ToString()) : "";
                    CustomerPayment.TxnAmount = array.Count() > 4 ? Convert.ToString(array[4].ToString()) : "";
                    CustomerPayment.BankID = array.Count() > 5 ? Convert.ToString(array[5].ToString()) : "";
                    CustomerPayment.BankMerchantID = array.Count() > 6 ? Convert.ToString(array[6].ToString()) : "";
                    CustomerPayment.TxnType = array.Count() > 7 ? Convert.ToString(array[7].ToString()) : "";
                    CustomerPayment.CurrencyName = array.Count() > 8 ? Convert.ToString(array[8].ToString()) : "";
                    CustomerPayment.ItemCode = array.Count() > 9 ? Convert.ToString(array[9].ToString()) : "";
                    CustomerPayment.SecurityType = array.Count() > 10 ? Convert.ToString(array[10].ToString()) : "";
                    CustomerPayment.SecurityType = array.Count() > 11 ? Convert.ToString(array[11].ToString()) : "";
                    CustomerPayment.SecurityPassword = array.Count() > 12 ? Convert.ToString(array[12].ToString()) : "";
                    CustomerPayment.TxnDate = array.Count() > 13 ? Convert.ToString(array[13].ToString()) : "";
                    CustomerPayment.AuthStatus = array.Count() > 14 ? Convert.ToString(array[14].ToString()) : "";
                    CustomerPayment.SettlementType = array.Count() > 15 ? Convert.ToString(array[15].ToString()) : "";
                    CustomerPayment.AdditionalInfo1 = array.Count() > 16 ? Convert.ToString(array[16].ToString()) : "";
                    CustomerPayment.AdditionalInfo2 = array.Count() > 17 ? Convert.ToString(array[17].ToString()) : "";
                    CustomerPayment.AdditionalInfo3 = array.Count() > 18 ? Convert.ToString(array[18].ToString()) : "";
                    CustomerPayment.AdditionalInfo4 = array.Count() > 19 ? Convert.ToString(array[19].ToString()) : "";
                    CustomerPayment.AdditionalInfo5 = array.Count() > 20 ? Convert.ToString(array[20].ToString()) : "";
                    CustomerPayment.AdditionalInfo6 = array.Count() > 21 ? Convert.ToString(array[21].ToString()) : "";
                    CustomerPayment.AdditionalInfo7 = array.Count() > 22 ? Convert.ToString(array[22].ToString()) : "";
                    CustomerPayment.ErrorStatus = array.Count() > 23 ? Convert.ToString(array[23].ToString()) : "";
                    CustomerPayment.ErrorDescription = array.Count() > 24 ? Convert.ToString(array[24].ToString()) : "";
                    CustomerPayment.CheckSum = array.Count() > 25 ? Convert.ToString(array[25].ToString()) : "";
                    CustomerPaymentDataModel = Mapper.Map<CustomerPayment, CustomerPaymentDataModel>(CustomerPayment);
                    string res = ObjBL.SaveCustomerPayment(CustomerPaymentDataModel);

                    Session["user"] = CustomerPayment.AdditionalInfo5;
                    Session["Name"] = CustomerPayment.AdditionalInfo1;
                    Session["CustomerNumber"] = CustomerPayment.AdditionalInfo2;
                    if (CustomerPayment.AuthStatus == "0300")
                    {
                        Payment payment = new Payment();
                        AutoMapper.Mapper.Reset();

                        objPDMB = ObjBL.GetAmountDetailsbyBillId(CustomerPayment.AdditionalInfo5, CustomerPayment.AdditionalInfo3);
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Payment, PaymentDataModel>();
                        });
                        payment = Mapper.Map<PaymentDataModel, Payment>(objPDMB);
                        if (payment.BillId != "")
                        {
                            payment.Amount = Convert.ToDecimal(CustomerPayment.TxnAmount);
                            var details = ObjBL.calculateDelayCharges(payment.UniqueReceiptNo, payment.BillDate, payment.LatePaymentFee, payment.DelayDays, payment.DelayMinAmount, payment.PaymentDue, payment.latefee, payment.closingBalance, payment.PreviousLateFree);
                            payment.ReceiptNo = ObjBL.GetReceiptno(payment.UniqueReceiptNo, payment.shortname, payment.YEAR, payment.MONTH, payment.ReceiptNo);
                            payment.BalanceDue = details.Item5;
                            payment.Delaychg = details.Item1;
                            payment.TotalAmount = details.Item4;
                            payment.closingBalance = details.Item3;
                            payment.latefee = details.Item2;
                            payment.UniqueReceiptNo = details.Item6;
                            payment.Number = Convert.ToString(details.Item7);
                            payment.BillId = CustomerPayment.AdditionalInfo3;
                            payment.TransactionFree = !String.IsNullOrEmpty(CustomerPayment.AdditionalInfo4) ? Convert.ToDecimal(CustomerPayment.AdditionalInfo4) : 0;
                            payment.CustomerID = CustomerPayment.AdditionalInfo5;
                            AutoMapper.Mapper.Reset();
                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<PaymentDataModel, Payment>();
                            });

                            objPDMB = Mapper.Map<Payment, PaymentDataModel>(payment);

                            AutoMapper.Mapper.Reset();
                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<PaymentDataModel, Payment>();
                            });

                            objPDMB = Mapper.Map<Payment, PaymentDataModel>(payment);
                            result = ObjBL.SavePayment(objPDMB);
                        }
                        else
                        {
                            payment.Amount = Convert.ToDecimal(CustomerPayment.TxnAmount);
                            payment.ReceiptNo = ObjBL.GetReceiptno(payment.UniqueReceiptNo, payment.shortname, payment.YEAR, payment.MONTH, payment.ReceiptNo);
                            payment.BalanceDue = payment.TotalAmount;
                            payment.Delaychg = 0;
                            payment.TotalAmount = Math.Round(payment.TotalAmount, 2);
                            payment.closingBalance = 0;
                            payment.latefee = 0;
                            payment.UniqueReceiptNo = payment.UniqueReceiptNo + 1;
                            payment.Number = Convert.ToString(payment.UniqueReceiptNo);
                            payment.BillId = "";
                            payment.TransactionFree = !String.IsNullOrEmpty(CustomerPayment.AdditionalInfo4) ? Convert.ToDecimal(CustomerPayment.AdditionalInfo4) : 0;
                            payment.CustomerID = CustomerPayment.AdditionalInfo5;
                            AutoMapper.Mapper.Reset();
                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<PaymentDataModel, Payment>();
                            });

                            objPDMB = Mapper.Map<Payment, PaymentDataModel>(payment);

                            AutoMapper.Mapper.Reset();
                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<PaymentDataModel, Payment>();
                            });

                            objPDMB = Mapper.Map<Payment, PaymentDataModel>(payment);
                            result = ObjBL.SavePaymentWithoutBill(objPDMB);

                        }


                        if (result == "Update")
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                Document doc = new Document(PageSize.A4);

                                //Document doc = new Document(PageSize.A4, 60, 60, 10, 10);
                                PdfWriter pw = PdfWriter.GetInstance(doc, ms);
                                doc.Open();
                                //your code to write something to the pdf

                                var logo = iTextSharp.text.Image.GetInstance(Server.MapPath("~/img/nmc_logo.png"));
                                logo.SetAbsolutePosition(300, 800);
                                logo.ScaleAbsoluteHeight(100);
                                logo.ScaleAbsoluteWidth(300);
                                //  doc.Add(logo);

                                PdfPTable BlankTable = new PdfPTable(1);
                                PdfPCell cell1Blank = new PdfPCell(logo);
                                cell1Blank.VerticalAlignment = Element.ALIGN_CENTER;
                                cell1Blank.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell1Blank.Border = 0;
                                BlankTable.AddCell(cell1Blank);

                                PdfPCell blankcell = new PdfPCell(new Phrase(Chunk.NEWLINE));
                                blankcell.FixedHeight = 10.0f;
                                blankcell.Border = 0;
                                BlankTable.AddCell(blankcell);

                                PdfPTable table1 = new PdfPTable(1);
                                table1.DefaultCell.Border = 0;
                                var titleFont = new Font(Font.FontFamily.UNDEFINED, 12, iTextSharp.text.Font.BOLD);
                                var subTitleFont = new Font(Font.FontFamily.UNDEFINED, 12, iTextSharp.text.Font.BOLD);
                                table1.AddCell("");

                                PdfPCell cell11 = new PdfPCell(new Phrase("NMC PIPED GAS", subTitleFont));
                                cell11.Border = 0; cell11.HorizontalAlignment = Element.ALIGN_CENTER;
                                PdfPCell cell12 = new PdfPCell(new Phrase("COMPANY PVT.LTD", subTitleFont));
                                cell12.Border = 0; cell12.HorizontalAlignment = Element.ALIGN_CENTER;

                                table1.AddCell(cell11);
                                table1.AddCell(cell12);


                                PdfPCell blankcel2 = new PdfPCell(new Phrase(Chunk.NEWLINE));
                                blankcel2.FixedHeight = 10.0f;
                                blankcel2.Border = 0;
                                table1.AddCell(blankcel2);


                                PdfPCell add1 = new PdfPCell(new Phrase("4th FLoor, Nav Maharashtra House, 43 Shaniwar Peth,", new Font(Font.FontFamily.UNDEFINED, 10)));
                                add1.Border = 0;
                                add1.HorizontalAlignment = Element.ALIGN_CENTER;
                                table1.AddCell(add1);

                                PdfPCell add2 = new PdfPCell(new Phrase("Pune­ 411030, Tel. No. 9028222283/64/65", new Font(Font.FontFamily.UNDEFINED, 10)));
                                add2.Border = 0;
                                add2.HorizontalAlignment = Element.ALIGN_CENTER;
                                table1.AddCell(add2);



                                PdfPCell add3 = new PdfPCell(new Phrase("Contact: Accounts 9823328222, 9823328245, 9873353335", new Font(Font.FontFamily.UNDEFINED, 8)));
                                add3.Border = 0;
                                add3.HorizontalAlignment = Element.ALIGN_CENTER;
                                table1.AddCell(add3);

                                PdfPCell blankcel3 = new PdfPCell(new Phrase(Chunk.NEWLINE));
                                blankcel3.FixedHeight = 10.0f;
                                blankcel3.Border = 0;
                                table1.AddCell(blankcel3);


                                Chunk c11 = new Chunk("Receipt No. : ", new Font(Font.FontFamily.UNDEFINED, 10));
                                Chunk c21 = new Chunk(payment.ReceiptNo, new Font(Font.FontFamily.UNDEFINED, 10, iTextSharp.text.Font.BOLD));
                                Phrase p22 = new Phrase();
                                p22.Add(c11); p22.Add(c21);
                                PdfPCell add4 = new PdfPCell(p22);
                                add4.Border = 0;
                                add4.HorizontalAlignment = Element.ALIGN_CENTER;
                                table1.AddCell(add4);

                                PdfPCell blankcel4 = new PdfPCell(new Phrase(Chunk.NEWLINE));
                                blankcel4.FixedHeight = 10.0f;
                                blankcel4.Border = 0;
                                table1.AddCell(blankcel4);

                                PdfPCell add5 = new PdfPCell(new Phrase("Bill Information", new Font(Font.FontFamily.UNDEFINED, 10, iTextSharp.text.Font.NORMAL)));
                                add5.Border = 0;
                                add5.HorizontalAlignment = Element.ALIGN_CENTER;
                                table1.AddCell(add5);


                                PdfPCell blankcel5 = new PdfPCell(new Phrase(Chunk.NEWLINE));
                                blankcel5.Border = 0;
                                table1.AddCell(blankcel5);



                                PdfPTable table2 = new PdfPTable(7);
                                float[] widths = new float[] { 10f, 1f, 10f, 1f, 10f, 1f, 10f };
                                table2.SetWidths(widths);
                                PdfPCell cell31 = new PdfPCell(new Phrase("Date", new Font(Font.FontFamily.UNDEFINED, 8)));
                                cell31.VerticalAlignment = Element.ALIGN_CENTER;
                                cell31.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell31.Border = 0;
                                table2.AddCell(cell31);

                                PdfPCell cell117 = new PdfPCell(new Phrase(""));
                                cell117.Border = 0;

                                table2.AddCell(cell117);


                                PdfPCell cell32 = new PdfPCell(new Phrase("Name", new Font(Font.FontFamily.UNDEFINED, 8)));
                                cell32.Border = 0;
                                cell32.VerticalAlignment = Element.ALIGN_CENTER;
                                cell32.HorizontalAlignment = Element.ALIGN_CENTER;
                                table2.AddCell(cell32);

                                PdfPCell cell113 = new PdfPCell(new Phrase(""));
                                cell113.Border = 0;

                                table2.AddCell(cell113);

                                PdfPCell cell33 = new PdfPCell(new Phrase("Payment Mode", new Font(Font.FontFamily.UNDEFINED, 8)));

                                cell33.Border = 0;
                                cell33.VerticalAlignment = Element.ALIGN_CENTER;
                                cell33.HorizontalAlignment = Element.ALIGN_CENTER;
                                table2.AddCell(cell33);

                                PdfPCell cell114 = new PdfPCell(new Phrase(""));
                                cell114.Border = 0;

                                table2.AddCell(cell114);

                                PdfPCell cell34 = new PdfPCell(new Phrase("Bill No", new Font(Font.FontFamily.UNDEFINED, 8)));
                                cell34.Border = 0;
                                cell34.VerticalAlignment = Element.ALIGN_TOP;
                                cell34.HorizontalAlignment = Element.ALIGN_CENTER;
                                table2.AddCell(cell34);

                                table2.SpacingAfter = 9.5f;
                                PdfPTable table4 = new PdfPTable(7);
                                float[] width = new float[] { 10f, 1f, 10f, 1f, 10f, 1f, 10f };
                                table4.SetWidths(width);
                                PdfPCell cell41 = new PdfPCell(new Phrase(payment.BillDate, new Font(Font.FontFamily.UNDEFINED, 8)));
                                cell41.Border = 0;
                                cell41.FixedHeight = 15.0f;
                                cell41.VerticalAlignment = Element.ALIGN_TOP;
                                cell41.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell41.BackgroundColor = BaseColor.LIGHT_GRAY;
                                table4.AddCell(cell41);


                                PdfPCell cell112 = new PdfPCell(new Phrase(""));
                                cell112.Border = 0;

                                table4.AddCell(cell112);

                                PdfPCell cell42 = new PdfPCell(new Phrase(payment.CutomerName, new Font(Font.FontFamily.UNDEFINED, 8)));
                                cell42.Border = 0;
                                cell42.FixedHeight = 15.0f;
                                cell42.VerticalAlignment = Element.ALIGN_TOP;
                                cell42.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell42.BackgroundColor = BaseColor.LIGHT_GRAY;
                                table4.AddCell(cell42);

                                PdfPCell cell115 = new PdfPCell(new Phrase(""));
                                cell115.Border = 0;

                                table4.AddCell(cell115);

                                PdfPCell cell43 = new PdfPCell(new Phrase("online", new Font(Font.FontFamily.UNDEFINED, 8)));
                                cell43.Border = 0;
                                cell43.FixedHeight = 15.0f;
                                cell43.VerticalAlignment = Element.ALIGN_TOP;
                                cell43.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell43.BackgroundColor = BaseColor.LIGHT_GRAY;
                                table4.AddCell(cell43);


                                PdfPCell cell116 = new PdfPCell(new Phrase(""));
                                cell116.Border = 0;

                                table4.AddCell(cell116);

                                PdfPCell cell44 = new PdfPCell(new Phrase(payment.BillNo, new Font(Font.FontFamily.UNDEFINED, 8)));
                                cell44.Border = 0;
                                cell44.FixedHeight = 15.0f;
                                cell44.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell44.VerticalAlignment = Element.ALIGN_TOP;
                                cell44.BackgroundColor = BaseColor.LIGHT_GRAY;
                                table4.AddCell(cell44);





                                PdfPTable table3 = new PdfPTable(1);
                                PdfPCell blankcel6 = new PdfPCell(new Phrase(Chunk.NEWLINE));
                                blankcel6.Border = 0;
                                table3.AddCell(blankcel6);


                                Chunk c1 = new Chunk("Bill Received : ", new Font(Font.FontFamily.UNDEFINED, 10));
                                Chunk c2 = new Chunk("Rs. " + payment.Amount, new Font(Font.FontFamily.UNDEFINED, 10, iTextSharp.text.Font.BOLD));
                                Phrase p2 = new Phrase();
                                p2.Add(c1); p2.Add(c2);
                                PdfPCell cell3 = new PdfPCell(p2);
                                cell3.Border = 0; cell3.HorizontalAlignment = Element.ALIGN_CENTER; cell3.VerticalAlignment = Element.ALIGN_CENTER;
                                table3.AddCell(cell3);

                                doc.Add(BlankTable);
                                doc.Add(table1);
                                doc.Add(table2);
                                doc.Add(table4);
                                doc.Add(table3);



                                pw.CloseStream = false; //set the closestream property
                                doc.Close(); //close the document without closing the underlying stream
                                ms.Position = 0;


                                MailMessage mm = new MailMessage("nmcgaspipe@gmail.com", payment.CutomerEmail);
                                mm.Subject = "Receipt";
                                string body = "Hello " + payment.CutomerName + " <br/><br/> Your Payment has been recieved.Thanks for making Payment.Please find the attached Reciept. <br/> <br/> Thank You. <br/>Team NMC";
                                mm.Body = body;


                                using (Attachment att = new Attachment(ms, "Receipt.pdf", MediaTypeNames.Application.Pdf))
                                {

                                    mm.IsBodyHtml = true;
                                    mm.Attachments.Add(att);
                                    SmtpClient smtp = new SmtpClient();
                                    smtp.Host = "smtp.gmail.com";
                                    smtp.EnableSsl = true;
                                    NetworkCredential NetworkCred = new NetworkCredential();
                                    NetworkCred.UserName = ConfigurationManager.AppSettings["smtpUser"].ToString();
                                    NetworkCred.Password = ConfigurationManager.AppSettings["smtpPass"].ToString();
                                    smtp.UseDefaultCredentials = true;
                                    smtp.Credentials = NetworkCred;
                                    smtp.Port = 587;
                                    smtp.Send(mm);
                                }

                            }


                            //string root = Server.MapPath("~/img/nmc_logo.png");
                            //ObjAreaBL.ErrorSave(root, "2", "", "");
                            //Payment obj = new Payment();
                            //payment.Logimg = root;

                            //string ActivateUser = RenderPartialViewToString("ReceiptPDF", payment);
                            //ObjAreaBL.ErrorSave(ActivateUser, "3", "", "");
                            //string l_Body = ActivateUser; //+ System.Environment.NewLine;

                            //SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                            ////SelectPdf.PdfCustomPageSize.A4.Height = 100;
                            ////SelectPdf.PdfCustomPageSize.A4.Width = 100;

                            //SelectPdf.PdfDocument doc = converter.ConvertHtmlString(l_Body);
                            //converter.Options.MaxPageLoadTime = 2;
                            //ObjAreaBL.ErrorSave(l_Body, "4", "", "");

                            //using (MemoryStream memoryStream = new MemoryStream())
                            //{
                            //    ObjAreaBL.ErrorSave(l_Body, "5", "", "");
                            //    byte[] bytes = doc.Save();
                            //    ObjAreaBL.ErrorSave(payment.CutomerEmail, "5", "", "");
                            //    MailMessage mm = new MailMessage("nmcgaspipe@gmail.com", payment.CutomerEmail);
                            //    mm.Subject = "Receipt";
                            //    string body = "Hello " + payment.CutomerName + " <br/><br/> Your Payment has been recieved.Thanks for making Payment.Please find the attached Reciept. <br/> <br/> Thank You. <br/>Team NMC";
                            //    mm.Body = body;
                            //    mm.Attachments.Add(new Attachment(new MemoryStream(bytes), "Receipt"));
                            //    mm.IsBodyHtml = true;
                            //    SmtpClient smtp = new SmtpClient();
                            //    smtp.Host = "smtp.gmail.com";
                            //    smtp.EnableSsl = true;
                            //    NetworkCredential NetworkCred = new NetworkCredential();
                            //    NetworkCred.UserName = "nmcgaspipe@gmail.com";
                            //    NetworkCred.Password = "nmc@admin";
                            //    smtp.UseDefaultCredentials = true;
                            //    smtp.Credentials = NetworkCred;
                            //    smtp.Port = 587;
                            //    smtp.Send(mm);

                            //}






                            //string root = Server.MapPath("~/img/nmc_logo.png");
                            //payment.Logimg = root;
                            //string ActivateUser = RenderPartialViewToString("ReceiptPDF", payment);
                            //string l_Body = ActivateUser;

                            //SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                            //SelectPdf.PdfCustomPageSize.A4.Height = 100;
                            //SelectPdf.PdfCustomPageSize.A4.Width = 100;
                            //SelectPdf.PdfDocument doc = converter.ConvertHtmlString(l_Body);

                            //using (MemoryStream memoryStream = new MemoryStream())
                            //{
                            //    byte[] bytes = doc.Save();
                            //    MailMessage mm = new MailMessage(ConfigurationManager.AppSettings["smtpUser"].ToString(), payment.CutomerEmail);
                            //    mm.Subject = "Receipt";
                            //    string body = "Hello " + payment.CutomerName + " <br/><br/> Your Payment has been recieved.Thanks for making Payment.Please find the attached Reciept. <br/> <br/> Thank You. <br/>Team NMC";

                            //    mm.Body = body;
                            //    mm.Attachments.Add(new Attachment(new MemoryStream(bytes), "Receipt"));
                            //    mm.IsBodyHtml = true;
                            //    SmtpClient smtp = new SmtpClient();
                            //    smtp.Host = ConfigurationManager.AppSettings["smtpServer"].ToString();
                            //    smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["smtpSSL"]);
                            //    NetworkCredential NetworkCred = new NetworkCredential();
                            //    NetworkCred.UserName = ConfigurationManager.AppSettings["smtpUser"].ToString();
                            //    NetworkCred.Password = ConfigurationManager.AppSettings["smtpPass"].ToString();
                            //    smtp.UseDefaultCredentials = true;
                            //    smtp.Credentials = NetworkCred;
                            //    smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["smtpPort"]);
                            //    smtp.Send(mm);

                            //}
                        }
                        else
                        {

                            ObjAreaBL.ErrorSave("Message:-" + result + "InnerException:-" + result, "", "", "");
                        }
                    }
                    else
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                ObjAreaBL.ErrorSave("Message:-" + ex.StackTrace + ex.Message + "InnerException:-" + ex.InnerException, "", "", "");
                CustomerPaymentDataModel = Mapper.Map<CustomerPayment, CustomerPaymentDataModel>(CustomerPayment);
                //string res = ObjBL.SaveCustomerPayment(CustomerPaymentDataModel);
                Add(ex.Message);

            }
            if (code != null)
            {
                Session["AuthStatus"] = code;
            }

            if (!String.IsNullOrEmpty(CustomerPayment.AuthStatus))
            { ViewBag.Student = CustomerPayment.AuthStatus; }

            //Response.Redirect("ThankYou?code=" + CustomerPayment.AuthStatus);
            return View("ThankYou");
        }

        public void Add(string message)
        {
            System.IO.StreamWriter sw = null;
            try
            {
                string root = Server.MapPath("~/ExcelFile");
                if (!System.IO.Directory.Exists(root)) { System.IO.Directory.CreateDirectory(root); }

                var path = System.IO.Path.Combine(root + "\\", "LogFile.txt");
                if (System.IO.File.Exists(path))
                {
                    sw = new System.IO.StreamWriter(path, true);
                    sw.WriteLine("\n" + DateTime.Now.ToString() + " : " + message.Trim());
                    sw.Close();
                }
                else
                {
                    System.IO.File.Create(path);

                    sw.WriteLine("\n" + DateTime.Now.ToString() + " : " + message.Trim());
                    sw.Close();
                }
            }
            catch { }

        }

        public ActionResult CustomerCreate()
        {
            CustomerLogin customerLogin = new CustomerLogin();
            customerLogin.Company = ObjCompanyBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
            //customerLogin.GoDown = objGodownBL.GetGodownByCompanyId();
            try
            {
                return View(customerLogin);
            }
            catch (Exception ex)
            {
                ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                ViewBag.Error = ex.Message;
                return View(customerLogin);
            }
        }

        [HttpPost]
        public ActionResult CustomerCreate(CustomerLogin customerLogin)
        {
            try
            {
                CustomerLogin customer = new CustomerLogin();
                if (!ModelState.IsValid)
                {
                    customerLogin.Company = ObjCompanyBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                    if (!string.IsNullOrEmpty(customerLogin.CompanyId))
                    {
                        customerLogin.GoDown = objGodownBL.GetGodownByCompanyId(customerLogin.CompanyId);
                    }
                    return View(customerLogin);
                }
                customerLogin.EmailId = customerLogin.EmailId != null ? customerLogin.EmailId.Trim() : null;
                customerLogin.Phone = customerLogin.Phone != null ? customerLogin.Phone.Trim() : null;

                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<CustomerLoginDataModel, CustomerLogin>();
                });
                objDM = Mapper.Map<CustomerLogin, CustomerLoginDataModel>(customerLogin);
                CustomerLoginDataModel dataModel = ObjBL.searchCustomer(objDM);
                if (!string.IsNullOrEmpty(dataModel.Name))
                {
                    customerLogin.Address = dataModel.Address;
                    customerLogin.EmailId = dataModel.EmailId;
                    customerLogin.Name = dataModel.Name;
                    customerLogin.CustomerId = dataModel.CustomerId;
                    customerLogin.Phone = dataModel.Phone;
                    customerLogin.Company = ObjCompanyBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                    customerLogin.GoDown = objGodownBL.GetGodownByCompanyId(customerLogin.CompanyId);
                }
                else if (string.IsNullOrEmpty(dataModel.Phone) && string.IsNullOrEmpty(dataModel.EmailId))
                {
                    customerLogin.Company = ObjCompanyBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                    customerLogin.GoDown = objGodownBL.GetGodownByCompanyId(customerLogin.CompanyId);
                    TempData["MessageFailed"] = "Your email or phone numebr is not nmc customer!";
                    return View(customerLogin);
                }
                else
                {
                    customerLogin.EmailId = dataModel.EmailId;
                    customerLogin.CustomerId = dataModel.CustomerId;
                    customerLogin.Phone = dataModel.Phone;
                    customerLogin.Company = ObjCompanyBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                    customerLogin.GoDown = objGodownBL.GetGodownByCompanyId(customerLogin.CompanyId);
                    customerLogin.isemail = true;
                    TempData["MessageFailed"] = "You already registerd.If you don't know your password please change!";
                    return View(customerLogin);
                }
                return View(customerLogin);
            }
            catch (Exception ex)
            {
                ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                ViewBag.Error = ex.Message;
                customerLogin.Company = ObjCompanyBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                if (!string.IsNullOrEmpty(customerLogin.CompanyId))
                {
                    customerLogin.GoDown = objGodownBL.GetGodownByCompanyId(customerLogin.CompanyId);
                }
                return View(customerLogin);
            }
        }

        //[Route("Login")]
        public ActionResult CustomerLogin()
        {
            CustLogin custLogin = new CustLogin();
            try
            {
                return View(custLogin);
            }
            catch (Exception ex)
            {
                ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                ViewBag.Error = ex.Message;
                return View(custLogin);
            }
        }

        [HttpPost]
        public ActionResult CustomerLogin(CustLogin custLogin, string submit)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(custLogin);
                }
                if (submit == "GenerateOTP" || submit == "Resend")
                {
                    string OTP = OTPGenerate();
                    custLogin.OTP = OTP;
                    custLogin.EmailId = custLogin.EmailId != null ? custLogin.EmailId.Trim() : "";
                    custLogin.UniqueId = custLogin.UniqueId.Trim();
                    custLogin.Phone = custLogin.Phone != null ? custLogin.Phone.Trim() : "";

                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<CustLoginDataModel, CustLogin>();
                    });
                    objcDM = Mapper.Map<CustLogin, CustLoginDataModel>(custLogin);
                    CustLoginDataModel _res = ObjBL.SaveCustomerLogin(objcDM);
                    if (_res != null)
                    {
                        custLogin.isOTP = true;
                        custLogin.OTP = null;
                        custLogin.Id = _res.CustomerId;
                        custLogin.Name = _res.Name;
                        custLogin.EmailId = _res.EmailId;

                        try
                        {
                            var context = System.Web.HttpContext.Current;
                            var callContext = context;
                            string body = Resource1.OTPEmail.Replace("@EmailId", custLogin.Name).Replace("@OTP", OTP);

                            var testA = Email.SendMailtoUser(custLogin.EmailId, "OTP", $"{body}");
                            try
                            {
                                testA.Start();
                            }
                            catch { }
                        }
                        catch (Exception ex)
                        {
                            var t = ex;
                        }
                        TempData["SuccessOTP"] = Resource1.OTPRecieved;
                        return View(custLogin);
                    }
                    else
                    {
                        custLogin.isOTP = false;
                        TempData["MessageFailed"] = Resource1.InvalidCredential;
                        return View(custLogin);
                    }
                }
                else
                {
                    custLogin.EmailId = custLogin.EmailId != null ? custLogin.EmailId.Trim() : "";
                    custLogin.UniqueId = custLogin.UniqueId.Trim();
                    custLogin.Phone = custLogin.Phone != null ? custLogin.Phone.Trim() : "";
                    custLogin.OTP = custLogin.OTP.Trim();

                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<CustLoginDataModel, CustLogin>();
                    });
                    objcDM = Mapper.Map<CustLogin, CustLoginDataModel>(custLogin);
                    CustLoginDataModel dataModel = ObjBL.OTPCheck(objcDM);
                    if (!string.IsNullOrEmpty(dataModel.UniqueId) && dataModel.OTP == custLogin.OTP)
                    {
                        Session["user"] = dataModel.UniqueId;
                        Session["userEmail"] = dataModel.EmailId;
                        return RedirectToAction("PaymentHistory");
                    }
                    else
                    {
                        custLogin.isOTP = true;
                        TempData["MessageOTP"] = Resource1.InvalidOTP;
                    }
                    return View(custLogin);
                }
            }
            catch (Exception ex)
            {
                ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                ViewBag.Error = ex.Message;
                return View(custLogin);
            }
        }


        public ActionResult BillHistory()
        {
            if (Session["user"] != null)
            {
                CustomerBillInformation CustomerBillInformation = new CustomerBillInformation();
                AutoMapper.Mapper.Reset();
                //objcDMB = ObjBL.GetAmountdetails("");
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<CustomerBillInformation, CustomerBillInformationData>();
                });
                CustomerBillInformation = Mapper.Map<CustomerBillInformationData, CustomerBillInformation>(objcDMB);

                //if (Session["user"] != null)
                //{
                return View(CustomerBillInformation);
            }
            else
            {
                return RedirectToAction("Login", "Payment");
            }
        }


        [HttpPost]
        public ActionResult LoadData()
        {
            //Get parameters
            //if (Session["user"] != null)
            //{
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
            billdata = ObjBL.GetBillHistory(pageNo, pageSize, search, sortColumn, sortColumnDir, Session["user"].ToString());
            if (billdata.Count() > 0)
            {
                totalRecords = billdata.LastOrDefault().TotalRows;
            }
            var data = billdata;
            return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    return RedirectToAction("CustomerLogin", "Payment");
            //}
        }



        [HttpPost]
        public ActionResult LoadPaymentData()
        {
            //if (Session["user"] != null)
            //{
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
            billdata = ObjBL.GetReceiptDetailsbyCustomerId(pageNo, pageSize, search, sortColumn, sortColumnDir, Session["user"].ToString());
            if (billdata.Count() > 0)
            {
                totalRecords = billdata.LastOrDefault().TotalRows;
            }
            var data = billdata;
            return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);
            //}
            //        else
            //        {
            //            return RedirectToAction("CustomerLogin", "Payment");
            //}
        }


        public ActionResult PaymentPage(string customerId)
        {
            //string root = Server.MapPath("~/img/nmc_logo.png");
            //Payment obj = new Payment();
            //obj.Logimg = root;
            //obj.AccountNo = "0";
            //string ActivateUser = RenderPartialViewToString("ReceiptPDF", obj);
            //string l_Body = ActivateUser + System.Environment.NewLine;
            //l_Body = l_Body + System.Environment.NewLine;

            //StringBuilder sb = new StringBuilder();
            //sb.Append(l_Body);
            //StringReader sr = new StringReader(sb.ToString());

            //Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
            //HTMLWorker htmlparser = new HTMLWorker(pdfDoc);

            //using (MemoryStream memoryStream = new MemoryStream())
            //{
            //    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
            //    pdfDoc.Open();
            //    htmlparser.Parse(sr);
            //    pdfDoc.Close();
            //    byte[] bytes = memoryStream.ToArray();

            //    memoryStream.Close();

            //    MailMessage mm = new MailMessage("nmcgaspipe@gmail.com", "Jagratisyn@gmail.com");
            //    mm.Subject = "iTextSharp PDF";
            //    mm.Body = "iTextSharp PDF Attachment";
            //    mm.Attachments.Add(new Attachment(new MemoryStream(bytes), "iTextSharpPDF.pdf"));
            //    mm.IsBodyHtml = true;
            //    SmtpClient smtp = new SmtpClient();
            //    smtp.Host = "smtp.gmail.com";
            //    smtp.EnableSsl = true;
            //    NetworkCredential NetworkCred = new NetworkCredential();
            //    NetworkCred.UserName = "nmcgaspipe@gmail.com";
            //    NetworkCred.Password = "nmc@admin";
            //    smtp.UseDefaultCredentials = true;
            //    smtp.Credentials = NetworkCred;
            //    smtp.Port = 587;
            //    smtp.Send(mm);

            //}


            // Document Doc;
            // Doc = new Document(PageSize.A4, 10f, 10f, 50f, 20f);

            // string filename = "PaySlip";
            // string outXml = RenderPartialViewToString("ReceiptPDF", obj);

            // //l_Body = l_Body + System.Environment.NewLine;
            // //string outXml = selectedhtml.Value;

            //outXml = outXml.Replace("px", "");
            // outXml = outXml.Replace("<br>", "<br/>");

            // MemoryStream memStream = new MemoryStream();
            // TextReader xmlString = new StringReader(outXml);
            // using (Document document = new Document())
            // {
            //     PdfWriter writer = PdfWriter.GetInstance(document, memStream);
            //     document.Open();
            //     byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(outXml);
            //     MemoryStream ms = new MemoryStream(byteArray);
            //     XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, ms, System.Text.Encoding.UTF8);
            //     document.Close();
            // }

            // Response.ContentType = "application/pdf";
            // Response.AddHeader("content-disposition", "attachment;filename=" + filename + ".pdf");
            // Response.Cache.SetCacheability(HttpCacheability.NoCache);
            // Response.BinaryWrite(memStream.ToArray());
            // Response.End();
            // Response.Flush();

            // return Content("df");


            if (Session["user"] != null)
            {
                Payment payment = new Payment();
                AutoMapper.Mapper.Reset();
                objPDMB = ObjBL.GetBilldetails(Session["user"].ToString());
                CompaniesPaymentModel pay = new CompaniesPaymentModel();
                pay = ObjBL.GetCompaniesPaymentSetup(objPDMB.CustomerID);

                //objPDMB=ObjBL.GetAmountDetailsbyBillId("d84e983a-273e-4ca9-9db6-5a2fbf15e73d", "a34db1aa-0482-4291-850e-990521550fda");
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Payment, PaymentDataModel>();
                });
                payment = Mapper.Map<PaymentDataModel, Payment>(objPDMB);


                if (payment.BillId != "")
                {
                    var details = ObjBL.calculateDelayCharges(payment.UniqueReceiptNo, payment.BillDate, payment.LatePaymentFee, payment.DelayDays, payment.DelayMinAmount, payment.PaymentDue, payment.latefee, payment.closingBalance, payment.PreviousLateFree);

                     payment.ReceiptNo = ObjBL.GetReceiptno(payment.UniqueReceiptNo, payment.shortname, payment.YEAR, payment.MONTH, payment.ReceiptNo);

                    payment.BalanceDue = Math.Round(details.Item5);
                    payment.Delaychg = details.Item1;
                    payment.TotalAmount = Math.Round(details.Item4);
                    payment.SecurityId = pay.SecurityId;
                    payment.MerchantId = pay.MerchantId;
                    payment.ChecksumKey = pay.ChecksumKey;
                    if (payment.BalanceDue > 800)
                    {
                        payment.istxt = true;
                        payment.BillAmount = payment.TotalAmount;
                        payment.TransactionFree = Math.Round(((payment.BalanceDue) * Convert.ToDecimal(0.9)) / 100, 2);
                        payment.TotalAmount = Math.Round(payment.TransactionFree + payment.BalanceDue);
                    }
                    else { payment.istxt = false; }
                    payment.Amount = payment.TotalAmount;
                    //  payment.Amount = payment.BalanceDue;
                    payment.closingBalance = details.Item3;
                    payment.latefee = details.Item2;
                    payment.UniqueReceiptNo = details.Item6;
                    payment.Number = Convert.ToString(details.Item7);

                }
                else
                {
                    payment.ReceiptNo = ObjBL.GetReceiptno(payment.UniqueReceiptNo, payment.shortname, payment.YEAR, payment.MONTH, payment.ReceiptNo);
                    payment.BalanceDue = Math.Round(payment.TotalAmount);
                    payment.Delaychg = 0;
                    payment.TotalAmount = Math.Round(payment.TotalAmount);
                    if (payment.BalanceDue > 800)
                    {
                        payment.istxt = true;
                        payment.BillAmount = payment.TotalAmount;
                        payment.TransactionFree = Math.Round(((payment.BalanceDue) * Convert.ToDecimal(0.9)) / 100, 2);
                        payment.TotalAmount = Math.Round(payment.TransactionFree + payment.BalanceDue);
                    }
                    else { payment.istxt = false; }
                    payment.Amount = payment.TotalAmount;
                    // payment.Amount = payment.BalanceDue;
                    payment.closingBalance = 0;
                    payment.latefee = 0;
                    payment.UniqueReceiptNo = payment.UniqueReceiptNo + 1;
                    payment.Number = Convert.ToString(payment.UniqueReceiptNo);

                }


                return View(payment);
            }
            else
            {
                return RedirectToAction("Login", "Payment");
            }
        }


        public class Options
        {
            public bool enableChildWindowPosting { get; set; }
            public bool enablePaymentRetry { get; set; }
            public int retry_attempt_count { get; set; }
            public string txtPayCategory { get; set; }
        }

        public class Root
        {
            public string msg { get; set; }
            public Options options { get; set; }
            public string callbackUrl { get; set; }
        }

        [HttpPost]
        public JsonResult CalculateTransactionFee(Payment payment)
        {
            dynamic showMessageString = string.Empty;

            if (Session["user"] != null)
            {
                if (ModelState.IsValid)
                {
                    if (payment.Amount > 800)
                    {
                        payment.TransactionFree = Math.Round(((payment.Amount) * Convert.ToDecimal(0.9)) / 100, 2);
                        payment.TotalAmount = Math.Round(payment.TransactionFree + payment.Amount, 2);
                        payment.BillAmount = payment.TotalAmount;
                    }
                    else
                    {
                        payment.TransactionFree = 0;
                        payment.TotalAmount = Math.Round(payment.TransactionFree + payment.Amount, 2);
                        payment.BillAmount = payment.TotalAmount;
                    }


                    showMessageString = new AmountJson
                    {
                        TransactionFee = Convert.ToDecimal(payment.TransactionFree),
                        TotalAmount = Convert.ToDecimal(payment.TotalAmount)
                    };

                }
                else
                {
                    showMessageString = new AmountJson
                    {
                        TransactionFee = 0,
                        TotalAmount = 0
                    };
                }
            }
            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult PaymentPage(Payment objPayment)
        {
            dynamic showMessageString = string.Empty;
            if (Session["user"] != null)
            {
                if (ModelState.IsValid)
                {
                    // if (objPayment.TotalAmount != 0)
                    if (objPayment.Amount != 0)
                    {
                        Dictionary<String, String> paytmParams = new Dictionary<String, String>();
                        // String merchantMid = ConfigurationManager.AppSettings["merchantMid"];
                        String merchantMid = objPayment.MerchantId;
                        String custId = Guid.NewGuid().ToString();
                        decimal txnAmount = objPayment.Amount;
                        // decimal txnAmount = objPayment.TotalAmount;
                        // decimal txnAmount = 1;
                        String callbackUrl = ConfigurationManager.AppSettings["callbackUrl"];
                        // string paytmChecksum = ConfigurationManager.AppSettings["paytmChecksum"];
                        string paytmChecksum = objPayment.ChecksumKey;
                        string TypeField1 = ConfigurationManager.AppSettings["TypeField1"];
                        string TypeField2 = ConfigurationManager.AppSettings["TypeField2"];
                        // string SecurityId = ConfigurationManager.AppSettings["SecurityId"];
                        string SecurityId = objPayment.SecurityId;
                        string billid = objPayment.BillId;
                        //objPayment.CutomerName = "gtryghj";
                        //objPayment.CustNumber = "rtytryrty1234";
                        PaymentDataModel objPaymentDataModel = new PaymentDataModel();
                        objPaymentDataModel.BillId = objPayment.BillId;
                        objPaymentDataModel.TransactionFree = objPayment.TransactionFree;

                        string data = "";
                        String commonkey = paytmChecksum;
                        String hash = String.Empty;
                        string msg = "";

                        if (!string.IsNullOrEmpty(billid))
                        {
                            data = "" + merchantMid + "|" + custId + "|NA|" + txnAmount + "|NA|NA|NA|INR|NA|" + TypeField1 + "|" + SecurityId + "|NA|NA|" + TypeField2 + "|" + objPayment.CutomerName + "|" + objPayment.CustNumber + "|" + billid + "|" + objPayment.TransactionFree + "|" + objPayment.CustomerID + "|NA|NA|NA";
                            hash = GetHMACSHA256(data, commonkey);
                            msg = "" + merchantMid + "|" + custId + "|NA|" + txnAmount + "|NA|NA|NA|INR|NA|" + TypeField1 + "|" + SecurityId + "|NA|NA|" + TypeField2 + "|" + objPayment.CutomerName + "|" + objPayment.CustNumber + "|" + billid + "|" + objPayment.TransactionFree + "|" + objPayment.CustomerID + "|NA|NA|NA|" + hash.ToUpper() + "";
                        }
                        else
                        {
                            data = "" + merchantMid + "|" + custId + "|NA|" + txnAmount + "|NA|NA|NA|INR|NA|" + TypeField1 + "|" + SecurityId + "|NA|NA|" + TypeField2 + "|" + objPayment.CutomerName + "|" + objPayment.CustNumber + "|NA|" + objPayment.TransactionFree + "|" + objPayment.CustomerID + "|NA|NA|NA";
                            hash = GetHMACSHA256(data, commonkey);
                            msg = "" + merchantMid + "|" + custId + "|NA|" + txnAmount + "|NA|NA|NA|INR|NA|" + TypeField1 + "|" + SecurityId + "|NA|NA|" + TypeField2 + "|" + objPayment.CutomerName + "|" + objPayment.CustNumber + "|NA|" + objPayment.TransactionFree + "|" + objPayment.CustomerID + "|NA|NA|NA|" + hash.ToUpper() + "";
                        }

                        showMessageString = new
                        {
                            param1 = msg,
                            param2 = callbackUrl
                        };
                        return Json(showMessageString, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        showMessageString = new
                        {
                            param1 = 3,
                            param2 = "You have enter correct amount !!!"
                        };
                        return Json(showMessageString, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    showMessageString = new
                    {
                        param1 = 2,
                        param2 = "We're Sorry!, but something went wrong. Please try again."
                    };
                    return Json(showMessageString, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                showMessageString = new
                {
                    param1 = 1,
                    param2 = "logout"
                };
                return Json(showMessageString, JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpPost]
        //public ActionResult PaymentPage(Payment objPayment)
        //{
        //    if (Session["user"] != null)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            if (objPayment.Amount != 0)
        //            {
        //                Dictionary<String, String> paytmParams = new Dictionary<String, String>();
        //                String merchantMid = "BDSKUATY"; //ConfigurationManager.AppSettings["merchantMid"];
        //                String custId = objPayment.CustomerID;
        //                decimal txnAmount = objPayment.Amount;
        //                String callbackUrl = ConfigurationManager.AppSettings["callbackUrl"];
        //                string paytmChecksum = ConfigurationManager.AppSettings["paytmChecksum"];
        //                string TypeField1 = "R";//ConfigurationManager.AppSettings["TypeField1"];
        //                string TypeField2 = "F";// ConfigurationManager.AppSettings["TypeField2"];
        //                string SecurityId = "bdskuaty";//ConfigurationManager.AppSettings["SecurityId"];
        //                string Redirecturl = "https://services.billdesk.com/checkout-widget/src/app.bundle.js"; //ConfigurationManager.AppSettings["Redirecturl"];
        //                string billid = objPayment.BillId;
        //                PaymentDataModel objPaymentDataModel = new PaymentDataModel();
        //                objPaymentDataModel.BillId = objPayment.BillId;
        //                objPaymentDataModel.TransactionFree = objPayment.TransactionFree;
        //                //string res = ObjBL.UpdateTransactionFree(objPaymentDataModel);



        //                string data = "" + merchantMid + "|" + custId + "|NA|" + txnAmount + "|NA|NA|NA|INR|NA|" + TypeField1 + "|" + SecurityId + "|NA|NA|" + TypeField2 + "|" + billid + "|NA|NA|NA|NA|NA|NA|NA";
        //                String commonkey = paytmChecksum;

        //                String hash = String.Empty;
        //                hash = GetHMACSHA256(data, commonkey);
        //                string msg = "" + merchantMid + "|" + custId + "|NA|" + txnAmount + "|NA|NA|NA|INR|NA|" + TypeField1 + "|" + SecurityId + "|NA|NA|" + TypeField2 + "|" + billid + "|NA|NA|NA|NA|NA|NA|NA|" + hash.ToUpper() + "";
        //                //Response.Redirect(Redirecturl + "?msg=" + msg);

        //                Options Options = new Options();
        //                Options.enableChildWindowPosting = true;
        //                Options.enablePaymentRetry = true;
        //                Options.txtPayCategory = "";
        //                Options.retry_attempt_count = 1;

        //                Root Root = new Root();
        //                Root.callbackUrl = callbackUrl;
        //                Root.msg = msg;
        //                Root.options = Options;


        //                //var client = new RestClient();
        //                //var request = new RestRequest();

        //                //IRestResponse response;
        //                //client = new RestClient(Redirecturl);
        //                //ServicePointManager.SecurityProtocol = (SecurityProtocolType)48 | (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
        //                //request = new RestRequest(Method.GET);
        //                //request.AddHeader("content-type", "application/json");
        //                //request.AddJsonBody(Root);
        //                //response = client.Execute(request);




        //                AutoMapper.Mapper.Reset();
        //                Mapper.Initialize(cfg =>
        //                {
        //                    cfg.CreateMap<PaymentDataModel, Payment>();
        //                });

        //                objPDMB = Mapper.Map<Payment, PaymentDataModel>(objPayment);

        //                AutoMapper.Mapper.Reset();
        //                Mapper.Initialize(cfg =>
        //                {
        //                    cfg.CreateMap<PaymentDataModel, Payment>();
        //                });

        //                objPDMB = Mapper.Map<Payment, PaymentDataModel>(objPayment);
        //                //string res = ObjBL.SavePayment(objPDMB);

        //                //ViewBag.Error = res;

        //                return View(objPayment);
        //            }
        //            else
        //            {
        //                ViewBag.Error1 = "Please enter a Amount bigger than {1}";
        //                return View(objPayment);
        //            }

        //        }
        //        else
        //        {
        //            Payment payment = new Payment();
        //            AutoMapper.Mapper.Reset();
        //            objPDMB = ObjBL.GetBilldetails("");
        //            //objPDMB=ObjBL.GetAmountDetailsbyBillId("d84e983a-273e-4ca9-9db6-5a2fbf15e73d", "a34db1aa-0482-4291-850e-990521550fda");
        //            Mapper.Initialize(cfg =>
        //            {
        //                cfg.CreateMap<Payment, PaymentDataModel>();
        //            });
        //            payment = Mapper.Map<PaymentDataModel, Payment>(objPDMB);

        //            var details = ObjBL.calculateDelayCharges(payment.UniqueReceiptNo, payment.BillDate, payment.LatePaymentFee, payment.DelayDays, payment.DelayMinAmount, payment.PaymentDue, payment.latefee, payment.closingBalance, payment.PreviousLateFree);
        //            payment.ReceiptNo = ObjBL.GetReceiptno(payment.UniqueReceiptNo, payment.shortname, payment.YEAR, payment.MONTH, payment.ReceiptNo);
        //            payment.BalanceDue = details.Item5;
        //            payment.Delaychg = details.Item1;
        //            payment.TotalAmount = details.Item4;
        //            payment.closingBalance = details.Item3;
        //            payment.latefee = details.Item2;
        //            payment.UniqueReceiptNo = details.Item6;
        //            payment.Number = Convert.ToString(details.Item7);

        //            string messages = string.Join(Environment.NewLine, ModelState.Values
        //                           .SelectMany(x => x.Errors)
        //                           .Select(x => x.Exception));
        //            ViewBag.Error = messages;

        //            return View(payment);
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("Login", "Payment");
        //    }
        //}


        public static string RenderPartialViewToString(string viewName, object model)
        {
            using (var sw = new StringWriter())
            {
                PaymentController controller = new PaymentController(); // instance of the required controller (you can pass this as a argument if needed)

                // Create an MVC Controller Context
                var wrapper = new HttpContextWrapper(System.Web.HttpContext.Current);

                RouteData routeData = new RouteData();

                routeData.Values.Add("controller", controller.GetType().Name
                                                            .ToLower()
                                                            .Replace("controller", ""));

                controller.ControllerContext = new System.Web.Mvc.ControllerContext(wrapper, routeData, controller);

                controller.ViewData.Model = model;

                var viewResult = System.Web.Mvc.ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);

                var viewContext = new System.Web.Mvc.ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.ToString();
            }
        }


        public ActionResult PaymentHistory()
        {
            if (Session["user"] != null)
            {
                CustomerBillInformation CustomerBillInformation = new CustomerBillInformation();
                AutoMapper.Mapper.Reset();
                objcDMB = ObjBL.GetAmountdetails(Session["user"].ToString());
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<CustomerBillInformation, CustomerBillInformationData>();
                });
                CustomerBillInformation = Mapper.Map<CustomerBillInformationData, CustomerBillInformation>(objcDMB);
                return View(CustomerBillInformation);
            }
            else
            {
                return RedirectToAction("Login", "Payment");
            }
        }

        public ActionResult ForgotPassword()
        {
            ForgotPwd objforgotPwd = new ForgotPwd();
            return View(objforgotPwd);
        }

        [HttpPost]
        public ActionResult ForgotPassword(ForgotPwd objforgotPwd)
        {
            if (!ModelState.IsValid)
            {
                return View(objforgotPwd);
            }
            ChangePwd chg = new ChangePwd();


            AutoMapper.Mapper.Reset();
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<ChangPwDataModel, ChangePwd>();
            });

            chg.EmailId = objforgotPwd.EmailId;
            objchang = Mapper.Map<ChangePwd, ChangPwDataModel>(chg);
            ChangPwDataModel res = ObjBL.EmailCheck(objchang);
            if (res.Id == null)
            {
                TempData["MessageFailed"] = "Customer Does Not Exists";
                return View(objforgotPwd);
            }
            else
            {
                string _allowedChars = ConfigurationManager.AppSettings["ForgotKey"].ToString();
                Random randNum = new Random();
                char[] chars = new char[6];
                int allowedCharCount = _allowedChars.Length;
                for (int i = 0; i < 6; i++)
                {
                    chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
                }
                string pwd = new string(chars);

                string msg = @"Hello " + res.Name + " <br/><br/> The temporary password for your account associated with  " + res.EmailId + " is " + pwd + " <br/> Please login  and change your password to something you can easily remember. <br/><br/> Thank you. <br/> NMC Team";

                // Send Email
                var _res = Email.SendMailtoUser(res.EmailId, "Forgot Password", msg);
                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<ChangPwDataModel, ForgotPwd>();
                });

                chg.Id = res.Id;
                chg.Password = GetEncrypted(pwd);
                objchang = Mapper.Map<ChangePwd, ChangPwDataModel>(chg);

                string result = ObjBL.changePwd(objchang);

                TempData["Success"] = "Temporary password has been sent to your registered email id";
            }
            return RedirectToAction("Login");
        }

        public ActionResult ChangesPassword()
        {
            ChangePwd changePwd = new ChangePwd();
            changePwd.Id = Session["user"] != null ? Session["user"].ToString() : "";
            return View(changePwd);
        }

        [HttpPost]
        public ActionResult ChangesPassword(ChangePwd changePwd)
        {
            if (!ModelState.IsValid)
            {
                return View(changePwd);
            }
            try
            {
                string Pwd = GetEncrypted(changePwd.Password);

                changePwd.Password = Pwd;

                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<ChangPwDataModel, ChangePwd>();
                });

                objchang = Mapper.Map<ChangePwd, ChangPwDataModel>(changePwd);
                string res = ObjBL.changePwd(objchang);
                TempData["Success"] = "Password changed successfully";

            }
            catch (Exception ex)
            {
                TempData["Success"] = ex.Message;
            }
            return RedirectToAction("Login");
        }

        private string GetEncrypted(string p_PlainText)
        {
            string l_GetEncrypted = string.Empty;
            if (p_PlainText.Trim().Length == 0) { return l_GetEncrypted; }
            BMSecurity3DES objCrypto = new BMSecurity3DES();
            try
            { l_GetEncrypted = objCrypto.Encrypt_(p_PlainText); }
            catch (System.Exception ex) { throw new Exception("-GE: " + ex.Message); }
            finally { objCrypto.Dispose(); objCrypto = null; }
            return l_GetEncrypted;
        }//end of function


        private string GetDecrypted(string p_EncryptedText)
        {
            string l_GetDecrypted = string.Empty;
            if (p_EncryptedText.Trim().Length == 0) { return l_GetDecrypted; }
            BMSecurity3DES objCrypto = new BMSecurity3DES();
            try
            { l_GetDecrypted = objCrypto.Decrypt_(p_EncryptedText); }
            catch (System.Exception ex) { throw new Exception("-GD: " + ex.Message); }
            finally { objCrypto.Dispose(); objCrypto = null; }
            return l_GetDecrypted;
        }//end of function


        public string test()
        {
            string str = "hello";
            char[] charArray = str.ToCharArray();

            for (int k = 0; k < str.Length - 1; k--)
            {
                charArray[k] = str[k];

            }
            for (int i = 0, j = str.Length - 1; i < j; i++, j--)
            {
                charArray[i] = str[j];
                charArray[j] = str[i];
            }
            string reversedstring = new string(charArray);
            return reversedstring;
        }

        [HttpPost]
        public JsonResult GodownData(string value)
        {
            List<GodownDataModel> godownDataModels = objGodownBL.GetGodownByCompanyId(value);
            return Json(godownDataModels);
        }

        //[HttpPost]
        //public JsonResult GeneratePassword(CustomerLogin customerLogin)
        //{
        //    string OTP = "";
        //    Random random = new Random();
        //    int n = random.Next(0, 100000);
        //    OTP += n.ToString("D5");
        //    string Pwd = GetEncrypted(customerLogin.Password);
        //    customerLogin.Password = Pwd;
        //    customerLogin.OTP = OTP;
        //    AutoMapper.Mapper.Reset();
        //    Mapper.Initialize(cfg =>
        //    {
        //        cfg.CreateMap<CustomerLoginDataModel, CustomerLogin>();
        //    });
        //    objDM = Mapper.Map<CustomerLogin, CustomerLoginDataModel>(customerLogin);
        //    string result = ObjBL.SaveCustomerLogin(objDM);
        //    try
        //    {
        //        var context = System.Web.HttpContext.Current;
        //        var callContext = context;
        //        var testA = Email.SendMailtoUser(customerLogin.EmailId, "OTP", $"Your OTP is : {OTP}");
        //        try
        //        {
        //            testA.Start();
        //        }
        //        catch { }
        //    }
        //    catch (Exception ex)
        //    {
        //        var t = ex;
        //    }
        //    TempData["Success"] = "Customer register successfully";
        //    return Json("CustomerLogin");
        //}

        public ActionResult GetPassword(string email)
        {
            try
            {
                string password = ObjBL.GetPassword(email);

                //New Encrypt Password update
                string Pwd = GetDecrypted(password);

                string msg = "Hello " + email + " <br /> <br /> Your Password is: " + Pwd + "<br /> <br /> Thank You.";
                // Send Email
                var _res = Email.SendMailtoUser(email, "Forgot Password", msg);

                TempData["Success"] = "Your password change successfully";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        public string OTPGenerate()
        {
            string OTP = "";
            Random random = new Random();
            int n = random.Next(0, 100000);
            OTP += n.ToString("D5");
            return OTP;
        }

        //
        // POST: /Account/LogOff
        [HttpGet]
        public ActionResult LogOff()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        public ActionResult TermsCondition()
        {
            //if (Session["user"] != null)
            //{
            return View();
            //}
            //else
            //{
            //    return RedirectToAction("Login", "Payment");
            //}

        }


        public ActionResult Login()
        {
            CustLogin custLogin = new CustLogin();
            try
            {
                return View(custLogin);
            }
            catch (Exception ex)
            {
                ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                ViewBag.Error = ex.Message;
                return View(custLogin);
            }
        }

        [HttpPost]
        public ActionResult Login(CustLogin custLogin)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(custLogin);
                }
                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<CustLoginDataModel, CustLogin>();
                });
                objcDM = Mapper.Map<CustLogin, CustLoginDataModel>(custLogin);
                objcDM.password = GetEncrypted(custLogin.password);
                var decrypt = GetDecrypted("i6bv7JEFMTI=");
                CustLoginDataModel dataModel = ObjBL.CustomerLogin(objcDM);
                if (dataModel.Id != null)
                {
                    Session["user"] = dataModel.Id;
                    Session["Name"] = dataModel.Name;
                    Session["CustomerNumber"] = dataModel.CustomerNumber;
                    Session["userEmail"] = "";
                    return RedirectToAction("PaymentHistory");
                }
                else
                {
                    ViewBag.Error = "Customer id and password does not match";
                    return View(custLogin);
                }

            }
            catch (Exception ex)
            {
                ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                ViewBag.Error = ex.Message;
                return View(custLogin);
            }
        }


        [HttpGet]
        public ActionResult CustomerReading()
        {
            if (Session["user"] != null)
            {
                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<CustomerBillReading, CustomerReading>();
                });
                objcustomerReading = Mapper.Map<CustomerReading, CustomerBillReading>(objCR);
                // objCR = ObjBL.CheckCustomerBillReading(Session["user"].ToString());
                //objcustomerReading = Mapper.Map<CustomerReading, CustomerBillReading>(objCR);


                return View("CustomerReading", objcustomerReading);


            }
            else
            {
                return RedirectToAction("Login", "Payment");
            }

        }

        [HttpGet]
        public ActionResult CustomerReading1()
        {

            if (Session["user"] != null)
            {
                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<CustomerBillReading, CustomerReading>();
                });
                objcustomerReading = Mapper.Map<CustomerReading, CustomerBillReading>(objCR);
                objCR = ObjBL.GetPreviousCustomerBillReading(Session["user"].ToString());
                objcustomerReading = Mapper.Map<CustomerReading, CustomerBillReading>(objCR);
                return View("CustomerReading1", objcustomerReading);

            }
            else
            {
                return RedirectToAction("Login", "Payment");
            }

        }

        [HttpPost]
        public ActionResult CustomerReading(CustomerBillReading customerBillReading)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (customerBillReading.MeterRedgImage != null || customerBillReading.UpdateMeterRedgImage != null)
                    {
                        string fileName = string.Empty;
                        string FullFileName = string.Empty;
                        customerBillReading.CustomerId = Session["user"].ToString();
                        AutoMapper.Mapper.Reset();
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<CustomerReading, CustomerBillReading>();
                        });
                        objCR = Mapper.Map<CustomerBillReading, CustomerReading>(customerBillReading);
                        ObjBL.SaveCustomerBillRedg(objCR);


                        return RedirectToAction("CustomerReadingHistory", "Payment");
                    }
                    else
                    {
                        ViewBag.ImageError = "Meter Reading Image is required";
                        return View(customerBillReading);
                    }

                }
                else
                {
                    string messages = string.Join(Environment.NewLine, ModelState.Values
                                                 .SelectMany(x => x.Errors)
                                                 .Select(x => x.Exception));
                    ViewBag.Error = messages;
                    return View(customerBillReading);

                }

            }
            catch (Exception ex)
            {
                TempData["Success"] = ex.Message;
                return View(customerBillReading);
            }


        }

        [HttpPost]
        public ActionResult CustomerReading1(CustomerBillReading customerBillReading)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (customerBillReading.MeterRedgImage != null || customerBillReading.UpdateMeterRedgImage != null)
                    {
                        string fileName = string.Empty;
                        string FullFileName = string.Empty;
                        customerBillReading.CustomerId = Session["user"].ToString();

                        BillInformation BillInformation = new BillInformation();
                        BillInformation = GetCustomerBill(customerBillReading.CustomerId, customerBillReading.CurrentRedg);
                        if (BillInformation.msg != "")
                        {
                            if (BillInformation.msg == "1")
                            {
                                ViewBag.Error = "A Bill " + BillInformation.BillNo + " already generated for this customer on " + BillInformation.Previousbilldate + ". Please generate next bill after 5 days from last bill date.";
                                return View("CustomerReading1", objcustomerReading);
                            }
                            else if (BillInformation.msg == "2")
                            {
                                ViewBag.Error = "New reading should be more than or equal to previous reading";
                                return View("CustomerReading1", objcustomerReading);
                            }
                        }

                        BillInformationData objDM = new BillInformationData();
                        AutoMapper.Mapper.Reset();
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<BillInformationData, BillInformation>();
                        });

                        objDM = Mapper.Map<BillInformation, BillInformationData>(BillInformation);

                        string save = ObjBL.Insertbill(objDM, BillInformation.duedt, BillInformation.PreviousBill, customerBillReading.MeterRedgImage);
                        if (save == "Save")
                        {
                            return View("GasBill", BillInformation);
                        }
                        else
                        {
                            ViewBag.ImageError = save;
                            return View(customerBillReading);
                        }

                        // return View("GasBill", BillInformation);
                        //return RedirectToAction("CustomerReadingHistory", "Payment");
                    }
                    else
                    {
                        ViewBag.ImageError = "Meter Reading Image is required";
                        return View(customerBillReading);
                    }

                }
                else
                {
                    string messages = string.Join(Environment.NewLine, ModelState.Values
                                                 .SelectMany(x => x.Errors)
                                                 .Select(x => x.Exception));
                    ViewBag.Error = messages;
                    return View(customerBillReading);

                }

            }
            catch (Exception ex)
            {
                TempData["Success"] = ex.Message;
                return View(customerBillReading);
            }


        }


        [HttpPost]
        public ActionResult GasBill(BillInformation BillInformation)
        {
            //if (Session["user"] != null)
            //{


            return View("GasBill", BillInformation);
            //}
            //else
            //{
            //    return RedirectToAction("Login", "Account");
            //}
        }

        public JsonResult customerRedgImg()
        {
            try
            {
                CustomerBillReading customerBillReading = new CustomerBillReading();
                string fileName = string.Empty;
                customerBillReading.CustomerId = Session["user"].ToString();
                var path = "";
                string FullFileName = string.Empty;
                if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
                {
                    var file = Request.Files["ImageFile"];
                    fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    string extension = Path.GetExtension(file.FileName);
                    fileName = customerBillReading.CustomerId + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + DateTime.Now.ToString("HHmmssfff") + extension;
                    path = Path.Combine(Server.MapPath("/CustomerReadingImage/"), fileName);
                    if (!Directory.Exists(Server.MapPath("~/") + "CustomerReadingImage/"))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/") + "CustomerReadingImage/");
                    }
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                    file.SaveAs(path);
                    FullFileName = @"/CustomerReadingImage/" + fileName;
                    customerBillReading.MeterRedgImage = FullFileName;
                }
                return Json(Convert.ToString(customerBillReading.MeterRedgImage), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult CustomerReadingHistory()
        {
            if (Session["user"] != null)
            {
                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<CustomerBillReading, CustomerReading>();
                });
                //objcustomerReading = Mapper.Map<CustomerReading, CustomerBillReading>(objCR);
                //objCR = ObjBL.CheckCustomerBillReading(Session["user"].ToString());
                //objcustomerReading = Mapper.Map<CustomerReading, CustomerBillReading>(objCR);


                // return View("CustomerReading", objcustomerReading);

                return View(objcustomerReading);
            }
            else
            {
                return RedirectToAction("Login", "Payment");
            }
        }

        [HttpPost]
        public ActionResult LoadCustomerReadingData()
        {
            //Get parameters
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            //Get Sort columns value
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            string search = Request.Params["search[value]"];
            if (sortColumn == "CurrentRedg" && sortColumnDir == "asc") sortColumn = "";
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int totalRecords = 0;
            var sEcho1 = Convert.ToInt32((Convert.ToInt32(start) + Convert.ToInt32(length)) / Convert.ToInt32(length));
            int pageNo = !string.IsNullOrEmpty(search) ? 1 : sEcho1;
            List<CustomerReading> objCR = new List<CustomerReading>();
            List<CustomerBillReading> Redgdata = new List<CustomerBillReading>();
            objCR = ObjBL.GetCustomerRedgHistory(pageNo, pageSize, search, sortColumn, sortColumnDir, Session["user"].ToString());
            if (objCR.Count() > 0)
            {
                totalRecords = objCR.LastOrDefault().TotalRows;
            }
            var data = objCR;
            return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult Edit(string CustomerRedgId)
        {
            AutoMapper.Mapper.Reset();
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<CustomerReading, CustomerBillReading>();
            });
            objCR = ObjBL.GetCustomerBillReading(CustomerRedgId);
            objcustomerReading = Mapper.Map<CustomerReading, CustomerBillReading>(objCR);
            return View("CustomerReading", objcustomerReading);

        }

        public decimal totalamt(decimal Totalamt, decimal PLateFee)
        {
            decimal totalAmount = Totalamt + PLateFee + (PLateFee * 18 / 100);
            return totalAmount;
        }

        [HttpPost]
        public ActionResult CustomerReadingHistoryList(string CustomerRedgId)
        {
            if (Session["user"] != null)
            {
                //Get parameters
                //CustomerRedgId = TempData["Id"].ToString();
                TempData.Keep("Id");
                // get Start (paging start index) and length (page size for paging)
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                //Get Sort columns value
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                string search = Request.Params["search[value]"];
                if (sortColumn == "CurrentRedg" && sortColumnDir == "asc") sortColumn = "";
                //string search = Request.Params["search[value]"];
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int totalRecords = 0;
                var sEcho1 = Convert.ToInt32((Convert.ToInt32(start) + Convert.ToInt32(length)) / Convert.ToInt32(length));
                int pageNo = sEcho1; //!string.IsNullOrEmpty(search) ? 1 : sEcho1;
                objCRList = ObjBL.GetCustomerReadingHistoryList(pageNo, pageSize, search, sortColumn, sortColumnDir, Session["user"].ToString());
                if (objCRList.Count() > 0)
                {
                    totalRecords = objCRList.LastOrDefault().TotalRows;
                }
                var data = objCRList;

                return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("CustomerReadingHistory", "Payment");
            }
        }

        public ActionResult CustomerReadingHistory1()
        {
            if (Session["user"] != null)
            {
                AutoMapper.Mapper.Reset();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<CustomerBillReading, CustomerReading>();
                });


                return View(objcustomerReading);
            }
            else
            {
                return RedirectToAction("Login", "Payment");
            }
        }

        [HttpGet]
        public ActionResult ViewBill(string BillId)
        {
            BillInformation BillInformation = new BillInformation();
            BillInformationData objDM = new BillInformationData();
            AutoMapper.Mapper.Reset();
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<BillInformationData, BillInformation>();
            });

            objDM = ObjBL.GetCustomerBillDeatils(BillId);

            decimal total = totalamt(objDM.TotalAmt, objDM.PreviousLateFree);
            //GasConsumtion.getTotalAmount(objDM.ServiceAmt, objDM.SGST, Convert.ToDecimal(objDM.ConsumeUnit), objDM.CGST, objDM.MinAmt, objDM.PreviousLateFree);


            decimal BalanceDue;

            if (objDM.PreviousBalance != 0)
            {
                BalanceDue = GasConsumtion.getBalanceDue(objDM.PreviousBalance, 0, total);//PaymentDue

            }
            else if (objDM.PreviousDiposite != 0)
            {
                BalanceDue = GasConsumtion.getBalanceDue(0, objDM.PreviousDiposite, total);

            }
            else
            {
                BalanceDue = GasConsumtion.getBalanceDue(0, 0, total);

            }

            decimal Round = Convert.ToInt32(BalanceDue);
            decimal differ = Round - BalanceDue;
            objDM.TotalAmt = Math.Round(total, 2);
            objDM.Round = Round;
            objDM.Diff = Math.Round(differ, 2);
            objDM.Dalychg = objDM.PreviousLateFree;
            BillInformation = Mapper.Map<BillInformationData, BillInformation>(objDM);
            BillInformation.Previousbilldate = BillInformation.Lastdat;
            decimal LateFeedealy;
            decimal dealychfg = GasConsumtion.DelayAmount(BillInformation.LatePaymentFee);
            BillInformation.LateFee = dealychfg;
            if (BillInformation.TotalAmt < BillInformation.DelayMinAmount)
            {
                LateFeedealy = 0;
            }
            else
            { LateFeedealy = BillInformation.LatePaymentFee; }


            string dlay = GasConsumtion.calculateDelayDaysAmount(DateTime.Now, BillInformation.DelayDays, Convert.ToDouble(total), Convert.ToDouble(BillInformation.PreviousBalance), Convert.ToDouble(LateFeedealy), Convert.ToDouble(BillInformation.PreviousDiposite));
            BillInformation.delaychglist = dlay;
            BillInformation.obj1 = "ViewBill";
            return View("GasBill", BillInformation);
        }

        public ActionResult ReadingHistoryList(string CustomerRedgId)
        {
            if (Session["user"] != null)
            {
                TempData["Id"] = CustomerRedgId;
                try
                {
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<CustomerReading, CustomerBillReading>();
                    });
                    objBCRList = Mapper.Map<List<CustomerReading>, List<CustomerBillReading>>(objCRList);

                    return View(objBCRList);
                }
                catch (Exception ex)
                {
                    ObjAreaBL.ErrorSave("Message:-" + ex.Message + "InnerException:-" + ex.InnerException, "", "", Session["user"].ToString());
                    return RedirectToAction("error", "Area", new { Error = ex.Message });
                }
            }
            else
            {
                return RedirectToAction("CustomerReadingHistory", "Payment");
            }
        }

        [HttpPost]
        public ActionResult LoadCustomerReading(reading objreading)
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                //Get parameters
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                //Get Sort columns value
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                string search = Request.Params["search[value]"];
                if (sortColumn == "Name" && sortColumnDir == "asc") sortColumn = "";
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int totalRecords = 0;
                var sEcho1 = Convert.ToInt32((Convert.ToInt32(start) + Convert.ToInt32(length)) / Convert.ToInt32(length));
                int pageNo = !string.IsNullOrEmpty(search) ? 1 : sEcho1;
                List<CustomerReading> objCR = new List<CustomerReading>();
                List<CustomerBillReading> Redgdata = new List<CustomerBillReading>();
                objCR = ObjBL.GetAllCustomerRedgHistory(pageNo, pageSize, search, sortColumn, sortColumnDir, objreading);
                if (objCR.Count() > 0)
                {
                    totalRecords = objCR.LastOrDefault().TotalRows;
                }
                var data = objCR;
                return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        public void calculateAmount(int fullChargeMonth)
        {
            godowServiceCharge = fullChargeMonth * godowServiceCharge;
            miniPayableAmount = fullChargeMonth * miniPayableAmount;

        }



        public Tuple<decimal, decimal> calculateServiceAndMinimumAmount(DateTime previousBillDate, DateTime billDate, List<MonthYear> monthYearList)
        {
            DateTime toDate = Convert.ToDateTime(billDate);

            int endDay = toDate.Day;

            DateTime fromDate = Convert.ToDateTime(previousBillDate);

            int startDay = fromDate.Day;

            if (monthYearList.Count() != 0 && monthYearList.Count() > 2)
            {
                if (startDay == endDay)
                {
                    int fullChargeMonth = monthYearList.Count() - 1;
                    billCount = fullChargeMonth;
                    calculateAmount(fullChargeMonth);
                    return Tuple.Create(godowServiceCharge, miniPayableAmount);
                }

                if (startDay > endDay)
                {
                    int fullChargeMonth = monthYearList.Count() - 2;
                    billCount = fullChargeMonth;
                    calculateAmount(fullChargeMonth);
                    int month = monthYearList[monthYearList.Count() - 1].getMonth();
                    int year = monthYearList[monthYearList.Count() - 1].getYear();
                    //calculatLastMonthDay(month, year, startDay, billDate);
                }
                else
                {
                    int fullChargeMonth = monthYearList.Count() - 1;
                    billCount = fullChargeMonth;
                    calculateAmount(fullChargeMonth);
                    int month = monthYearList[0].getMonth();
                    int year = monthYearList[0].getYear();
                    //calculatLastMonthDay(month, year, startDay, billDate);
                }
            }
            else if (monthYearList.Count() != 0)
            {
                if (startDay <= endDay)
                {
                    int fullChargeMonth = monthYearList.Count() - 1;
                    billCount = fullChargeMonth;
                    calculateAmount(fullChargeMonth);
                    int month = monthYearList[0].getMonth();
                    int year = monthYearList[0].getYear();
                    //calculatLastMonthDay(month, year, startDay, billDate);
                }
                else
                {
                    long noOfDays = GasConsumtion.getNumberOfDaysBetweenDates(Convert.ToDateTime(previousBillDate), Convert.ToDateTime(billDate));

                    if (noOfDays < 10)
                    {
                        godowServiceCharge = 0;
                        miniPayableAmount = 0;
                        return Tuple.Create(godowServiceCharge, miniPayableAmount);
                    }

                    if (noOfDays < 20)
                    {
                        billCount = billCount + float.Parse("0.5");
                        godowServiceCharge = godowServiceChargeMaster / 2;
                        miniPayableAmount = miniPayableAmountMaster / 2;
                    }
                    else
                    {
                        billCount = billCount + 1;
                        godowServiceCharge = godowServiceChargeMaster;
                        miniPayableAmount = miniPayableAmountMaster;
                    }

                }
            }
            return Tuple.Create(godowServiceCharge, miniPayableAmount);
        }

        public BillInformation GetCustomerBill(string CustomerId, decimal CurrentRedg)
        {
            BillInformation BillInformation = new BillInformation();
            DataSet ds = new DataSet();
            ds = ObjBL.GetCustomerDeatils(CustomerId, "", "");

            var customer = (from DataRow dr in ds.Tables[0].Rows
                            select new
                            {
                                Id = dr["id"].ToString(),
                                GodownId = dr["GodownId"].ToString(),
                                Name = dr["Name"].ToString(),
                                Address = dr["Address"].ToString(),
                                ClosingBalance = Convert.ToDecimal(dr["ClosingBalance"]),
                                PreviousBillNo = dr["PreviousBillNo"].ToString(),
                                PreviousBillDate = Convert.ToDateTime(dr["PreviousBillDate"]),
                                PreviousRedg = Convert.ToDecimal(dr["PreviousRedg"]),
                                PreviousDue = Convert.ToDecimal(dr["PreviousDue"]),
                                ShortName = dr["ShortName"].ToString(),
                                godownInputRate = Convert.ToDouble(dr["godownInputRate"]),
                                NewServiceCharge = Convert.ToDecimal(dr["NewServiceCharge"]),
                                MinimumGasCharges = Convert.ToDecimal(dr["MinimumGasCharges"]),
                                LatePaymentFee = Convert.ToDecimal(dr["LatePaymentFee"]),
                                DueDays = Convert.ToInt16(dr["DueDays"]),
                                DelayDays = Convert.ToInt16(dr["DelayDays"]),
                                CreatedAt = Convert.ToDateTime(dr["CreatedAt"]),
                                UserId = dr["UserId"].ToString(),
                                DelayMinAmount = Convert.ToDecimal(dr["DelayMinAmount"]),
                                DelayDaysLimit = Convert.ToInt16(dr["DelayDaysLimit"]),
                                billId = Convert.ToString(dr["billId"]),
                            }).FirstOrDefault();

            var CustomerBillNo = (from DataRow dr in ds.Tables[1].Rows
                                  select new
                                  {
                                      BillNo = dr["BillNo"].ToString()
                                  }).FirstOrDefault();
            if (customer.billId != "")
            {
                DateTime currentdate = DateTime.Now;
                DateTime lastbilldate = customer.PreviousBillDate.AddDays(30);
                if (lastbilldate > currentdate)
                {
                    BillInformation.msg = "1";
                    BillInformation.BillNo = CustomerBillNo.BillNo;
                    BillInformation.Previousbilldate = customer.PreviousBillDate.ToString("dd/MM/yyyy");
                    return BillInformation;

                }
            }
            if (CurrentRedg < customer.PreviousRedg)
            {
                //if (CustomerBillNo != null)
                //{
                BillInformation.msg = "2";
                BillInformation.BillNo = CustomerBillNo != null ? CustomerBillNo.BillNo : "";
                BillInformation.Previousbilldate = customer.PreviousBillDate.ToString("dd/MM/yyyy");
                //}
                return BillInformation;
            }


            var CustomerBillInformation = (from DataRow dr in ds.Tables[2].Rows
                                           select new
                                           {
                                               BillId = dr["id"].ToString(),
                                               BillNo = dr["BillNo"].ToString(),
                                               BillDate = Convert.ToDateTime(dr["BillDate1"]),
                                               ClosingBalance = Convert.ToDecimal(dr["ClosingBalance"]),
                                               ClosingRedg = Convert.ToDecimal(dr["ClosingRedg"]),
                                               PreviousBalance = Convert.ToDecimal(dr["PreviousBalance"]),
                                               PaymentDue = Convert.ToDecimal(dr["PaymentDue"]),
                                               PreviousRedg = Convert.ToDecimal(dr["PreviousRedg"]),
                                               LateFee = Convert.ToDecimal(dr["LateFee"]),
                                               BillType = Convert.ToString(dr["BillType"]),
                                           }).FirstOrDefault();


            var StockItemGasRate = (from DataRow dr in ds.Tables[3].Rows
                                    select new StockItemGasRate
                                    {
                                        StockItemId = Convert.ToString(dr["StockItemId"]),
                                        Rate = Convert.ToDecimal(dr["Rate"]),
                                        Ratemonth = Convert.ToString(dr["Ratemonth"]),
                                        RateYear = Convert.ToString(dr["RateYear"]),
                                        weight = Convert.ToDecimal(dr["weight"]),
                                        ToCreatedDate = Convert.ToDateTime(dr["ToCreatedDate1"]),
                                        CreatedDate = Convert.ToDateTime(dr["CreatedDate1"]),
                                    }).ToList();


            GasConsumtion GasConsumtion = new GasConsumtion();
            DateTime PreviousDate = DateTime.Now;
            if (CustomerBillInformation != null && CustomerBillInformation.BillType.ToLower() == "deposit")
            {
                PreviousDate = customer.PreviousBillDate;
            }
            else
            {
                if (customer.billId != "" || !string.IsNullOrEmpty(customer.PreviousBillDate.ToString()))
                {
                    PreviousDate = customer.PreviousBillDate.AddDays(1);
                }
                else
                {
                    PreviousDate = customer.PreviousBillDate;
                }
            }




            string billNo = GasConsumtion.getBillReceiptNo(true, customer.ShortName, CustomerBillNo != null ? CustomerBillNo.BillNo : "");
            string billdate = DateTime.Now.ToString("dd/MM/yyyy");
            string billperiod = GasConsumtion.getBillPeriod(PreviousDate.ToString("dd/MM/yyyy"), DateTime.Now.ToString("dd/MM/yyyy"), customer.CreatedAt.ToString("dd/MM/yyyy"));
            string duedate = GasConsumtion.getDueDate(DateTime.Now, customer.DueDays);
            DateTime duedt = DateTime.Now.AddDays(customer.DueDays);
            decimal closereg = CurrentRedg;
            string cloasingdate = DateTime.Now.ToString("dd/MM/yyyy");

            decimal PreviousRedg = customer.PreviousRedg;
            string Previousbilldate = customer.PreviousBillDate.ToString("dd/MM/yyyy");
            decimal currentscm = GasConsumtion.getCurrentSCM(closereg, PreviousRedg);
            decimal currentkgs = GasConsumtion.getCurrentKGS(currentscm, Convert.ToDecimal(customer.godownInputRate));
            List<MonthYear> MonthYear = GasConsumtion.monthsBetweenDates(PreviousDate, DateTime.Now);
            decimal gasConsume1 = 0;
            DateTime fromDate = PreviousDate;


            List<StockItemGasRate> stockItemGasRateCopy = new List<StockItemGasRate>(StockItemGasRate);

            // remove entry of if rate to And from available before previouse bill date.
            int count = 0;
            foreach (var itemGasRate in stockItemGasRateCopy)
            {

                count++;
                if (count == stockItemGasRateCopy.Count())
                {
                    break;
                }

                DateTime createToDateOfRate = itemGasRate.ToCreatedDate;
                if (createToDateOfRate < fromDate)
                {
                    StockItemGasRate.Remove(itemGasRate);
                }
            }
            if (StockItemGasRate.Count() == 0)
            {

            }
            else
            {

                DateTime createDateOfRate = StockItemGasRate[0].CreatedDate;
                if ((fromDate < createDateOfRate) || (fromDate == createDateOfRate))
                {
                    gasConsume1 = GasConsumtion.gasConsumtionCal(StockItemGasRate, MonthYear, currentkgs, PreviousDate, DateTime.Now);

                }
                else
                {
                    //get previous rate entry.
                    DateTime previousRateToDate = GasConsumtion.formateDateTogetPreviousRateEntry(StockItemGasRate[0].CreatedDate);
                    var l = StockItemGasRate.ToList().Where(x => x.ToCreatedDate == previousRateToDate).FirstOrDefault();
                    if (l != null)
                    {
                        StockItemGasRate.Insert(0, l);
                    }
                    gasConsume1 = GasConsumtion.gasConsumtionCal(StockItemGasRate, MonthYear, currentkgs, PreviousDate, DateTime.Now);
                }
            }

            decimal rate = GasConsumtion.getRate(gasConsume1, currentkgs);
            decimal gasConsume = GasConsumtion.getconsumeunit(rate, currentkgs);

            godowServiceCharge = customer.NewServiceCharge;
            miniPayableAmount = customer.MinimumGasCharges;
            godowServiceChargeMaster = customer.NewServiceCharge;
            miniPayableAmountMaster = customer.MinimumGasCharges;

            long daysDifference = GasConsumtion.getNumberOfDays(PreviousDate, DateTime.Now);
            //var result = calculateServiceAndMinimumAmount(PreviousDate, DateTime.Now, MonthYear);
            decimal godowServiceCharge1 = (godowServiceCharge / 30) * daysDifference;
            //decimal miniAmount = GasConsumtion.calculateGSTMin((miniPayableAmount / 30) * daysDifference);
            decimal miniAmount = (miniPayableAmount / 30) * daysDifference;
            decimal msgst, mcgst = 0;
            decimal delygst, delycgst = 0;
            if (gasConsume == 0)
            {
                msgst = GasConsumtion.calculateGST(miniAmount);
                mcgst = GasConsumtion.calculateGST(miniAmount);
            }
            else
            {
                msgst = 0;
                mcgst = 0;
            }

            decimal ssgst = GasConsumtion.calculateGST(Math.Round(godowServiceCharge1, 2));
            decimal scgst = GasConsumtion.calculateGST(Math.Round(godowServiceCharge1, 2));
            decimal sgst = msgst + ssgst;
            decimal cgst = mcgst + scgst;
            decimal PrevioudDue = customer.PreviousDue;  //PreviousBalance
            decimal deposit = customer.ClosingBalance; //PreviousDiposite
            string PBillID = "";
            decimal PLateFee = 0;
            decimal Dalychg = 0;
            decimal customerPreviousDue = customer.PreviousDue;
            decimal totaldue = 0;
            if (CustomerBillInformation != null)
            {
                PBillID = CustomerBillInformation.BillId;
                if (Convert.ToInt32(customer.PreviousDue) != 0)
                {
                    decimal taxableDelayAmount;
                    if (CustomerBillInformation.PaymentDue < customer.DelayMinAmount)
                    {
                        taxableDelayAmount = 0;
                    }
                    else /*if (lastBillInformation.getBillType().equals(NMCConstant.BILL_TYPE.GAS_CONSUME))*/
                    {
                        // calculate delay charges
                        decimal delayCharge = GasConsumtion.calculateDelayCharges(CustomerBillInformation.BillDate,
                                DateTime.Now, customer.DelayDays, customer.LatePaymentFee);
                        Dalychg = delayCharge;

                        decimal gst = GasConsumtion.DelayAmount(delayCharge);

                        if (CustomerBillInformation.LateFee == 0)
                        {
                            taxableDelayAmount = gst;
                            //taxableDelayAmount = delayCharge + gst;//change for match prious due
                            // update late fee to bean
                            PLateFee = customer.DelayMinAmount;
                        }
                        else
                        {
                            taxableDelayAmount = (gst) - CustomerBillInformation.LateFee;
                            if ((gst) > CustomerBillInformation.LateFee)
                            {
                                PLateFee = taxableDelayAmount + CustomerBillInformation.LateFee;
                            }
                        }
                    }
                    // add late fee amount to previous due amount if applicable
                    customerPreviousDue = CustomerBillInformation.PaymentDue; //+ taxableDelayAmount;
                    totaldue = CustomerBillInformation.PaymentDue + taxableDelayAmount;


                }
            }
            else
            { // callculate delay charge on customer previous due according to previous bill date when customer bill information not available in case of telly import customer as per Anjali ma'am discussion on 26 Nov 2019


                if (Convert.ToInt32(customer.PreviousDue) != 0)
                {
                    decimal taxableDelayAmount;
                    if (customer.PreviousDue < customer.DelayMinAmount)
                    {
                        taxableDelayAmount = 0;
                    }
                    else /*if (lastBillInformation.getBillType().equals(NMCConstant.BILL_TYPE.GAS_CONSUME))*/
                    {
                        decimal delayCharge = GasConsumtion.calculateDelayCharges(PreviousDate,
                                DateTime.Now, customer.DelayDays, customer.LatePaymentFee);

                        Dalychg = delayCharge;
                        decimal gst = GasConsumtion.DelayAmount(delayCharge);

                        taxableDelayAmount = gst;
                    }
                    customerPreviousDue = customer.PreviousDue; //+ taxableDelayAmount;
                    totaldue = customer.PreviousDue + taxableDelayAmount;
                }
            }
            decimal Stotal = GasConsumtion.getTotalAmount(Math.Round(godowServiceCharge1, 2), Math.Round(sgst, 2), Convert.ToDecimal(gasConsume), Math.Round(cgst, 2), Math.Round(miniAmount, 2), 0);
            delygst = GasConsumtion.calculateGST(Dalychg);
            delycgst = GasConsumtion.calculateGST(Dalychg);
            sgst = delygst + Math.Round(sgst, 2);
            cgst = delycgst + Math.Round(cgst, 2);

            decimal total = GasConsumtion.getTotalAmount(Math.Round(godowServiceCharge1, 2), Math.Round(sgst, 2), Convert.ToDecimal(gasConsume), Math.Round(cgst, 2), Math.Round(miniAmount, 2), Dalychg);


            decimal BalanceDue;
            decimal SBalanceDue;
            if (PrevioudDue != 0)
            {
                BalanceDue = GasConsumtion.getBalanceDue(customerPreviousDue, 0, total);//PaymentDue
                SBalanceDue = GasConsumtion.getBalanceDue(customerPreviousDue, 0, Stotal);
            }
            else if (deposit != 0)
            {
                BalanceDue = GasConsumtion.getBalanceDue(0, deposit, total);
                SBalanceDue = GasConsumtion.getBalanceDue(0, deposit, Stotal);
            }
            else
            {
                BalanceDue = GasConsumtion.getBalanceDue(0, 0, total);
                SBalanceDue = GasConsumtion.getBalanceDue(0, 0, Stotal);
            }

            decimal Round = Convert.ToInt32(Math.Round(BalanceDue, 2));
            decimal differ = Round - (Math.Round(BalanceDue, 2));
            decimal SRound = Convert.ToInt32(SBalanceDue);
            decimal Sdiffer = SRound - SBalanceDue;

            decimal LateFeedealy;
            decimal dealychfg = GasConsumtion.DelayAmount(customer.LatePaymentFee);
            //StringBuilder dlay = GasConsumtion.calculateDelayDaysAmount(DateTime.Now, customer.DelayDays, Convert.ToDouble(total), Convert.ToDouble(customer.PreviousDue), Convert.ToDouble(customer.LatePaymentFee), Convert.ToDouble(customer.ClosingBalance));
            //string dlay = GasConsumtion.calculateDelayDaysAmount(DateTime.Now, customer.DelayDays, Convert.ToDouble(total), Convert.ToDouble(customer.PreviousDue), Convert.ToDouble(customer.LatePaymentFee), Convert.ToDouble(customer.ClosingBalance));
            if (total < customer.DelayMinAmount)
            {
                LateFeedealy = 0;
            }
            else
            { LateFeedealy = customer.LatePaymentFee; }
            //customer.LatePaymentFee
            string dlay = GasConsumtion.calculateDelayDaysAmount(DateTime.Now, customer.DelayDays, Convert.ToDouble(total), Convert.ToDouble(customerPreviousDue), Convert.ToDouble(LateFeedealy), Convert.ToDouble(customer.ClosingBalance));


            //var closingdue = GasConsumtion.ClosingBalance(total, customer.ClosingBalance, customer.PreviousDue);
            var closingdue = GasConsumtion.ClosingBalance(Stotal, customer.ClosingBalance, customerPreviousDue);
            BillInformation.CustomerId = customer.Id;
            BillInformation.GodownId = customer.GodownId;
            BillInformation.StockItemId = StockItemGasRate.FirstOrDefault().StockItemId;
            BillInformation.UserId = customer.UserId;
            BillInformation.BillNo = billNo;
            BillInformation.BillDate = billdate;
            BillInformation.ClosingBalance = closingdue.Item1;
            BillInformation.ClosingRedg = Math.Round(closereg, 3);
            BillInformation.PreviousBalance = customerPreviousDue;//PrevioudDue;
            BillInformation.PreviousRedg = Math.Round(PreviousRedg, 3);
            BillInformation.PreviousDue = Math.Round(closingdue.Item2, 2);
            BillInformation.DueDate = duedate;
            BillInformation.ConsumeUnit = Math.Round(gasConsume, 2);
            BillInformation.Rate = Math.Round(rate, 2);
            BillInformation.BillMonth = DateTime.Now.Month;
            BillInformation.ServiceAmt = Math.Round(godowServiceCharge1, 2);
            BillInformation.isPaid = 0;
            BillInformation.TotalAmt = Math.Round(total);
            BillInformation.CGST = Math.Round(cgst, 2);
            BillInformation.SGST = Math.Round(sgst, 2);
            BillInformation.BillType = "F2653C96-46D8-4609-A5ED-8568C129BAA3";
            BillInformation.CurrentScm = Math.Round(currentscm, 3);
            BillInformation.CurrentKGS = Math.Round(currentkgs, 3);
            BillInformation.BillPeriod = billperiod;
            BillInformation.MinAmt = Math.Round(miniAmount, 2);
            BillInformation.PreviousDiposite = Math.Round(deposit, 2);
            BillInformation.BillCount = billCount;
            BillInformation.Round = Round;
            BillInformation.Diff = Math.Round(differ, 2);
            BillInformation.cloasingdate = cloasingdate;
            BillInformation.PartyName = customer.Name;
            BillInformation.Address = customer.Address;
            BillInformation.PreviousRedg = PreviousRedg;
            BillInformation.Previousbilldate = Previousbilldate;
            BillInformation.Balcencedue = Math.Round(BalanceDue);//Math.Round(BalanceDue, 2);
            BillInformation.PaymentDue = Math.Round(customerPreviousDue, 2);


            // Math.Round(BalanceDue, 2);// Math.Round(customerPreviousDue, 2); //;
            BillInformation.LateFee = Math.Round(dealychfg);
            BillInformation.delaychglist = dlay;
            BillInformation.duedt = duedt;
            BillInformation.PreviousBill = customer.PreviousBillDate;
            BillInformation.msg = "";
            BillInformation.TotalPaid = 0;
            BillInformation.obj1 = "";
            BillInformation.PLateFee = GasConsumtion.DelayAmount(Dalychg); //PLateFee;
            BillInformation.PBillId = PBillID;
            BillInformation.Dalychg = Dalychg;
            BillInformation.PreviousLateFree = Dalychg;

            BillInformation.Stotal = Math.Round(Stotal);
            BillInformation.SRound = SRound;
            BillInformation.SDiff = Math.Round(Sdiffer, 2);

            BillInformation.SCurrentScm = Math.Round(currentscm, 3);
            BillInformation.SCurrentKGS = Math.Round(currentkgs, 3);
            return BillInformation;
        }

        //public BillInformation GetCustomerBill(string CustomerId, decimal CurrentRedg)
        //{
        //    BillInformation BillInformation = new BillInformation();
        //    DataSet ds = new DataSet();
        //    ds = ObjBL.GetCustomerDeatils(CustomerId, "", "");

        //    var customer = (from DataRow dr in ds.Tables[0].Rows
        //                    select new
        //                    {
        //                        Id = dr["id"].ToString(),
        //                        GodownId = dr["GodownId"].ToString(),
        //                        Name = dr["Name"].ToString(),
        //                        Address = dr["Address"].ToString(),
        //                        ClosingBalance = Convert.ToDecimal(dr["ClosingBalance"]),
        //                        PreviousBillNo = dr["PreviousBillNo"].ToString(),
        //                        PreviousBillDate = Convert.ToDateTime(dr["PreviousBillDate"]),
        //                        PreviousRedg = Convert.ToDecimal(dr["PreviousRedg"]),
        //                        PreviousDue = Convert.ToDecimal(dr["PreviousDue"]),
        //                        ShortName = dr["ShortName"].ToString(),
        //                        godownInputRate = Convert.ToDouble(dr["godownInputRate"]),
        //                        NewServiceCharge = Convert.ToDecimal(dr["NewServiceCharge"]),
        //                        MinimumGasCharges = Convert.ToDecimal(dr["MinimumGasCharges"]),
        //                        LatePaymentFee = Convert.ToDecimal(dr["LatePaymentFee"]),
        //                        DueDays = Convert.ToInt16(dr["DueDays"]),
        //                        DelayDays = Convert.ToInt16(dr["DelayDays"]),
        //                        CreatedAt = Convert.ToDateTime(dr["CreatedAt"]),
        //                        UserId = dr["UserId"].ToString(),
        //                        DelayMinAmount= Convert.ToDecimal(dr["DelayMinAmount"]),
        //                        DelayDaysLimit= Convert.ToInt16(dr["DelayDaysLimit"]),
        //                        billId= Convert.ToString(dr["billId"]),
        //                    }).FirstOrDefault();

        //    var CustomerBillNo = (from DataRow dr in ds.Tables[1].Rows
        //                          select new
        //                          {
        //                              BillNo = dr["BillNo"].ToString()
        //                          }).FirstOrDefault();
        //    if (customer.billId != "")
        //    {
        //        DateTime currentdate = DateTime.Now;
        //        DateTime lastbilldate = customer.PreviousBillDate.AddDays(30);
        //        if (lastbilldate > currentdate)
        //        {
        //            BillInformation.msg = "1";
        //            BillInformation.BillNo = CustomerBillNo.BillNo;
        //            BillInformation.Previousbilldate = customer.PreviousBillDate.ToString("dd/MM/yyyy");
        //            return BillInformation;

        //        }
        //    }
        //    if (CurrentRedg < customer.PreviousRedg)
        //    {
        //        BillInformation.msg = "2";
        //        BillInformation.BillNo = CustomerBillNo.BillNo;
        //        BillInformation.Previousbilldate = customer.PreviousBillDate.ToString("dd/MM/yyyy");
        //        return BillInformation;
        //    }


        //    var CustomerBillInformation = (from DataRow dr in ds.Tables[2].Rows
        //                                   select new
        //                                   {
        //                                       BillId = dr["id"].ToString(),
        //                                       BillNo = dr["BillNo"].ToString(),
        //                                       BillDate =Convert.ToDateTime(dr["BillDate1"]),
        //                                       ClosingBalance = Convert.ToDecimal(dr["ClosingBalance"]),
        //                                       ClosingRedg = Convert.ToDecimal(dr["ClosingRedg"]),
        //                                       PreviousBalance = Convert.ToDecimal(dr["PreviousBalance"]),
        //                                       PaymentDue = Convert.ToDecimal(dr["PaymentDue"]),
        //                                       PreviousRedg = Convert.ToDecimal(dr["PreviousRedg"]),
        //                                       LateFee = Convert.ToDecimal(dr["LateFee"]),
        //                                   }).FirstOrDefault();


        //    var StockItemGasRate = (from DataRow dr in ds.Tables[3].Rows
        //                            select new StockItemGasRate
        //                            {
        //                                StockItemId = Convert.ToString(dr["StockItemId"]),
        //                                Rate = Convert.ToDecimal(dr["Rate"]),
        //                                Ratemonth = Convert.ToString(dr["Ratemonth"]),
        //                                RateYear = Convert.ToString(dr["RateYear"]),
        //                                weight = Convert.ToDecimal(dr["weight"]),
        //                                ToCreatedDate = Convert.ToDateTime(dr["ToCreatedDate"]).ToString("dd/MM/yyyy"),
        //                                CreatedDate = Convert.ToDateTime(dr["CreatedDate"]).ToString("dd/MM/yyyy"),
        //                            }).ToList();


        //    GasConsumtion GasConsumtion = new GasConsumtion();



        //    string billNo = GasConsumtion.getBillReceiptNo(true, customer.ShortName, CustomerBillNo !=null ? CustomerBillNo.BillNo:"");
        //    string billdate = DateTime.Now.ToString("dd/MM/yyyy");
        //    string billperiod = GasConsumtion.getBillPeriod(customer.PreviousBillDate.ToString("dd/MM/yyyy"), DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy"), customer.CreatedAt.ToString("dd/MM/yyyy"));
        //    string duedate = GasConsumtion.getDueDate(DateTime.Now, customer.DueDays);
        //    DateTime duedt = DateTime.Now.AddDays(customer.DueDays);
        //    decimal closereg = CurrentRedg;
        //    string cloasingdate = DateTime.Now.ToString("dd/MM/yyyy");

        //    decimal PreviousRedg = customer.PreviousRedg;
        //    string Previousbilldate = customer.PreviousBillDate.ToString("dd/MM/yyyy");
        //    decimal currentscm = GasConsumtion.getCurrentSCM(closereg, PreviousRedg);
        //    decimal currentkgs = GasConsumtion.getCurrentKGS(currentscm, Convert.ToDecimal(customer.godownInputRate));
        //    List<MonthYear> MonthYear = GasConsumtion.monthsBetweenDates(customer.PreviousBillDate, DateTime.Now);
        //    decimal gasConsume1 = GasConsumtion.gasConsumtionCal(StockItemGasRate, MonthYear, currentkgs, customer.PreviousBillDate, DateTime.Now);
        //    decimal rate = GasConsumtion.getRate(gasConsume1, currentkgs);
        //    decimal gasConsume = GasConsumtion.getconsumeunit(rate, currentkgs);

        //    //string obj1 = JsonConvert.SerializeObject(MonthYear);
        //    //string obj2 = JsonConvert.SerializeObject(StockItemGasRate);
        //    //long daysDifference = GasConsumtion.getNumberOfDaysBetweenDates(customer.PreviousBillDate, DateTime.Now) - 1;
        //    //decimal perDayConsume = currentkgs / daysDifference;
        //    //ViewBag.currentkgs = currentkgs;
        //    //ViewBag.daysDifference = daysDifference;
        //    //ViewBag.perDayConsume = perDayConsume;
        //    //ViewBag.obj1 = obj1;
        //    //ViewBag.obj2 = obj2;
        //    //ViewBag.gasConsume1 = gasConsume1;
        //    //ViewBag.PreviousBillDate = customer.PreviousBillDate;
        //    //ViewBag.DateTime = DateTime.Now;

        //    godowServiceCharge = customer.NewServiceCharge;
        //    miniPayableAmount = customer.MinimumGasCharges;
        //    godowServiceChargeMaster = customer.NewServiceCharge;
        //    miniPayableAmountMaster = customer.MinimumGasCharges;

        //    var result = calculateServiceAndMinimumAmount(customer.PreviousBillDate, DateTime.Now, MonthYear);
        //    decimal godowServiceCharge1 = result.Item1;
        //    decimal miniAmount = GasConsumtion.calculateGSTMin(result.Item2);
        //    decimal sgst = GasConsumtion.calculateGST(godowServiceCharge1);
        //    decimal cgst = GasConsumtion.calculateGST(godowServiceCharge1);
        //    decimal total = GasConsumtion.getTotalAmount(godowServiceCharge, sgst, Convert.ToDecimal(gasConsume), cgst, miniAmount);
        //    decimal PrevioudDue = customer.PreviousDue;  //PreviousBalance
        //    decimal deposit = customer.ClosingBalance; //PreviousDiposite
        //    //decimal BalanceDue;
        //    //if (PrevioudDue != 0)
        //    //{
        //    //    BalanceDue = GasConsumtion.getBalanceDue(customer.PreviousDue, 0, total);//PaymentDue
        //    //}
        //    //else if (deposit != 0)
        //    //{
        //    //    BalanceDue = GasConsumtion.getBalanceDue(0, deposit, total);
        //    //}
        //    //else
        //    //{
        //    //    BalanceDue = GasConsumtion.getBalanceDue(0, 0, total);
        //    //}

        //    //decimal Round = Convert.ToInt32(BalanceDue);
        //    //decimal differ = Round - BalanceDue;

        //    string PBillID = "";
        //    decimal PLateFee = 0;

        //    decimal customerPreviousDue = customer.PreviousDue;
        //    if (CustomerBillInformation != null)
        //    {
        //        PBillID = CustomerBillInformation.BillId;
        //        if (customer.PreviousDue != 0)
        //        {
        //            decimal taxableDelayAmount;
        //            if (CustomerBillInformation.PaymentDue < customer.DelayMinAmount)
        //            {
        //                taxableDelayAmount = 0;
        //            }
        //            else /*if (lastBillInformation.getBillType().equals(NMCConstant.BILL_TYPE.GAS_CONSUME))*/
        //            {
        //                // calculate delay charges


        //                decimal delayCharge = GasConsumtion.calculateDelayCharges(CustomerBillInformation.BillDate,
        //                        DateTime.Now, customer.DelayDays, customer.LatePaymentFee);


        //                decimal gst = GasConsumtion.DelayAmount(delayCharge);

        //                 if (CustomerBillInformation.LateFee == 0)
        //                {
        //                    taxableDelayAmount =  gst;
        //                    //taxableDelayAmount = delayCharge + gst;//change for match prious due
        //                    // update late fee to bean
        //                    PLateFee = customer.DelayMinAmount;
        //                }
        //                else
        //                {
        //                    taxableDelayAmount = (gst) - CustomerBillInformation.LateFee;
        //                    if ((gst) > CustomerBillInformation.LateFee)
        //                    {
        //                        PLateFee = taxableDelayAmount + CustomerBillInformation.LateFee;


        //                    }
        //                }
        //            }
        //            // add late fee amount to previous due amount if applicable
        //            customerPreviousDue = CustomerBillInformation.PaymentDue + taxableDelayAmount;
        //            //customerPreviousDue = Math.round(customerPreviousDue);

        //        }
        //    }
        //    else
        //    { // callculate delay charge on customer previous due according to previous bill date when customer bill information not available in case of telly import customer as per Anjali ma'am discussion on 26 Nov 2019
        //        if (customer.PreviousDue != 0)
        //        {
        //            decimal taxableDelayAmount;
        //            if (customer.PreviousDue < customer.DelayMinAmount)
        //            {
        //                taxableDelayAmount = 0;
        //            }
        //            else /*if (lastBillInformation.getBillType().equals(NMCConstant.BILL_TYPE.GAS_CONSUME))*/
        //            {


        //                decimal delayCharge = GasConsumtion.calculateDelayCharges(customer.PreviousBillDate,
        //                        DateTime.Now, customer.DelayDays, customer.LatePaymentFee);


        //                decimal gst = GasConsumtion.DelayAmount(delayCharge);

        //                taxableDelayAmount =  gst;
        //            }
        //            customerPreviousDue = customer.PreviousDue + taxableDelayAmount;

        //        }
        //    }


        //    decimal BalanceDue;
        //    if (PrevioudDue != 0)
        //    {
        //        BalanceDue = GasConsumtion.getBalanceDue(customerPreviousDue, 0, total);//PaymentDue
        //    }
        //    else if (deposit != 0)
        //    {
        //        BalanceDue = GasConsumtion.getBalanceDue(0, deposit, total);
        //    }
        //    else
        //    {
        //        BalanceDue = GasConsumtion.getBalanceDue(0, 0, total);
        //    }

        //    decimal Round = Convert.ToInt32(BalanceDue);
        //    decimal differ = Round - BalanceDue;
        //    decimal LateFeedealy;
        //    decimal dealychfg = GasConsumtion.DelayAmount(customer.LatePaymentFee);
        //    //StringBuilder dlay = GasConsumtion.calculateDelayDaysAmount(DateTime.Now, customer.DelayDays, Convert.ToDouble(total), Convert.ToDouble(customer.PreviousDue), Convert.ToDouble(customer.LatePaymentFee), Convert.ToDouble(customer.ClosingBalance));
        //    //string dlay = GasConsumtion.calculateDelayDaysAmount(DateTime.Now, customer.DelayDays, Convert.ToDouble(total), Convert.ToDouble(customer.PreviousDue), Convert.ToDouble(customer.LatePaymentFee), Convert.ToDouble(customer.ClosingBalance));
        //    if (total < customer.DelayMinAmount)
        //    {
        //        LateFeedealy = 0;
        //    }
        //    else
        //    { LateFeedealy = customer.LatePaymentFee; }
        //    //customer.LatePaymentFee
        //    string dlay = GasConsumtion.calculateDelayDaysAmount(DateTime.Now, customer.DelayDays, Convert.ToDouble(total), Convert.ToDouble(customerPreviousDue), Convert.ToDouble(LateFeedealy), Convert.ToDouble(customer.ClosingBalance));


        //    //var closingdue = GasConsumtion.ClosingBalance(total, customer.ClosingBalance, customer.PreviousDue);
        //    var closingdue = GasConsumtion.ClosingBalance(total, customer.ClosingBalance, customerPreviousDue);
        //    BillInformation.CustomerId = customer.Id;
        //    BillInformation.GodownId = customer.GodownId;
        //    BillInformation.StockItemId = StockItemGasRate.FirstOrDefault().StockItemId;
        //    BillInformation.UserId = customer.UserId;
        //    BillInformation.BillNo = billNo;
        //    BillInformation.BillDate = billdate;
        //    BillInformation.ClosingBalance = closingdue.Item1;
        //    BillInformation.ClosingRedg = Math.Round(closereg, 3);
        //    BillInformation.PreviousBalance = customerPreviousDue;//PrevioudDue;
        //    BillInformation.PreviousRedg = Math.Round(PreviousRedg, 3);
        //    BillInformation.PreviousDue = closingdue.Item2;
        //    BillInformation.DueDate = duedate;
        //    BillInformation.ConsumeUnit = Math.Round(gasConsume, 2);
        //    BillInformation.Rate = Math.Round(rate, 2);
        //    BillInformation.BillMonth = DateTime.Now.Month;
        //    BillInformation.ServiceAmt = Math.Round(godowServiceCharge1, 2);
        //    BillInformation.isPaid = 0;
        //    BillInformation.TotalAmt = Math.Round(total, 2);
        //    BillInformation.CGST = Math.Round(cgst, 2);
        //    BillInformation.SGST = Math.Round(sgst, 2);
        //    BillInformation.BillType = "F2653C96-46D8-4609-A5ED-8568C129BAA3";
        //    BillInformation.CurrentScm = Math.Round(currentscm, 3);
        //    BillInformation.CurrentKGS = Math.Round(currentkgs, 3);
        //    BillInformation.BillPeriod = billperiod;
        //    BillInformation.MinAmt = Math.Round(miniAmount, 2);
        //    BillInformation.PreviousDiposite = Math.Round(deposit, 2);
        //    BillInformation.BillCount = billCount;
        //    BillInformation.Round = Round;
        //    BillInformation.Diff = Math.Round(differ, 2);
        //    BillInformation.cloasingdate = cloasingdate;
        //    BillInformation.PartyName = customer.Name;
        //    BillInformation.Address = customer.Address;
        //    BillInformation.PreviousRedg = PreviousRedg;
        //    BillInformation.Previousbilldate = Previousbilldate;
        //    BillInformation.Balcencedue = Math.Round(BalanceDue, 2);
        //    BillInformation.PaymentDue = Math.Round(customerPreviousDue, 2); //Math.Round(PrevioudDue, 2);
        //    BillInformation.LateFee = Math.Round(dealychfg);
        //    BillInformation.delaychglist = dlay;
        //    BillInformation.duedt = duedt;
        //    BillInformation.PreviousBill = customer.PreviousBillDate;
        //    BillInformation.msg = "";
        //    BillInformation.TotalPaid = 0;
        //    BillInformation.obj1 = "";
        //    BillInformation.PLateFee = PLateFee;
        //    BillInformation.PBillId = PBillID;
        //    return BillInformation;
        //}
    }


    public class AmountJson
    {
        public decimal TotalAmount { get; set; }
        public decimal TransactionFee { get; set; }
    }
}