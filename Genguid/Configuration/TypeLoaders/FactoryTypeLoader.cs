using Genguid.Factories;

namespace Genguid.Configuration.TypeLoaders
{
	/// <summary>
	/// The default loader for creating GUID factory instances from their corresponding types.
	/// This class cannot be inherited.
	/// </summary>
	public sealed class FactoryTypeLoader : TypeLoader<GuidFactory>
	{
		private static readonly object instanceLock = new();
		private static TypeLoader<GuidFactory> instance;

		private FactoryTypeLoader()
		{
		}

		static FactoryTypeLoader()
		{
			instance = new FactoryTypeLoader();
		}

		/// <summary>
		/// Gets the current factory type loader for the app.
		/// </summary>
		public static TypeLoader<GuidFactory> Current
		{
			get
			{
				return instance;
			}
		}

		/// <summary>
		/// Changes the current factory type loader for the app to the one specified.
		/// </summary>
		/// <param name="typeLoader">The new factory type loader.</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public static void SetCurrent(TypeLoader<GuidFactory> typeLoader)
		{
			lock (instanceLock)
			{
				instance = typeLoader ?? throw new ArgumentNullException(nameof(typeLoader), "Argument cannot be null.");
			}
		}
	}
}
