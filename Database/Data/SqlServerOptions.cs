namespace Creative.Database.Data
{
	public class SqlServerOptions : DatabaseContextOptions
	{
		public override DatabaseSrc DatabaseSrc => DatabaseSrc.SqlServer;
		public string ConnectionString { get; init; } = string.Empty;
	}
}