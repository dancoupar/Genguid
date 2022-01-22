using Newtonsoft.Json;
using System.IO;

namespace Genguid.FactoryObservers
{
	/// <summary>
	/// A writer for writing previously generated GUIDs to a JSON format text file. This calss
	/// cannot be inherited.
	/// </summary>
	internal sealed class JsonFileLogWriter : IGuidGenerationLogWriter
	{
		private static readonly object writeLock = new();

		private readonly string logFilePath;

		/// <summary>
		/// Creates a new instance of a JSON file based 
		/// </summary>
		/// <param name="logFilePath">A fully qualified path to the JSON log file.</param>
		public JsonFileLogWriter(string logFilePath)
		{
            this.logFilePath = logFilePath ?? throw new ArgumentNullException(nameof(logFilePath), "Argument cannot be null.");
		}

		/// <summary>
		/// Appends the specified GUID to the log.
		/// </summary>
		/// <param name="packet">A packet containing the information to append to the log.</param>
		public void Append(GuidPacket packet)
		{
			lock (writeLock)
			{
				try
				{
                    using StreamWriter streamWriter = this.CreateStreamWriter();
					FileStream fileStream = (FileStream)streamWriter.BaseStream;

                    if (fileStream.Length == 0)
                    {
                        // New file; open JSON array
                        streamWriter.WriteLine("[");
                    }
                    else
                    {
                        // Existing file; remove the trailing ] and the final line break
                        fileStream.SetLength(fileStream.Length - 2);

                        // Move position to end of stream and write comma in preparation for next element in JSON array
                        fileStream.Position = fileStream.Length - 1;
                        streamWriter.WriteLine(",");
                    }

                    // Create and write JSON for GUID being logged
                    streamWriter.WriteLine(CreateJson(packet));

                    // Close JSON array
                    streamWriter.Write("]");
                }
				catch (Exception e)
				{
					throw new ApplicationException("Error writing to log file. This is usually indicative of a corrupt log file.", e);
				}
			}
		}

		private StreamWriter CreateStreamWriter()
		{
			var fileStream = new FileStream(logFilePath, FileMode.OpenOrCreate, FileAccess.Write);
			var streamWriter = new StreamWriter(fileStream);

			return streamWriter;
		}

		private static string CreateJson(GuidPacket packet)
		{
			return JsonConvert.SerializeObject(
				new
				{
					n = packet.SequenceNumber,
					t = packet.TimeStamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
					g = packet.Value
				}
			);
		}
	}
}
