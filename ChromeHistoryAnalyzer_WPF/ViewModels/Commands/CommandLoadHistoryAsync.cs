using BrowserHistoryAnalyzer_WPF.Models;
using BrowserHistoryParser_ClassLib;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using System.Windows;

namespace BrowserHistoryAnalyzer_WPF.ViewModels.Commands
{
    public class CommandLoadHistoryAsync : CommandAsync
    {
        private readonly BrowserHistoryViewModel _viewModel;
        public CommandLoadHistoryAsync(BrowserHistoryViewModel browserHistoryViewModel)
        {
            this._viewModel = browserHistoryViewModel;
        }
        
        public override bool CanExecute()
        {
            return !RunningTasks.Any();
        }

        public override async Task ExecuteAsync()
        {
            _viewModel.IsLoading = true;
    
            await Task.Run(() =>
            {
                try
                {
                    _viewModel.HistoryItems = null;
                    _viewModel.Websites = null;

                    string[] mustContain = [];
                    string[] dontContain = [];

                    if (_viewModel.HistoryOptions.MustContain is not null && _viewModel.HistoryOptions.MustContain.Length > 0)
                    {
                        Regex regex = new Regex(",\\s*");
                        var wordsList = new List<string>(regex.Split(_viewModel.HistoryOptions.MustContain));
                        wordsList.RemoveAll(string.IsNullOrEmpty);
                        mustContain = wordsList.ToArray();
                    }
                    if (_viewModel.HistoryOptions.MustNotContain is not null && _viewModel.HistoryOptions.MustNotContain.Length > 0)
                    {
                        Regex regex = new Regex(",\\s*");
                        var wordsList = new List<string>(regex.Split(_viewModel.HistoryOptions.MustNotContain));
                        wordsList.RemoveAll(string.IsNullOrEmpty);
                        dontContain = wordsList.ToArray();
                    }

                    List<HistoryItem> history = new List<HistoryItem>();

                    if (_viewModel.HistoryOptions.IsChromeChecked)
                    {
                        history.AddRange(_viewModel._parser.GetChromeHistoryItems(mustContain, dontContain));
                    }

                    if (_viewModel.HistoryOptions.IsEdgeChecked)
                    {
                        history.AddRange(_viewModel._parser.GetEdgeHistoryItems(mustContain, dontContain));
                    }
                    if (_viewModel.HistoryOptions.IsFirefoxChecked)
                    {
                        history.AddRange(_viewModel._parser.GetFirefoxHistoryItems(mustContain, dontContain));
                    }

                    _viewModel.HistoryItems = _viewModel._mapper.Map<ObservableCollection<HistoryItemViewModel>>(history);

                    // Fill Websites
                    var websites = history.GroupBy(
                        h => h.URL.Host,
                        (key, g) => new Website
                        {
                            Url = key == "" ? "file" : key,
                            VisitCount = g.Sum(x => x.VisitCount),
                            TypedCount = g.Sum(x => x.TypedCount),
                            FirstVisitedTime = g.Min(x => x.VisitedTime),
                            LastVisitedTime = g.Max(x => x.VisitedTime),
                        });

                    _viewModel.Websites = _viewModel._mapper.Map<ObservableCollection<WebsiteViewModel>>(websites);                 
                }
                catch (SQLiteException e)
                {
                    MessageBox.Show("\"" + e.Message + "\" Try to close all browsers and their processes");
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            });

            _viewModel.IsLoading = false;
        }
    }
}
