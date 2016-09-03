using Newtonsoft.Json;
using System;
using System.IO;

namespace Genguid.Counting
{
	/// <summary>
	/// A reader for reading the current GUID count from a JSON format text file. This class cannot
	/// be inherited.
	/// </summary>
	internal sealed class JsonFileGuidCountStoreReader : IGuidCountStoreReader
	{
		private readonly string jsonFilePath;

		/// <summary>
		/// Creates a new reader for reading the current GUID count from a JSON format text file.
		/// </summary>
		/// <param name="jsonFilePath">The fully qualified path to the JSON file.</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public JsonFileGuidCountStoreReader(string jsonFilePath)
		{
			if (jsonFilePath == null)
			{
				throw new ArgumentNullException(nameof(jsonFilePath), "Argument cannot be null.");
			}

			if (!File.Exists(jsonFilePath))
			{
				throw new FileNotFoundException(String.Format("The file {0} does not exist.", jsonFilePath));
			}

			this.jsonFilePath = jsonFilePath;
		}

		/// <summary>
		/// Reads the current count from the store.
		/// </summary>
		public long Read()
		{
			string json = null;

			using (StreamReader streamReader = this.CreateStreamReader())
			{
				json = streamReader.ReadLine();
			}

			return JsonConvert.DeserializeObject<long>(json);			
		}

		private StreamReader CreateStreamReader()
		{
			FileStream fileStream = new FileStream(this.jsonFilePath, FileMode.Open, FileAccess.Read);
			return new StreamReader(fileStream);
		}
	}
}
