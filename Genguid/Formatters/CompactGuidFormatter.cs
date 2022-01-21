namespace Genguid.Formatters
{
	/// <summary>
	/// Represents a formatter which can format GUIDs as lowercase strings containing only those
	/// characters which represent the value of the GUID. This class cannot be inherited.
	/// </summary>
	internal sealed class CompactGuidFormatter : GuidFormatter
	{
		/// <summary>
		/// Creates a new compact GUID formatter.
		/// </summary>
		public CompactGuidFormatter()
		{
		}

		/// <summary>
		/// Applies formatting and returns the formatted GUID as a string.
		/// </summary>
		/// <param name="guid">
		/// The GUID to be formatted.
		/// </param>
		public override string Format(Guid guid)
		{
			return guid.ToString("N").ToLower();
		}
	}
}
