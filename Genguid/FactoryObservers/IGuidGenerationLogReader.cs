namespace Genguid.FactoryObservers
{
	/// <summary>
	/// Describes a reader for reading previously generated GUIDs from a log.
	/// </summary>
	public interface IGuidGenerationLogReader
	{
		/// <summary>
		/// Retrieves a packet containing the most recently generated GUID and its associated
		/// meta-data from the log.
		/// </summary>
		GuidPacket Fetch();

		/// <summary>
		/// Retrieves a packet containing the specified GUID and its associated meta-data from the
		/// log.
		/// </summary>
		/// <param name="sequenceNumber">
		/// The sequence number of the GUID to be retrieved.
		/// </param>
		GuidPacket Fetch(long sequenceNumber);
	}
}
