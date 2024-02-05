using BrowserHistoryAnalyzer_WPF.Views.Modals;
using BrowserHistoryParser_ClassLib;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Windows.Input;

namespace BrowserHistoryAnalyzer_WPF.ViewModels
{
    public class BrowserHistoryViewModel : ViewModelBase
    {
        private readonly BrowserHistoryParser _parser = new();

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
            GetAllHistory = new Command(getAllHistory);
            //ShowErrorModal = new Command(showErrorModal);
        }

        public ICommand GetAllHistory { get; set; }
        private void getAllHistory(object o)
        {
            try
            {
                HistoryItems = _mapper.Map<ObservableCollection<HistoryItemViewModel>>(_parser.GetAllHistoryItems());
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
