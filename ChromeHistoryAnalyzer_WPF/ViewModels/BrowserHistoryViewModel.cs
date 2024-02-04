using BrowserHistoryParser_ClassLib;
using System.Collections.ObjectModel;
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
		}

		public ICommand GetAllHistory {get; set;}
		private void getAllHistory(object o)
		{
            HistoryItems = _mapper.Map<ObservableCollection<HistoryItemViewModel>>(_parser.GetAllHistoryItems());
		}
	}
}
