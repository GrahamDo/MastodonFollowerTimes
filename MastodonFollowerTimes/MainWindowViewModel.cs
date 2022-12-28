using System.Collections.ObjectModel;

namespace MastodonFollowerTimes
{
    internal class MainWindowViewModel
    {
        public string WindowTitle => "Mastodon Follower Times: Fine the best time to post";
        public WpfSettings Settings { get; }
        public ObservableCollection<StatusPerHour> StatusesPerHour { get; set; }
        public bool EnableControls { get; set; }

        public MainWindowViewModel()
        {
            Settings = WpfSettings.Load();
            StatusesPerHour = new ObservableCollection<StatusPerHour>();
            EnableControls = true;
        }

        public void LoadData()
        {
            EnableControls = false;
            try
            {
                StatusesPerHour.Clear();
                for (byte i = 0; i < 25; i++)
                {
                    StatusesPerHour.Add(new StatusPerHour
                        { Hour = i, StatusCount = 25 + (uint)i, TotalStatuses = 240 });
                }
            }
            finally
            {
                EnableControls = true;
            }
        }
    }
}
