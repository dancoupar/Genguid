using Genguid.Configuration;
using Genguid.Factories;
using Genguid.Formatters;
using Prism.Commands;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;

namespace Genguid.UI
{
	internal class MainWindowViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		private string currentGuid = Guid.Empty.ToString("b");
		private long sequenceNumber;
		private readonly ICommand previousCommand;
		private readonly ICommand nextCommand;

		public MainWindowViewModel()
		{
			this.previousCommand = new DelegateCommand(this.OnPrevious);
			this.nextCommand = new DelegateCommand(this.OnNext);

			AppConfiguration.CurrentProvider.Factory.Restore(AppConfiguration.CurrentProvider.GenerationLog);
			this.SetGuid(AppConfiguration.CurrentProvider.Factory.CurrentGuid);
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
			this.sequenceNumber--;
			this.SetGuid(AppConfiguration.CurrentProvider.GenerationLog.Fetch(this.sequenceNumber));
		}

		private void OnNext()
		{
			if (Factory.CurrentGuid.SequenceNumber == this.SequenceNumber)
			{
				Factory.GenerateNextGuid();
				this.SetGuid(Factory.CurrentGuid);
				this.ScrambleDigitsAsync();
			}
			else
			{
				this.sequenceNumber++;
				this.SetGuid(AppConfiguration.CurrentProvider.GenerationLog.Fetch(this.sequenceNumber));
			}			
		}

		private void ScrambleDigitsAsync()
		{
			using var scrambleDigitsWorker = new BackgroundWorker();
			scrambleDigitsWorker.DoWork += this.ScrambleDigits;
			scrambleDigitsWorker.RunWorkerAsync();
		}

		private void ScrambleDigits(object? sender, EventArgs e)
		{
			string originalString = this.currentGuid;
			byte digitCount = Formatter.Digits;
			char[] chars = this.currentGuid.ToCharArray();

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
				// Keep the loop running at a sensible speed
				Thread.Sleep(10);

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
							chars[digitIndex] = originalString[digitIndex];
							digitDone[i] = true;
							doneCount++;
						}
					}
				}

				this.currentGuid = new string(chars);
				this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CurrentGuid)));

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

		private void SetGuid(GuidPacket guidPacket)
		{
			this.currentGuid = guidPacket.FormattedValue;
			this.sequenceNumber = guidPacket.SequenceNumber;

			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CurrentGuid)));
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.SequenceNumber)));
		}
	}
}
