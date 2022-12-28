﻿using System;
using System.Threading.Tasks;
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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = DataContext as MainWindowViewModel;
                if (vm == null)
                    throw new ApplicationException("ViewModel is null");

                await vm.LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
