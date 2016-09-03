using System;

namespace Genguid.Formatters
{
	/// <summary>
	/// A decorator for a GUID formatter which can augment formatted GUIDs with encompassing
	/// parentheses. This class cannot be inherited.
	/// </summary>
	internal sealed class ParenthesisedGuidFormatter : GuidFormatterDecorator
	{
		/// <summary>
		/// Creates a new parenthesised GUID formatter, decorating and thereby extending the
		/// specified formatter.
		/// </summary>
		/// <param name="baseFormatter">
		/// The formatter to decorate.
		/// </param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public ParenthesisedGuidFormatter(GuidFormatter baseFormatter) : base(baseFormatter)
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
			return String.Format("({0})", this.BaseFormatter.Format(guid));
		}
	}
}
