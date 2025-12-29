using System;
using System.IO;
using FluentMigrator;
using Netcore.Migrations.Common.Extensions;

namespace NetCore.PortalAPI.Migrations;

[Migration(1, "Create Stored Procedures for BillingDB")]
public class M001_InitializeDatabase : Migration
{
    private const string DatabaseName = "VMG.BillingDB";

    public override void Up()
    {
        var spDirectory = MigrationExtensions.GetStoredProceduresPath(DatabaseName);

        if (Directory.Exists(spDirectory))
        {
            Console.WriteLine($"Executing SQL files from: {spDirectory}");
            this.ExecuteAllSqlFilesInDirectory(spDirectory);
        }
        else
        {
            Console.WriteLine($"Warning: SQL directory not found: {spDirectory}");
        }
    }

    public override void Down() { }
}
