using ChromeHistoryParser_ClassLib;


namespace ChromeHistoryAnalyzer_WPF.Models
{
    public class BrowserHistoryModel
    {
        public IEnumerable<HistoryItem>? HistoryItems { get; set; }

        public BrowserHistoryModel() { }
        public BrowserHistoryModel(IEnumerable<HistoryItem> historyItems)
        {
            this.HistoryItems = historyItems;
        }
    }
}
