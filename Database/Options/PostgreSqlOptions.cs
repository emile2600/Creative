using Creative.Database.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace Creative.Database.Options;

public class PostgreSqlOptions<TContext> : DatabaseContextOptions where TContext : DatabaseContext<TContext>
{

    public override DatabaseSrc DatabaseSrc => DatabaseSrc.PostgreSql;

	public string ConnectionString { get; init; } = string.Empty;

	public PostgreSqlOptions(string connectionString = "")
	{
		ConnectionString = connectionString;
        DbOptions = new DbContextOptionsBuilder<TContext>().UseNpgsql(ConnectionString, delegate (NpgsqlDbContextOptionsBuilder options)
		{
			options.EnableRetryOnFailure();
		}).Options;
	}

	public void OnConfiguring(DbContextOptionsBuilder builder) => builder.UseNpgsql(ConnectionString);
}
