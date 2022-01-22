using System.ComponentModel;
using System.Configuration;

namespace Genguid.Configuration
{
	internal class FactoryObserverConfigurationElement : ConfigurationElement
	{
		[ConfigurationProperty("name", IsRequired = true, IsKey = true)]
		public string Name
		{
			get
			{
				return (string)this["name"];
			}
		}

		[TypeConverter(typeof(TypeNameConverter))]
		[ConfigurationProperty("type", IsRequired = true)]
		public Type FactoryObserverType
		{
			get
			{
				return (Type)this["type"];
			}
		}
	}
}
