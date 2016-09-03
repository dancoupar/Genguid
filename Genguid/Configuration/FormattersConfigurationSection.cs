using System.Configuration;

namespace Genguid.Configuration
{
	internal class FormattersConfigurationSection : ConfigurationSection
	{
		[ConfigurationProperty("", IsDefaultCollection = true, IsRequired = true)]
		[ConfigurationCollection(typeof(FormatterConfigurationElementCollection))]
		public FormatterConfigurationElementCollection Formatters
		{
			get
			{
				return (FormatterConfigurationElementCollection)this[""];
			}
		}
	}
}
