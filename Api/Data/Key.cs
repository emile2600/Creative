namespace Creative.Api.Data
{
	public struct Key
	{
		public Key(string name, object? value = null)
		{
			Name = name;
			Value = value;
		}
		public string Name { get; private set; }
		public object? Value { get; set; }
		public void SetNull() => Value = null;
	}
}