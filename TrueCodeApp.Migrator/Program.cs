using CommandLine;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TrueCodeApp.Core.Infrastructure.Persistence;
using TrueCodeApp.Migrator.DbInitializer;

namespace TrueCodeApp.Migrator;

public static class Program
{
    public static void Main(string[] args)
    {
        if (args.Contains("--dryrun"))
            return;

        CommandLineOptions? options = null;

        if (args.Length > 0)
            Parser
                .Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(c =>
                {
                    Console.WriteLine(c.ConnectionString);

                    options = c;
                });

        MigrateDatabase(options);

        Console.WriteLine("Finished migration");
    }

    private static void MigrateDatabase(CommandLineOptions? options)
    {
        const string jsonFile = "appsettings.json";

        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(jsonFile, optional: true, reloadOnChange: true)
            .AddUserSecrets(typeof(Program).Assembly, optional: true)
            .Build();

        var connectionString = config.GetConnectionString(nameof(ApplicationDbContext));
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(opt =>
            opt.UseNpgsql(connectionString));

        EnsureDatabaseExists(connectionString);

        var migrationRunner = new MigratorRunner(connectionString);
        migrationRunner.Migrate();
    }

    private static void EnsureDatabaseExists(string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        var databaseName = builder.Database;
        builder.Database = "postgres";

        using var connection = new NpgsqlConnection(builder.ConnectionString);
        connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = $"SELECT 1 FROM pg_database WHERE datname = '{databaseName}'";
        var exists = cmd.ExecuteScalar() != null;

        if (!exists)
        {
            Console.WriteLine($"Database '{databaseName}' not found. Creating...");
            using var createCmd = connection.CreateCommand();
            createCmd.CommandText = $"CREATE DATABASE \"{databaseName}\"";
            createCmd.ExecuteNonQuery();
            Console.WriteLine($"Database '{databaseName}' created successfully.");
        }
        else
        {
            Console.WriteLine($"Database '{databaseName}' already exists.");
        }
    }
}