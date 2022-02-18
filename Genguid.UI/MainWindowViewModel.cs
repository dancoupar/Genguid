using Genguid.Configuration;
using Genguid.Factories;
using Genguid.Formatters;
using Prism.Commands;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Genguid.UI
{
	internal class MainWindowViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		private GuidPacket currentGuid;
		private string displayedGuid;
		private long sequenceNumber;
		private string timestamp;
		private readonly ICommand copyButtonClickCommand;
		private readonly ICommand previousButtonClickCommand;
		private readonly ICommand nextButtonClickCommand;		
		private readonly object currentGuidLock = new();

		public MainWindowViewModel()
		{
			this.displayedGuid = string.Empty;
			this.timestamp = string.Empty;
			this.copyButtonClickCommand = new DelegateCommand(this.OnCopy);
			this.previousButtonClickCommand = new DelegateCommand(this.OnPrevious);
			this.nextButtonClickCommand = new DelegateCommand(this.OnNext);
			GuidPacket initialGuid = AppConfiguration.CurrentProvider.Factory.CurrentGuid;
			bool showTimestamp = initialGuid != GuidPacket.NullPacket;
			this.SetGuid(AppConfiguration.CurrentProvider.Factory.CurrentGuid, showTimestamp);
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

		public string DisplayedGuid
		{
			get
			{
				return this.displayedGuid;
			}
		}	
		
		public string Timestamp
		{
			get
			{
				return this.timestamp;
			}
		}

		public ICommand CopyButtonClickCommand
		{
			get
			{
				return this.copyButtonClickCommand;
			}
		}

		public ICommand PreviousButtonClickCommand
		{
			get
			{
				return this.previousButtonClickCommand;
			}
		}

		public ICommand NextButtonClickCommand
		{
			get
			{
				return this.nextButtonClickCommand;
			}
		}

		private void OnCopy()
		{
			Clipboard.SetText(this.currentGuid.FormattedValue);
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
						this.displayedGuid = new string(chars);
						this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.DisplayedGuid)));
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
				this.currentGuid = guidPacket;
				this.displayedGuid = this.currentGuid.FormattedValue;
				this.sequenceNumber = this.currentGuid.SequenceNumber;
				this.timestamp = showTimestamp ? FormatTimestamp(this.currentGuid.TimeStamp) : string.Empty;

				this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.DisplayedGuid)));
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
