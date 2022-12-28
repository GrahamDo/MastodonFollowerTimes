using System;
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
            DataContext = new MainWindowViewModel();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MainWindowViewModel;
            if (vm == null)
                throw new ApplicationException("ViewModel is null");

            vm.LoadData();
        }
    }
}
