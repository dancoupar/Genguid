using System;

namespace Genguid.Configuration.TypeLoaders
{
	public class ConfigurationTypeLoadException : TypeLoadException
	{
		public ConfigurationTypeLoadException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
