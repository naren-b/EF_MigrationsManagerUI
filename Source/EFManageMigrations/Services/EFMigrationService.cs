
namespace NB.Apps.EFMigrationsManager.Services
{
    using EFMigrationsManager.Models;
    using EFMigrationsManager.Settings;
    using System;
    using System.Configuration;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Collections.Generic;
    using System.Security.Principal;

    public class EFMigrationService
    {
        #region Constants
        public const string Const_AuthorizedUsers_Key = "EFMigrationsManager:AuthorizedUsers";
        #endregion

        #region <Constructors>
        public EFMigrationService()
        {
        }
        #endregion

        #region <Properties>
        #endregion

        #region Public Methods
        public virtual EFMigrationDetails LoadMigrationDetails(bool isRollback)
        {
            EFMigrationDetails result = new EFMigrationDetails(isRollback);
            var migrator = CreateDBMigrator();
            result.CurrentMigration = migrator.GetDatabaseMigrations().FirstOrDefault() ?? "N/A";
            if (isRollback)
            {
                PrepareDatabaseMigrations(migrator, result);
            }
            else
            {
                PreparePendingMigrations(migrator, result);
            }

            return result;
        }

        private void PreparePendingMigrations(DbMigrator migrator, EFMigrationDetails migrationDetails)
        {
            var migrations = migrator.GetPendingMigrations().ToList();
            var pendingMigrations = migrations.Select(v => new SelectListItem
            {
                Text = EFMigrationDetails.GetDisplayName(v, string.Compare(migrations.LastOrDefault(), v, true) == 0),
                Value = v
            });

            migrationDetails.Migrations = new SelectList(pendingMigrations, "Value", "Text", pendingMigrations.LastOrDefault());
            migrationDetails.TargetMigration = pendingMigrations.Select(si => si.Value).LastOrDefault();
        }

        private void PrepareDatabaseMigrations(DbMigrator migrator, EFMigrationDetails migrationDetails)
        {
            var databaseMigrations = migrator.GetDatabaseMigrations().Select(v => new SelectListItem
            {
                Text = EFMigrationDetails.GetDisplayName(v),
                Value = v
            }).ToList();

            SelectListItem disabledItem = new SelectListItem
            {
                Text = "--------------------------------------",
                Value = "",
                Disabled = true,
            };
            if (databaseMigrations.Any())
            {
                databaseMigrations = databaseMigrations.Skip(1).ToList();
                if (databaseMigrations.Any())
                {
                    databaseMigrations.Add(disabledItem);
                }
                databaseMigrations.Add(new SelectListItem
                {
                    Text = "<<Remove All Migrations>>",
                    Value = "0"
                });
            }


            migrationDetails.Migrations = new SelectList(databaseMigrations, "Value", "Text", databaseMigrations.FirstOrDefault(m => !m.Disabled), new List<string> { disabledItem.Value });
            migrationDetails.TargetMigration = databaseMigrations.Where(m => !m.Disabled).Select(si => si.Value).FirstOrDefault();
        }

        public virtual void Update(string targetMigration)
        {
            if(!IsAuthorizedUser())
            {
                throw new UnauthorizedAccessException();
            }

            var migrator = CreateDBMigrator();
            var currentMigration = migrator.GetDatabaseMigrations().FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(targetMigration) && string.Compare(currentMigration, targetMigration, true) != 0)
            {
                migrator.Update(targetMigration);
            }
        }

        public virtual bool IsLatestVersion()
        {
            return !CreateDBMigrator().GetPendingMigrations().Any();
        }

        public virtual bool IsAuthorizedUser()
        {
            string csvUserNames = ConfigurationManager.AppSettings[Const_AuthorizedUsers_Key];

            return !string.IsNullOrWhiteSpace(csvUserNames) && csvUserNames.Split(new[] { ',' })
                .Any(un => string.Compare(un.Trim(), GetLoggedInUserName(), true) == 0);
        }

        public virtual string GetLoggedInUserName()
        {
            if(HttpContext.Current?.User is WindowsPrincipal) //Windows authentication
                return HttpContext.Current?.Request?.LogonUserIdentity?.Name;
            else //Forms authentication
                return HttpContext.Current?.User?.Identity?.Name;
        }
        #endregion

        #region Private Methods

        private DbMigrator CreateDBMigrator()
        {
            return EFMigrationsManagerSettings.CreateDBMigrator();
        }

        #endregion
    }
}
