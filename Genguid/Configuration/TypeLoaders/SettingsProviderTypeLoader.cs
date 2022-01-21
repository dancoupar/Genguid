namespace Genguid.Configuration.TypeLoaders
{
	/// <summary>
	/// The default loader for creating settings provider instances from their corresponding types.
	/// This class cannot be inherited.
	/// </summary>
	public sealed class SettingsProviderTypeLoader : TypeLoader<ISettingsProvider>
	{
		private static readonly object instanceLock = new();
		private static TypeLoader<ISettingsProvider> instance;

		private SettingsProviderTypeLoader()
		{
		}

		static SettingsProviderTypeLoader()
		{
			instance = new SettingsProviderTypeLoader();
		}

		/// <summary>
		/// Gets the current settings provider type loader for the app.
		/// </summary>
		public static TypeLoader<ISettingsProvider> Current
		{
			get
			{
				return instance;
			}
		}

		/// <summary>
		/// Changes the current settings provider type loader for the app to the one specified.
		/// </summary>
		/// <param name="typeLoader">The new settings provider type loader.</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public static void SetCurrent(TypeLoader<ISettingsProvider> typeLoader)
		{
			lock (instanceLock)
			{
				instance = typeLoader ?? throw new ArgumentNullException(nameof(typeLoader), "Argument cannot be null."); ;
			}
		}
	}
}
