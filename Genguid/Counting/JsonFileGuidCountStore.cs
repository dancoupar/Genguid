using System.IO;
using System.Reflection;

namespace Genguid.Counting
{
	/// <summary>
	/// A store for storing the current GUID count in a JSON format text file. This class cannot be
	/// inherited.
	/// </summary>
	internal sealed class JsonFileGuidCountStore : GuidCountStore
	{
		private readonly IGuidCountStoreReader reader;
		private readonly IGuidCountStoreWriter writer;

		private const string jsonFileName = "count.json";

		/// <summary>
		/// Creates a new instance of a store for storing the current GUID count in a JSON format
		/// text file.
		/// </summary>
		public JsonFileGuidCountStore()
		{
			string filePath = this.GetJsonFilePath();			
			this.writer = new JsonFileGuidCountStoreWriter(filePath);

			if (!File.Exists(filePath))
			{
				this.CreateNewJsonCountFile(filePath);
			}

			// Reader will throw exception if file doesn't exist
			this.reader = new JsonFileGuidCountStoreReader(filePath);
		}

		/// <summary>
		/// Gets the reader used to read the current count from the store.
		/// </summary>
		protected override IGuidCountStoreReader Reader
		{
			get
			{
				return this.reader;
			}
		}

		/// <summary>
		/// Gets the writer used to write the current count to the store.
		/// </summary>
		protected override IGuidCountStoreWriter Writer
		{
			get
			{
				return this.writer;
			}
		}

		private string GetJsonFilePath()
		{
			string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;			
			return Path.Combine(directoryPath, jsonFileName);
		}

		private void CreateNewJsonCountFile(string jsonFilePath)
		{
			// File creation itself is delegated to the writer
			this.writer.Write(0);
		}
	}
}
