using System.IO;
using System.Reflection;

namespace Genguid.FactoryObservers
{
	/// <summary>
	/// A log for storing and retrieving previously generated GUIDs to and from a JSON format text
	/// file. This class cannot be inherited.
	/// </summary>
	public sealed class JsonFileLog : GuidGenerationLog
	{
		private const string logFileName = "log.json";

		private readonly IGuidGenerationLogReader reader;
		private readonly IGuidGenerationLogWriter writer;

		/// <summary>
		/// Creates a new log for storing and retrieving previously generated GUIDs to and from a
		/// JSON format text file.
		/// </summary>
		public JsonFileLog()
		{
			string filePath = this.GetLogFilePath();

			// Create a new, empty log file if one doesn't already exist
			if (!File.Exists(filePath))
			{
				this.CreateEmptyLogFile(filePath);
			}

			this.reader = new JsonFileLogReader(filePath);
			this.writer = new JsonFileLogWriter(filePath);
		}

		/// <summary>
		/// Gets a reader for retrieving previously generated GUIDs from the log.
		/// </summary>
		protected override IGuidGenerationLogReader Reader
		{
			get
			{
				return this.reader;
			}
		}

		/// <summary>
		/// Gets a writer for appending GUIDs to the log.
		/// </summary>
		protected override IGuidGenerationLogWriter Writer
		{
			get
			{
				return this.writer;
			}
		}

		private string GetLogFilePath()
		{
			string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			return Path.Combine(directoryPath, logFileName);
		}

		private void CreateEmptyLogFile(string logFilePath)
		{
			// Create a new empty file and immediately dispose of the resulting file stream
			File.Create(logFilePath).Dispose();
		}
	}
}
