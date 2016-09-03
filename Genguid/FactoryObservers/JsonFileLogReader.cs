using System;

namespace Genguid.FactoryObservers
{
	internal sealed class JsonFileLogReader : IGuidGenerationLogReader
	{
		private readonly string logFilePath;

		public JsonFileLogReader(string logFilePath)
		{
			if (logFilePath == null)
			{
				throw new ArgumentNullException(nameof(logFilePath), "Argument cannot be null.");
			}

			this.logFilePath = logFilePath;
		}

		public GuidPacket Fetch(long sequenceNumber)
		{
			return GuidPacket.NullPacket;
		}
	}
}
