namespace ChromeHistoryParser
{
    public class HistoryItem
    {
        public Uri URL { get; set; }

        public string Title { get; set; }

        public DateTime VisitedTime { get; set; }
    }
}
