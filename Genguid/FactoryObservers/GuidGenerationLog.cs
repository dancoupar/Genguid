namespace Genguid.FactoryObservers
{
	/// <summary>
	/// Describes a log for storing and retrieving previously generated GUIDs.
	/// </summary>
	public abstract class GuidGenerationLog : IGuidFactoryObserver
	{
		/// <summary>
		/// Gets a reader for retrieving previously generated GUIDs from the log.
		/// </summary>
		protected abstract IGuidGenerationLogReader Reader
		{
			get;
		}

		/// <summary>
		/// Gets a writer for appending GUIDs to the log.
		/// </summary>
		protected abstract IGuidGenerationLogWriter Writer
		{
			get;
		}

		/// <summary>
		/// Appends the specified GUID to the log.
		/// </summary>
		/// <param name="packet">
		/// A packet containing the information to append to the log.
		/// </param>
		internal void Append(GuidPacket packet)
		{
			this.Writer.Append(packet);
		}

		/// <summary>
		/// Fetches the previously generated GUID with the specified sequence number.
		/// </summary>
		/// <param name="sequenceNumber">
		/// The sequence number of the GUID to fetch.
		/// </param>
		public GuidPacket Fetch(long sequenceNumber)
		{
			return this.Reader.Fetch(sequenceNumber);
		}

		void IGuidFactoryObserver.NotifyOfGeneratedGuid(GuidPacket packet)
		{
			this.Append(packet);
		}
	}
}
