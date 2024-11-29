using Creative.Database.Data;
using Microsoft.EntityFrameworkCore;

namespace Creative.Database.Options;

public class SqliteOptions<TContext> : DatabaseContextOptions where TContext : DatabaseContext<TContext>
{

    public override DatabaseSrc DatabaseSrc => DatabaseSrc.Sqlite;

    public string Path { get; init; }

public SqliteOptions(string path = @"main") : base()
{
	Path = path;
	DbOptions = new DbContextOptionsBuilder<TContext>().UseSqlite(Path).Options;
}

public void OnConfiguring(DbContextOptionsBuilder builder) => builder.UseSqlite($"Data source={Path}");
}