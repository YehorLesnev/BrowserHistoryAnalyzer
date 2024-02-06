using BrowserHistoryParser_ClassLib;

namespace BrowserHistoryAnalyzer_WPF.ViewModels
{
    public class HistoryItemViewModel : ViewModelBase
    {
        private long? _id;
        public long? Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        private Uri? _url;
        public Uri? Url
        {
            get => _url;
            set
            {
                _url = value;
                OnPropertyChanged();
            }
        }

        private string? _title;
        public string? Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _visitedTime;
        public DateTime? VisitedTime
        {
            get => _visitedTime;
            set
            {
                _visitedTime = value;
                OnPropertyChanged();
            }
        }

        private long? _visitCount;
        public long? VisitCount
        {
            get => _visitCount;
            set
            {
                _visitCount = value;
                OnPropertyChanged();
            }
        }

        private int? _typedCount;
        public int? TypedCount
        {
            get => _typedCount;
            set
            {
                _typedCount = value;
                OnPropertyChanged();
            }
        }

        private BrowserName? _browserName;
        public BrowserName? BrowserName
        {
            get => _browserName;
            set
            {
                _browserName = value;
                OnPropertyChanged();
            }
        }

        public override string ToString()
        {
            return $"====================================================================\n" +
                $"ID: {Id}\n" +
                $"URL: {Url}\n" +
                $"Title: {Title}\n" +
                $"Visited time: {VisitedTime}\n" +
                $"Visit count: {VisitCount}\n" +
                $"Typed count: {TypedCount}\n" +
                $"Browser: {BrowserName}\n"
                + $"====================================================================";
        }
    }
}