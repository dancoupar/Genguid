namespace Genguid.FactoryObservers
{
	/// <summary>
	/// Describes a writer for writing newly generated GUIDs to a log.
	/// </summary>
	public interface IGuidGenerationLogWriter
	{
		/// <summary>
		/// Appends the specified GUID to the log.
		/// </summary>
		/// <param name="packet">
		/// A packet containing the information to append to the log.
		/// </param>
		void Append(GuidPacket packet);
	}
}
