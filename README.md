# .NET Seed Script (seed.cs)

A single-file C# script for quickly seeding dummy data into any SQL Server database table.  
Leverages .NET 10’s top-level statements and file-level directives—no `.csproj` required.

---

## 🚀 Features

- **Zero boilerplate**: Just one `.cs` file.  
- **All major CLR types**: `byte`, `short`, `int`, `long`, `float`, `double`, `decimal`, `bool`, `char`, `string`, `DateTime`, `DateTimeOffset`, `TimeSpan`, `Guid`, `byte[]`.  
- **Automatic dummy data**: Randomized values appropriate to each type.  
- **Inline NuGet**: Pull in EF Core SQL Server with a single `#:package` directive.  
- **Safe parameterization**: Uses `ExecuteSqlInterpolatedAsync` to avoid SQL injection.  
- **Interactive or scripted**: Pass args or enter values at runtime.  
- **Seamless growth**: `dotnet project convert seed.cs` scaffolds a full `.csproj`.

---

## 📋 Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com) Preview 4 or higher  
- SQL Server reachable at `localhost` (customize connection string if needed)  

---

## 📂 Repository Structure

