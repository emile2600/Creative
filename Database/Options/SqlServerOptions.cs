using Creative.Database.Converters;
using Creative.Database.Data;
using Microsoft.EntityFrameworkCore;

namespace Creative.Database.Options;

public class SqlServerOptions<TContext> : DatabaseContextOptions where TContext : DatabaseContext<TContext>
{
    public override DatabaseSrc DatabaseSrc => DatabaseSrc.SqlServer;

    public string ConnectionString { get; init; }

    public SqlServerOptions(string connectionString = "") 
    {
        ConnectionString = connectionString;
	DbOptions = new DbContextOptionsBuilder<TContext>().UseSqlServer(ConnectionString, options => options.EnableRetryOnFailure()).Options;
}

public void OnConfiguring(DbContextOptionsBuilder builder) => builder.UseSqlServer(ConnectionString);

/// <summary> Configure the conventions for SQL Server. </summary>
public static void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
{
	configurationBuilder.Properties<DateOnly>().HaveConversion<DateOnlyConverter>();
	configurationBuilder.Properties<TimeOnly>().HaveConversion<TimeOnlyConverter>();
}
}