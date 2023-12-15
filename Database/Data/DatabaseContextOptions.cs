using Microsoft.EntityFrameworkCore;

namespace Creative.Database.Data
{
	public class DatabaseContextOptions
	{
		public DbContextOptions? DbOptions { get; set; }
		public virtual DatabaseSrc DatabaseSrc => DatabaseSrc.None;
		public virtual string DbName { get; set; } = "database";
	}
}