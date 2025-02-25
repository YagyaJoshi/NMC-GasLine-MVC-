using IdentitySample.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using NMCDataAccesslayer.BL;
using NMCDataAccesslayer.DataModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace NMCPipedGasLineAPI.Controllers
{
    [RoutePrefix("api/PaymentAPI")]
    public class PaymentAPIController : ApiController
    {
        
        [HttpGet]
        [Route("GetCustomerBillDataById")]
        public HttpResponseMessage GetCustomerBillDataById(string CustomerId)
        {
            Dictionary<string, Object> _res = new Dictionary<string, object>();
            try
            {

                CustomerBillData model = new CustomerBillData();
                CustomerBL objBl = new CustomerBL();
                if (CustomerId != null)
                {
                    var customer = objBl.GetCustomerBillData(CustomerId);

                    if (customer != null)
                    {
                        if (customer.BillAmount == "")
                        {
                            _res.Add("Status", 1);
                            _res.Add("Data", customer);
                            _res.Add("Message", "NO PENDING DUE");
                        }

                        else
                        {
                            _res.Add("Status", 1);
                            _res.Add("Data", customer);
                            _res.Add("Message", "PENDING DUE");
                        }
                    }
                    else
                    {
                        _res.Add("Status", 1);
                        _res.Add("Data", customer);
                        _res.Add("Message", "No Record Found");
                    }
                }

                else
                {
                    _res.Add("Message", "Please enter CustomerId");
                }

                    return Request.CreateResponse(HttpStatusCode.OK, _res);

            }

            catch (Exception ex)
            {
                _res.Add("Status", 0);
                _res.Add("Message", ex.Message);
                return Request.CreateResponse(HttpStatusCode.BadRequest, _res);
            }
        }

        [HttpPost]
        [Route("SavePaymentDetails")]
        public HttpResponseMessage SavePaymentDetails(CustomerPaymentData payment)
        {
            Dictionary<string, Object> _res = new Dictionary<string, object>();
            try
            {
                CustomerLoginBL ObjBL = new CustomerLoginBL();
                CustomerBL objBl = new CustomerBL();
                
                if (payment.Auth2 != "")
                {
                    payment.Amount = Convert.ToDecimal(payment.TxnAmount);
                    var details = ObjBL.calculateDelayCharges(payment.UniqueReceiptNo, payment.BillDate, payment.LatePaymentFee, payment.DelayDays,
                        payment.DelayMinAmount, payment.PaymentDue, payment.latefee, payment.closingBalance, payment.PreviousLateFree);
                   payment.ReceiptNo = ObjBL.GetReceiptno(payment.UniqueReceiptNo, payment.shortname, payment.YEAR, payment.MONTH, payment.ReceiptNo);
                    payment.BalanceDue = details.Item5;
                    payment.Delaychg = details.Item1;
                    payment.TotalAmount = details.Item4;
                    payment.closingBalance = details.Item3;
                    payment.latefee = details.Item2;
                    payment.UniqueReceiptNo = details.Item6;
                    payment.Number = Convert.ToString(details.Item7);
                    payment.BillId = payment.Auth2;
                    // payment.TransactionFree = !String.IsNullOrEmpty(CustomerPayment.AdditionalInfo4) ? Convert.ToDecimal(CustomerPayment.AdditionalInfo4) : 0;
                    payment.CustomerID = payment.Auth1;

                var customer = objBl.SavePayment(payment);

                    
                    if (customer == "message")
                    {
                        _res.Add("Message", "BillNo does not exists");
                    }

                    else if (customer == "Error")
                    {
                        _res.Add("Status", 0);
                        _res.Add("TransactionId", payment.TxnReferenceNo);
                        _res.Add("CustomerId", payment.Auth1);
                        _res.Add("InvoiceId", payment.Auth2);
                        _res.Add("Message", "Payment received for Invoice id through other transaction id");
                    }
                   
                    else {
                        _res.Add("Status", 1);
                        _res.Add("TransactionId", payment.TxnReferenceNo);
                        _res.Add("CustomerId", payment.Auth1);
                        _res.Add("InvoiceId", payment.Auth2);
                        _res.Add("Message", "Payment Successful");
                    }
                }
                else
                {
                    _res.Add("Message", "Enter Bill no");
                }

               
                return Request.CreateResponse(HttpStatusCode.OK, _res);
            }
            catch (Exception ex)
            {
                _res.Add("Status", 0);
                _res.Add("Message", ex.Message);
                return Request.CreateResponse(HttpStatusCode.BadRequest, _res);
            }
        }


    }
}
