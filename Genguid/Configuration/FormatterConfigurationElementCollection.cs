using System.Configuration;

namespace Genguid.Configuration
{
	[ConfigurationCollection(typeof(FormatterConfigurationElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
	internal class FormatterConfigurationElementCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new FormatterConfigurationElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((FormatterConfigurationElement)element).Name;
		}
	}
}
