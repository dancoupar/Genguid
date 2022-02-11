using Newtonsoft.Json;

namespace Genguid.FactoryObservers
{
	/// <summary>
	/// A reader for retrieinvg previously generated GUIDs from a JSON format text file. This class
	/// cannot be inherited.
	/// </summary>
	internal sealed class JsonFileLogReader : IGuidGenerationLogReader
	{
		private static readonly object readLock = new();

		private readonly string logFilePath;
		private readonly IDictionary<long, GuidPacket> cache;

		/// <summary>
		/// Creates a new instance of a JSON file log reader.
		/// </summary>
		/// <param name="logFilePath">
		/// A fully qualified path to the JSON log file.
		/// </param>
		/// <exception cref="ArgumentNullException"></exception>
		public JsonFileLogReader(string logFilePath)
		{
			this.logFilePath = logFilePath ?? throw new ArgumentNullException(nameof(logFilePath), "Argument cannot be null.");
			this.cache = new Dictionary<long, GuidPacket>();
		}

		/// <summary>
		/// Retrieves a packet containing the most recently generated GUID and its associated
		/// meta-data from the log.
		/// </summary>
		public GuidPacket Fetch()
		{
			GuidPacket value = GuidPacket.NullPacket;

			lock (readLock)
			{
				this.PopulateCache();

				if (this.cache.Any())
				{
					value = this.cache[this.cache.Count];
				}
			}

			return value;
		}

		/// <summary>
		/// Retrieves a packet containing the specified GUID and its associated meta-data from the
		/// log.
		/// </summary>
		/// <param name="sequenceNumber">
		/// The sequence number of the GUID to be retrieved.
		/// </param>
		public GuidPacket Fetch(long sequenceNumber)
		{
			GuidPacket value = GuidPacket.NullPacket;

			lock (readLock)
			{
				if (!this.cache.ContainsKey(sequenceNumber))
				{
					this.PopulateCache();
				}

				this.cache.TryGetValue(sequenceNumber, out value);
			}

			return value;
		}

		private void PopulateCache()
		{
			this.cache.Clear();

			using StreamReader streamReader = this.CreateStreamReader();

			var serializer = new JsonSerializer();
			dynamic[]? jsonLog = (dynamic[]?)serializer.Deserialize(streamReader, typeof(dynamic[]));

			if (jsonLog is not null)
			{
				foreach (dynamic jsonLogEntry in jsonLog)
				{
					if (jsonLogEntry is null)
					{
						throw new ApplicationException("Unexpected null entry in log.");
					}

					long number = jsonLogEntry.n;
					Guid guid = jsonLogEntry.g;
					DateTimeOffset timestamp = jsonLogEntry.t;

					cache.Add(number, new GuidPacket(number, guid, timestamp));
				}
			}
		}

		private StreamReader CreateStreamReader()
		{
			var fileStream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read);
			var streamReader = new StreamReader(fileStream);
			
			return streamReader;
		}
	}
}
