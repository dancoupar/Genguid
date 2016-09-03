using System;
using System.IO;

namespace Genguid.Counting
{
	/// <summary>
	/// A writer for writing the current GUID count to a JSON format text file. This class cannot be
	/// inherited.
	/// </summary>
	internal sealed class JsonFileGuidCountStoreWriter : IGuidCountStoreWriter
	{
		private static readonly object writeLock = new object();

		private readonly string jsonFilePath;

		/// <summary>
		/// Creates a new writer for writing the current GUID count to a JSON format text file.
		/// </summary>
		/// <param name="jsonFilePath">The fully qualified path to the JSON file.</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public JsonFileGuidCountStoreWriter(string jsonFilePath)
		{
			if (jsonFilePath == null)
			{
				throw new ArgumentNullException(nameof(jsonFilePath), "Argument cannot be null.");
			}

			this.jsonFilePath = jsonFilePath;
		}

		/// <summary>
		/// Writes the current count to the store.
		/// </summary>
		/// <param name="count">The current GUID count.</param>
		public void Write(long count)
		{
			lock (writeLock)
			{
				try
				{
					using (StreamWriter streamWriter = this.GetStreamWriter())
					{
						streamWriter.WriteLine(count.ToString());
					}
				}
				catch (Exception e)
				{
					throw new ApplicationException("Error writing to count file. This is usually indicative of a corrupt count file.", e);
				}
			}
		}
		
		private StreamWriter GetStreamWriter()
		{
			FileStream fileStream = new FileStream(jsonFilePath, FileMode.OpenOrCreate, FileAccess.Write);
			StreamWriter streamWriter = new StreamWriter(fileStream);

			return streamWriter;
		}
	}
}
