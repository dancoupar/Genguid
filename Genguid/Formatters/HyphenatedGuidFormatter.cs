using System;

namespace Genguid.Formatters
{
	/// <summary>
	/// A decorator for a GUID formatter which can insert hyphens into formatted GUIDs. This class
	/// cannot be inherited.
	/// </summary>
	internal sealed class HyphenatedGuidFormatter : GuidFormatterDecorator
	{
		/// <summary>
		/// Creates a new hyphenated GUID formatter, decorating and thereby extending the specified
		/// formatter.
		/// </summary>
		/// <param name="baseFormatter">
		/// The formatter to decorate.
		/// </param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public HyphenatedGuidFormatter(GuidFormatter baseFormatter) : base(baseFormatter)
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
			return this.BaseFormatter.Format(guid).Insert(20, "-").Insert(16, "-").Insert(12, "-").Insert(8, "-");
		}
	}
}
