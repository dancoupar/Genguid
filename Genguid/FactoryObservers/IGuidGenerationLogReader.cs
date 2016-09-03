namespace Genguid.FactoryObservers
{
	public interface IGuidGenerationLogReader
	{
		GuidPacket Fetch(long sequenceNumber);
	}
}
