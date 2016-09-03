namespace Genguid.Counting
{
	/// <summary>
	/// Describes a writer for writing the current GUID count to a store.
	/// </summary>
	internal interface IGuidCountStoreWriter
	{
		/// <summary>
		/// Writes the current count to the store.
		/// </summary>
		/// <param name="count">The current GUID count.</param>
		void Write(long count);
	}
}
