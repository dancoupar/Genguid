using Genguid.Configuration.TypeLoaders;
using Genguid.Factories;
using Genguid.FactoryObservers;
using Genguid.Formatters;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Genguid.Configuration
{
	/// <summary>
	/// Represents a user settings based settings provider.
	/// </summary>
	public class UserSettings : ISettingsProvider
	{
		private static readonly object settingsLock = new();

		private readonly ISettingsProvider defaultProvider;
		private GuidFactory factory = null!;
		private readonly IList<IGuidFactoryObserver> factoryObservers = null!;
		private GuidFormatter formatter = null!;

		/// <summary>
		/// Creates a new instance of a user settings based settings provider, based on the current
		/// provider.
		/// </summary>
		public UserSettings() : this(AppConfiguration.CurrentProvider)
		{
		}

		/// <summary>
		/// Creates a new instance of a user settings based settings provider, based on the
		/// provider specified.
		/// </summary>
		/// <param name="defaultSettingsProvider">
		/// The default settings provider from which the new instance should be based.
		/// </param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public UserSettings(ISettingsProvider defaultProvider)
		{
			this.defaultProvider = defaultProvider ?? throw new ArgumentNullException(nameof(defaultProvider), "Argument cannot be null.");
			this.factoryObservers = new List<IGuidFactoryObserver>();

			// Use the default values in the event these user settings are
			// missing any neccessary values.

			if (!this.HasValues())
			{
				this.CopyFrom(this.defaultProvider);
			}
			else
			{
				Type factoryType = this.ReadFactoryType()!;
				Type[] factoryObserverTypes = this.ReadFactoryObserverTypes()!;
				Type[] formatterTypes = this.ReadFormatterTypes()!;

				this.LoadFactory(factoryType);
				this.LoadFactoryObservers(factoryObserverTypes);
				this.LoadFormatters(formatterTypes);
			}
		}

		/// <summary>
		/// Gets the current GUID factory.
		/// </summary>
		public GuidFactory Factory
		{
			get
			{
				return this.factory;
			}
		}

		/// <summary>
		/// Gets the list of currently registered factory observers.
		/// </summary>
		public IReadOnlyList<IGuidFactoryObserver> FactoryObservers
		{
			get
			{
				return new ReadOnlyCollection<IGuidFactoryObserver>(this.factoryObservers);
			}
		}

		/// <summary>
		/// Gets the current GUID formatter. The formatter may decorate other formatters.
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
				// Generation log handling is delegated to the default provider
				return this.defaultProvider.GenerationLog;
			}
		}

		/// <summary>
		/// Reads the settings and returns the <see cref="System.Type"/> associated with the
		/// currently registered factory.
		/// </summary>
		public Type? ReadFactoryType()
		{
			string factoryTypeName = Genguid.Default.guidFactory;

			if (!String.IsNullOrEmpty(factoryTypeName))
			{
				return Type.GetType(factoryTypeName);
			}

			return null;
		}

		/// <summary>
		/// Reads the settings and returns the <see cref="System.Type"/> associated with the
		/// currently registered factory observers.
		/// </summary>
		public Type[]? ReadFactoryObserverTypes()
		{
			// A lock is required since we don't want
			// Genguid.Default.guidFormatters to change
			// during execution of this method.

			lock (settingsLock)
			{
				StringCollection factoryObservers = Genguid.Default.guidFactoryObservers;

				if (factoryObservers is not null)
				{
					Type?[] factoryObserverTypes = new Type[Genguid.Default.guidFactoryObservers.Count];
					int index = 0;

					foreach (string? factoryObserverTypeName in Genguid.Default.guidFactoryObservers)
					{
						factoryObserverTypes[index++] = Type.GetType(factoryObserverTypeName!);
					}

					return factoryObserverTypes!;
				}

				return null;
			}
		}

		/// <summary>
		/// Reads the settings and returns an array of <see cref="System.Type"/> objects associated
		/// with the currently registered formatters.
		/// </summary>
		public Type[]? ReadFormatterTypes()
		{
			// A lock is required since we don't want
			// Genguid.Default.guidFormatters to change
			// during execution of this method.

			lock (settingsLock)
			{
				StringCollection formatters = Genguid.Default.guidFormatters;

				if (formatters is not null)
				{
					Type?[] formatterTypes = new Type[Genguid.Default.guidFormatters.Count];
					int index = 0;

					foreach (string? formatterTypeName in Genguid.Default.guidFormatters)
					{
						formatterTypes[index++] = Type.GetType(formatterTypeName!);
					}

					return formatterTypes!;
				}

				return null;
			}
		}

		/// <summary>
		/// Returns the <see cref="System.Type"/> associated with the currently registered GUID
		/// generation log.
		/// </summary>
		public Type? ReadGenerationLogType()
		{
			// Generation log handling is delegated to the default provider.
			// No lock required.

			return this.defaultProvider.ReadGenerationLogType();
		}

		/// <summary>
		/// Registers the factory associated with the specified type, replacing the currently
		/// registered factory. If the type is not valid, an exception will be thrown.
		/// </summary>
		/// <param name="factoryType">
		/// The <see cref="System.Type"/> of the factory to register.
		/// </param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public void RegisterFactory(Type factoryType)
		{
			// A lock is required since we don't want
			// Genguid.Default.guidFactory to change
			// between modification and Save.

			lock (settingsLock)
			{
				if (factoryType is null)
				{
					throw new ArgumentNullException(nameof(factoryType), "Argument cannot be null.");
				}

				Genguid.Default.guidFactory = factoryType.AssemblyQualifiedName;
				Genguid.Default.Save();

				this.factory = FactoryTypeLoader.Current.Load(factoryType);
			}
		}

		/// <summary>
		/// Registers the factory observer associated with the specified type, appending to the
		/// list of currently registered factory observers. If the type is not valid, an exception
		/// will be thrown.
		/// </summary>
		/// <param name="factoryObserverType">
		/// The <see cref="System.Type"/> of the formatter to register.
		/// </param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public void RegisterFactoryObserver(Type factoryObserverType)
		{
			// A lock is required since we don't want
			// Genguid.Default.guidFactoryObservers to
			// change during execution of this method.

			lock (settingsLock)
			{
				if (factoryObserverType is null)
				{
					throw new ArgumentNullException(nameof(factoryObserverType), "Argument cannot be null.");
				}

				if (Genguid.Default.guidFactoryObservers is null || Genguid.Default.guidFactoryObservers.Count == 0)
				{
					Genguid.Default.guidFactoryObservers = new StringCollection();
				}

				Genguid.Default.guidFactoryObservers.Add(factoryObserverType.AssemblyQualifiedName);
				Genguid.Default.Save();

				this.LoadFactoryObservers(this.ReadFactoryObserverTypes()!);
			}
		}

		/// <summary>
		/// De-registers the formatter associated with the specified type, removing it from the
		/// list of currently registered formatters.
		/// </summary>
		/// <param name="factoryObserverType">
		/// The <see cref="System.Type"/> of the formatter to de-register.
		/// </param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public void DeregisterFactoryObserver(Type factoryObserverType)
		{
			// A lock is required since we don't want
			// Genguid.Default.guidFactoryObservers to
			// change during execution of this method.

			lock (settingsLock)
			{
				if (factoryObserverType is null)
				{
					throw new ArgumentNullException(nameof(factoryObserverType), "Argument cannot be null.");
				}

				Type[]? registeredFactoryObservers = this.ReadFactoryObserverTypes();

				if (registeredFactoryObservers is not null && registeredFactoryObservers.Contains(factoryObserverType))
				{
					// Clear all currently registered factory observers,
					// then re-register all except the one we want to
					// remove.

					ClearFactoryObservers();

					foreach (Type registeredFactoryObserverType in registeredFactoryObservers)
					{
						if (!registeredFactoryObserverType.Equals(factoryObserverType))
						{
							this.RegisterFactoryObserver(registeredFactoryObserverType);
						}
					}
				}

				Genguid.Default.Save();
			}
		}

		/// <summary>
		/// Registers the formatter associated with the specified type, appending to the list of
		/// currently registered formatters. If the type is not valid, an exception will be thrown.
		/// </summary>
		/// <param name="formatterType">
		/// The <see cref="System.Type"/> of the formatter to register.
		/// </param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public void RegisterFormatter(Type formatterType)
		{
			// A lock is required since we don't want
			// Genguid.Default.guidFormatters to change
			// during execution of this method.

			lock (settingsLock)
			{
				if (formatterType is null)
				{
					throw new ArgumentNullException(nameof(formatterType), "Argument cannot be null.");
				}

				if (Genguid.Default.guidFormatters is null || Genguid.Default.guidFormatters.Count == 0)
				{
					Genguid.Default.guidFormatters = new StringCollection();
				}

				Genguid.Default.guidFormatters.Add(formatterType.AssemblyQualifiedName);
				Genguid.Default.Save();

				this.formatter = FormatterTypeLoader.Current.Load(this.ReadFormatterTypes()!);
			}
		}

		/// <summary>
		/// De-registers the formatter associated with the specified type, removing it from the
		/// list of currently registered formatters.
		/// </summary>
		/// <param name="formatterType">
		/// The <see cref="System.Type"/> of the formatter to de-register.
		/// </param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public void DeregisterFormatter(Type formatterType)
		{
			// A lock is required since we don't want
			// Genguid.Default.guidFormatters to change
			// during execution of this method.

			lock (settingsLock)
			{
				if (formatterType is null)
				{
					throw new ArgumentNullException(nameof(formatterType), "Argument cannot be null.");
				}

				Type[]? registeredFormatters = this.ReadFormatterTypes();

				if (registeredFormatters is not null && registeredFormatters.Contains(formatterType))
				{
					// Clear all currently registered formatters, then
					// re-register all except the one we want to remove.

					ClearFormatters();

					foreach (Type registeredFormatterType in registeredFormatters)
					{
						if (!registeredFormatterType.Equals(formatterType))
						{
							this.RegisterFormatter(registeredFormatterType);
						}
					}
				}

				Genguid.Default.Save();
			}
		}

		/// <summary>
		/// Copies the settings provided by the specified provider onto this one.
		/// </summary>
		/// <param name="settingsProvider">
		/// The settings provider to copy settings from.
		/// </param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public void CopyFrom(ISettingsProvider settingsProvider)
		{
			if (settingsProvider is null)
			{
				throw new ArgumentNullException(nameof(settingsProvider), "Argument cannot be null.");
			}

			// A lock is required since we don't want
			// settings to change between modification
			// and Save.

			lock (settingsLock)
			{
				Type? factoryType = settingsProvider.ReadFactoryType();

				if (factoryType is not null)
				{
					this.RegisterFactory(factoryType);
				}

				ClearFactoryObservers();
				Type[]? factoryObserverTypes = settingsProvider.ReadFactoryObserverTypes();

				if (factoryObserverTypes is not null)
				{
					foreach (Type factoryObserverType in factoryObserverTypes)
					{
						this.RegisterFactoryObserver(factoryObserverType);
					}
				}

				ClearFormatters();
				Type[]? formatterTypes = settingsProvider.ReadFormatterTypes();

				if (formatterTypes is not null)
				{
					foreach (Type formatterType in formatterTypes)
					{
						this.RegisterFormatter(formatterType);
					}
				}

				Genguid.Default.Save();
			}
		}

		/// <summary>
		/// Gets a value indicating whether the user settings contain all necessary values.
		/// </summary>
		private bool HasValues()
		{
			Type? factoryType = this.ReadFactoryType();
			Type[]? factoryObserverTypes = this.ReadFactoryObserverTypes();
			Type[]? formatterTypes = this.ReadFormatterTypes();

			if (factoryType is not null
				&& factoryObserverTypes is not null
				&& factoryObserverTypes.Length > 0
				&& formatterTypes is not null
				&& formatterTypes.Length > 0)
			{
				return true;
			}

			return false;
		}

		private static void ClearFactoryObservers()
		{
			// A lock is required since we don't want
			// Genguid.Default.guidFactoryObservers to
			// change between modification and Save.

			lock (settingsLock)
			{
				// Clear the current list of formatters
				Genguid.Default.guidFactoryObservers = new StringCollection();
				Genguid.Default.Save();
			}
		}

		private static void ClearFormatters()
		{
			// A lock is required since we don't want
			// Genguid.Default.guidFormatters to change
			// between modification and Save.

			lock (settingsLock)
			{
				// Clear the current list of formatters
				Genguid.Default.guidFormatters = new StringCollection();
				Genguid.Default.Save();
			}
		}

		/// <summary>
		/// Resets the settings to their default state.
		/// </summary>
		public void Reset()
		{
			// A lock is required since we don't want
			// Genguid.Default changing between Reset
			// and CopyFrom.

			lock (settingsLock)
			{
				Genguid.Default.Reset();

				// There is no direct relationship between the
				// user settings (Genguid.settings) and the
				// default settings defined in the App.config
				// file, so need to reset the defaults from
				// App.config manually.

				this.CopyFrom(this.defaultProvider);
			}
		}

		private void LoadFactory(Type factoryType)
		{
			this.factory = FactoryTypeLoader.Current.Load(factoryType);
		}

		private void LoadFactoryObservers(Type[] factoryObserverTypes)
		{
			this.factoryObservers.Clear();

			foreach (Type factoryObserverType in factoryObserverTypes)
			{
				this.factoryObservers.Add(FactoryObserverTypeLoader.Current.Load(factoryObserverType));
			}
		}

		private void LoadFormatters(Type[] formatterTypes)
		{
			this.formatter = FormatterTypeLoader.Current.Load(formatterTypes);
		}
	}
}
