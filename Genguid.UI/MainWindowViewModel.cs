using Genguid.Configuration;
using Genguid.Factories;
using Genguid.Formatters;
using Prism.Commands;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Genguid.UI
{
	internal class MainWindowViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		private string currentGuid;
		private long sequenceNumber;
		private string timestamp;
		private readonly ICommand previousCommand;
		private readonly ICommand nextCommand;
		
		private readonly object currentGuidLock = new object();

		public MainWindowViewModel()
		{
			this.currentGuid = Guid.Empty.ToString("b");
			this.timestamp = string.Empty;
			this.previousCommand = new DelegateCommand(this.OnPrevious);
			this.nextCommand = new DelegateCommand(this.OnNext);
			
			this.SetGuid(AppConfiguration.CurrentProvider.Factory.CurrentGuid, showTimestamp: true);
		}

		private static GuidFactory Factory
		{
			get
			{
				return AppConfiguration.CurrentProvider.Factory;
			}
		}

		private static GuidFormatter Formatter
		{
			get
			{
				return AppConfiguration.CurrentProvider.Formatter;
			}
		}

		public long SequenceNumber
		{
			get
			{
				return this.sequenceNumber;
			}
		}

		public string CurrentGuid
		{
			get
			{
				return this.currentGuid;
			}
		}	
		
		public string Timestamp
		{
			get
			{
				return this.timestamp;
			}
		}

		public ICommand PreviousButtonClickCommand
		{
			get
			{
				return this.previousCommand;
			}
		}

		public ICommand NextButtonClickCommand
		{
			get
			{
				return this.nextCommand;
			}
		}

		private void OnPrevious()
		{
			if (sequenceNumber > 1)
			{
				GuidPacket guidPacket = AppConfiguration.CurrentProvider.GenerationLog.Fetch(this.sequenceNumber - 1);
				this.SetGuid(guidPacket, showTimestamp: true);
			}
		}

		private void OnNext()
		{
			if (Factory.CurrentGuid.SequenceNumber == this.SequenceNumber)
			{
				Factory.GenerateNextGuid();
				GuidPacket guidPacket = Factory.CurrentGuid;
				this.SetGuid(guidPacket, showTimestamp: false);
				this.ScrambleDigitsAsync(guidPacket);
			}
			else
			{
				this.sequenceNumber++;
				this.SetGuid(AppConfiguration.CurrentProvider.GenerationLog.Fetch(this.sequenceNumber), showTimestamp: true);
			}
		}

		private void ScrambleDigitsAsync(GuidPacket guidPacket)
		{
			// Scramble the digits without locking the UI thread
			using var scrambleDigitsWorker = new BackgroundWorker();
			scrambleDigitsWorker.DoWork += this.ScrambleDigits;
			scrambleDigitsWorker.RunWorkerAsync(guidPacket);
		}

		private void ScrambleDigits(object? sender, DoWorkEventArgs e)
		{
			GuidPacket guidPacket = (GuidPacket)e.Argument!;
			long sequenceNumberSnapshot = guidPacket.SequenceNumber;
			byte digitCount = Formatter.Digits;
			char[] chars = guidPacket.FormattedValue.ToCharArray();

			// Assign a random scramble time for each digit in ticks.
			// Possible scramble time is between 0.3 and 0.6 seconds.

			var rng = new Random(DateTime.Now.Millisecond);
			long[] scrambleTimes = new long[digitCount];

			for (int i = 0; i < digitCount; i++)
			{
				scrambleTimes[i] = rng.Next(300, 600) * TimeSpan.TicksPerMillisecond;
			}

			long startTicks = DateTime.Now.Ticks;
			bool[] digitDone = new bool[digitCount];
			int doneCount = 0;

			do
			{
				for (int i = 0; i < digitCount; i++)
				{
					if (!digitDone[i])
					{
						int digitIndex = FindDigitIndex(i);

						if (DateTime.Now.Ticks - startTicks < scrambleTimes[i])
						{
							chars[digitIndex] = Convert.ToChar(rng.Next(16).ToString("X"));
						}
						else
						{
							chars[digitIndex] = guidPacket.FormattedValue[digitIndex];
							digitDone[i] = true;
							doneCount++;
						}
					}
				}

				lock (currentGuidLock)
				{
					// Don't update the UI and abort if the sequence number has changed,
					// as this means the user has requested a different GUID and there's
					// danger of race conditions.

					if (this.sequenceNumber == sequenceNumberSnapshot)
					{
						this.currentGuid = new string(chars);
						this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CurrentGuid)));
					}
					else
					{
						break;
					}
				}				

			} while (doneCount < digitCount);
		}

		private static int FindDigitIndex(int nthDigit)
		{
			char templateChar = GuidFormatter.TemplateChar;
			char[] templateString = Formatter.TemplateString.ToCharArray();
			int digitCount = 0;

			for (int i = 0; i < templateString.Length; i++)
			{
				if (templateString[i] == templateChar)
				{
					if (digitCount == nthDigit)
					{
						return i;
					}

					digitCount++;
				}
            }

			return -1;
		}

		private void SetGuid(GuidPacket guidPacket, bool showTimestamp)
		{
			lock (currentGuidLock)
			{
				this.currentGuid = guidPacket.FormattedValue;
				this.sequenceNumber = guidPacket.SequenceNumber;								
				this.timestamp = showTimestamp ? FormatTimestamp(guidPacket.TimeStamp) : string.Empty;

				this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CurrentGuid)));
				this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.SequenceNumber)));
				this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Timestamp)));
			}
		}

		private static string FormatTimestamp(DateTimeOffset timestamp)
		{
			DateTime local = timestamp.LocalDateTime;
			return $"{local.DayOfWeek}, {local.ToLongDateString()}, {local.ToLongTimeString()}";
		}
	}
}
