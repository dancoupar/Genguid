using Genguid.Configuration;
using Genguid.Configuration.TypeLoaders;
using Genguid.FactoryObservers;
using System;
using System.Windows;

namespace Genguid.UI
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		/// <summary>
		/// Occurs immediately before launch starts.
		/// </summary>
		private static event EventHandler? LaunchStarting;

		/// <summary>
		/// Occurs when launch completes.
		/// </summary>
		private static event EventHandler? LaunchCompleted;

		protected override void OnStartup(StartupEventArgs e)
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

			// Restore the most recently generated GUID
			AppConfiguration.CurrentProvider.Factory.Restore(AppConfiguration.CurrentProvider.GenerationLog);

			LaunchCompleted?.Invoke(null, EventArgs.Empty);

			base.OnStartup(e);
		}
		
		private static void SetSettingsProvider(TypeLoader<ISettingsProvider> typeLoader)
		{
			AppConfiguration.SetCurrentProvider(typeLoader.Load(typeof(UserSettings)));
		}
	}
}
