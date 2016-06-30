using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Kimserey.Rating.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "AppRouteFull",
                url: "app/{controller}/{action}/{id}",
                defaults: new
                {
                    id = UrlParameter.Optional
                });

            routes.MapRoute(
                name: "AppRouteController",
                url: "app/{controller}",
                defaults: new 
                { 
                    action = "Index"
                });

            routes.MapRoute(
                name: "Default",
                url: "{*url}",
                defaults: new { controller = "Home", action = "Index" });
        }
    }
}
