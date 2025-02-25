using System.Globalization;
using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using NMCDataAccesslayer.BL;
using NMCDataAccesslayer.DataModel;
using AutoMapper;
using NMCPipedGasLineAPI.Models;
using System.Collections.Generic;
using System.Data;
using NMCPipedGasLineAPI.Properties;
using RestSharp;
using System.Net;

namespace IdentitySample.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        AreaBL ObjAreaBL = new AreaBL();
        CompanyBL ObjBL = new CompanyBL();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            //var client = new RestClient();
            //var request = new RestRequest();

            //IRestResponse response;
            //string pl = "http://www.alots.in/sms-panel/api/http/index.php?username=pravin123&apikey=FB53D-D6D2B&apirequest=Text&sender=TSTMSG&mobile=9479630784&message=SMSMessage&route=TRANS&format=JSON";
            ////string pl = "http://www.global91sms.in/app/smsapi/index.php?key=45DF9D277EB80B&routeid=459&type=text&contacts="+ contacts + "&senderid=NMCGAS&msg="+ WebUtility.UrlEncode(msg) + "";
            //client = new RestClient(pl);
            //ServicePointManager.SecurityProtocol = (SecurityProtocolType)48 | (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
            //request = new RestRequest(Method.POST);
            //request.AddHeader("content-type", "application/json");
            //response = client.Execute(request);


            //if(Session["user"] !=null)
            //{ 
            Session.Clear();
            Session.Abandon();
            ViewBag.ReturnUrl = returnUrl;
            return View();
            //}else
            //{ return RedirectToAction("Login", "Account"); }
        }

        private SignInHelper _helper;

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

        //
        // POST: /Account/Login
        [HttpPost]

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            UserBL objBL = new UserBL();
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doen't count login failures towards lockout only two factor authentication
            // To enable password failures to trigger lockout, change to shouldLockout: true
            var result = await SignInHelper.PasswordSignIn(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case IdentitySample.Models.SignInStatus.Success:

                    var details = await UserManager.FindAsync(model.Email, model.Password);

                    if (details != null)
                    {
                        //var role = await UserManager.GetRolesAsync(details.Id);
                        UserDataModel u = await objBL.GetUserDetail(details.Id);
                        Session["user"] = u.Id;
                        if (u.RoleName != "Super Admin")
                        {
                            Session["City"] = u.CityId;
                        }
                        else
                        { Session["City"] = ""; }
                        Session["RoleName"] = u.RoleName;//  u.RoleName;
                        if (u.RoleName == "Super Admin" || u.RoleName == "Admin")
                        {
                            return RedirectToAction("List", "Company");
                        }
                        else
                        {
                            ModelState.AddModelError("", "invalid username or password");
                            return View(model);
                        }

                    }
                    else
                    {
                        ModelState.AddModelError("", "invalid username or password");
                        return View(model);

                    }
                case IdentitySample.Models.SignInStatus.LockedOut:
                    return View("Lockout");
                case IdentitySample.Models.SignInStatus.RequiresTwoFactorAuthentication:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
                case IdentitySample.Models.SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "invalid username or password");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInHelper.HasBeenVerified())
            {
                return View("Error");
            }
            var user = await UserManager.FindByIdAsync(await SignInHelper.GetVerifiedUserIdAsync());
            if (user != null)
            {
                // To exercise the flow without actually sending codes, uncomment the following line
                ViewBag.Status = "For DEMO purposes the current " + provider + " code is: " + await UserManager.GenerateTwoFactorTokenAsync(user.Id, provider);
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await SignInHelper.TwoFactorSignIn(model.Provider, model.Code, isPersistent: false, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case IdentitySample.Models.SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case IdentitySample.Models.SignInStatus.LockedOut:
                    return View("Lockout");
                case IdentitySample.Models.SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                AutoMapper.Mapper.Reset();
                User objuser = new User();
                UserDataModel modleDATA = new UserDataModel();
                UserBL objBL = new UserBL();
                modleDATA.Country = objBL.GetCountry();
                modleDATA.Role = objBL.GetRole();
          
                modleDATA.EmpCode = objBL.GetMaxEmpCode().EmpCode+1;
              
                if (Session["RoleName"].ToString() != "Super Admin")
                {
                    modleDATA.CountryId = modleDATA.Country.ToList().FirstOrDefault().CountryId.ToString();
                    modleDATA.State = objBL.GetState(modleDATA.CountryId, Session["City"].ToString());
                    modleDATA.StateId = modleDATA.State.ToList().FirstOrDefault().StateId.ToString();
                    modleDATA.City = objBL.GetCity(modleDATA.StateId, Session["City"].ToString());
                    modleDATA.CityId = modleDATA.City.ToList().FirstOrDefault().CityId.ToString();
                    modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), Session["City"].ToString());
                    modleDATA.CompanyId = modleDATA.Company.ToList().FirstOrDefault().Id.ToString();
                    modleDATA.Area = ObjAreaBL.GetArea(modleDATA.CompanyId);
                }


                Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<UserDataModel, User>();
            });
                objuser = Mapper.Map<UserDataModel, User>(modleDATA);
                if (Session["RoleName"].ToString() != "Super Admin")
                {
                    objuser.RoleId = "Employee";
                    objuser.oldRoleName = "Employee";
                    //objuser.CountryId = modleDATA.Country.ToList().FirstOrDefault().CountryId.ToString();
                    //modleDATA.State = objBL.GetState(objuser.CountryId, Session["City"].ToString());
                    //objuser.StateId= modleDATA.State.ToList().FirstOrDefault().StateId.ToString();
                    //modleDATA.City = objBL.GetCity(objuser.StateId, Session["City"].ToString());
                    //modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(),  Session["City"].ToString());
                    //objuser.CompanyId = modleDATA.Company.ToList().FirstOrDefault().Id.ToString();
                    //modleDATA.Area = ObjAreaBL.GetArea(objDM.CompanyId);
                }
                AutoMapper.Mapper.Reset();


                return View(objuser);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Register(User model)
        {
            if (Session["user"] != null && Session["RoleName"] != null && Session["City"] != null)
            {
                try
                {

                    if (ModelState.IsValid)
                    {
                        UserBL objBL = new UserBL();
                        UserDataModel objDM = new UserDataModel();
                        User objUser = new User();
                        AutoMapper.Mapper.Reset();
                        if (model.Id == null)
                        {
                            //var h = UserManager.FindByEmail("jagrati@gmail.com");
                            //var emaildata = UserManager.FindByEmail("rishu2y@gmail.com");
                            //objDM = objBL.GetUser(emaildata.Id);
                            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                            var result = await UserManager.CreateAsync(user, model.Password);


                            if (result.Succeeded)
                            {
                                var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                                //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                                //await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                                //ViewBag.Link = callbackUrl;



                                var res = await UserManager.AddToRoleAsync(user.Id, model.RoleId);
                                objUser.Id = model.Id;
                                objUser.Name = model.Name;
                                objUser.Pincode = model.Pincode;
                                objUser.Phone = model.Phone;
                                objUser.Address = model.Address;
                                objUser.UserId = user.Id;
                                objUser.CountryId = model.CountryId;
                                objUser.StateId = model.StateId;
                                objUser.CityId = model.CityId;
                                objUser.AreaId = model.AreaId;
                                objUser.Pincode = model.Pincode;
                                objUser.CompanyId = model.CompanyId;
                                objUser.EmpCode = model.EmpCode;

                                Mapper.Initialize(cfg =>
                                {
                                    cfg.CreateMap<UserDataModel, User>();
                                });

                                objDM = Mapper.Map<User, UserDataModel>(objUser);
                                objBL.SaveUser(objDM);
                                TempData["Msg"] = Resource1.SaveSuccess;
                                //return RedirectToAction("LogIn", "Account", new { area = "" });
                                return RedirectToAction("List", "Employee");
                                //return View("DisplayEmail");
                            }
                            else
                            {

                            }
                            AddErrors(result);

                            UserDataModel modleDATA = new UserDataModel();
                            List<CountryDataModel> country = objBL.GetCountry();
                            modleDATA.Country = objBL.GetCountry();
                            modleDATA.State = objBL.GetState(model.CountryId, Session["City"].ToString());
                            modleDATA.City = objBL.GetCity(model.StateId, Session["City"].ToString());
                            modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), model.CityId);
                            modleDATA.Area = ObjAreaBL.GetArea(model.CompanyId);
                            List<CountryMaster> CountryMaster = new List<NMCPipedGasLineAPI.Models.CountryMaster>();
                            modleDATA.Role = objBL.GetRole();
                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<UserDataModel, User>();
                            });
                            model = Mapper.Map<UserDataModel, User>(modleDATA);

                            string error1 = "";
                            foreach (var error in result.Errors)
                            {
                                error1 = error;
                                break;
                            }
                            if (error1.Contains("is already taken"))
                            { ViewBag.Email = "Email id is already registered"; }
                            else if (error1.Contains("Please Enter Role"))
                            {
                                ViewBag.Error = "";
                            }
                            else { ViewBag.Error = error1; }


                        }
                        else
                        {

                            if (model.oldRoleName != model.RoleId)
                            {
                                if (model.oldRoleName != null)
                                {
                                    var f = await UserManager.RemoveFromRoleAsync(model.UserId, model.oldRoleName);
                                    var res = await UserManager.AddToRoleAsync(model.UserId, model.RoleId);
                                }
                            }


                            objUser.Id = model.Id;
                            objUser.Name = model.Name;
                            objUser.Pincode = model.Pincode;
                            objUser.Phone = model.Phone;
                            objUser.Address = model.Address;
                            objUser.UserId = model.UserId;
                            objUser.CountryId = model.CountryId;
                            objUser.StateId = model.StateId;
                            objUser.CityId = model.CityId;
                            objUser.AreaId = model.AreaId;
                            objUser.Pincode = model.Pincode;
                            objUser.CompanyId = model.CompanyId;
                            objUser.EmpCode = model.EmpCode;
                            var passresult = true;
                            if (model.UpdatePassword != null)
                            {
                                objUser.Password = model.UpdatePassword;
                                var code = await UserManager.GeneratePasswordResetTokenAsync(model.UserId);
                                var result = await UserManager.ResetPasswordAsync(model.UserId, code, model.UpdatePassword);
                                if (result.Succeeded)
                                {
                                    passresult = true;
                                }
                                else
                                { passresult = false; AddErrors(result); }
                            }

                            //objUser.Password = model.UpdatePassword;
                            //var code = await UserManager.GeneratePasswordResetTokenAsync(model.UserId);
                            //var result = await UserManager.ResetPasswordAsync(model.UserId, code, model.Password);
                            if (passresult == true)
                            {
                            }
                            else
                            {


                                UserDataModel modleDATA = new UserDataModel();
                                List<CountryDataModel> country = objBL.GetCountry();
                                modleDATA.Country = objBL.GetCountry();
                                List<CountryMaster> CountryMaster = new List<NMCPipedGasLineAPI.Models.CountryMaster>();

                                Mapper.Initialize(cfg =>
                                {
                                    cfg.CreateMap<UserDataModel, User>();
                                });
                                modleDATA.Country = objBL.GetCountry();
                                modleDATA.State = objBL.GetState(model.CountryId, Session["City"].ToString());
                                modleDATA.City = objBL.GetCity(model.StateId, Session["City"].ToString());
                                modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), model.CityId);
                                modleDATA.Area = ObjAreaBL.GetArea(model.CompanyId);
                                modleDATA.Role = objBL.GetRole();
                                model = Mapper.Map<UserDataModel, User>(modleDATA);
                                AutoMapper.Mapper.Reset();
                                return View(model);
                            }

                            Mapper.Initialize(cfg =>
                           {
                               cfg.CreateMap<UserDataModel, User>();
                           });

                            objDM = Mapper.Map<User, UserDataModel>(objUser);
                            AutoMapper.Mapper.Reset();
                            objBL.UpdateUser(objDM);
                            TempData["Msg"] = Resource1.UpdateSuccess;
                            return RedirectToAction("List", "Employee");
                        }

                    }
                    else
                    {

                        UserDataModel modleDATA = new UserDataModel();
                        UserBL objBL = new UserBL();
                        List<CountryDataModel> country = objBL.GetCountry();
                        modleDATA.Country = objBL.GetCountry();
                        modleDATA.State = objBL.GetState(string.IsNullOrEmpty(model.CountryId) ? "" : model.CountryId, Session["City"].ToString());
                        modleDATA.City = objBL.GetCity(string.IsNullOrEmpty(model.StateId) ? "" : model.StateId, Session["City"].ToString());
                        modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), string.IsNullOrEmpty(model.CityId) ? "" : model.CityId);
                        modleDATA.Area = ObjAreaBL.GetArea(string.IsNullOrEmpty(model.CompanyId) ? "" : model.CompanyId);
                        modleDATA.Role = objBL.GetRole();
                        List<CountryMaster> CountryMaster = new List<NMCPipedGasLineAPI.Models.CountryMaster>();
                        AutoMapper.Mapper.Reset();

                        Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<UserDataModel, User>();
                            });


                        model = Mapper.Map<UserDataModel, User>(modleDATA);
                        AutoMapper.Mapper.Reset();
                        string messages = string.Join(Environment.NewLine, ModelState.Values
                                        .SelectMany(x => x.Errors));
                        string error1 = "";
                        foreach (var error in ModelState.Values.ToList())
                        {
                            foreach (var error2 in error.Errors)
                            {
                                error1 = error2.ErrorMessage;
                                break;
                            }
                        }

                        if (error1.Contains("is already taken"))
                        { ViewBag.Email = "Email id is already registered with us"; }
                        else if (error1.Contains("Please Enter Role"))
                        {
                            ViewBag.Error = "";
                        }
                        else { ViewBag.Error = error1; }
                        //ViewBag.Error = messages;
                    }
                    // If we got this far, something failed, redisplay form


                }
                catch (Exception ex)
                {
                    UserDataModel modleDATA = new UserDataModel();
                    UserBL objBL = new UserBL();
                    List<CountryDataModel> country = objBL.GetCountry();
                    modleDATA.Country = objBL.GetCountry();
                    modleDATA.State = objBL.GetState(string.IsNullOrEmpty(model.CountryId) ? "" : model.CountryId, Session["City"].ToString());
                    modleDATA.City = objBL.GetCity(string.IsNullOrEmpty(model.StateId) ? "" : model.StateId, Session["City"].ToString());
                    modleDATA.Company = ObjBL.GetCompany(Session["RoleName"].ToString(), string.IsNullOrEmpty(model.CityId) ? "" : model.CityId);
                    modleDATA.Area = ObjAreaBL.GetArea(string.IsNullOrEmpty(model.CompanyId) ? "" : model.CompanyId);
                    List<CountryMaster> CountryMaster = new List<NMCPipedGasLineAPI.Models.CountryMaster>();
                    modleDATA.Role = objBL.GetRole();
                    AutoMapper.Mapper.Reset();
                    Mapper.Initialize(cfg =>
                       {
                           cfg.CreateMap<UserDataModel, User>();
                       });

                    model = Mapper.Map<UserDataModel, User>(modleDATA);
                    ViewBag.Error = ex.Message;

                }
                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            if (result.Succeeded)
            {
                return View("ConfirmEmail");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                string body = Resource1.ForgotEmail.Replace("@Fname", model.Email).Replace("@callbackUrl", callbackUrl).Replace("@NMCEmail", model.Email);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
                ViewBag.Link = callbackUrl;
                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                ViewBag.Error = Resource1.ResetPassword;
                // Don't reveal that the user does not exist
                //return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            else
            {
                var code = model.Code.Replace(" ", "+");
                var result = await UserManager.ResetPasswordAsync(user.Id, code, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("ResetPasswordConfirmation", "Account");
                }
                else
                {
                    string error1 = "";
                    foreach (var error in result.Errors.ToList())
                    {
                        error1 = error.ToString();

                    }

                    ViewBag.Error = error1;
                }
                AddErrors(result);
            }
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            //Hello;
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl)
        {
            var userId = await SignInHelper.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            // Generate the token and send it
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (!await SignInHelper.SendTwoFactorCode(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInHelper.ExternalSignIn(loginInfo, isPersistent: false);
            switch (result)
            {
                case IdentitySample.Models.SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case IdentitySample.Models.SignInStatus.LockedOut:
                    return View("Lockout");
                case IdentitySample.Models.SignInStatus.RequiresTwoFactorAuthentication:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
                case IdentitySample.Models.SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInHelper.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpGet]
        public ActionResult LogOff()
        {
            Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

        #region "Employee"
        [HttpPost]
        [AllowAnonymous]
        public ActionResult get()
        {
            return RedirectToAction("Index", "Home");
        }


        /// <summary>
        /// Create For Login In system
        /// </summary>
        /// <param name="objLogin">UserName(Email)</param>
        /// <param name="objLogin">Password</param>
        /// <returns>Employee Details</returns>


        #endregion
    }
}