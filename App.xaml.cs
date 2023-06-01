using System.Windows;
using System.Windows.Threading;
using TaskDemo.ViewModel;

namespace TaskDemo
{
	/// <summary>
	/// App.xaml 的交互逻辑
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			var window = new MainWindow();
			var viewModel = new MainWindowViewModel();
			window.DataContext = viewModel;
            DispatcherHelper.SetDispatcher(window.Dispatcher);
            window.Show();
		}
	}

    public static class DispatcherHelper
    {
		public static Dispatcher MainDispatcher;

        public static void SetDispatcher(Dispatcher dispatcher)
        {
            MainDispatcher = dispatcher;
        }
    }
}
