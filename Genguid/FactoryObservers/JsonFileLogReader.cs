namespace Genguid.FactoryObservers
{
	internal sealed class JsonFileLogReader : IGuidGenerationLogReader
	{
		private readonly string logFilePath;

		public JsonFileLogReader(string logFilePath)
		{
			this.logFilePath = logFilePath ?? throw new ArgumentNullException(nameof(logFilePath), "Argument cannot be null."); ;
		}

		public GuidPacket Fetch(long sequenceNumber)
		{
			return GuidPacket.NullPacket;
		}
	}
}
