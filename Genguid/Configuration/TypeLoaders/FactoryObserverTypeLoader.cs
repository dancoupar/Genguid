using Genguid.FactoryObservers;

namespace Genguid.Configuration.TypeLoaders
{
	/// <summary>
	/// The default loader for creating GUID factory observer instances from their corresponding
	/// types. This class cannot be inherited.
	/// </summary>
	public sealed class FactoryObserverTypeLoader : TypeLoader<IGuidFactoryObserver>
	{
		private static readonly object instanceLock = new();
		private static TypeLoader<IGuidFactoryObserver> instance;

		private FactoryObserverTypeLoader()
		{
		}

		static FactoryObserverTypeLoader()
		{
			instance = new FactoryObserverTypeLoader();
		}

		/// <summary>
		/// Gets the current factory observer type loader for the app.
		/// </summary>
		public static TypeLoader<IGuidFactoryObserver> Current
		{
			get
			{
				return instance;
			}
		}

		/// <summary>
		/// Changes the current factory observer type loader for the app to the one specified.
		/// </summary>
		/// <param name="typeLoader">The new factory observer type loader.</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public static void SetCurrent(TypeLoader<IGuidFactoryObserver> typeLoader)
		{
			lock (instanceLock)
			{
				instance = typeLoader ?? throw new ArgumentNullException(nameof(typeLoader), "Argument cannot be null.");
			}
		}
	}
}
