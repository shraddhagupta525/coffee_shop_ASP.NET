using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace coffee_shop
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            // Define your custom routes here
            routes.MapRoute(
                name: "Login",
                url: "login",
                defaults: new { controller = "Account", action = "Login" } // Assuming "Account" is your controller name and "Login" is your action method
            );
            routes.MapRoute(
                name: "Admin",
                url: "admin",
                defaults: new { controller = "Account", action = "Admin" } // Assuming "Account" is your controller name and "Admin" is your action method
            );

            // default route
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}