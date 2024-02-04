using BrowserHistoryParser_ClassLib;

namespace BrowserHistoryAnalyzer_WPF.Models
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
