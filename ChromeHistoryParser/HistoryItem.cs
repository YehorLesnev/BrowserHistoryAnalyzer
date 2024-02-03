namespace ChromeHistoryParser
{
    public class HistoryItem
    {
        public Uri URL { get; set; }

        public string Title { get; set; }

        public DateTime VisitedTime { get; set; }
        public long VisitCount { get; set; }
        public long TypedCount { get; set; }

    }
}
