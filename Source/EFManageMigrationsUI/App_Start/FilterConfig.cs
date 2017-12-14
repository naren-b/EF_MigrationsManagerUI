using System.Web;
using System.Web.Mvc;

namespace ManageDatabase
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new NB.Apps.EFMigrationsManager.CustomAttributes.VerifyIsEFMigrationUpToDateAttribute());
        }
    }
}
