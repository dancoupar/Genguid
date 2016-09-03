using System;

namespace Genguid.Formatters
{
	/// <summary>
	/// A decorator for a GUID formatter which can convert formatted GUIDs to upper case. This
	/// class cannot be inherited.
	/// </summary>
	internal sealed class UpperCaseGuidFormatter : GuidFormatterDecorator
	{
		/// <summary>
		/// Creates a new upper case GUID formatter, decorating and thereby extending the specified
		/// formatter.
		/// </summary>
		/// <param name="baseFormatter">
		/// The formatter to decorate.
		/// </param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public UpperCaseGuidFormatter(GuidFormatter baseFormatter) : base(baseFormatter)
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
			return this.BaseFormatter.Format(guid).ToUpper();
		}
	}
}
