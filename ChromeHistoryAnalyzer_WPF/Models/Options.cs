namespace BrowserHistoryAnalyzer_WPF.Models
{
    public class Options
    {
        public string? MustContain { get; set; }
        public string? MustNotContain { get; set; }
        public bool IsChromeChecked { get; set; }
        public bool IsEdgeChecked { get; set; }
        public bool IsFirefoxChecked { get; set; }

    }
}
