using System.Windows;

namespace BrowserHistoryAnalyzer_WPF.Views.Modals
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        public Options()
        {
            InitializeComponent();
        }

        private void ButtonOptionsCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
