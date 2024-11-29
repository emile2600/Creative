using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Creative.Database.Converters;

internal class TimeOnlyConverter : ValueConverter<TimeOnly, DateTime>
{
	public TimeOnlyConverter() : base(t => ToDateTime(t), t => TimeOnly.FromDateTime(t))
	{
	}

	/// <summary> Converts <see cref="TimeOnly"/> to <seealso cref="DateTime"/>. </summary>
	/// <remarks> Not tested.</remarks>
	private static DateTime ToDateTime(TimeOnly time) => DateTime.MinValue + time.ToTimeSpan();
}