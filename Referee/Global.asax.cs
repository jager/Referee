using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Data.Entity;
using Referee.DAL;
using Referee.Filters;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Referee
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new RefereeHandleError());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            routes.MapRoute(
                "Availability", // Route name
                "avail/create/{RefereeId}", // URL with parameters
                new { controller = "Avail", action = "Create", Token = UrlParameter.Optional } // Parameter defaults
            );
            routes.MapRoute(
                "RestorePassword", // Route name
                "Account/RestorePassword/{Token}", // URL with parameters
                new { controller = "Account", action = "RestorePassword", Token = UrlParameter.Optional } // Parameter defaults
            );
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            Database.SetInitializer<RefereeContext>(new RefInitializer());
            AreaRegistration.RegisterAllAreas();        

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
    }
}