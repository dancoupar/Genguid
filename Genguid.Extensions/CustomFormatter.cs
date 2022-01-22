using Genguid.Formatters;

namespace Genguid.Extensions
{
	public class CustomFormatter : GuidFormatterDecorator
	{
		public CustomFormatter(GuidFormatter baseFormatter) : base(baseFormatter)
		{
		}

		public override string Format(Guid guid)
		{
			return this.BaseFormatter.Format(guid) + ".ABC";
		}
	}
}
