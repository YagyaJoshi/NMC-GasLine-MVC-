using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using IdentitySample.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using NMCDataAccesslayer.BL;
using NMCDataAccesslayer.DataModel;
using NMCDataAccesslayer.Helper;
using NMCPipedGasLineAPI.Properties;
using NMCPipedGasLineAPI.Models;
using System.Configuration;
using System.IO;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;

namespace NMCPipedGasLineAPI.Controllers
{
    [RoutePrefix("api/UserAPI")]
    public class UserAPIController : ApiController
    {

        public UserAPIController()
        {
        }

        public UserAPIController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        private SignInHelper _helper;
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return Request.GetOwinContext().Authentication;
            }
        }
        private SignInHelper SignInHelper
        {
            get
            {
                if (_helper == null)
                {
                    _helper = new SignInHelper(UserManager, AuthenticationManager);
                }
                return _helper;
            }
        }
        /// <summary>
        /// User Login Api 
        /// </summary>
        /// <param name="objLogin"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Login")]
        public async Task<HttpResponseMessage> Login(LoginViewModel objLogin)
        {
            try
            {

                Dictionary<string, Object> _res = new Dictionary<string, object>();
                if (!ModelState.IsValid)
                {
                    string messages = string.Join(Environment.NewLine, ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    _res.Add("Status", 0);
                    _res.Add("Message", messages);
                    return Request.CreateResponse(HttpStatusCode.OK, _res);
                }

                var result = await UserManager.FindAsync(objLogin.Email, objLogin.Password);

                if (result != null)
                {

                    UserBL bl = new UserBL();
                    UserDataModel model = new UserDataModel();
                    model = await bl.GetUserDetail(result.Id);
                    GodownBL objBl = new GodownBL();
                    if (model.RoleName == "Employee")
                    {
                        if (model.isActive != 0)
                        {
                            if (!string.IsNullOrEmpty(model.Id))
                            {
                                var listGodown = await objBl.GetGoDownAreawise(model.AreaId);
                                _res.Add("Status", 1);
                                _res.Add("Message", "Success");
                                _res.Add("UserData", model);
                                _res.Add("GodownList", listGodown);
                            }
                            else
                            {
                                _res.Add("Status", 0);
                                _res.Add("Message", "Failed to Login");
                            }
                        }
                        else
                        {
                            _res.Add("Status", 0);
                            _res.Add("Message", Resource1.Deactivatedbyadmin);
                        }
                    }
                    else
                    {
                        _res.Add("Status", 0);
                        _res.Add("Message", Resource1.InvalidUsernamePassword);
                    }
                }
                else
                {
                    _res.Add("Status", 0);
                    _res.Add("Message", Resource1.InvalidUsernamePassword);
                }
                return Request.CreateResponse(HttpStatusCode.OK, _res);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }


        [HttpGet]
        [Route("GetGoDownList")]
        public async Task<HttpResponseMessage> GetGoDownList(string AreaId)
        {
            Dictionary<string, Object> _res = new Dictionary<string, object>();
            try
            {


                GodownBL objBl = new GodownBL();
                var listGodown = await objBl.GetGoDownAreawise(AreaId);
                if (listGodown.Count > 0)
                {
                    _res.Add("Status", 1);
                    _res.Add("Message", "Success");
                    _res.Add("Data", listGodown.ToList());
                }
                else
                {
                    _res.Add("Status", 1);
                    _res.Add("Message", Resource1.NoRecordFound);

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
        [Route("ForgotPassword")]
        public async Task<HttpResponseMessage> ForgotPassword(string EmailId)
        {
            Dictionary<string, Object> _res = new Dictionary<string, object>();
            try
            {
                var user = await UserManager.FindByNameAsync(EmailId);
                UserBL bl = new UserBL();
                UserDataModel model = new UserDataModel();

                if (user == null)
                {
                    _res.Add("Status", 0);
                    _res.Add("Message", Resource1.EmailIdNotRegister);
                }
                else
                {
                    model = await bl.GetUserDetail(user.Id);
                    //EmailId = "jagratisyn@gmail.com";
                    if (model.RoleName == "Employee")
                    {

                        string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                        string host = ConfigurationManager.AppSettings["baseUrl"];// "http://nmcadmin.azurewebsites.net/";//HttpContext.Current.Request.Url.Authority;
                        string url = $"{host}/Account/ResetPassword?code={code}";
                        string msg = Resource1.ResetEmail.Replace("@Name", model.Name).Replace("@url", url);

                        //"Hello " + model.Name+ " <br />  <br /> Please reset your password by using this given link : " + url + " <br/> <br/> Thank You.";
                        await Email.SendMailtoUser(EmailId, "Reset your Password", msg);
                        _res.Add("Status", 1);
                        _res.Add("Message", Resource1.Resetlink);
                    }
                    else
                    {
                        _res.Add("Status", 0);
                        _res.Add("Message", Resource1.InvalidUsername);
                    }

                }
                //await UserManager.SendEmailAsync(user.Id, "Reset Password", $"Please reset your password by using this {code}");
                return Request.CreateResponse(HttpStatusCode.OK, _res);
            }
            catch (Exception ex)
            {
                _res.Add("Status", 0);
                _res.Add("Message", ex.Message);
                return Request.CreateResponse(HttpStatusCode.BadRequest, _res);
            }
        }


        [HttpGet]
        [Route("GetCustomerList")]
        public async Task<HttpResponseMessage> GetCustomerList(string AreaId)
        {
            Dictionary<string, Object> _res = new Dictionary<string, object>();
            try
            {


                CustomerBL objBl = new CustomerBL();
                var listCustomer = await objBl.GetCustomerAreawise(AreaId);
                if (listCustomer.Count > 0)
                {
                    _res.Add("Status", 1);
                    _res.Add("Message", "Success");
                    _res.Add("Data", listCustomer.ToList());
                }
                else
                {
                    _res.Add("Status", 1);
                    _res.Add("Message", "No Record Found");

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



        [HttpGet]
        [Route("GetAllMasterList")]
        public async Task<HttpResponseMessage> GetAllMasterList()
        {
            Dictionary<string, Object> _res = new Dictionary<string, object>();
            try
            {
                CustomerBL objBl = new CustomerBL();
                //var listCustomer = await objBl.GetCustomerAreawise(AreaId);

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
        public HttpResponseMessage PostCustEmail(List<CustomerEmail> CustomerEmail)
        {
            CustomerLoginBL ObjchL = new CustomerLoginBL();
            Dictionary<string, Object> _res = new Dictionary<string, object>();
            try
            {
                var res = "";
                try
                {
                    foreach (var item in CustomerEmail)
                    {
                        if (item.Email != "")
                        {
                            foreach (var email in item.Email.Split(','))
                            {
                                string Password = ObjchL.GetPasswordID(item.CustomerId);
                                if (Password == "")
                                {
                                    Password = ObjchL.GenratePass();
                                }
                                else
                                {
                                    Password = ObjchL.GetDecrypted(Password);
                                }
                                string Msgbody = Resource1.CustomerEmail.Replace("@Fname", item.Name).Replace("@CustomerNumber", item.CustomerNumber).Replace("@password", Password);
                                string CustomerSms = Resource1.CustomerSms.Replace("@Fname", item.Name).Replace("@CustomerNumber", item.CustomerNumber).Replace("@password", Password).Replace("@n", Environment.NewLine);
                                res = ObjchL.SendEMailSms(item.CustomerId.ToString(), item.Name, item.MobileNumber, item.Email, Msgbody, CustomerSms, "API", Password);
                                if (res != "Save")
                                {
                                    _res.Add("Message", res);
                                    _res.Add("Status", 0);
                                    return Request.CreateResponse(HttpStatusCode.OK, _res);
                                }

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var t = ex;
                    _res.Add("Status", 0);
                    _res.Add("Message", t);
                }
                _res.Add("Message", res);
                _res.Add("Status", 1);
                return Request.CreateResponse(HttpStatusCode.OK, _res);
            }
            catch (Exception ex)
            {
                _res.Add("Status", 0);
                _res.Add("Message", ex.Message);
                return Request.CreateResponse(HttpStatusCode.BadRequest, _res);
            }
        }


        public string FileNameSplit(string fileName, string Name)
        {
            try
            {
                if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                {
                    fileName = fileName.Trim('"');
                }
                if (Name.StartsWith("\"") && Name.EndsWith("\""))
                {
                    Name = Name.Trim('"');
                }
                if (fileName.Contains(@"/") || fileName.Contains(@"\"))
                {
                    fileName = Path.GetFileName(fileName);
                }
                return fileName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("FileUpload")]
        [HttpPost]
        public async Task<HttpResponseMessage> FileUpload()
        {
            Dictionary<string, Object> _res = new Dictionary<string, object>();
            try
            {
                string root = HttpContext.Current.Server.MapPath("~/DataFiles");
            

                var provider = new MultipartFormDataStreamProvider(root);
                if (!System.IO.Directory.Exists(root))
                {
                    System.IO.Directory.CreateDirectory(root);
                }
                if (!Request.Content.IsMimeMultipartContent())
                {
                    _res.Add("Status", 0);
                    _res.Add("ProfileUrl", String.Empty);

                    _res.Add("Message", "FileNotFound");
                    return Request.CreateResponse(HttpStatusCode.OK, _res);
                }
                await Request.Content.ReadAsMultipartAsync(provider);

                //upload file  to server directory  in multipart
                string fileName = String.Empty;
                string msg = provider.FormData["Exception"];
                string UserId = provider.FormData["UserId"];
                string FileName = "";
                string BillFileName = ""; string PaymentFileName = "";
                foreach (var file in provider.FileData)
                {
                    fileName = file.Headers.ContentDisposition.FileName;
                    string Name = file.Headers.ContentDisposition.Name;
                    fileName = FileNameSplit(fileName, Name);
                    FileInfo fi = new FileInfo(fileName);
                    StringBuilder builder = new StringBuilder();
                    Guid obj = Guid.NewGuid();
                    if (fileName == "BillFileName.csv")
                    {
                        fileName = obj.ToString() + fi.Extension;
                        BillFileName = fileName ;
                    }
                    else if (fileName == "PaymentFileName.csv")
                    {
                        fileName = obj.ToString() + fi.Extension;
                        PaymentFileName = fileName;
                    }
                    builder.AppendFormat("{0}\\{1}", new string[] { root, fileName });
                    File.Move(file.LocalFileName, builder.ToString());
                    FileName += "," + fileName;
                }

                dynamic sendemail;
                
                CustomerBL objBl = new CustomerBL();
                objBl.InsertEmployeeFile(UserId, msg, BillFileName,PaymentFileName,FileName);
                //pravinsingh1@gmail.com,narsing.m@synsoftglobal.com
                 Parallel.Invoke(() => sendemail = Email.SendMailtoMultipleUser("Jagratisyn@gmail.com,narsing.m@synsoftglobal.com", "Exception In Syn Data", msg));
                _res.Add("Status", 1);
                _res.Add("Message", "File uploaded successfully.");
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
