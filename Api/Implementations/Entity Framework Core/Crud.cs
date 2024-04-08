using Creative.Api.Data;
using Creative.Api.Exceptions;
using Creative.Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Creative.Api.Implementations.EntityFrameworkCore
{
	/// <summary>Implementation of the Create Read Update Delete operations on a ef core database. </summary>
	/// <typeparam name="T">Object in the database.</typeparam>
	public class Crud<T> : ICrud<T> where T : class, IModel
	{
		/// <summary> The DbContext used for database operations. </summary>
		private DbContext DbContext { get; init; }

		private DbSet<T> DbSet { get; init; }

		private IIncludableQueryable<T, object>? EagerLoadedSet { get; init; }

		/// <summary> Initializes a new instance of the <see cref="Crud{T}"/> class. </summary>
		/// <param name="dbContext">The DbContext used for database operations.</param>
		public Crud(DbContext dbContext, IIncludableQueryable<T, object>? eagerLoad = null) 
		{  
			DbContext = dbContext;
			DbSet = DbContext.Set<T>();
			EagerLoadedSet = eagerLoad;
		}

		#region Create
		/// <summary> Adds one or more objects to the database. </summary>
		/// <param name="autoIncrementPrimaryKey">Whether to auto-increment the primary key of the added objects.</param>
		/// <param name="objs">The objects to add to the database.</param>
		/// <returns>The added objects.</returns>
		public async Task<T[]> Add(bool autoIncrementPrimaryKey = true, params T[] objs)
		{
			foreach (var obj in objs)
			{
				if (autoIncrementPrimaryKey) obj.AutoIncrementPrimaryKey();
				await DbSet.AddAsync(obj);
			}
			await DbContext.SaveChangesAsync();
			return objs!;
		}

		/// <summary> Attempts to add one or more objects to the database. </summary>
		/// <param name="autoIncrementPrimaryKey">Whether to auto-increment the primary key of the added objects.</param>
		/// <param name="objs">The objects to add to the database.</param>
		/// <returns>The added objects, or null if an exception occurred.</returns>
		public async Task<T[]?> TryAdd(bool autoIncrementPrimaryKey = true, params T[] objs)
		{
			try
			{
				return await Add(autoIncrementPrimaryKey, objs);
			}
			catch (ArgumentException)
			{
				return null;
			}
		}
		#endregion

		#region Read

		/// <summary> Gets all objects from the database. </summary>
		public async Task<T[]> Get() => EagerLoadedSet is not null ? await EagerLoadedSet.ToArrayAsync() : await DbSet.ToArrayAsync();

		/// <summary> Gets an object from the database by its primary key. </summary>
		/// <param name="key">The primary key of the object to get.</param>
		/// <returns>The object with the specified primary key, or null if it does not exist.</returns>
		public async Task<T?> TryGet(HashSet<Key> key)
		{
			try
			{
				return await Get(key);
			}
			catch(ObjectNotFoundException)
			{
				return null;
			}
		}

		/// <summary> Gets an object from the database by its primary key. </summary>
		/// <param name="key">The primary key of the object to get.</param>
		/// <returns>The object with the specified primary key.</returns>
		/// <exception cref="ObjectNotFoundException">Thrown if the object with the specified primary key does not exist.</exception>
		public async Task<T> Get(HashSet<Key> key)
		=> (await Get(obj => obj.GetPrimaryKey().SetEquals(key))).SingleOrDefault() ?? throw new ObjectNotFoundException();

		/// <summary> Gets objects from the database by their primary keys. </summary>
		/// <param name="filter">The function to filter the entities.</param>
		/// <returns> The objects that fit the filter criteria. </returns>
		public async Task<T[]> Get(Func<T, bool> filter)
		=> await Task.FromResult(EagerLoadedSet is not null ? EagerLoadedSet.Where(filter).ToArray() : DbSet.Where(filter).ToArray());
		#endregion

		#region Update
		/// <inhertidoc/>
		public async Task<T[]> Update(params T[] objs)
		=> await Task.WhenAll(objs.Select(async obj => await Update(obj)));

		/// <inheritdoc/>
		public async Task<T> Update(T obj)
		{
			var orignalObj = await Get(obj.GetPrimaryKey());
			foreach (var property in obj.GetType().GetProperties())
			{
				// Skip properties that are not mapped to the database
				if (Attribute.IsDefined(property, typeof(NotMappedAttribute))) continue;
				try
				{
					// If the property is a value, update the value (think of int's , string's, etc.)
					DbContext.Entry(orignalObj).Property(property.Name).CurrentValue = property.GetValue(obj);
				}
				catch (InvalidOperationException)
				{
					try
					{
						// If the property is a reference, update the reference (think of objects)
						DbContext.Entry(orignalObj).Reference(property.Name).CurrentValue = property.GetValue(obj);
					}
					catch(InvalidOperationException)
					{
						// If the property is a collection, update the collection (think of lists, arrays, etc.)
						DbContext.Entry(orignalObj).Collection(property.Name).CurrentValue = (IEnumerable?)property.GetValue(obj);
					}
				}
			}
			await DbContext.SaveChangesAsync();
			return await Get(obj.GetPrimaryKey());
		}
		#endregion

		#region Delete
		/// <summary> Deletes objects from the database by their primary keys. </summary>
		/// <param name="keys">The primary keys of the objects to delete.</param>
		/// <returns>True if the objects were deleted successfully, false otherwise.</returns>
		/// <summary>
		/// Deletes the entities with the specified keys from the database.
		/// </summary>
		/// <param name="keys">The keys of the entities to delete.</param>
		/// <returns>A boolean indicating whether the deletion was successful.</returns>
		public async Task<bool> Delete(params HashSet<Key>[] keys)
		{
			var objs = await Task.WhenAll(keys.Select(TryGet));

			if (objs is null || objs.Any(obj => obj is null))
			{
				return false;
			}

			DbSet.RemoveRange(objs!);
			try
			{
				await DbContext.SaveChangesAsync();
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}
		#endregion
	}
}