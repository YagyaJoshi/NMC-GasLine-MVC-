using NMCDataAccesslayer.BL;
using NMCDataAccesslayer.DataModel;
using NMCPipedGasLineAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IdentitySample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> MasterData(string type, string value, string pagename)
        {
            UserDataModel modleDATA = new UserDataModel();
            UserBL objBL = new UserBL();
            AreaBL objarea = new AreaBL();
            CompanyBL objcompany = new CompanyBL();
            GodownBL objGodown = new GodownBL();
            StockItemBL objstock = new StockItemBL();
            List<CountryDataModel> country = objBL.GetCountry();
            User model = new User();

            switch (type)
            {
                case "CountryId":
                    modleDATA.State = objBL.GetState(value, Session["City"].ToString());
                    break;
                case "StateId":
                    modleDATA.City = objBL.GetCity(value,Session["City"].ToString());
                    break;
                case "CityId":
                    modleDATA.Company = objcompany.GetCompany(Session["RoleName"].ToString(), value);
                    break;
                case "CompanyId":
                    modleDATA.Area = objarea.GetArea(value);
                    if (pagename == "customer")
                    {
                        modleDATA.StockItemList = objstock.GetStockItemByCompany(value);
                    }
                    break;
                case "AreaId":
                    modleDATA.GoDown = await objGodown.GetGoDownAreawise(value);
                    break;
            }
            return Json(modleDATA);
        }

        public ActionResult EmployeeList()
        {
            User objuser = new NMCPipedGasLineAPI.Models.User();
            return View(objuser);
        }

    }
}
