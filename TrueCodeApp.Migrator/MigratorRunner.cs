using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace TrueCodeApp.Migrator;

public class MigratorRunner(string connectionString)
{
    public void Migrate()
    {
        CreateDatabaseIfNotExists();

        var service = CreateService();
        using var scope = service.CreateScope();

        UpdateDatabase(scope.ServiceProvider.GetRequiredService<IMigrationRunner>());
    }

    private void CreateDatabaseIfNotExists()
    {
        // Extract database name from connection string
        var databaseName = GetDatabaseNameFromConnectionString();

        // Connect to postgres database (default database)
        var masterConnectionString = GetMasterConnectionString();
        using var connection = new NpgsqlConnection(masterConnectionString);
        connection.Open();

        // Check if database exists
        var command = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname = '{databaseName}'", connection);
        var exists = command.ExecuteScalar() != null;

        // Create database if it doesn't exist
        if (exists)
            return;

        command.CommandText = $"CREATE DATABASE {databaseName}";
        command.ExecuteNonQuery();
        Console.WriteLine($"Database '{databaseName}' created.");
    }

    private IServiceProvider CreateService()
    {
        Console.WriteLine(typeof(MigratorRunner).Assembly.FullName);
        return new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb =>
                rb.AddPostgres()
                    .WithVersionTable(new VersionTable())
                    .WithGlobalConnectionString(connectionString)
                    .WithGlobalCommandTimeout(Timeout.InfiniteTimeSpan)
                    .ScanIn(typeof(MigratorRunner).Assembly)
                    .For.Migrations()
            )
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider(false);
    }

    private void UpdateDatabase(IMigrationRunner runner)
    {
        runner.MigrateUp();

        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        connection.ReloadTypes();
    }

    private string? GetDatabaseNameFromConnectionString()
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        return builder.Database;
    }

    private string GetMasterConnectionString()
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Database = "postgres"
        };

        return builder.ConnectionString;
    }
}