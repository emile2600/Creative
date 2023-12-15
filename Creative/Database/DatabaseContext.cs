using Creative.Database.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Creative.Database;

/// <summary> Has some helper functions for creating a <see cref="DbContext"/>. </summary>
public abstract class DatabaseContext : DbContext
{
    protected DatabaseContext(DatabaseContextOptions options) : base(options.DbOptions!)
    {
        Options = options;
    }

    protected DatabaseContext(DbContextOptions options) : base(options) { }

    /// <summary> The name of the current project. </summary>
    private static string ProjectName => Assembly.GetCallingAssembly().GetName().Name
        ?? throw new InvalidOperationException("Project name is null.");


    /// <summary> Database options. </summary>
    protected static DatabaseContextOptions Options { get; private set; } = null!;

    /// <summary> The absolute path of the database. </summary>
    /// <remarks> Assumes that the database name is in a folder called "Database". </remarks>
    protected static string DbPath()
        => Path.Join(Environment.CurrentDirectory.Split(ProjectName)[0], $"Data\\{Options.DbName}.db");

    /// <summary> The absolute path of the database. </summary>
    /// <remarks> Assumes that the database name is in a folder called "Database". </remarks>
    protected static string DbPath(string DbName)
        => Path.Join(Environment.CurrentDirectory.Split(ProjectName)[0], $"Data\\{DbName}.db");
}
