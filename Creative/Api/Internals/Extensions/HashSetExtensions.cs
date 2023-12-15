namespace System.Collections.Generic;

internal static class HashSetExtensions
{
	public static bool Equals<T>(this HashSet<T> current, HashSet<T> other) 
	{
		if (current.Count != other.Count) return false;
		foreach (var x in current)
		{
			if(!other.Contains(x)) return false;
		}
		return true;
	}
}
