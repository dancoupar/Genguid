using Genguid.Configuration;
using Genguid.Formatters;
using System;

namespace Genguid.Extensions
{
	public class CustomFormatter : GuidFormatterDecorator
	{
		public CustomFormatter(GuidFormatter baseFormatter) : base(baseFormatter)
		{
			AppConfiguration.CurrentProvider.Factory.RegisterObserver(new CustomObserver());			
		}

		public override string Format(Guid guid)
		{
			return this.BaseFormatter.Format(guid) + ".ABC";
		}
	}
}
