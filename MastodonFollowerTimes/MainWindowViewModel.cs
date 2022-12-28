namespace MastodonFollowerTimes
{
    internal class MainWindowViewModel
    {
        public string WindowTitle => "Mastodon Follower Times: Fine the best time to post";
        public WpfSettings Settings { get; }

        public MainWindowViewModel()
        {
            Settings = WpfSettings.Load();
        }
    }
}
