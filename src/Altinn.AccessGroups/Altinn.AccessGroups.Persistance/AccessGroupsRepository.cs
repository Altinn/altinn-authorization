using Altinn.AccessGroups.Core;
using Microsoft.Extensions.Options;

namespace Altinn.AccessGroups.Persistance
{
    public class AccessGroupsRepository : IAccessGroupsRepository
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessGroupsRepository"/> class
        /// </summary>
        /// <param name="postgresSettings">The postgreSQL configurations for AuthorizationDB</param>
        /// <param name="logger">logger</param>
        public AccessGroupsRepository(IOptions<PostgreSQLSettings> postgresSettings, ILogger<AccessGroupsRepository> logger)
        {
            _logger = logger;
            _connectionString = string.Format(
                postgresSettings.Value.ConnectionString,
                postgresSettings.Value.AuthorizationDbPwd);
        }
    }
}
