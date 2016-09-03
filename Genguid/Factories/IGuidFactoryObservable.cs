using Genguid.FactoryObservers;

namespace Genguid.Factories
{
	/// <summary>
	/// Describes a GUID factory to which observers can subscribe. Observers are notified of any
	/// new GUIDs the factory generates.
	/// </summary>
	internal interface IGuidFactoryObservable
	{
		/// <summary>
		/// Registers the specified observer, to be notified when a new GUID is generated.
		/// </summary>
		/// <param name="observer">The observer to be notified.</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="System.InvalidOperationException"></exception>
		void RegisterObserver(IGuidFactoryObserver observer);

		/// <summary>
		/// Removes the specified observer, so that it will no longer be notified when a new GUID
		/// is generated.
		/// </summary>
		/// <param name="observer">The observer to remove.</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		void RemoveObserver(IGuidFactoryObserver observer);
	}
}
