namespace NB.Apps.EFMigrationsManager.Settings
{
    using System;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Migrations;
    using System.Text;

    public static class EFMigrationsManagerSettings
    {
        #region <Properties>
        internal static string ConnectionName { get; set; }

        internal static string ConnectionString { get; set; }

        internal static string ProviderInvariantName { get; set; }

        internal static DbMigrationsConfiguration Configuration { get; set; }

        #endregion

        #region Internal Methods
        internal static DbMigrator CreateDBMigrator()
        {
            Validate();
            var connectionInfo = GetDbConnectionInfo();
            if(connectionInfo != null)
            {
                Configuration.TargetDatabase = connectionInfo;
            }
            
            return new DbMigrator(Configuration);
        }
        #endregion

        #region Private Methods
        private static void Validate()
        {
            if(Configuration == null)
            {
                throw new ArgumentException(@"Register method from EFMigrationsManagerConfig class: Call the EFMigrationsManagerSettings.SetEFConfiguration method by passing the EFConfiguration instance as parameter.");
            }
        }
       
        #endregion

        #region Public Methods

        public static void SetConnectionString(string connectionName) 
        {
            ConnectionName = connectionName;
        }

        public static void SetEFConfiguration(DbMigrationsConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static void SetConnectionString(string connectionString, string providerInvariantName)
        {
            ConnectionString = connectionString;
            ProviderInvariantName = providerInvariantName;
        }

        public static DbConnectionInfo GetDbConnectionInfo()
        {
            if (!string.IsNullOrWhiteSpace(ConnectionName))
            {
                return new DbConnectionInfo(ConnectionName);
            }
            else if (!string.IsNullOrWhiteSpace(ConnectionString))
            {
                return new DbConnectionInfo(ConnectionString, ProviderInvariantName ?? "System.Data.SqlClient");
            }

            return null;
        }

        #endregion
    }
}
