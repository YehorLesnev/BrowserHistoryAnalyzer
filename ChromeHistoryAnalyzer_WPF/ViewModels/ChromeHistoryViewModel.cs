using System.Collections.ObjectModel;

namespace ChromeHistoryAnalyzer_WPF.ViewModels
{
    public class ChromeHistoryViewModel : ViewModelBase
    {
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
	}
}
