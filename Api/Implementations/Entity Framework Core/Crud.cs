using Creative.Api.Data;
using Creative.Api.Exceptions;
using Creative.Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;

namespace Creative.Api.Implementations.EntityFrameworkCore
{
	/// <summary>Implementation of the Create Read Update Delete operations on a ef core database. </summary>
	/// <typeparam name="T">Object in the database.</typeparam>
	// TODO add a crud options to constructer to determine if primary keys get auto-incremented
	public class Crud<T> : ICrud<T> where T : class, IModel
	{
		/// <summary> The DbContext used for database operations. </summary>
		private DbContext DbContext { get; init; }

		/// <summary> Initializes a new instance of the <see cref="Crud{T}"/> class. </summary>
		/// <param name="dbContext">The DbContext used for database operations.</param>
		public Crud(DbContext dbContext) { DbContext = dbContext; }

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
				await DbContext.Set<T>().AddAsync(obj);
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
		/// <returns>All objects from the database.</returns>
		public async Task<T[]> GetAll()
		=> await DbContext.Set<T>().ToArrayAsync();

		/// <summary> Gets an object from the database by its primary key. </summary>
		/// <param name="key">The primary key of the object to get.</param>
		/// <returns>The object with the specified primary key, or null if it does not exist.</returns>
		public async Task<T?> TryGet(HashSet<Key> key)
		=> (await TryGet(new[] { key }))?.Single();

		/// <summary> Gets an object from the database by its primary key. </summary>
		/// <param name="key">The primary key of the object to get.</param>
		/// <returns>The object with the specified primary key.</returns>
		/// <exception cref="ObjectNotFoundException">Thrown if the object with the specified primary key does not exist.</exception>
		public async Task<T> Get(HashSet<Key> key)
		=> (await Get(new[] { key })).SingleOrDefault() ?? throw new ObjectNotFoundException();

		/// <summary> Gets objects from the database by their primary keys. </summary>
		/// <param name="keys">The primary keys of the objects to get.</param>
		/// <returns>The objects with the specified primary keys, or null if one does not exist.</returns>
		public async Task<T[]?> TryGet(params HashSet<Key>[] keys)
		{
			try
			{
				return await Get(keys);
			}
			catch (ObjectNotFoundException)
			{
				return null;
			}
		}

		/// <summary> Gets objects from the database by their primary keys. </summary>
		/// <param name="keys"> The primary keys of the objects to get. </param>
		/// <returns> The objects with the specified primary keys. </returns>
		/// <exception cref="ObjectNotFoundException"> Thrown if an object with one of the specified primary keys does not exist. </exception>
		public async Task<T[]> Get(params HashSet<Key>[] keys)
		=> await Task.WhenAll(keys.Select(async key => (await GetAll()).SingleOrDefault(obj => obj.GetPrimaryKey().Equals<Key>(key)) ?? throw new ObjectNotFoundException()));
		#endregion

		#region Update
		/// <inheritdoc/>
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
			var objs = await TryGet(keys);

			if (objs == null)
			{
				return false;
			}

			DbContext.Set<T>().RemoveRange(objs);
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