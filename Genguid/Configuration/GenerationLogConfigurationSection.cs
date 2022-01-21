using System.ComponentModel;
using System.Configuration;

namespace Genguid.Configuration
{
	internal class GenerationLogConfigurationSection : ConfigurationSection
	{
		[ConfigurationProperty("name", IsRequired = true, IsKey = true)]
		public string Name
		{
			get
			{
				return (string)this["name"];
			}
			set
			{
				this["name"] = value;
			}
		}

		[TypeConverter(typeof(TypeNameConverter))]
		[ConfigurationProperty("type", IsRequired = true)]
		public Type GenerationLogType
		{
			get
			{
				return (Type)this["type"];
			}
			set
			{
				this["type"] = value;
			}
		}
	}
}
