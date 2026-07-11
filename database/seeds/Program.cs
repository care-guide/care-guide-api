using CareGuide.Security.Helpers;
using Npgsql;

const string userPassword = "CareGuide@User2026!";
const string adminPassword = "CareGuide@Admin2026!";

var repositoryRoot = FindRepositoryRoot();
var envPath = GetArgumentValue(args, "--env-file") ?? Path.Combine(repositoryRoot, ".env");
var settings = ReadEnvironmentFile(envPath);

var connectionString = new NpgsqlConnectionStringBuilder
{
    Host = "localhost",
    Port = int.Parse(GetRequired(settings, "DB_EXTERNAL_PORT")),
    Database = GetRequired(settings, "DB_NAME"),
    Username = GetRequired(settings, "DB_USER"),
    Password = GetRequired(settings, "DB_PASSWORD")
}.ConnectionString;

var sqlPath = Path.Combine(AppContext.BaseDirectory, "development.sql");
var sql = await File.ReadAllTextAsync(sqlPath);
sql = sql.Replace("{{USER_PASSWORD_HASH}}", PasswordManagerHelper.HashPassword(userPassword), StringComparison.Ordinal)
         .Replace("{{ADMIN_PASSWORD_HASH}}", PasswordManagerHelper.HashPassword(adminPassword), StringComparison.Ordinal);

await using var connection = new NpgsqlConnection(connectionString);
await connection.OpenAsync();
await using var command = new NpgsqlCommand(sql, connection) { CommandTimeout = 60 };
await command.ExecuteNonQueryAsync();

Console.WriteLine("Development seed applied successfully.");
Console.WriteLine("User:  user@careguide.local / CareGuide@User2026!");
Console.WriteLine("Admin: admin@careguide.local / CareGuide@Admin2026!");

static string FindRepositoryRoot()
{
    var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
    while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "CareGuide.slnx")))
        directory = directory.Parent;

    return directory?.FullName ?? Directory.GetCurrentDirectory();
}

static string? GetArgumentValue(string[] arguments, string name)
{
    var index = Array.IndexOf(arguments, name);
    return index >= 0 && index + 1 < arguments.Length ? Path.GetFullPath(arguments[index + 1]) : null;
}

static Dictionary<string, string> ReadEnvironmentFile(string path)
{
    if (!File.Exists(path)) throw new FileNotFoundException("Environment file not found.", path);

    return File.ReadLines(path)
        .Select(line => line.Trim())
        .Where(line => line.Length > 0 && !line.StartsWith('#') && line.Contains('='))
        .Select(line => line.Split('=', 2))
        .ToDictionary(parts => parts[0].Trim(), parts => parts[1].Trim().Trim('"', '\''), StringComparer.OrdinalIgnoreCase);
}

static string GetRequired(IReadOnlyDictionary<string, string> settings, string key)
    => settings.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value)
        ? value
        : throw new InvalidOperationException($"Required setting '{key}' is missing.");
