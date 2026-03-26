using Microsoft.Maui.Controls;

namespace MastodonFollowerTimes;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new NavigationPage(new MainPage());
    }
}
