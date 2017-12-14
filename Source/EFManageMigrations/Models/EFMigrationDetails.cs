namespace NB.Apps.EFMigrationsManager.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using System.Linq;

    public class EFMigrationDetails
    {
        #region Constructors
        public EFMigrationDetails(bool isRollback)
        {
            IsRollback = isRollback;
        }

        public EFMigrationDetails()
        {
            IsRollback = false;
        }
        #endregion

        #region Properties
        public bool IsRollback { get; set; }

        public SelectList Migrations { get; set; }

        public string CurrentMigration { get; set; }

        [Display(Name = "Current Migration")]
        public string CurrentMigrationDisplayValue
        {
            get
            {
                return GetDisplayName(CurrentMigration);
            }
        }

        [Display(Name = "Target Migration")]
        public string TargetMigration { get; set; }

        [Display(Name = "Rollback to Previous Migration")]
        public string PreviousMigration { get; set; }

        public bool HasTargetMigrationExists
        {
            get
            {
                return Migrations != null && Migrations.Items != null && Migrations.Any();
            }
        }

        public bool IsLatestMigration
        {
            get
            {
                return !IsRollback && !string.IsNullOrWhiteSpace(CurrentMigration) 
                    && string.IsNullOrWhiteSpace(TargetMigration);
            }
        }

        #endregion

        #region Methods
        public static string GetDisplayName(string migration, bool isLatestVersion = false)
        {
            if (string.IsNullOrWhiteSpace(migration))
            {
                return string.Empty;
            }

            var result =  migration.ToLower().Contains("_") ? migration.Replace(migration.Split('_')[0] + '_', string.Empty).ToLowerInvariant() : migration;
            if (isLatestVersion)
            {
                result = string.Format("{0} (Latest Migration)", result);
            }
            return result;
        }
        #endregion
    }
}
