using ChromeHistoryParser;

namespace ChromeHistoryAnalyzer_WPF.Models
{
    public class ChromeHistoryModel
    {
        public IEnumerable<HistoryItem>? HistoryItems { get; set; }

        public ChromeHistoryModel() { }
        public ChromeHistoryModel(IEnumerable<HistoryItem> historyItems)
        {
            this.HistoryItems = historyItems;
        }
    }
}
