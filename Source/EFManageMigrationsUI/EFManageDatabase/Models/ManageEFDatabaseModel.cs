using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManageDatabase.EFManageDatabase.Models
{
    public class ManageEFDatabaseModel
    {
        #region Constructors
        public ManageEFDatabaseModel()
        {

        }
        #endregion

        #region <Properties>
        public bool IsAuthorized { get; set; }

        public List<KeyValuePair<string, string>> PendingMigrations { get; set; }

        #endregion

        #region Public Methods

        public void Load()
        {
            LoadMigrationDetails();
        }

        public void Update(string version)
        {
            if (!string.IsNullOrWhiteSpace(version))
            {
                GetMigrator().Update(version);
            }
        }
        #endregion

        #region Private Methods
        private void LoadMigrationDetails()
        {
            var migrator = GetMigrator();
            var selectItems = migrator.GetPendingMigrations().Select(v => new SelectListItem
            {
                Text = GetEFVersionDisplayText(v),
                Value = v
            });

            var currentVersion = migrator.GetDatabaseMigrations().FirstOrDefault() ?? "N/A";
            PendingMigrations = new SelectList(selectItems, "Value", "Text", selectItems.LastOrDefault());
            Entity = new MigrationDetails
            {
                CurrentVersion = currentVersion,
                CurrentVersionDisplayValue = GetEFVersionDisplayText(currentVersion),
                TargetVersion = selectItems.Select(si => si.Value).LastOrDefault(),
            };

        }

        private string GetEFVersionDisplayText(string version)
        {
            return version.ToLower().Contains('_') ? version.Replace(version.Split('_')[0] + '_', string.Empty).ToLowerInvariant() : version;
        }

        private void Init()
        {
            IsAuthorized = IsCurrentUserAuthorizedForManage();
        }

        #endregion
    }
}