namespace Genguid.FactoryObservers
{
	/// <summary>
	/// Describes an observer of a GUID factory which receives a notification each time the factory
	/// generates a new GUID.
	/// </summary>
	public interface IGuidFactoryObserver
	{
		/// <summary>
		/// Notifies the observer of a newly generated GUID.
		/// </summary>
		/// <param name="packet">A packet containing the information to append to the log.</param>
		void NotifyOfGeneratedGuid(GuidPacket packet);
	}
}
