namespace Genguid.Counting
{
	/// <summary>
	/// Describes a reader for reading the current GUID count from a store.
	/// </summary>
	internal interface IGuidCountStoreReader
	{
		/// <summary>
		/// Reads the current count from the store.
		/// </summary>
		long Read();
	}
}
