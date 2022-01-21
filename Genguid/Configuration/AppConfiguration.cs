using Genguid.Configuration.TypeLoaders;
using Genguid.Factories;
using Genguid.FactoryObservers;
using Genguid.Formatters;
using System.Configuration;

namespace Genguid.Configuration
{
	/// <summary>
	/// Represents a config file based settings provider. This is the default settings provider.
	/// This class cannot be inherited.
	/// </summary>
	public sealed class AppConfiguration : ISettingsProvider
	{
		/// <summary>
		/// Occurs when the current settings provider for the app is changed.
		/// </summary>
		public static event EventHandler? SettingsProviderChanged;

		private static readonly object instanceLock = new();
		private static readonly object configLock = new();
		private static ISettingsProvider currentProvider;

		private const string guidFactoryConfigSection = "guidFactory";
		private const string guidFormattersConfigSection = "guidFormatters";
		private const string guidGenerationLogConfigSection = "guidGenerationLog";

		private GuidFactory factory = null!;
		private GuidFormatter formatter = null!;
		private GuidGenerationLog generationLog = null!;

		private Type factoryType = null!;
		private IList<Type> formatterTypes = null!;
		private Type generationLogType = null!;

		/// <summary>
		/// Creates a new instance of a config file based settings provider.
		/// </summary>
		private AppConfiguration()
		{
			this.Reset();
		}

		/// <summary>
		/// Creates a new app configuration singleton.
		/// </summary>
		static AppConfiguration()
		{
			currentProvider = new AppConfiguration();
		}

		/// <summary>
		/// Gets the current settings provider for the app.
		/// </summary>
		public static ISettingsProvider CurrentProvider
		{
			get
			{
				return currentProvider;
			}
		}

		/// <summary>
		/// Gets the current factory.
		/// </summary>
		public GuidFactory Factory
		{
			get
			{
				return this.factory;
			}
		}

		/// <summary>
		/// Gets the current formatter. The formatter may decorate other formatters.
		/// </summary>
		public GuidFormatter Formatter
		{
			get
			{
				return this.formatter;
			}
		}

		/// <summary>
		/// Gets the current log for storing previously generated GUIDs.
		/// </summary>
		public GuidGenerationLog GenerationLog
		{
			get
			{
				return this.generationLog;
			}
		}

