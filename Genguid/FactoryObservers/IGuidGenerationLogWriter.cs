namespace Genguid.FactoryObservers
{
	public interface IGuidGenerationLogWriter
	{
		/// <summary>
		/// Appends the specified GUID to the log.
		/// </summary>
		/// <param name="packet">A packet containing the information to append to the log.</param>
		void Append(GuidPacket packet);
	}
}
