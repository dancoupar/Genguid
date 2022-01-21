using Genguid.Configuration;

namespace Genguid
{
	/// <summary>
	/// A structure representing a generated GUID and its associated meta-data.
	/// </summary>
	public struct GuidPacket
	{
		private readonly long sequenceNumber;
		private readonly Guid value;
		private readonly DateTimeOffset timestamp;

		/// <summary>
		/// Returns a non-initialised GUID packet representing the absence of a GUID.
		/// </summary>
		public static readonly GuidPacket NullPacket = new();
		
		/// <summary>
		/// Creates a new GUID packet with the specified sequence number and value.
		/// </summary>
		/// <param name="sequenceNumber">The sequence number of the GUID.</param>
		/// <param name="value">The GUID itself.</param>
		/// <exception cref="System.ArgumentOutOfRangeException"></exception> 
		/// <exception cref="System.ArgumentException"></exception>
		public GuidPacket(long sequenceNumber, Guid value)
		{
			if (sequenceNumber < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(sequenceNumber), sequenceNumber, "Argument must be greater than 0.");
			}

			if (value == default)
			{
				throw new ArgumentException("Argument has not been initialised.", nameof(value));
			}

			this.sequenceNumber = sequenceNumber;
			this.value = value;
			this.timestamp = DateTimeOffset.Now;
		}

		/// <summary>
		/// Gets the sequence number of the GUID.
		/// </summary>
		public long SequenceNumber
		{
			get
			{
				return this.sequenceNumber;
			}
		}

		/// <summary>
		/// Gets the GUID itself.
		/// </summary>
		public Guid Value
		{
			get
			{
				return this.value;
			}
		}

		/// <summary>
		/// Gets a string representation of the GUID, formatted in accordance with the currently configured
		/// formatter(s).
		/// </summary>
		public string FormattedValue
		{
			get
			{
				return AppConfiguration.CurrentProvider.Formatter.Format(value);
			}
		}

		/// <summary>
		/// Gets the date and time the GUID was generated.
		/// </summary>
		public DateTimeOffset TimeStamp
		{
			get
			{
				return this.timestamp;
			}
		}

		/// <summary>
		/// Gets a string representation of the GUID, formatted in accordance with the currently configured
		/// formatter(s).
		/// </summary>
		public override string ToString()
		{
			return this.FormattedValue;
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		public override int GetHashCode()
		{
			return this.value.GetHashCode();
		}

		/// <summary>
		/// Returns a value indicating whether this instance and the specified GuidPacket object are
		/// equivalent.
		/// </summary>
		/// <param name="obj">An object to compare to this instance.</param>
		public override bool Equals(object? obj)
		{
			if (obj is GuidPacket packet)
			{
				return
					packet.sequenceNumber.Equals(this.sequenceNumber) &&
					packet.value.Equals(this.value) &&
					packet.timestamp.Equals(this.timestamp);
			}

			return false;
		}

		public static bool operator ==(GuidPacket left, GuidPacket right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(GuidPacket left, GuidPacket right)
		{
			return !(left == right);
		}
	}
}
