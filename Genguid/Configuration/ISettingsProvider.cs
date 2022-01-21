using Genguid.Factories;
using Genguid.FactoryObservers;
using Genguid.Formatters;

namespace Genguid.Configuration
{
	/// <summary>
	/// Describes an object which provides configuration settings for the app.
	/// </summary>
	public interface ISettingsProvider
	{
		/// <summary>
		/// Gets the current GUID factory.
		/// </summary>
		GuidFactory Factory { get; }

		/// <summary>
		/// Gets the current GUID formatter. The formatter may decorate other formatters.
		/// </summary>
		GuidFormatter Formatter { get; }

		/// <summary>
		/// Gets the current log for storing previously generated GUIDs.
		/// </summary>
		GuidGenerationLog GenerationLog { get; }

		/// <summary>
		/// Returns the <see cref="System.Type"/> associated with the currently registered factory.
		/// </summary>
		Type? ReadFactoryType();

		/// <summary>
		/// Returns an array of <see cref="System.Type"/> objects associated with the currently
		/// registered formatters.
		/// </summary>
		Type[]? ReadFormatterTypes();

		/// <summary>
		/// Returns an array of <see cref="System.Type"/> objects associated with the currently
		/// registered generation log.
		/// </summary>
		Type? ReadGenerationLogType();

		/// <summary>
		/// Registers the factory associated with the specified type, replacing the currently
		/// registered factory. If the type is not valid, an exception will be thrown.
		/// </summary>
		/// <param name="factory">
		/// The <see cref="System.Type"/> of the factory to register.
		/// </param>
		/// <exception cref="System.ArgumentNullException"></exception>
		void RegisterFactory(Type factoryType);

		/// <summary>
		/// Registers the formatter associated with the specified type, appending to the list of
		/// currently registered formatters. If the type is not valid, an exception will be thrown.
		/// </summary>
		/// <param name="formatter">
		/// The <see cref="System.Type"/> of the formatter to register.
		/// </param>
		/// <exception cref="System.ArgumentNullException"></exception>
		void RegisterFormatter(Type formatterType);

		/// <summary>
		/// De-registers the formatter associated with the specified type, removing it from the
		/// list of currently registered formatters.
		/// </summary>
		/// <param name="formatter">
		/// The <see cref="System.Type"/> of the formatter to de-register.
		/// </param>
		/// <exception cref="System.ArgumentNullException"></exception>
		void DeregisterFormatter(Type formatterType);

		/// <summary>
		/// Copies the settings provided by the specified provider onto this one.
		/// </summary>
		/// <param name="settingsProvider">The settings provider to copy settings from.</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		void CopyFrom(ISettingsProvider settingsProvider);

		/// <summary>
		/// Resets the settings to their default state.
		/// </summary>
		void Reset();
	}
}
