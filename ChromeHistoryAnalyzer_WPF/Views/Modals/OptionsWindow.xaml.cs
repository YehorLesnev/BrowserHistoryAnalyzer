using System.Windows;

namespace BrowserHistoryAnalyzer_WPF.Views.Modals
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        public OptionsWindow()
        {
            InitializeComponent();
        }

        private void ButtonOptionsClose(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void ButtonOptionsCancel_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
