using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Currency
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        public SettingsControl()
        {
            InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.ApiKeyTextBox.Text = Properties.Settings.Default.apiKey;
        }

        private void NavigateToApiKeyRetrieval(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.apiKey = this.ApiKeyTextBox.Text;
            Properties.Settings.Default.Save();
        }
    }
}