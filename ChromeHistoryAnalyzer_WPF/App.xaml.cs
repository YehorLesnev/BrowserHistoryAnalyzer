using ChromeHistoryParser_ClassLib;
using System.Configuration;
using System.Data;
using System.Windows;

namespace ChromeHistoryAnalyzer_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainWindow _mainwindow;

        public MainWindow mainwindow
        {
            get { return _mainwindow ??= new MainWindow(); }
            set { _mainwindow = value; }
        }

        public App()
        {
            mainwindow = new MainWindow();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _mainwindow.Show();
        }
    }
}
