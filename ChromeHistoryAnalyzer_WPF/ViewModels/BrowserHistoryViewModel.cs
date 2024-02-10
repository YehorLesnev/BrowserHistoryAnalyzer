using System.Collections;
using BrowserHistoryAnalyzer_WPF.Views.Modals;
using BrowserHistoryParser_ClassLib;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Windows.Input;
using BrowserHistoryAnalyzer_WPF.Models;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Media;
using BrowserHistoryAnalyzer_WPF.Enums;
using BrowserHistoryAnalyzer_WPF.ViewModels.Commands;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace BrowserHistoryAnalyzer_WPF.ViewModels
{
    public class BrowserHistoryViewModel : ViewModelBase
    {
        public readonly BrowserHistoryParser _parser = new();
        public Options HistoryOptions { get; set; } = new();

        private int _currentTabIndex;
        public int CurrentTabIndex
        {
            get => _currentTabIndex;
            set
            {
                _currentTabIndex = value;
                OnPropertyChanged();

                if ((int)MainTabIndex.Statistics == value)
                {
                    refreshCharts(null);
                }
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<HistoryItemViewModel>? _historyItems;
        public ObservableCollection<HistoryItemViewModel>? HistoryItems
        {
            get => _historyItems;
            set
            {
                _historyItems = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<WebsiteViewModel>? _websites;
        public ObservableCollection<WebsiteViewModel>? Websites
        {
            get => _websites;
            set
            {
                _websites = value;
                OnPropertyChanged();
            }
        }

        // For charts
        // Browsers Pie Chart
        public SeriesCollection BrowserUsageChartPieSeriesCollection { get; set; } = new();

        // Websites chart
        public ChartValues<Website> WebsitesChartCollection { get; set; } = new();
        public ObservableCollection<string> WebsitesChartLabels { get; set; } = new();
        public object WebsitesChartMapper { get; set; }

        // Activity chart
        public SeriesCollection ActivityVisitCountSeries { get; set; } = new();
        public Func<double, string> ActivityDateTimeFormatter { get; set; }

        private ZoomingOptions _activityZoomingMode;
        public ZoomingOptions ActivityZoomingMode
        {
            get { return _activityZoomingMode; }
            set
            {
                _activityZoomingMode = value;
                OnPropertyChanged();
            }
        }

        public BrowserHistoryViewModel()
        {
            GetAllHistoryWithOptionsAsync = new CommandLoadHistoryAsync(this);
            ShowOptionsWindow = new Command(showOptionsWindow);
            ShowAboutWindow = new Command(showAboutWindow);
            SaveSelectedItemsToFile = new Command(saveSelectedItemsToFile);
            CopySelectedUrls = new Command(copySelectedUrls);
            OpenUrlInWebBrowser = new Command(openUrlInWebBrowser);
            SaveAll = new Command(saveAll);
            RefreshCharts = new Command(refreshCharts);
            WebsiteChartSearchTextChanged = new Command(websiteChartSearchTextChanged);
            DeleteSelectedHistoryItems = new Command(deleteSelectedHistoryItems);

            // configure the chart to plot websites
            WebsitesChartMapper = Mappers.Xy<Website>()
                .X((website, index) => index)
                .Y(website => website.VisitCount);

            // Initialize the ActivityDateTimeFormatter for X-axis labels
            ActivityDateTimeFormatter = value => new DateTime(
                (DateTime.MinValue.Ticks < value && DateTime.MaxValue.Ticks > value) ? (long)value : 0)
                .ToString("MM/dd/yyyy");

            ActivityZoomingMode = ZoomingOptions.X;
        }

        public ICommand WebsiteChartSearchTextChanged { get; set; }
        private void websiteChartSearchTextChanged(object o)
        {
            var text = ((string)o);

            if (Websites != null)
            {
                var records = Websites
                    .Where(x => x.Url != null && x.Url.Contains(text))
                    .OrderByDescending(x => x.VisitCount)
                    .Take(15);

                var websites = _mapper.Map<ObservableCollection<Website>>(records);

                WebsitesChartCollection.Clear();
                WebsitesChartCollection.AddRange(websites);

                WebsitesChartLabels.Clear();

                foreach (var website in websites)
                {
                    WebsitesChartLabels.Add(website.Url);
                }
            }
        }

        public ICommandAsync GetAllHistoryWithOptionsAsync { get; set; }

        public ICommand ShowOptionsWindow { get; set; }
        private void showOptionsWindow(object o)
        {
            var optionsWindow = new OptionsWindow()
            {
                DataContext = this,
            };

            optionsWindow.ShowDialog();
        }

        public ICommand SaveSelectedItemsToFile { get; set; }
        private void saveSelectedItemsToFile(object o)
        {
            IList selectedItems = (IList)o;

            if (selectedItems is not null && selectedItems.Count > 0)
            {
                string text = "";

                foreach (var item in selectedItems)
                {
                    text += $"{item}\n";
                }

                try
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Text file (*.txt)|*.txt";
                    saveFileDialog.Title = "Save as...";

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        File.WriteAllText(saveFileDialog.FileName, text);
                    }
                }
                catch (Exception e)
                {
                    showErrorModal(e.Message);
                }
            }
        }

        public ICommand CopySelectedUrls { get; set; }
        private void copySelectedUrls(object o)
        {
            if ((int)MainTabIndex.BrowserHistory == CurrentTabIndex)
            {
                List<HistoryItemViewModel> selectedItems = null;

                try
                {
                    selectedItems = (o as IList ?? throw new InvalidOperationException()).Cast<HistoryItemViewModel>().ToList();
                }
                catch (Exception e)
                {
                    showErrorModal(e.Message);
                }

                if (selectedItems is not null && selectedItems.Count > 0)
                {
                    string text = "";

                    foreach (var item in selectedItems)
                    {
                        text += $"{item.Url}\n";
                    }

                    Clipboard.SetText(text);
                }
            }
            else if ((int)MainTabIndex.WebsitesHistory == CurrentTabIndex)
            {
                List<WebsiteViewModel> selectedItems = null;

                try
                {
                    selectedItems = (o as IList ?? throw new InvalidOperationException()).Cast<WebsiteViewModel>().ToList();
                }
                catch (Exception e)
                {
                    return;
                }

                if (selectedItems is not null && selectedItems.Count > 0)
                {
                    string text = "";

                    foreach (var item in selectedItems)
                    {
                        text += $"{item.Url}\n";
                    }

                    Clipboard.SetText(text);
                }
            }
        }

        public ICommand OpenUrlInWebBrowser { get; set; }
        private void openUrlInWebBrowser(object o)
        {
            if ((int)MainTabIndex.BrowserHistory == CurrentTabIndex)
            {
                var historyItem = (HistoryItemViewModel)o;

                if (historyItem?.Url is not null)
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = historyItem.Url.AbsoluteUri,
                        UseShellExecute = true
                    });
                }
            }
            else if ((int)MainTabIndex.WebsitesHistory == CurrentTabIndex)
            {
                var website = (WebsiteViewModel)o;

                if (website?.Url is not null)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = website.Url,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception e)
                    {
                        showErrorModal(e.Message); return;
                    }
                }
            }
        }

        public ICommand SaveAll { get; set; }
        private void saveAll(object o)
        {
            string text = "";

            if ((int)MainTabIndex.BrowserHistory == CurrentTabIndex && HistoryItems is not null && HistoryItems.Count > 0)
            {

                foreach (var item in HistoryItems)
                {
                    text += $"{item}\n";
                }
            }
            else if ((int)MainTabIndex.WebsitesHistory == CurrentTabIndex && Websites is not null && Websites.Count > 0)
            {
                foreach (var item in Websites)
                {
                    text += $"{item}\n";
                }
            }

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Text file (*.txt)|*.txt";
                saveFileDialog.Title = "Save as...";

                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllText(saveFileDialog.FileName, text);
                }
            }
            catch (Exception e)
            {
                showErrorModal(e.Message);
            }
        }

        public ICommand DeleteSelectedHistoryItems { get; set; }
        private void deleteSelectedHistoryItems(object o)
        {
            if(HistoryItems is null || HistoryItems.Count == 0) return;

            List<HistoryItemViewModel> selectedItems = null;

            try
            {
                selectedItems = (o as IList ?? throw new InvalidOperationException()).Cast<HistoryItemViewModel>().ToList();
            }
            catch (Exception e)
            {
                showErrorModal(e.Message);
            }

            try
            {
                if (selectedItems is not null && selectedItems.Count > 0)
                {
                    _parser.DeleteHistoryItemsById(_mapper.Map<ObservableCollection<HistoryItem>>(selectedItems));

                    var successDeleted = new SuccessfulDeletionWindow()
                    {
                        DataContext = this
                    };

                    foreach (var item in selectedItems)
                    {
                        HistoryItems.Remove(item);
                    }

                    successDeleted.ShowDialog();
                }
            }
            catch (SQLiteException e)
            {
                showErrorModal("\"" + e.Message + "\" Try to close all browsers and their processes");
            }
            catch (Exception e)
            {
                showErrorModal(e.Message);
            }
        }

        public ICommand RefreshCharts { get; set; }
        private void refreshCharts(object? o)
        {
            try
            {
                if (HistoryItems != null)
                {
                    // Browsers Pie Chart
                    BrowserUsageChartPieSeriesCollection.Clear();
                    BrowserUsageChartPieSeriesCollection.AddRange(
                        [
                            new PieSeries
                            {
                                Title = "Chrome",
                                Values = new ChartValues<ObservableValue>
                                    { new(HistoryItems.Count(x => x.BrowserName == BrowserName.Chrome)) },
                                DataLabels = true,
                                LabelPoint = chartPoint => $"({chartPoint.Participation:P})",
                                Fill = Brushes.Red
                            },
                            new PieSeries
                            {
                                Title = "Firefox",
                                Values = new ChartValues<ObservableValue> { new(HistoryItems.Count(x => x.BrowserName == BrowserName.Firefox)) },
                                DataLabels = true,
                                LabelPoint = chartPoint => $"({chartPoint.Participation:P})",
                                Fill = Brushes.Orange
                            },
                            new PieSeries
                            {
                                Title = "Edge",
                                Values = new ChartValues<ObservableValue> { new(HistoryItems.Count(x => x.BrowserName == BrowserName.Edge)) },
                                DataLabels = true,
                                LabelPoint = chartPoint => $"({chartPoint.Participation:P})",
                                Fill = Brushes.DeepSkyBlue
                            },
                        ]
                        );


                    // Websites chart
                    // take the first 15 records by default
                    if (Websites != null)
                    {
                        var records = _mapper.Map<ObservableCollection<Website>>(Websites.OrderByDescending(x => x.VisitCount).Take(15)).ToArray();

                        WebsitesChartCollection.Clear();
                        WebsitesChartCollection.AddRange(records);

                        WebsitesChartLabels.Clear();
                        var urls = records.Select(x => x.Url);
                        foreach (var u in urls)
                        {
                            WebsitesChartLabels.Add(u);
                        }
                    }

                    // Activity chart
                    ActivityVisitCountSeries.Clear();

                    var historyItems = _mapper.Map<ObservableCollection<HistoryItem>>(HistoryItems);

                    // Group history items by VisitedTime date and sum VisitCount
                    var groupedData = historyItems
                        .GroupBy(item => item.VisitedTime.Date)
                        .Select(group => new { Date = group.Key, VisitCount = group.Sum(item => item.VisitCount) })
                        .Where(group => group.Date > new DateTime(1990, 1, 1));

                    // Convert the grouped data to chart points
                    var chartValues = new ChartValues<ObservablePoint>();
                    foreach (var dataPoint in groupedData)
                    {
                        var dateTimeValue = ((DateTime)dataPoint.Date).Ticks; // Convert DateTime to a numeric value
                        chartValues.Add(new ObservablePoint(dateTimeValue, (long)dataPoint.VisitCount));
                    }

                    var gradientBrush = new LinearGradientBrush { StartPoint = new Point(0, 0), EndPoint = new Point(0, 1) };
                    gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(33, 148, 241), 0));
                    gradientBrush.GradientStops.Add(new GradientStop(Colors.Transparent, 1));

                    // if there's only one ObservablePoint in ChartValues than program crashes for some reason (LiveCharts issue)
                    if (chartValues.Count == 1)
                    {
                        chartValues.Add(new ObservablePoint(DateTime.MinValue.Date.Ticks - 1, 0));
                    }

                    // Add the LineSeries to the SeriesCollection
                    ActivityVisitCountSeries.Add(new LineSeries
                    {
                        Title = "Visit Count",
                        Values = chartValues,
                        Fill = gradientBrush,
                        StrokeThickness = 1,
                        PointGeometrySize = 0
                    });
                }
            }
            catch (Exception e)
            {
                showErrorModal(e.Message);
            }
        }

        public ICommand ShowAboutWindow { get; set; }
        private void showAboutWindow(object o)
        {
            var aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }

        private void showErrorModal(object o)
        {
            var errWindow = new ErrorMessageWindow
            {
                ErrorMessage =
                {
                    Text = (string) o,
                }
            };

            errWindow.ShowDialog();
        }
    }
}