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
			this.baseFormatter = baseFormatter ?? throw new ArgumentNullException(nameof(baseFormatter), "Argument cannot be null."); ;
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
