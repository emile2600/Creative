namespace Creative.Api.Interfaces;

public interface IUpdate<T>
{
    /// <summary> Updates object in the database. </summary>
    /// <param name="obj"> Object to update. </param>
    /// <returns> The updated object. </returns>
    Task<T> Update(T obj);

	/// <summary> Updates objects in the database. </summary>
	/// <param name="objs">The objects to update.</param>
	/// <returns>The updated objects.</returns>
	Task<T[]> Update(params T[] objs);
}
