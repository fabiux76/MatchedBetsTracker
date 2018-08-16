using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MatchedBetsTracker
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DEfaultApiWithAction",
                routeTemplate: "api/{controller}/{action}",
                defaults: new
                {
                    action = "index"
                }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
