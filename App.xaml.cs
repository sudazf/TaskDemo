using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
			MainWindow mainV = new TaskDemo.MainWindow();
			MainWindowViewModel mainVM = new MainWindowViewModel();
			mainV.DataContext = mainVM;
			mainV.Show();
		}
	}
}
