using Altinn.Platform.Authorization.Configuration;
using Altinn.ResourceRegistry.Core;
using Altinn.ResourceRegistry.Core.Models;
using Altinn.ResourceRegistry.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Data;
using System.Runtime.Serialization;
using System.Text.Encodings.Web;

namespace Altinn.ResourceRegistry.Persistence
{
    public class ResourceRepository : IResourceRegistryRepository
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;
        private readonly string getResourceById = "SELECT * from resourceregistrytest.get_resource_by_id(@_identifier)";

        public ResourceRepository(
    IOptions<PostgreSQLSettings> postgresSettings,
    ILogger<ResourceRepository> logger)
        {
            _logger = logger;
            _connectionString = string.Format(
                postgresSettings.Value.ConnectionString,
                postgresSettings.Value.AuthorizationDbPwd);
        }

        public Task<List<ServiceResource>> Search(ResourceSearch resourceSearch)
        {
            throw new NotImplementedException();
        }

        Task IResourceRegistryRepository.CreateResource(ServiceResource resource)
        {
            throw new NotImplementedException();
        }

        Task IResourceRegistryRepository.DeleteResource(string id)
        {
            throw new NotImplementedException();
        }

        async Task<ServiceResource> IResourceRegistryRepository.GetResource(string id)
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                NpgsqlCommand pgcom = new NpgsqlCommand(getResourceById, conn);
                pgcom.Parameters.AddWithValue("_identifier", id);
                
                using NpgsqlDataReader reader = pgcom.ExecuteReader();
                if (reader.Read())
                {
                    return getServiceResource(reader);
                }

                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Authorization // ResourceRegistryRepository // GetResource // Exception");
                throw;
            }
        }

        Task IResourceRegistryRepository.UpdateResource(ServiceResource resource)
        {
            throw new NotImplementedException();
        }

        private static ServiceResource getServiceResource(NpgsqlDataReader reader)
        {
            if (reader["serviceresourcejson"] != DBNull.Value)
            {
                var jsonb = reader.GetString("serviceresourcejson");

                ServiceResource? resource = System.Text.Json.JsonSerializer.Deserialize<ServiceResource>(jsonb, new System.Text.Json.JsonSerializerOptions() { PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase }) as ServiceResource;
                return resource;
            }
            return null;
        }
    }
}