namespace KRFCommon.Database
{
    using System.Collections.Generic;
    public class KRFDatabases
    {
        public IDictionary<string, KRFDatabaseConfig> Databases { get; set; }
        public string MigrationAssembly { get; set; }
        public bool EnableAutomaticMigration { get; set; }
    }
}
