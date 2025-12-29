using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Netcore.Migrations.Common;

/// <summary>
/// Helper class để chạy FluentMigrator migrations
/// </summary>
public static class MigrationRunner
{
    /// <summary>
    /// Chạy tất cả migrations chưa được apply
    /// </summary>
    /// <param name="connectionString">Connection string đến database</param>
    /// <param name="migrationAssemblyType">Type từ assembly chứa migrations (e.g., typeof(CreateTables))</param>
    /// <param name="createDatabaseIfNotExists">Tự động tạo database nếu chưa tồn tại</param>
    public static void RunMigrations(
        string connectionString,
        Type migrationAssemblyType,
        bool createDatabaseIfNotExists = true)
    {
        if (createDatabaseIfNotExists)
        {
            EnsureDatabaseExists(connectionString);
        }

        var serviceProvider = CreateServices(connectionString, migrationAssemblyType);

        using var scope = serviceProvider.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

        Console.WriteLine("=== Running FluentMigrator Migrations ===");
        runner.MigrateUp();
        Console.WriteLine("=== Migrations Completed ===");
    }

    /// <summary>
    /// Rollback về version cụ thể
    /// </summary>
    /// <param name="connectionString">Connection string đến database</param>
    /// <param name="migrationAssemblyType">Type từ assembly chứa migrations</param>
    /// <param name="version">Version number để rollback về</param>
    public static void RollbackToVersion(
        string connectionString,
        Type migrationAssemblyType,
        long version)
    {
        var serviceProvider = CreateServices(connectionString, migrationAssemblyType);

        using var scope = serviceProvider.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

        Console.WriteLine($"=== Rolling back to version {version} ===");
        runner.MigrateDown(version);
        Console.WriteLine("=== Rollback Completed ===");
    }

    /// <summary>
    /// Tạo database nếu chưa tồn tại
    /// </summary>
    private static void EnsureDatabaseExists(string connectionString)
    {
        var builder = new SqlConnectionStringBuilder(connectionString);
        var databaseName = builder.InitialCatalog;

        // Chuyển sang master database để tạo database mới
        builder.InitialCatalog = "master";

        using var connection = new SqlConnection(builder.ConnectionString);
        connection.Open();

        var checkDbSql = $@"
            IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{databaseName}')
            BEGIN
                CREATE DATABASE [{databaseName}];
                PRINT 'Database [{databaseName}] created successfully.';
            END
            ELSE
            BEGIN
                PRINT 'Database [{databaseName}] already exists.';
            END";

        using var command = new SqlCommand(checkDbSql, connection);
        command.ExecuteNonQuery();

        Console.WriteLine($"Database [{databaseName}] is ready.");
    }

    /// <summary>
    /// Tạo service provider cho FluentMigrator
    /// </summary>
    private static IServiceProvider CreateServices(string connectionString, Type migrationAssemblyType)
    {
        return new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddSqlServer()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(migrationAssemblyType.Assembly).For.Migrations())
            .AddLogging(lb => lb
                .AddFluentMigratorConsole()
                .SetMinimumLevel(LogLevel.Information))
            .BuildServiceProvider(false);
    }
}
