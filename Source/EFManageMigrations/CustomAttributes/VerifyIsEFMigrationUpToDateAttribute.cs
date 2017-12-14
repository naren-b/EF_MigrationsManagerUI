namespace NB.Apps.EFMigrationsManager.CustomAttributes
{
    using EFMigrationsManager.Services;
    using System.Web.Mvc;
    using System.Web.Routing;

    public class VerifyIsEFMigrationUpToDateAttribute : ActionFilterAttribute
    {
        #region <Constructors>
        public VerifyIsEFMigrationUpToDateAttribute()
        {
            _service = new EFMigrationService();
            SkipVerification = false;
        }

        public VerifyIsEFMigrationUpToDateAttribute(bool skipVerification)
        {
            SkipVerification = skipVerification;
        }
        #endregion

        #region Members
        private EFMigrationService _service { get; set; }

        #endregion

        #region <Properties>
        public bool SkipVerification { get; set; }
      
        #endregion

        #region Override Methods
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (SkipVerification || _service.IsLatestVersion())
            {
                return;
            }

            if (_service.IsAuthorizedUser())
            {
                RedirectToPublishDbPage(filterContext);
                return;
            }

            RedirectToMaintenanceModePage(filterContext);
        }

        #endregion

        #region Virtual Methods

        protected virtual void RedirectToPublishDbPage(ActionExecutingContext filterContext)
        {
            var redirectTargetDictionary = new RouteValueDictionary
                                                   {
                                                       {"action", "Publish"},
                                                       {"controller", "EFMigrationsManager"},
                                                       {"redirectUrl", filterContext.HttpContext.Request.Url},
                                                   };

            filterContext.Result = new RedirectToRouteResult(redirectTargetDictionary);
        }

        protected virtual void RedirectToMaintenanceModePage(ActionExecutingContext filterContext)
        {
            var redirectTargetDictionary = new RouteValueDictionary
                                                   {
                                                       {"action", "DbMaintenance"},
                                                       {"controller", "EFMigrationsManager"},
                                                       {"redirectUrl", filterContext.HttpContext.Request.Url},
                                                   };

            filterContext.Result = new RedirectToRouteResult(redirectTargetDictionary);
        }

        #endregion
    }
}