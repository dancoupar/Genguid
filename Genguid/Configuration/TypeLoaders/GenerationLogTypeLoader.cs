using Genguid.FactoryObservers;

namespace Genguid.Configuration.TypeLoaders
{
	/// <summary>
	/// The default loader for creating GUID factory instances from their corresponding types.
	/// This class cannot be inherited.
	/// </summary>
	public sealed class GenerationLogTypeLoader : TypeLoader<GuidGenerationLog>
	{
		private readonly static TypeLoader<GuidGenerationLog> instance;

		private GenerationLogTypeLoader()
		{
		}

		static GenerationLogTypeLoader()
		{
			instance = new GenerationLogTypeLoader();
		}

		/// <summary>
		/// Gets the current generation log type loader for the app.
		/// </summary>
		public static TypeLoader<GuidGenerationLog> Current
		{
			get
			{
				return instance;
			}
		}
	}
}
