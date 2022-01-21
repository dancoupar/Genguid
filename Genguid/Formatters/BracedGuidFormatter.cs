namespace Genguid.Formatters
{
	/// <summary>
	/// A decorator for a GUID formatter which can augment formatted GUIDs with encompassing curly
	/// braces. This class cannot be inherited.
	/// </summary>
	internal sealed class BracedGuidFormatter : GuidFormatterDecorator
	{
		/// <summary>
		/// Creates a new braced GUID formatter, decorating and thereby extending the specified
		/// formatter.
		/// </summary>
		/// <param name="formatter">
		/// The formatter to decorate.
		/// </param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public BracedGuidFormatter(GuidFormatter formatter) : base(formatter)
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
			return String.Format("{{{0}}}", this.BaseFormatter.Format(guid));
		}
	}
}
