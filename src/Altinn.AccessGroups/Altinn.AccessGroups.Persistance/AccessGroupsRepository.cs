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
        private readonly string insertGroupMembershipFunc = "select * from accessgroup.insert_accessgroupmembership(@_coveredByUserId, @_coveredByPartyId, @_offeredByPartyId, @_groupId)";
        private readonly string deleteGroupMembershipFunc = "select * from accessgroup.delete_accessgroupmembership(@_coveredByUserId, @_coveredByPartyId, @_offeredByPartyId, @_groupId)";

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
                _logger.LogError(e, "AccessGroups // AccessGroupsRepository // InsertAccessGroup // Exception");
                throw;
            }
        }

        public async Task<GroupMembership> InsertGroupMembership(GroupMembership membership)
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                NpgsqlCommand pgcom = new NpgsqlCommand(insertGroupMembershipFunc, conn);

                if (membership.CoveredByUserId != null)
                {
                    pgcom.Parameters.AddWithValue("_coveredByUserId", membership.CoveredByUserId);
                }
                else
                {
                    pgcom.Parameters.AddWithValue("_coveredByUserId", DBNull.Value);
                }
                
                if (membership.CoveredByPartyId != null)
                {
                    pgcom.Parameters.AddWithValue("_coveredByPartyId", membership.CoveredByPartyId);
                }
                else
                {
                    pgcom.Parameters.AddWithValue("_coveredByPartyId", DBNull.Value);
                }
                
                pgcom.Parameters.AddWithValue("_offeredByPartyId", membership.OfferedByPartyId);
                pgcom.Parameters.AddWithValue("_groupId", membership.GroupId);

                using NpgsqlDataReader reader = await pgcom.ExecuteReaderAsync();
                if (reader.Read())
                {
                    return GetGroupMembership(reader);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "AccessGroups // AccessGroupsRepository // InsertGroupMembership // Exception");
                throw;
            }
        }

        public async Task<GroupMembership> RevokeGroupMembership(GroupMembership membership)
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                NpgsqlCommand pgcom = new NpgsqlCommand(deleteGroupMembershipFunc, conn);

                if (membership.CoveredByUserId != null)
                {
                    pgcom.Parameters.AddWithValue("_coveredByUserId", membership.CoveredByUserId);
                }
                else
                {
                    pgcom.Parameters.AddWithValue("_coveredByUserId", DBNull.Value);
                }

                if (membership.CoveredByPartyId != null)
                {
                    pgcom.Parameters.AddWithValue("_coveredByPartyId", membership.CoveredByPartyId);
                }
                else
                {
                    pgcom.Parameters.AddWithValue("_coveredByPartyId", DBNull.Value);
                }

                pgcom.Parameters.AddWithValue("_offeredByPartyId", membership.OfferedByPartyId);
                pgcom.Parameters.AddWithValue("_groupId", membership.GroupId);

                using NpgsqlDataReader reader = await pgcom.ExecuteReaderAsync();
                if (reader.Read())
                {
                    return GetGroupMembership(reader);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "AccessGroups // AccessGroupsRepository // DeleteGroupMembership // Exception");
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

        private static GroupMembership GetGroupMembership(NpgsqlDataReader reader)
        {
            return new GroupMembership
            {
                CoveredByUserId = reader.GetValue<int?>("CoveredByUserId"),
                CoveredByPartyId = reader.GetValue<int?>("CoveredByPartyId"),
                OfferedByPartyId = reader.GetValue<int>("OfferedByPartyId"),
                GroupId = reader.GetValue<string>("GroupId")
            };
        }
    }
}
