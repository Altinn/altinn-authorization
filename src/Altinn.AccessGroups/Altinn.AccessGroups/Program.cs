using Altinn.AccessGroups;
using Altinn.AccessGroups.Core;
using Altinn.AccessGroups.Interfaces;
using Altinn.AccessGroups.Persistance;
using Altinn.AccessGroups.Services;
using Microsoft.Extensions.Logging;
using Npgsql.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using Yuniql.AspNetCore;
using Yuniql.PostgreSql;
using Altinn.AccessGroups.Integrations;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services, builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

await SetConfigurationProviders(builder.Configuration);

ConfigurePostgreSql();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

async Task SetConfigurationProviders(ConfigurationManager config)
{
    string basePath = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;

    //logger.LogInformation($"Program // Loading Configuration from basePath={basePath}");

    config.SetBasePath(basePath);
    string configJsonFile1 = $"{basePath}/altinn-appsettings/altinn-dbsettings-secret.json";
    string configJsonFile2 = $"{Directory.GetCurrentDirectory()}/appsettings.json";

    if (basePath == "/")
    {
        configJsonFile2 = "/app/appsettings.json";
    }

    //logger.LogInformation($"Loading configuration file: '{configJsonFile1}'");
    config.AddJsonFile(configJsonFile1, optional: true, reloadOnChange: true);

    //logger.LogInformation($"Loading configuration file2: '{configJsonFile2}'");
    config.AddJsonFile(configJsonFile2, optional: false, reloadOnChange: true);

    config.AddEnvironmentVariables();
    config.AddCommandLine(args);

    //await ConnectToKeyVaultAndSetApplicationInsights(config);
}

void ConfigureServices(IServiceCollection services, IConfiguration config)
{
    services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

    services.AddSingleton(config);

    services.Configure<PostgreSQLSettings>(config.GetSection("PostgreSQLSettings"));
    services.Configure<SBLBridgeSettings>(config.GetSection("SBLBridgeSettings"));
    services.AddSingleton<IAccessGroupsRepository, AccessGroupsRepository>();
    services.AddSingleton<IAccessGroup, AccessGroupService>();
    services.AddHttpClient<SBLBridgeClient>();
    services.AddSingleton<IAltinnRolesClient, AltinnRolesClient>();
    services.AddSingleton<IMemberships, MembershipService>();    
}

void ConfigurePostgreSql()
{
    if (builder.Configuration.GetValue<bool>("PostgreSQLSettings:EnableDBConnection"))
    {
        NpgsqlLogManager.Provider = new ConsoleLoggingProvider(NpgsqlLogLevel.Trace, true, true);

        ConsoleTraceService traceService = new ConsoleTraceService { IsDebugEnabled = true };

        string connectionString = string.Format(
            builder.Configuration.GetValue<string>("PostgreSQLSettings:AdminConnectionString"),
            builder.Configuration.GetValue<string>("PostgreSQLSettings:authorizationDbAdminPwd"));

        app.UseYuniql(
            new PostgreSqlDataService(traceService),
            new PostgreSqlBulkImportService(traceService),
            traceService,
            new Yuniql.AspNetCore.Configuration
            {
                Workspace = Path.Combine(Environment.CurrentDirectory, builder.Configuration.GetValue<string>("PostgreSQLSettings:WorkspacePath")),
                ConnectionString = connectionString,
                IsAutoCreateDatabase = false,
                IsDebug = true
            });
    }
}