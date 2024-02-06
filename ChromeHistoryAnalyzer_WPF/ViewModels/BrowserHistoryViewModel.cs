using System.Collections;
using BrowserHistoryAnalyzer_WPF.Views.Modals;
using BrowserHistoryParser_ClassLib;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Input;
using BrowserHistoryAnalyzer_WPF.Models;
using Microsoft.Win32;
using System.Windows;

namespace BrowserHistoryAnalyzer_WPF.ViewModels
{
    public class BrowserHistoryViewModel : ViewModelBase
    {
        private readonly BrowserHistoryParser _parser = new();
        public Options HistoryOptions { get; set; } = new();

        private ObservableCollection<string>? _selectedItems;
        public ObservableCollection<string>? SelectedItems
        {
            get => _selectedItems;
            set
            {
                _selectedItems = value;
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

        public BrowserHistoryViewModel()
        {
            GetAllHistoryWithOptions = new Command(getAllHistoryWithOptions);
            ShowOptionsWindow = new Command(showOptionsWindow);
            SaveSelectedItemsToFile = new Command(saveSelectedItemsToFile);
            CopySelectedUrls = new Command(copySelectedUrls);
            OpenUrlInWebBrowser = new Command(openUrlInWebBrowser);
            SaveAll = new Command(saveAll);
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
            List<HistoryItemViewModel> selectedItems = null;

            try
            {
                selectedItems = (o as IList ?? throw new InvalidOperationException()).Cast<HistoryItemViewModel>().ToList();
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

        public ICommand OpenUrlInWebBrowser { get; set; }
        private void openUrlInWebBrowser(object o)
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

        public ICommand SaveAll { get; set; }
        private void saveAll(object o)
        {
            if (HistoryItems is not null && HistoryItems.Count > 0)
            {
                string text = "";

                foreach (var item in HistoryItems)
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
