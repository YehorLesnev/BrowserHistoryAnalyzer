using System.Collections;
using BrowserHistoryAnalyzer_WPF.Views.Modals;
using BrowserHistoryParser_ClassLib;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using System.Windows.Input;
using BrowserHistoryAnalyzer_WPF.Models;

namespace BrowserHistoryAnalyzer_WPF.ViewModels
{
    public class BrowserHistoryViewModel : ViewModelBase
    {
        private readonly BrowserHistoryParser _parser = new();
        public Options HistoryOptions { get; set; } = new();

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

        public BrowserHistoryViewModel()
        {
            GetAllHistoryWithOptions = new Command(getAllHistoryWithOptions);
            ShowOptionsWindow = new Command(showOptionsWindow);
            //ShowErrorModal = new Command(showErrorModal);
        }

        public ICommand GetAllHistoryWithOptions { get; set; }
        private void getAllHistoryWithOptions(object o)
        {
            try
            {
                HistoryItems = null;

                string[] mustContain = [];
                string[] dontContain = [];

                if (HistoryOptions.MustContain is not null && HistoryOptions.MustContain.Length > 0)
                {
                    Regex regex = new Regex(",\\s*");
                    var wordsList = new List<string>(regex.Split(HistoryOptions.MustContain));
                    wordsList.RemoveAll(string.IsNullOrEmpty);
                    mustContain = wordsList.ToArray();
                }
                if (HistoryOptions.MustNotContain is not null && HistoryOptions.MustNotContain.Length > 0)
                {
                    Regex regex = new Regex(",\\s*");
                    var wordsList = new List<string>(regex.Split(HistoryOptions.MustNotContain));
                    wordsList.RemoveAll(string.IsNullOrEmpty);
                    dontContain = wordsList.ToArray();
                }

                List<HistoryItem> history = new List<HistoryItem>();

                if (HistoryOptions.IsChromeChecked)
                {
                    history.AddRange(_parser.GetChromeHistoryItems(mustContain, dontContain));
                }

                if (HistoryOptions.IsEdgeChecked)
                {
                    history.AddRange(_parser.GetEdgeHistoryItems(mustContain, dontContain));
                }

                if (HistoryOptions.IsFirefoxChecked)
                {
                    history.AddRange(_parser.GetFirefoxHistoryItems(mustContain, dontContain));
                }

                HistoryItems = _mapper.Map<ObservableCollection<HistoryItemViewModel>>(history);
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

        public ICommand ShowOptionsWindow { get; set; }
        private void showOptionsWindow(object o)
        {
            var optionsWindow = new OptionsWindow()
            {
                DataContext = this,
            };

            optionsWindow.ShowDialog();
        }


        //public ICommand ShowErrorModal { get; set; }
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
