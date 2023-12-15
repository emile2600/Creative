using Creative.Api.Data;
using Creative.Api.Exceptions;
using Creative.Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Creative.Api.Implementations.EntityFrameworkCore;

[TestFixture]
public class _Crud
{
    private DbContextOptions<DbContext> _dbContextOptions;
    private DbContext _dbContext;
    private Crud<TestModel> _crud;

    [SetUp]
    public void SetUp()
    {
        _dbContextOptions = new DbContextOptionsBuilder<DbContext>()
		   .UseInMemoryDatabase(Guid.NewGuid().ToString())
		   .Options;
		_dbContext = new TestContext(_dbContextOptions);
        _crud = new Crud<TestModel>(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

	#region Create
	[Test]
    public async Task Add_should_add_object_to_database()
    {
        // Arrange
        var testModel = new TestModel { Id = 1, Name = "Test" };

        // Act
        await _crud.Add(objs: testModel);

        // Assert
        _dbContext.Set<TestModel>().Should().Contain(testModel)
            .And.HaveCount(1);
    }

    [Test]
    public async Task Add_should_on_default_increment_primary_key_when_adding_object()
    {
		// Arrange   
		var model = new TestModel { Id = 1, Name = "Test" };
		await _crud.Add(objs: model);

		// Act
		await _crud.Add(objs: model);

		// Assert
        _dbContext.Set<TestModel>().Should().HaveCount(2)
            .And.ContainEquivalentOf(model)
            .And.ContainEquivalentOf(new TestModel { Id = 2, Name = "Test" });
	}

    [Test]
    public async Task Add_should_throw_ArgumentException_if_object_already_exists()
    {
        // Arrange
        var model = new TestModel { Id = 1, Name = "Test" };
        await _crud.Add(autoIncrementPrimaryKey: false, objs: model);

        // Act
        var result = async () => await _crud.Add(autoIncrementPrimaryKey: false, objs: model);

        // Assert
        await result.Should().ThrowAsync<ArgumentException>($"An item with the same key has already been added. Key: {model.Id}");
    }

    [Test]
    public async Task Add_returns_object_that_has_been_added()
    {
        // Arrange
        var model = new TestModel { Id = 1, Name = "Test" };

        // Act
        var result = await _crud.Add(objs: model);

        // Assert
        result.Should().HaveCount(1)
            .And.ContainEquivalentOf(model);
    }

    [Test]
    public async Task TryAdd_returns_null_when_object_cannot_be_added()
    {
		// Arrange
		var model = new TestModel { Id = 1, Name = "Test" };
		await _crud.TryAdd(autoIncrementPrimaryKey: false, objs: model);

		// Act
		var result = await _crud.TryAdd(autoIncrementPrimaryKey: false, objs: model);

		// Assert
		result.Should().BeNull();
	}

	[Test]
	public async Task TryAdd_returns_object_that_has_been_added()
	{
		// Arrange
		var model = new TestModel { Id = 1, Name = "Test" };

		// Act
		var result = await _crud.TryAdd(objs: model);

		// Assert
		result.Should().HaveCount(1)
			.And.ContainEquivalentOf(model);
	}

	[Test]
	public async Task TryAdd_should_add_object_to_database()
	{
		// Arrange
		var testModel = new TestModel { Id = 1, Name = "Test" };

		// Act
		await _crud.TryAdd(objs: testModel);

		// Assert
		_dbContext.Set<TestModel>().Should().Contain(testModel)
			.And.HaveCount(1);
	}

	[Test]
	public async Task TryAdd_should_on_default_increment_primary_key_when_adding_object()
	{
		// Arrange   
		var model = new TestModel { Id = 1, Name = "Test" };
		await _crud.TryAdd(objs: model);

		// Act
		await _crud.TryAdd(objs: model);

		// Assert
		_dbContext.Set<TestModel>().Should().HaveCount(2)
			.And.ContainEquivalentOf(model)
			.And.ContainEquivalentOf(new TestModel { Id = 2, Name = "Test" });
	}
	#endregion

	#region Read
	[Test]
	public async Task GetAll_should_return_relations()
	{
		// Arrange 
		var model1 = new TestModel() { Id = 1, Name = "Test", OtherId = 2 };
		var model2 = new TestModel() { Id = 2, Name = "Test", OtherId = 1 };
		await _dbContext.AddRangeAsync(new TestModel[] { model1, model2 });
		await _dbContext.SaveChangesAsync();

		// Act
		var result = await _crud.GetAll();

		// Assert
		result.Should().HaveCount(2)
			.And.ContainEquivalentOf(new TestModel() { Id = 1, Name = "Test", OtherId = 2, Other = model2 })
			.And.ContainEquivalentOf(new TestModel() { Id = 2, Name = "Test", OtherId = 1, Other = model1 });
	}

    [Test]
    public async Task GetAll_should_return_all_objects_in_the_database()
    {
        // Arrange
        var model1 = new TestModel() { Id = 1, Name = "Test" };
        var model2 = new TestModel() { Id = 2, Name = "Test" };
        await _dbContext.AddRangeAsync(new TestModel[] { model1, model2 });
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _crud.GetAll();

        // Assert
        result.Should().HaveCount(2)
            .And.ContainEquivalentOf(model1)
            .And.ContainEquivalentOf(model2);
    }

    [Test]
    public async Task Get_should_return_object_with_primarykey()
    {
		// Arrange
		var model1 = new TestModel() { Id = 1, Name = "Test" };
		var model2 = new TestModel() { Id = 2, Name = "Test" };
		await _dbContext.AddRangeAsync(new TestModel[] { model1, model2 });
		await _dbContext.SaveChangesAsync();

		// Act
		var result = await _crud.Get(model1.GetPrimaryKey());

		// Assert
		result.Should().BeEquivalentTo(model1);
	}

    [Test]
	public async Task TryGet_should_return_object_with_primarykey()
	{
		// Arrange
		var model1 = new TestModel() { Id = 1, Name = "Test" };
		var model2 = new TestModel() { Id = 2, Name = "Test" };
		await _dbContext.AddRangeAsync(new TestModel[] { model1, model2 });
		await _dbContext.SaveChangesAsync();

		// Act
		var result = await _crud.TryGet(model1.GetPrimaryKey());

		// Assert
		result.Should().BeEquivalentTo(model1);
	}

    [Test]
	public async Task Get_should_throw_ObjectNotFoundException_when_object_not_found()
	{
		// Arrange
		var model1 = new TestModel() { Id = 1, Name = "Test" };
		var model2 = new TestModel() { Id = 2, Name = "Test" };
		await _dbContext.AddRangeAsync(new TestModel[] { model1, model2 });
		await _dbContext.SaveChangesAsync();

		// Act
		var result = async () => await _crud.Get(new HashSet<Key>() { new(nameof(TestModel.Id), 3) });

		// Assert
		await result.Should().ThrowAsync<ObjectNotFoundException>();
	}

    [Test]
	public async Task TryGet_should_return_null_when_object_cant_be_found()
	{
		// Arrange
		var model1 = new TestModel() { Id = 1, Name = "Test" };
		var model2 = new TestModel() { Id = 2, Name = "Test" };
		await _dbContext.AddRangeAsync(new TestModel[] { model1, model2 });
		await _dbContext.SaveChangesAsync();

        // Act
        var result = await _crud.TryGet(new HashSet<Key>() { new(nameof(TestModel.Id), 3) });

		// Assert
		result.Should().BeNull();
	}

    [Test]
    public async Task Get_should_return_multiple_object_when_multiple_primary_key_are_passed() 
    {
		// Arrange
		var model1 = new TestModel() { Id = 1, Name = "Test" };
		var model2 = new TestModel() { Id = 2, Name = "Test" };
		await _dbContext.AddRangeAsync(new TestModel[] { model1, model2 });
		await _dbContext.SaveChangesAsync();

		// Act
		var result = await _crud.Get(model1.GetPrimaryKey(), model2.GetPrimaryKey());

		// Assert
		result.Should().HaveCount(2)
            .And.ContainEquivalentOf(model1)
            .And.ContainEquivalentOf(model2);
	}

    [Test]
    public async Task TryGet_should_return_multiple_object_when_multiple_primary_key_are_passed() 
    {
		// Arrange
		var model1 = new TestModel() { Id = 1, Name = "Test" };
		var model2 = new TestModel() { Id = 2, Name = "Test" };
		await _dbContext.AddRangeAsync(new TestModel[] { model1, model2 });
		await _dbContext.SaveChangesAsync();

		// Act
		var result = await _crud.TryGet(model1.GetPrimaryKey(), model2.GetPrimaryKey());

		// Assert
		result.Should().HaveCount(2)
			.And.ContainEquivalentOf(model1)
			.And.ContainEquivalentOf(model2);
	}

	#endregion

	#region Update
	[Test]
	public async Task Update_single_object_properties_values()
	{
		// Arrange
		var model = new TestModel() { Id = 1, Name = "Test" };
		var changeTo = new TestModel() { Id = model.Id, Name = "Changed" };
		await _dbContext.AddAsync(model);
		await _dbContext.SaveChangesAsync();

		// Act
		await _crud.Update(changeTo);
		

		// Assert
		_dbContext.Set<TestModel>().Should().HaveCount(1)
			.And.ContainEquivalentOf(changeTo)
			.And.NotContainEquivalentOf(new TestModel { Id = 1, Name = "Test" });
	}

	[Test]
	public async Task Update_multiple_object_properties_values()
	{
		// Arrange
		var model1 = new TestModel() { Id = 1, Name = "Test" };
		var model2 = new TestModel() { Id = 2, Name = "Test" };
		var changeTo1 = new TestModel() { Id = model1.Id, Name = "Changed" };
		var changeTo2 = new TestModel() { Id = model2.Id, Name = "Changed" };
		await _dbContext.AddRangeAsync(model1, model2);
		await _dbContext.SaveChangesAsync();

		// Act
		await _crud.Update(changeTo1, changeTo2);

		// Assert
		_dbContext.Set<TestModel>().Should().HaveCount(2)
			.And.ContainEquivalentOf(changeTo1)
			.And.NotContainEquivalentOf(new TestModel() { Id = 1, Name = "Test" })
			.And.ContainEquivalentOf(changeTo2)
			.And.NotContainEquivalentOf(new TestModel() { Id = 2, Name = "Test" });
	}

	[Test]
	public async Task Update_throws_ObjectNotFoundException_when_object_does_not_exist_in_the_database()
	{
		// Arrange
		var model = new TestModel() { Id = 1, Name = "Test" };
		await _dbContext.AddAsync(model);
		await _dbContext.SaveChangesAsync();

		// Act
		var result = async () => await _crud.Update(new TestModel { Id = 2, Name = "Test" });


		// Assert
		await result.Should().ThrowAsync<ObjectNotFoundException>();
	}

	[Test]
	public async Task Update_returns_all_elemets_that_have_gotten_updated()
	{
		// Arrange
		var model1 = new TestModel() { Id = 1, Name = "Test" };
		var model2 = new TestModel() { Id = 2, Name = "Test" };
		var changeTo1 = new TestModel() { Id = model1.Id, Name = "Changed" };
		var changeTo2 = new TestModel() { Id = model2.Id, Name = "Changed" };
		await _dbContext.AddRangeAsync(model1, model2);
		await _dbContext.SaveChangesAsync();

		// Act
		var result = await _crud.Update(changeTo1, changeTo2);

		// Assert
		result.Should().BeEquivalentTo(new[] { changeTo1, changeTo2 });
	}



	[Test]
	public async Task Update_supports_reference_type_objects()
	{
		// Arrange
		var model1 = new TestModel() { Id = 1, Name = "Test", OtherId = 2 };
		var model2 = new TestModel() { Id = 2, Name = "Test", OtherId = 1 };
		var changeTo1 = new TestModel() { Id = model1.Id, Name = "Changed", OtherId = null };
		var changeTo2 = new TestModel() { Id = model2.Id, Name = "Changed", OtherId = null };
		await _dbContext.AddRangeAsync(model1, model2);
		await _dbContext.SaveChangesAsync();

		// Act
		await _crud.Update(changeTo1, changeTo2);

		// Assert
		_dbContext.Set<TestModel>().Should().HaveCount(2)
			.And.ContainEquivalentOf(changeTo1)
			.And.ContainEquivalentOf(changeTo2)
			.And.NotContainEquivalentOf(new TestModel { Id = model1.Id, Name = "Changed", OtherId = 2 })
			.And.NotContainEquivalentOf(new TestModel { Id = model2.Id, Name = "Changed", OtherId = 1 });
	}
	#endregion

	#region Delete

	[Test]
	public async Task Delete_removes_objects_from_database()
	{
		// Arrange
		var model1 = new TestModel() { Id = 1, Name = "Test" };
		var model2 = new TestModel() { Id = 2, Name = "Test" };
		await _dbContext.AddRangeAsync(model1, model2);
		await _dbContext.SaveChangesAsync();

		// Act
		await _crud.Delete(model1.GetPrimaryKey(), model2.GetPrimaryKey());

		// Assert
		_dbContext.Set<TestModel>().Should().BeEmpty();
	}

	[Test]
	public async Task Delete_removes_object_from_database()
	{
		// Arrange
		var model1 = new TestModel() { Id = 1, Name = "Test" };
		var model2 = new TestModel() { Id = 2, Name = "Test" };
		await _dbContext.AddRangeAsync(model1, model2);
		await _dbContext.SaveChangesAsync();

		// Act
		await _crud.Delete(model1.GetPrimaryKey());

		// Assert
		_dbContext.Set<TestModel>().Should().HaveCount(1)
			.And.NotContain(model1)
			.And.Contain(model2);
	}

	[Test]
	public async Task Delete_returns_true_if_item_has_been_succesfully_deleted()
	{
		// Arrange
		var model1 = new TestModel() { Id = 1, Name = "Test" };
		await _dbContext.AddAsync(model1);
		await _dbContext.SaveChangesAsync();

		// Act
		var result = await _crud.Delete(model1.GetPrimaryKey());

		// Assert
		result.Should().BeTrue();
	}

	[Test]
	public async Task Delete_returns_falase_if_item_has_not_been_succesfully_deleted()
	{
		// Arrange

		// Act
		var result = await _crud.Delete(new HashSet<Key> { new Key(name: nameof(TestModel.Id), value: 1) });

		// Assert
		result.Should().BeFalse();
	}

	#endregion
}

internal class TestContext : DbContext
{
    protected TestContext() { }
    public DbSet<TestModel> Models { get; set; }
    public TestContext(DbContextOptions options): base(options)
    {

    }
}

internal class TestModel : IModel
{
    [Key]
    public int? Id { get; set; }
    public string? Name { get; set; }
	[ForeignKey(nameof(Other))]
	public int? OtherId { get; set; }
	public TestModel? Other { get; set; }
	public TestModel() { }


    public void AutoIncrementPrimaryKey()
    => Id = null;

    public HashSet<Key> GetPrimaryKey()
    => new() { new(nameof(Id), Id) };

    public void SetPrimaryKey(HashSet<Key> primaryKey)
    => Id = (int?)primaryKey.Single(x => x.Name == nameof(Id)).Value;
}
