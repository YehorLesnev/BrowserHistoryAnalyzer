﻿using System.Collections.ObjectModel;

namespace BrowserHistoryAnalyzer_WPF.ViewModels
{
    public class BrowserHistoryViewModel : ViewModelBase
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
