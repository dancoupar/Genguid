using System;

namespace Genguid.Formatters
{
	/// <summary>
	/// Describes an object which can apply formatting to a globally-unique identifier.
	/// </summary>
	public abstract class GuidFormatter
	{
		public const char TemplateChar = 'X';

		/// <summary>
		/// Gets a template string describing the format of all GUID strings produced by this
		/// formatter.
		/// </summary>
		public string TemplateString
		{
			get
			{
				return this.Format(Guid.Empty).Replace('0', TemplateChar);
			}
		}

		/// <summary>
		/// Gets the number of data digits (as opposed to enclosing and padding characters)
		/// contained in the strings produced by this formatter.
		/// </summary>
		public byte Digits
		{
			get
			{
				byte digits = 0;

				foreach (char c in this.TemplateString)
				{
					if (c == TemplateChar)
					{
						digits++;
					}
				}

				return digits;
			}
		}

		/// <summary>
		/// Applies formatting and returns the formatted GUID as a string.
		/// </summary>
		public abstract string Format(Guid guid);
	}
}
