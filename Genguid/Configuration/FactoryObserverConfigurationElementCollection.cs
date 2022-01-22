using System.Configuration;

namespace Genguid.Configuration
{
	[ConfigurationCollection(typeof(FactoryObserverConfigurationElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
	internal class FactoryObserverConfigurationElementCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new FactoryObserverConfigurationElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((FactoryObserverConfigurationElement)element).Name;
		}
	}
}
