using Microsoft.Maui.Controls;

namespace MastodonFollowerTimes;

public partial class MainPage : ContentPage
{
    private readonly MainWindowViewModel _vm;

    public MainPage()
    {
        InitializeComponent();
        _vm = new MainWindowViewModel();
        BindingContext = _vm;
    }

    private async void FindBestTimes_Click(object? sender, System.EventArgs e)
    {
        await _vm.LoadData();
    }
}
