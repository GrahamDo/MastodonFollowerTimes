using System;
using System.Diagnostics;
using System.Windows;

namespace MastodonFollowerTimes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var vm = new MainWindowViewModel(this);
            DataContext = vm;
        }

        private void ProcessButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MainWindowViewModel;
            if (vm == null)
                throw new ApplicationException("ViewModel is null");

            vm.LoadData();
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
