namespace Creative.Database.Data;

public enum DatabaseSrc
{
	None = 0,
	Sqlite = 1,
	SqlServer = 2,

	/// <summary>
	/// W.I.P.
	/// </summary>
	/// <remarks>
	/// Does not work
	/// </remarks>
	/// 
	InMemory = 3,
	PostgreSql = 4,
}