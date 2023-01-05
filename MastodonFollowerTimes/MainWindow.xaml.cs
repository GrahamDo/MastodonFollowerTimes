using System;
using System.Diagnostics;
using System.Windows;

namespace MastodonFollowerTimes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private async void ProcessButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DataContext is not MainWindowViewModel vm)
                    throw new ApplicationException("ViewModel is null");

                await vm.LoadData();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(this, ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            var psi = new ProcessStartInfo("https://github.com/GrahamDo/MastodonFollowerTimes/releases")
                {
                    UseShellExecute = true
                };
            Process.Start(psi);
        }
    }
}
