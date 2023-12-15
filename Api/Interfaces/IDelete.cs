using Creative.Api.Data;

namespace Creative.Api.Interfaces
{
	public interface IDelete<T>
	{
		/// <summary> Delete object(s) with id. </summary>
		/// <param name="keys"> Object's primary key(s) to delete. </param>
		Task<bool> Delete(params HashSet<Key>[] keys);
	}
}