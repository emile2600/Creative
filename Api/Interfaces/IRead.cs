using Creative.Api.Data;

namespace Creative.Api.Interfaces
{
	/// <summary> Interface for the Read operations on a database. </summary>
	public interface IRead<T>
	{
		/// <summary> Gets all <see cref="T"/> entities. </summary>
		Task<T[]> Get();

		/// <summary> Gets specific <see cref="T"/> entity based on primary key. </summary>
		Task<T?> TryGet(HashSet<Key> key);

		/// <summary> Gets specific <see cref="T"/> entity based on primary key. </summary>
		Task<T> Get(HashSet<Key> key);

		/// <summary> Gets specific <see cref="T"/> entity based on primary key. </summary>
		/// <param name="filter">The function to filter the entities.</param>
		Task<T[]> Get(Func<T, bool> filter);
	}
}