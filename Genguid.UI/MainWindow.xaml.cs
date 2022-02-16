using System.Windows;
using System.Windows.Input;

namespace Genguid.UI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				this.DragMove();
			}
		}

		private void MinimiseButton_Click(object sender, RoutedEventArgs e)
		{
			this.WindowState = WindowState.Minimized;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{

		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}		
	}
}
