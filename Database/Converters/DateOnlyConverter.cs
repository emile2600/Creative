using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Creative.Database.Converters
{
	/// <summary> Converts <see cref="DateOnly"/> to <seealso cref="DateTime"/> and the reverse. </summary>
	/// <remarks> The time when converting to <see cref="DateOnly"/>. </remarks>
	internal class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
	{
		public DateOnlyConverter() : base(d => d.ToDateTime(TimeOnly.MinValue), d => DateOnly.FromDateTime(d))
		{
		}
	}
}