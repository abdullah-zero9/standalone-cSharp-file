// seed.cs
// -----------------------------
// Bring in EF Core SQL Server support without a .csproj
#:package Microsoft.EntityFrameworkCore.SqlServer@7.0.12

using System;
using Microsoft.EntityFrameworkCore;

// Helper to get argument or prompt
string GetArgOrPrompt(string[] args, int idx, string name)
{
    if (args.Length > idx && !string.IsNullOrWhiteSpace(args[idx]))
        return args[idx]!;
    Console.Write($"Enter {name}: ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input))
        throw new ArgumentException($"You must supply a valid {name}.");
    return input;
}

// 1) Read inputs
var dbName = GetArgOrPrompt(args, 0, "database name");
var tableName = GetArgOrPrompt(args, 1, "table name");
var fieldName = GetArgOrPrompt(args, 2, "field name");
var fieldType = GetArgOrPrompt(
    args, 3,
    "field data type (byte, short, int, long, float, double, decimal, bool, char, string, datetime, datetimeoffset, timespan, guid, byte[])"
).ToLowerInvariant();

// 2) Build connection string (adjust Server as needed)
var connectionString = $"Server=.;Database={dbName};Trusted_Connection=True;";

// 3) Ask how many rows to insert
Console.Write("How many rows to insert? ");
if (!int.TryParse(Console.ReadLine(), out var count) || count < 1)
    throw new ArgumentException("You must enter a positive integer for row count.");

// 4) Prepare DbContext options
var options = new DbContextOptionsBuilder<SeedContext>()
    .UseSqlServer(connectionString)
    .Options;

// 5) Seed within try/catch
try
{
    await using var ctx = new SeedContext(options);
    Console.WriteLine($"Seeding {count} row(s) into [{tableName}]([{fieldName}] as {fieldType})...");

    var rnd = new Random();
    for (var i = 1; i <= count; i++)
    {
        object value = fieldType switch
        {
        // integer family
        "byte" => (byte)rnd.Next(0, 256),
        "short" => (short)rnd.Next(short.MinValue, short.MaxValue),
        "int" => i,
        "long" => ((long)rnd.Next() << 32) | (uint)rnd.Next(),

        // floating-point
        "float" => (float)(rnd.NextDouble() * 1000),
        "double" => rnd.NextDouble() * 1000,
        "decimal" => Math.Round((decimal)(rnd.NextDouble() * 1000), 2),

        // boolean
        "bool" => i % 2 == 0,

        // character and text
        "char" => (char)rnd.Next('A', 'Z'),
        "string" => $"Str_{Guid.NewGuid():N}".Substring(0, 20),

        // date & time
        "datetime" => DateTime.Now.AddMinutes(rnd.Next(-1000, 1000)),
        "datetimeoffset" => DateTimeOffset.Now.AddHours(rnd.Next(-24, 24)),
        "timespan" => TimeSpan.FromMinutes(rnd.Next(0, 24 * 60)),

        // GUID
        "guid" => Guid.NewGuid(),

        // binary
        "byte[]" => {
        var buf = new byte[16];
                rnd.NextBytes(buf);
                return buf;
    },

            _ => throw new NotSupportedException($"Type '{fieldType}' is not supported.")
        };

// Parameterize safely
await ctx.Database.ExecuteSqlInterpolatedAsync(
    $"INSERT INTO [{tableName}]([{fieldName}]) VALUES ({value})"
);

// Log progress
if (i % 100 == 0)
    Console.WriteLine($"  → Inserted {i} rows...");
    }

    Console.WriteLine("✅ Seeding complete!");
}
catch (Exception ex)
{
    Console.Error.WriteLine($"❌ Error during seeding: {ex.Message}");
Environment.Exit(1);
}

// Minimal DbContext for raw SQL execution
class SeedContext : DbContext
{
    public SeedContext(DbContextOptions opts) : base(opts) { }
}
