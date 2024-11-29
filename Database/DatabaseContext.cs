using Creative.Database.Data;
using Creative.Database.Options;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Creative.Database;

    /// <summary> Has some helper functions for creating a <see cref="DbContext"/>. </summary>
    public abstract class DatabaseContext<TContext> : DbContext where TContext : DatabaseContext<TContext>
	{

	/// <summary> The name of the current project. </summary>
	private static string ProjectName => Assembly.GetCallingAssembly().GetName().Name
		?? throw new InvalidOperationException("Project name is null.");

	private DatabaseContextOptions? _options;

	/// <summary> Database options. </summary>
	public DatabaseContextOptions Options
	{
		get { return _options ?? throw new NullReferenceException($"The database context can not be created with a DatabseContextOptions that is null"); }
		private set { _options = value; }
	}

	protected DatabaseContext(DatabaseContextOptions options) : base(options.DbOptions!) => Options = options;

	/// <summary>
	/// Should only be used by the command line and never in the program.cs when initializing the database.
	/// </summary>
	protected DatabaseContext(DbContextOptions options) : base(options) { }

	/// <summary> The absolute path of the database. </summary>
	/// <remarks> Assumes that the database name is in a folder called "Database". </remarks>
	protected string DbPath() => Path.Join(Environment.CurrentDirectory.Split(ProjectName)[0], $"Data\\{Options.DbName}.db");

	/// <summary> The absolute path of the database. </summary>
	/// <remarks> Assumes that the database name is in a folder called "Database". </remarks>
	protected static string DbPath(string DbName)
		=> Path.Join(Environment.CurrentDirectory.Split(ProjectName)[0], $"Data\\{DbName}.db");

	protected override void OnConfiguring(DbContextOptionsBuilder builder)
	{
		base.OnConfiguring(builder);
		switch (Options.DatabaseSrc)
		{
			case DatabaseSrc.Sqlite:
				((SqliteOptions<TContext>)Options).OnConfiguring(builder);
				break;
			case DatabaseSrc.SqlServer:
				((SqlServerOptions<TContext>)Options).OnConfiguring(builder);
				break;
			case DatabaseSrc.PostgreSql:
				((PostgreSqlOptions<TContext>)Options).OnConfiguring(builder);
				break;
			default:
				throw new NotImplementedException("Database src is not supported.");
		}
	}
}