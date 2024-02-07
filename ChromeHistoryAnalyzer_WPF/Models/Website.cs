namespace BrowserHistoryAnalyzer_WPF.Models
{
    public struct Website
    {
        public string Url { get; set; }
        public long VisitCount { get; set; }
        public long TypedCount { get; set; }
        public DateTime FirstVisitedTime { get; set; }
        public DateTime LastVisitedTime { get; set; }
    }
}
