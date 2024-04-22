using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using coffee_shop.Models;

namespace coffee_shop.Controllers
{
    [RoutePrefix("api/dashboard")]
    public class DashboardController : ApiController
    {
        Coffee_shopEntities db = new Coffee_shopEntities();

        [HttpGet, Route("details")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetDetails()
        {
            try
            {
                var data = new
                {
                    category = db.Categories.Count(),
                    product = db.Products.Count(),
                    bill = db.Bills.Count(),
                    user = db.Users.Count()
                };
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }catch(Exception e) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}
