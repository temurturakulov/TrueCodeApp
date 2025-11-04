using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TrueCodeApp.Core.Infrastructure.Persistence;

namespace TrueCodeApp.Migrator.DbInitializer;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        await EnsureTablesAsync(context);
    }

    private static async Task EnsureTablesAsync(ApplicationDbContext context)
    {
        var tableNames = context.Model.GetEntityTypes()
            .Select(t => t.GetTableName())
            .Distinct()
            .ToList();

        var missingTables = new List<string>();

        foreach (var tableName in tableNames)
        {
            var exists = await TableExistsAsync(context, tableName);

            if (!exists)
            {
                missingTables.Add(tableName);
            }
            else
            {
                Console.WriteLine($"✅ Table '{tableName}' already exists");
            }
        }

        if (missingTables.Count == 0)
        {
            Console.WriteLine("✅ All tables exist, nothing to create.");
            return;
        }

        Console.WriteLine("⚙️ Missing tables:");
        foreach (var name in missingTables)
            Console.WriteLine($"   - {name}");

        Console.WriteLine("🚀 Creating missing tables...");
        var createSql = context.Database.GenerateCreateScript();
        await context.Database.ExecuteSqlRawAsync(createSql);
        Console.WriteLine("✅ All missing tables created successfully.");
    }


    private static async Task<bool> TableExistsAsync(ApplicationDbContext context, string tableName)
    {
        var sql = $"SELECT CAST(to_regclass('public.{tableName}') AS text)";
        await using var cmd = context.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = sql;

        if (cmd.Connection.State != System.Data.ConnectionState.Open)
            await cmd.Connection.OpenAsync();

        var result = await cmd.ExecuteScalarAsync();

        // Не закрываем соединение вручную — EF сам управляет
        return result != DBNull.Value && result != null;
    }

}