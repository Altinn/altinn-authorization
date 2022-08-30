namespace Altinn.AccessGroups.Persistance
{
    /// <summary>
    /// Represents settings needed to communicate with the PostgreSQL database server.
    /// </summary>
    public class PostgreSQLSettings
    {
        /// <summary>
        /// Connection string to the PostgresSQL database server.
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Password for app user for the postgres db
        /// </summary>
        public string AuthorizationDbPwd { get; set; } = string.Empty;
    }
}
