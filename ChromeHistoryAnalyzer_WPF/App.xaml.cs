using System.Windows;
using BrowserHistoryAnalyzer_WPF.ViewModels;
using BrowserHistoryAnalyzer_WPF.Views.Modals;

namespace BrowserHistoryAnalyzer_WPF
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

        private readonly BrowserHistoryViewModel _browserHistoryViewModel = new();

        public App()
        {
            mainwindow = new MainWindow()
            {
                DataContext = _browserHistoryViewModel
            };
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _mainwindow.Show();

            var opt = new OptionsWindow()
            {
                DataContext = _browserHistoryViewModel
            };

            opt.ShowDialog();
        }
    }
}