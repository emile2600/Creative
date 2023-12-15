using Creative.Database.Converters;
using Creative.Database.Data;
using Microsoft.EntityFrameworkCore;

namespace Creative.Database;

public static class SqlServerContextTool
{
    /// <summary> Initialize <see cref="DbContextOptions{TContext}"/> for <see cref="SqliteContext"/>. </summary>
    public static DbContextOptions<TContext> InitDbContextOptions<TContext>(SqlServerOptions options) where TContext : DbContext
    => new DbContextOptionsBuilder<TContext>()
        .UseSqlServer(options.ConnectionString, options => options.EnableRetryOnFailure())
        .Options;

    public static void OnConfiguring(DbContextOptionsBuilder builder, SqlServerOptions options)
    => builder
        .UseSqlServer(options.ConnectionString);

    /// <summary> Configure the conventions for SQL Server. </summary>
    public static void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateOnly>().HaveConversion<DateOnlyConverter>();
        configurationBuilder.Properties<TimeOnly>().HaveConversion<TimeOnlyConverter>();
    }
}
