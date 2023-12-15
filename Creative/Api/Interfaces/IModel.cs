using Creative.Api.Data;

namespace Creative.Api.Interfaces;


/// <summary>
/// Interface for defining a model.
/// </summary>
public interface IModel
{
	/// <summary> Get the primary key of the object. </summary>
	public HashSet<Key> GetPrimaryKey();

	/// <summary> Set the primary key of the object. </summary>
	/// <remarks> Use tuple field names corresponding with primary key(s) name.</remarks>
	public void SetPrimaryKey(HashSet<Key> keys);

	/// <summary> Set auto-increment primary-key to null. </summary>
	/// <remarks> Set all auto-increment primary-keys to null here. </remarks>
	public void AutoIncrementPrimaryKey();
}
