namespace Creative.Api.Interfaces;

public interface ICreate<T>
{
    /// <summary> Adds object to the database. </summary>
    /// <exception> Throws exception when object cannot be added. </exception>
    /// <param name="AutoIncrementPrimaryKey"> Is true on default. The primary keys will be auto incremented. </param>
    /// <returns> The objects that have been successfully added. </returns>
    Task<T[]> Add(bool AutoIncrementPrimaryKey = true, params T[] objs);

    /// <summary> Adds object to the database. </summary>\
    /// <param name="AutoIncrementPrimaryKey"> Is true on default. The primary keys will be auto incremented. </param>
    /// <returns> The objects that have been successfully added or null when the objects could not be added. </returns>
    Task<T[]?> TryAdd(bool AutoIncrementPrimaryKey = true, params T[] objs);
}