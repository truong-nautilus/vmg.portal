using FluentMigrator;

namespace Netcore.Migrations.Common.Extensions;

/// <summary>
/// Extension methods cho FluentMigrator để hỗ trợ chạy SQL files
/// </summary>
public static class MigrationExtensions
{
    /// <summary>
    /// Execute SQL từ file path
    /// </summary>
    /// <param name="migration">Migration instance</param>
    /// <param name="sqlFilePath">Đường dẫn tuyệt đối đến file SQL</param>
    public static void ExecuteSqlFile(this Migration migration, string sqlFilePath)
    {
        if (!File.Exists(sqlFilePath))
            throw new FileNotFoundException($"SQL file không tồn tại: {sqlFilePath}");

        var sql = File.ReadAllText(sqlFilePath);
        migration.Execute.Sql(sql);
    }

    /// <summary>
    /// Execute tất cả SQL files trong thư mục theo thứ tự tên file
    /// </summary>
    /// <param name="migration">Migration instance</param>
    /// <param name="directoryPath">Đường dẫn thư mục chứa SQL files</param>
    public static void ExecuteAllSqlFilesInDirectory(this Migration migration, string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
            throw new DirectoryNotFoundException($"Thư mục không tồn tại: {directoryPath}");

        var sqlFiles = Directory.GetFiles(directoryPath, "*.sql", SearchOption.TopDirectoryOnly);

        foreach (var file in sqlFiles.OrderBy(f => Path.GetFileName(f)))
        {
            Console.WriteLine($"  Executing: {Path.GetFileName(file)}");
            var sql = File.ReadAllText(file);
            migration.Execute.Sql(sql);
        }
    }

    /// <summary>
    /// Lấy đường dẫn thư mục StoredProcedures dựa trên database name
    /// </summary>
    /// <param name="databaseName">Tên database (e.g., "VMG.ChickenRoadDB")</param>
    /// <returns>Đường dẫn tuyệt đối đến thư mục SPs</returns>
    public static string GetStoredProceduresPath(string databaseName)
    {
        return Path.Combine(
            AppContext.BaseDirectory,
            "Infrastructure",
            "Persistence",
            "StoredProcedures",
            databaseName);
    }
}
