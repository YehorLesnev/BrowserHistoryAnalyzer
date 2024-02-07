namespace BrowserHistoryAnalyzer_WPF.ViewModels
{
    public class WebsiteViewModel : ViewModelBase
    {
        private string? _url;
        public string? Url
        {
            get => _url;
            set
            {
                _url = value;
                OnPropertyChanged();
            }
        }

        private long _visitCount;
        public long VisitCount
        {
            get => _visitCount;
            set
            {
                _visitCount = value;
                OnPropertyChanged();
            }
        }

        private long _typedCount;
        public long TypedCount
        {
            get => _typedCount;
            set
            {
                _typedCount = value;
                OnPropertyChanged();
            }
        }

        private DateTime _firstVisitedTime;
        public DateTime FirstVisitedTime
        {
            get => _firstVisitedTime;
            set
            {
                _firstVisitedTime = value;
                OnPropertyChanged();
            }
        }

        private DateTime _lastVisitedTime;
        public DateTime LastVisitedTime
        {
            get => _lastVisitedTime;
            set
            {
                _lastVisitedTime = value;
                OnPropertyChanged();
            }
        }

        public override string ToString()
        {
            return $"====================================================================\n" +
                   $"URL: {Url}\n" +
                   $"First Visited time: {FirstVisitedTime}\n" +
                   $"Last Visited time: {LastVisitedTime}\n" +
                   $"Visit count: {VisitCount}\n" +
                   $"Typed count: {TypedCount}\n" +
                   $"====================================================================";
        }
    }
}
