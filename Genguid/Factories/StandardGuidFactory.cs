namespace Genguid.Factories
{
	/// <summary>
	/// A factory for generating standard GUIDs. This class cannot be inherited.
	/// </summary>
	internal sealed class StandardGuidFactory : GuidFactory
	{
		/// <summary>
		/// Generates and returns a new standard GUID.
		/// </summary>
		protected override Guid Generate()
		{
			return Guid.NewGuid();
		}
	}
}
