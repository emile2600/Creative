using Creative.Api.Data;

namespace Creative.Api.Interfaces;

/// <summary> Interface for the Read operations on a database. </summary>
public interface IRead<T>
{
    /// <summary> Gets all <see cref="T"/> entities from the database. </summary>
	Task<T[]> GetAll() ;

	/// <summary> Gets specific <see cref="T"/> entity based on primary key. </summary>
    Task<T?> TryGet(HashSet<Key> key);

	/// <summary> Gets specific <see cref="T"/> entity based on primary key. </summary>
    Task<T> Get(HashSet<Key> key);

	/// <summary> Gets specific <see cref="T"/> entity based on primary key. </summary>
    Task<T[]?> TryGet(params HashSet<Key>[] keys);

	/// <summary> Gets specific <see cref="T"/> entity based on primary key. </summary>
    Task<T[]> Get(params HashSet<Key>[] keys);
}
