﻿using Genguid.Configuration;
using Genguid.Configuration.TypeLoaders;
using Genguid.UI;
using System.Windows;

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
			
			var wpfApp = new Application();
			wpfApp.Run(new MainWindow());
			
			LaunchCompleted?.Invoke(null, EventArgs.Empty);			
		}

		private static void SetSettingsProvider(TypeLoader<ISettingsProvider> typeLoader)
		{
			AppConfiguration.SetCurrentProvider(typeLoader.Load(typeof(UserSettings)));
		}
	}
}
