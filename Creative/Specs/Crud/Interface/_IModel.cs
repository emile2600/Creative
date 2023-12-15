using Creative.Api.Data;
using Creative.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Creative.Api.Interfaces;

[TestFixture]
internal sealed class _IModel
{
	/*[Test]
	public async Task IncludeAll()
	{
		var _dbContextOptions = new DbContextOptionsBuilder<DbContext>()
		   .UseInMemoryDatabase(Guid.NewGuid().ToString())
		   .Options;
		var db = new Database(_dbContextOptions);
		var model1 = new Model() { Id = 1, OtherId = 2, Value = "value" };
		var model2 = new Model() { Id = 2, OtherId = 1, Value = "value" };
		await db.AddRangeAsync(model1, model2);
		await db.SaveChangesAsync();

		var query = IModel.IncludeAll(db.Models);

		var expected = db.Set<Model>()
			.Include(model => model.Id)
			.Include(model => model.Value)
			.Include(model => model.OtherId)
			.Include(model => model.Other);
		query.Should().BeEquivalentTo(expected);
	}*/
}

internal class Database : DbContext
{
	public Database(DbContextOptions options ) : base(options) { }
	public DbSet<Model> Models { get; set; }
	protected Database() { }
}

internal sealed class Model : IModel
{
	public int? Id { get; set; }
	public string? Value { get; set; }
	public int? OtherId { get; set; }
	public Model? Other { get; set; }

	public void AutoIncrementPrimaryKey()
	=> Id = null;

	public HashSet<Key> GetPrimaryKey()
	=> new() { new Key(nameof(Id), Id) };

	public void SetPrimaryKey(HashSet<Key> keys)
	=> Id = (int?)keys.Single(key => key.Name == nameof(Id)).Value;
}
