using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ManageDatabase
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            EFMigrationsManagerConfig.Register();
        }

        public void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();

            if (EFMigrationsManagerConfig.HandleEFMigrationException(exception, Server, Response, Context))
                return;
        }
    }
}
