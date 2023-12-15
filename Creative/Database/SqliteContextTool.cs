using Creative.Database.Data;
using Microsoft.EntityFrameworkCore;

namespace Creative.Database;

/// <summary> <see cref="DatabaseContext"/> that uses SQLite. </summary>
public static class SqliteContextTool
{   
    /// <summary> Initialize <see cref="DbContextOptions{TContext}"/> for <see cref="SqliteContext"/>. </summary>
    public static DbContextOptions<TContext> InitDbContextOptions<TContext>(SqliteOptions options) where TContext : DbContext
    => new DbContextOptionsBuilder<TContext>()
        .UseSqlite(options.Path)
        .Options;

    /// <summary> Sets the configuration of the context to use SQLite. </summary>
    /// <param name="path"> The path to the SQLite database. </param>
    public static void OnConfiguring(DbContextOptionsBuilder builder, SqliteOptions options)
    => builder
        .UseSqlite($"Data source={options.Path}");
}
