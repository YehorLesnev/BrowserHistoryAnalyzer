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

        private void ButtonOptionsCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
