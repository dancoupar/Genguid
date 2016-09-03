using System;
using System.Collections.Generic;

namespace Genguid.Formatters
{
	/// <summary>
	/// Describes an object which can decorate a GUID formatter.
	/// </summary>
	public abstract class GuidFormatterDecorator : GuidFormatter
	{
		private readonly GuidFormatter baseFormatter;

		/// <summary>
		/// Creates a new instance of a GUID formatter decorator.
		/// </summary>
		/// <param name="baseFormatter">
		/// The formatter to decorate.
		/// </param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public GuidFormatterDecorator(GuidFormatter baseFormatter)
		{
			if (baseFormatter == null)
			{
				throw new ArgumentNullException(nameof(baseFormatter), "Argument cannot be null.");
			}

			this.baseFormatter = baseFormatter;
		}

		/// <summary>
		/// Gets a reference to the formatter being decorated.
		/// </summary>
		protected GuidFormatter BaseFormatter
		{
			get
			{
				return this.baseFormatter;
			}
		}
	}
}
