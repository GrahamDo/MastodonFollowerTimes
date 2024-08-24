using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

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
            Debug.Assert(Title != null, nameof(Title) + " != null");
            await MessageBox(Title, ex.Message, MsBox.Avalonia.Enums.Icon.Warning);
        }
        catch (Exception ex)
        {
            Debug.Assert(Title != null, nameof(Title) + " != null");
            await MessageBox(Title, ex.ToString(), MsBox.Avalonia.Enums.Icon.Error);
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

    private async Task MessageBox(string caption, string text, Icon icon)
    {
        var box = MessageBoxManager.GetMessageBoxStandard(caption, text, ButtonEnum.Ok, icon);
        await box.ShowWindowDialogAsync(this);
    }
}