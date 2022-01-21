namespace Genguid.Counting
{
	/// <summary>
	/// Describes a store for the storing current GUID count.
	/// </summary>
	internal abstract class GuidCountStore
	{
		/// <summary>
		/// Gets the reader used to read the current count from the store.
		/// </summary>
		protected abstract IGuidCountStoreReader Reader
		{
			get;
		}

		/// <summary>
		/// Gets the writer used to write the current count to the store.
		/// </summary>
		protected abstract IGuidCountStoreWriter Writer
		{
			get;
		}

		/// <summary>
		/// Reads the current count from the store.
		/// </summary>
		public long Read()
		{
			return this.Reader.Read();
		}

		/// <summary>
		/// Writes the current count to the store.
		/// </summary>
		/// <param name="count">The current GUID count.</param>
		/// <exception cref="System.ArgumentOutOfRangeException"></exception>
		public void Write(long count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(count), count, "Argument must not be a negative value.");
			}

			this.Writer.Write(count);
		}
	}
}
