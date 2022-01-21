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
		public event PropertyChangedEventHandler PropertyChanged;

		private string currentGuid = Guid.Empty.ToString("b");
		private long sequenceNumber;
		private ICommand previousGuidCommand;
		private ICommand nextGuidCommand;

		public MainWindowViewModel()
		{
			this.previousGuidCommand = new DelegateCommand(this.PreviousGuid);
			this.nextGuidCommand = new DelegateCommand(this.NextGuid);
		}

		private GuidFactory Factory
		{
			get
			{
				return AppConfiguration.CurrentProvider.Factory;
			}
		}

		private GuidFormatter Formatter
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

		public ICommand PreviousButtonClick
		{
			get
			{
				return this.previousGuidCommand;
			}
		}

		public ICommand NextButtonClick
		{
			get
			{
				return this.nextGuidCommand;
			}
		}

		public void PreviousGuid()
		{
			this.sequenceNumber--;
			this.currentGuid = AppConfiguration.CurrentProvider.GenerationLog.Fetch(this.sequenceNumber).FormattedValue;

			this.ScrambleDigitsAsync();
		}

		public void NextGuid()
		{
			this.Factory.GenerateNextGuid();

			this.currentGuid = this.Factory.CurrentGuid.FormattedValue;
			this.sequenceNumber = this.Factory.CurrentGuid.SequenceNumber;

			this.ScrambleDigitsAsync();
		}

		private void ScrambleDigitsAsync()
		{
			var scrambleDigitsWorker = new BackgroundWorker();
			scrambleDigitsWorker.DoWork += this.ScrambleDigits;
			scrambleDigitsWorker.RunWorkerAsync();
		}

		private void ScrambleDigits(object sender, EventArgs e)
		{
			string originalString = this.currentGuid;
			byte digitCount = this.Formatter.Digits;
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

				int previousDigitIndex = 0;

				for (int i = 0; i < digitCount; i++)
				{
					if (!digitDone[i])
					{
						int digitIndex = this.FindDigitIndex(i);

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

						previousDigitIndex = digitIndex;
					}
				}

				this.currentGuid = new string(chars);
				this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CurrentGuid)));
				this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.SequenceNumber)));

			} while (doneCount < digitCount - 1);
		}

		private int FindDigitIndex(int nthDigit)
		{
			char templateChar = GuidFormatter.TemplateChar;
			char[] templateString = this.Formatter.TemplateString.ToCharArray();
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
	}
}
