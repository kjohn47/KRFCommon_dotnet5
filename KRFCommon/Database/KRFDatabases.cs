namespace KRFCommon.Database
{
    using System.Collections.Generic;
    public class KRFDatabases
    {
        public IEnumerable<KRFDatabaseConfig> Databases { get; set; }
        public string MigrationAssembly { get; set; }
        public bool EnableAutomaticMigration { get; set; }
    }
}
