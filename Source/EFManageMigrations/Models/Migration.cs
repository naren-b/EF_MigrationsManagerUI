namespace NB.Apps.EFMigrationsManager.Models
{
    using System.Linq;

    public class Migration
    {
        #region Constructors
        public Migration(string name)
        {
            Name = name;
        }
        #endregion

        #region Properties
        public string Name { get; set; }

        public string DisplayName
        {
            get
            {
                return GetDisplayName(Name);
            }
        }
        #endregion

        #region Static Methods

        public static string GetDisplayName(string migration)
        {
            if (string.IsNullOrWhiteSpace(migration))
            {
                return string.Empty;
            }

            return migration.ToLower().Contains('_') ? migration.Replace(migration.Split('_')[0] + '_', string.Empty).ToLowerInvariant() : migration;
        }
        #endregion
    }
}
