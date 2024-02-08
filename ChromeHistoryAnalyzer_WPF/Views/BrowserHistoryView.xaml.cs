using System.Windows.Controls;
using System.Windows.Input;

namespace BrowserHistoryAnalyzer_WPF.Views
{
    /// <summary>
    /// Interaction logic for BrowserHistoryView.xaml
    /// </summary>
    public partial class BrowserHistoryView : UserControl
    {
        public DataGrid DataGridHistory => DataGridBrowserHistory;

        public BrowserHistoryView()
        {
            InitializeComponent();
        }
    }
}
