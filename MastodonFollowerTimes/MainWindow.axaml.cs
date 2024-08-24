using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MastodonFollowerTimes;

public partial class MainWindow : Window
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
            //TODO MessageBox.Show(this, ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
        catch (Exception ex)
        {
            //TODO MessageBox.Show(this, ex.ToString(), Title, MessageBoxButton.OK, MessageBoxImage.Error);
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