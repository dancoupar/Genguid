using Genguid.FactoryObservers;
using System;
using System.Collections.Generic;

namespace Genguid.Factories
{
	/// <summary>
	/// An object responsible for dispatching new GUIDs to the relevant observers upon generation.
	/// </summary>
	internal class GuidDispatcher : IGuidFactoryObservable
	{
		private readonly ICollection<IGuidFactoryObserver> observers;

		/// <summary>
		/// Creates a new instance of a GUID dispatcher.
		/// </summary>
		public GuidDispatcher()
		{
			this.observers = new List<IGuidFactoryObserver>();
		}

		/// <summary>
		/// Registers the specified observer, to be notified when a new GUID is generated.
		/// </summary>
		/// <param name="observer">The observer to be notified.</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="System.InvalidOperationException"></exception>
		public void RegisterObserver(IGuidFactoryObserver observer)
		{
			if (observer == null)
			{
				throw new ArgumentNullException(nameof(observer), "Argument cannot be null.");
			}

			if (this.observers.Contains(observer))
			{
				throw new InvalidOperationException("The specified observer is already subscribed.");
			}

			this.observers.Add(observer);
		}

		/// <summary>
		/// Removes the specified observer, so that it will no longer be notified when a new GUID
		/// is generated.
		/// </summary>
		/// <param name="observer">The observer to remove.</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public void RemoveObserver(IGuidFactoryObserver observer)
		{
			if (observer == null)
			{
				throw new ArgumentNullException(nameof(observer), "Argument cannot be null.");
			}

			this.observers.Remove(observer);
		}

		/// <summary>
		/// Notifies all currently registered observers that a new GUID has been generated.
		/// </summary>
		/// <param name="packet">A packet containing the information to append to the log.</param>
		public void NotifyObservers(GuidPacket packet)
		{
			foreach (IGuidFactoryObserver observer in this.observers)
			{
				observer.NotifyOfGeneratedGuid(packet);
			}
		}
	}
}
