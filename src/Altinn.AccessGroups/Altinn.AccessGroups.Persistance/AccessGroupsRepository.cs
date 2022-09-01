using Altinn.AccessGroups.Core;
using Altinn.AccessGroups.Core.Models;
using Altinn.AccessGroups.Persistance.Extensions;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Altinn.AccessGroups.Persistance
{
    public class AccessGroupsRepository : IAccessGroupsRepository
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;

        private readonly string insertAccessGroupFunc = "select * from accessgroup.insert_accessgroup(@_accessGroupCode, @_accessGroupType, @_hidden)";

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
            NpgsqlConnection.GlobalTypeMapper.MapEnum<AccessGroupType>("accessgroup.accessgrouptype");
        }

        /// <inheritdoc/>
        public async Task<AccessGroup> InsertAccessGroup(AccessGroup accessGroup)
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                NpgsqlCommand pgcom = new NpgsqlCommand(insertAccessGroupFunc, conn);
                pgcom.Parameters.AddWithValue("_accessGroupCode", accessGroup.AccessGroupCode);
                pgcom.Parameters.AddWithValue("_accessGroupType", accessGroup.AccessGroupType);
                pgcom.Parameters.AddWithValue("_hidden", accessGroup.Hidden);

                using NpgsqlDataReader reader = await pgcom.ExecuteReaderAsync();
                if (reader.Read())
                {
                    return GetAccessGroup(reader);
                }

                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "AccessGroups // AccessGroupsRepository // Insert // Exception");
                throw;
            }
        }

        private static AccessGroup GetAccessGroup(NpgsqlDataReader reader)
        {
            return new AccessGroup
            {
                AccessGroupId = reader.GetValue<int>("AccessGroupId"),
                AccessGroupCode = reader.GetValue<string>("AccessGroupCode"),
                AccessGroupType = reader.GetValue<AccessGroupType>("AccessGroupType"),                
                Hidden = reader.GetValue<bool>("Hidden"),
                Created = reader.GetValue<DateTime>("Created"),
                Modified = reader.GetValue<DateTime>("Modified")
            };
        }
    }
}
