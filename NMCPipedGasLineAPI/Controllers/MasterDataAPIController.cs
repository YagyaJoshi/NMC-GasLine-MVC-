using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
using System.Data;
using NMCDataAccesslayer.DAL;
using System.Web.Http.Results;
using System.Web.Mvc;
using NMCDataAccesslayer.BL;
using NMCPipedGasLineAPI.Models;
using NMCDataAccesslayer.DataModel;

namespace NMCPipedGasLineAPI.Controllers
{
    public class MasterDataAPIController : ApiController
    {
        //[HttpGet]
        //public HttpResponseMessage GetAllRole()
        //{
        //    Dictionary<string, Object> _res = new Dictionary<string, object>();
        //    try
        //    {
        //        //DataTable dtRoles = objSQL.FetchDT(objCmd);
        //        //var lstRoles = (from DataRow dr in dtRoles.Rows
        //        //                select new
        //        //                {
        //        //                    RoleId = dr["RoleId"] != DBNull.Value ? Convert.ToInt64(dr["RoleId"]) : 0,
        //        //                    RoleName = dr["RoleName"],
        //        //                    IsSubRole = dr["IsSubRole"],
        //        //                }).ToList();

        //        //if (lstRoles.Count() > 0)
        //        //{
        //        //    _res.Add("Status", 1);
        //        //    _res.Add("Data", lstRoles);
        //        //    return Request.CreateResponse(HttpStatusCode.OK, _res);
        //        //}
        //        //else
        //        //{
        //        //    _res.Add("Status", 0);
        //        //    return Request.CreateResponse(HttpStatusCode.OK, _res);
        //        //}
        //    }
        //    catch (Exception ex)
        //    {

        //        _res.Add("Status", 0);
        //        _res.Add("Message", ex.Message);
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, _res);
        //    }
        //    finally
        //    {

        //    }
        //}
    }
}
