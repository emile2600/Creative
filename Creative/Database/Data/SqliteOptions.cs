namespace Creative.Database.Data;

public class SqliteOptions : DatabaseContextOptions
{
    public override DatabaseSrc DatabaseSrc => DatabaseSrc.Sqlite;
    public string Path { get; init; } = @"main";
}
