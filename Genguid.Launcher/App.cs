using Genguid.Configuration;
using Genguid.Configuration.TypeLoaders;
using Genguid.FactoryObservers;
using Genguid.UI;
using System;
using System.Windows;

namespace Genguid.Launcher
{
	public static class App
    {
		private static event EventHandler launchStart;
		private static event EventHandler launchComplete;

		private static readonly object launchStartEventLock = new object();
		private static readonly object launchCompleteEventLock = new object();

		[STAThread]
		public static void Main(string[] args)
		{
			App.Launch();
		}

		/// <summary>
		/// Launches the app.
		/// </summary>
		public static void Launch()
		{
			lock (launchStartEventLock)
			{
				launchStart?.Invoke(null, EventArgs.Empty);
			}

			SetSettingsProvider(SettingsProviderTypeLoader.Current);

			// Register the generation log as an observer of the factory
			AppConfiguration.CurrentProvider.Factory.RegisterObserver(AppConfiguration.CurrentProvider.GenerationLog);
			
			Application wpfApp = new Application();
			wpfApp.Run(new MainWindow());

			lock (launchCompleteEventLock)
			{
				launchComplete?.Invoke(null, EventArgs.Empty);
			}
		}

		private static void SetSettingsProvider(TypeLoader<ISettingsProvider> typeLoader)
		{
			AppConfiguration.SetCurrentProvider(typeLoader.Load(typeof(UserSettings)));
		}

		/// <summary>
		/// Occurs immediately before launch starts.
		/// </summary>
		public static event EventHandler LaunchStart
		{
			add
			{
				lock (launchStartEventLock)
				{
					launchStart += value;
				}
			}
			remove
			{
				lock (launchStartEventLock)
				{
					launchStart -= value;
				}
			}
		}

		/// <summary>
		/// Occurs when launch completes.
		/// </summary>
		public static event EventHandler LaunchComplete
		{
			add
			{
				lock (launchCompleteEventLock)
				{
					launchComplete += value;
				}
			}
			remove
			{
				lock (launchCompleteEventLock)
				{
					launchComplete -= value;
				}
			}
		}
	}
}
