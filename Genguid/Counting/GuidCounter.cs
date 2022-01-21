namespace Genguid.Counting
{
	/// <summary>
	/// A counter for counting the number of GUIDs that have been previously generated. This class
	/// cannot be inherited.
	/// </summary>
	internal sealed class GuidCounter
	{
		private readonly GuidCountStore store;

		/// <summary>
		/// Creates a new GUID counter.
		/// </summary>
		/// <param name="store">A store for storing the current GUID count.</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public GuidCounter(GuidCountStore store)
		{
			this.store = store ?? throw new ArgumentNullException(nameof(store), "Argument cannot be null."); ;
		}

		/// <summary>
		/// Returns the current GUID count, i.e. the total number of GUIDs that have been previously
		/// generated.
		/// </summary>
		public long Count()
		{
			return this.store.Read();
		}

		/// <summary>
		/// Increments the counter.
		/// </summary>
		public void Increment()
		{
			this.store.Write(this.Count() + 1);
		}
	}
}
