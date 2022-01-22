using System.Configuration;

namespace Genguid.Configuration
{
	internal class FactoryObserversConfigurationSection : ConfigurationSection
	{
		[ConfigurationProperty("", IsDefaultCollection = true, IsRequired = true)]
		[ConfigurationCollection(typeof(FactoryObserverConfigurationElementCollection))]
		public FactoryObserverConfigurationElementCollection FactoryObservers
		{
			get
			{
				return (FactoryObserverConfigurationElementCollection)this[""];
			}
		}
	}
}