		/// <summary>
		/// Changes the current settings provider for the app to the one specified.
		/// </summary>
		/// <param name="settingsProvider">The new settings provider.</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public static void SetCurrentProvider(ISettingsProvider settingsProvider)
		{
			if (settingsProvider is null)
			{
				throw new ArgumentNullException(nameof(settingsProvider), "Argument cannot be null.");
			}

			lock (instanceLock)
			{
				currentProvider = settingsProvider;

				// Trigger the settings provider changed event if there are any registered handlers
				SettingsProviderChanged?.Invoke(settingsProvider, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Returns the <see cref="System.Type"/> representing the default factory for this
		/// settings provider.
		/// </summary>
		public Type ReadFactoryType()
		{
			lock (configLock)
			{
				FactoryConfigurationSection factoryConfig = (FactoryConfigurationSection)ConfigurationManager.GetSection(guidFactoryConfigSection);

				if (factoryConfig is null)
				{
					throw new ApplicationException(String.Format("There is no {0} section in app.config.", guidFactoryConfigSection));
				}

				return factoryConfig.FactoryType;
			}
		}

		/// <summary>
		/// Returns an array of <see cref="System.Type"/> objects representing the default
		/// formatters for this settings provider.
		/// </summary>
		public Type[] ReadFormatterTypes()
		{
			lock (configLock)
			{
				FormattersConfigurationSection formattersConfig = (FormattersConfigurationSection)ConfigurationManager.GetSection(guidFormattersConfigSection);

				if (formattersConfig is null)
				{
					throw new ApplicationException(String.Format("There is no {0} section in app.config.", guidFormattersConfigSection));
				}

				Type[] formatterTypes = new Type[formattersConfig.Formatters.Count];
				int index = 0;

				foreach (FormatterConfigurationElement configElement in formattersConfig.Formatters)
				{
					formatterTypes[index++] = configElement.FormatterType;
				}

				return formatterTypes;
			}
		}

		/// <summary>
		/// Returns the <see cref="System.Type"/> representing the default generation log for this
		/// this settings provider.
		/// </summary>
		public Type ReadGenerationLogType()
		{
			lock (configLock)
			{
				GenerationLogConfigurationSection generationLogConfig = (GenerationLogConfigurationSection)ConfigurationManager.GetSection(guidGenerationLogConfigSection);

				if (generationLogConfig is null)
				{
					throw new ApplicationException(String.Format("There is no {0} section in app.config.", guidGenerationLogConfigSection));
				}

				return generationLogConfig.GenerationLogType;
			}
		}

		public void RegisterFactory(Type factoryType)
		{
			lock (configLock)
			{
				if (factoryType is null)
				{
					throw new ArgumentNullException(nameof(factoryType), "Argument cannot be null.");
				}

				this.LoadFactory();
			}
		}

		public void RegisterFormatter(Type formatterType)
		{
			lock (configLock)
			{
				if (formatterType is null)
				{
					throw new ArgumentNullException(nameof(formatterType), "Argument cannot be null.");
				}
								
				this.formatterTypes.Add(formatterType);
				this.LoadFormatters();
			}			
		}

		public void DeregisterFormatter(Type formatterType)
		{
			lock (configLock)
			{
				if (formatterType is null)
				{
					throw new ArgumentNullException(nameof(formatterType), "Argument cannot be null.");
				}
				
				this.formatterTypes.Remove(formatterType);
				this.LoadFormatters();
			}			
		}

		public void RegisterGenerationLog(Type generationLogType)
		{
			lock (configLock)
			{
                this.generationLogType = generationLogType ?? throw new ArgumentNullException(nameof(generationLogType), "Argument cannot be null.");
				this.LoadGenerationLog();
			}
		}

		/// <summary>
		/// Copies the settings provided by the specified provider onto this one.
		/// </summary>
		/// <param name="settingsProvider">The settings provider to copy settings from.</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public void CopyFrom(ISettingsProvider settings)
		{
			lock (configLock)
			{
				if (settings is null)
				{
					throw new ArgumentNullException(nameof(settings), "Argument cannot be null.");
				}

				this.RegisterFactory(settings.ReadFactoryType()!);

				// Clear all registered formatter types and re-register individually
				this.formatterTypes.Clear();
				
				foreach (Type formatterType in settings.ReadFormatterTypes()!)
				{
					this.RegisterFormatter(formatterType);
				}

				this.RegisterGenerationLog(settings.ReadGenerationLogType()!);
			}
		}

		/// <summary>
		/// Resets the settings to their default state.
		/// </summary>
		/// <exception cref="System.ApplicationException"></exception>
		public void Reset()
		{
			lock (configLock)
			{
				try
				{
					this.factoryType = this.ReadFactoryType();
					this.LoadFactory();
				}
				catch (ConfigurationTypeLoadException e)
				{
					throw new ApplicationException(String.Format("There is a misconfiguration in the {0} section of the App.config file.", guidFactoryConfigSection), e);
				}

				try
				{
					this.formatterTypes = this.ReadFormatterTypes().ToList();
					this.LoadFormatters();
				}
				catch (ConfigurationTypeLoadException e)
				{
					throw new ApplicationException(String.Format("There is a misconfiguration in the {0} section of the App.config file.", guidFormattersConfigSection), e);
				}

				try
				{
					this.generationLogType = this.ReadGenerationLogType();
					this.LoadGenerationLog();
				}
				catch (ConfigurationTypeLoadException e)
				{
					throw new ApplicationException(String.Format("There is a misconfiguration in the {0} section of the App.config file.", guidGenerationLogConfigSection), e);
				}
			}
		}

		private void LoadFactory()
		{
			this.factory = FactoryTypeLoader.Current.Load(factoryType);
		}

		private void LoadFormatters()
		{
			this.formatter = FormatterTypeLoader.Current.Load(this.formatterTypes.ToArray());
		}

		private void LoadGenerationLog()
		{
			this.generationLog = GenerationLogTypeLoader.Current.Load(this.generationLogType);
		}
	}
}
