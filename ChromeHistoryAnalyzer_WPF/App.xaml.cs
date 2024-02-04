using System.Collections.ObjectModel;
using System.Windows;
using AutoMapper;
using BrowserHistoryAnalyzer_WPF.Base.Mapping;
using BrowserHistoryAnalyzer_WPF.ViewModels;
using BrowserHistoryParser_ClassLib;

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

            // FOR TESTING ONLY !
            Mapper _mapper = new Mapper(BrowserHistoryMappingConfig.GetConfig());
            var parser = new BrowserHistoryParser();
            _browserHistoryViewModel.HistoryItems = _mapper.Map<ObservableCollection<HistoryItemViewModel>>(parser.GetFirefoxHistoryItems(new string[]{ "file", "bing" }, false));
            //
        }
    }
}
