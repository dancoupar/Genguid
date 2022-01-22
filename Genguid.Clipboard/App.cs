using Genguid.Configuration;
using Genguid.Configuration.TypeLoaders;
using Genguid.FactoryObservers;

namespace Genguid.Launcher
{
	public static class App
    {
		/// <summary>
		/// Occurs immediately before launch starts.
		/// </summary>
		private static event EventHandler? LaunchStarting;

		/// <summary>
		/// Occurs when launch completes.
		/// </summary>
		private static event EventHandler? LaunchCompleted;

		[STAThread]
		public static void Main()
		{
			App.Launch();

			// Generate a GUID!
			AppConfiguration.CurrentProvider.Factory.GenerateNextGuid();
		}

		/// <summary>
		/// Launches the app.
		/// </summary>
		public static void Launch()
		{
			LaunchStarting?.Invoke(null, EventArgs.Empty);
			
			SetSettingsProvider(SettingsProviderTypeLoader.Current);

			// Register the generation log as an observer of the factory
			AppConfiguration.CurrentProvider.Factory.RegisterObserver(AppConfiguration.CurrentProvider.GenerationLog);

			// Register any other configured observers
			foreach (IGuidFactoryObserver factoryObserver in AppConfiguration.CurrentProvider.FactoryObservers)
			{
				AppConfiguration.CurrentProvider.Factory.RegisterObserver(factoryObserver);
			}

			LaunchCompleted?.Invoke(null, EventArgs.Empty);			
		}

		private static void SetSettingsProvider(TypeLoader<ISettingsProvider> typeLoader)
		{
			AppConfiguration.SetCurrentProvider(typeLoader.Load(typeof(UserSettings)));
		}
	}
}
