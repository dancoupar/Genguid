using Genguid.Formatters;

namespace Genguid.Configuration.TypeLoaders
{
	/// <summary>
	/// Describes an object that can create GUID formatter instances from their corresponding
	/// types.
	/// </summary>
	public interface IFormatterTypeLoader
	{
		GuidFormatter Load(Type[] formatterTypes);
	}
}
