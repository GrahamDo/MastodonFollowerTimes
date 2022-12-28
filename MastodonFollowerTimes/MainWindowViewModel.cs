using System.Collections.ObjectModel;
using System.IO.Pipes;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task LoadData()
        {
            StatusesPerHour.Clear();
            EnableControls = false;
            try
            {
                var client = new ApiClient();
                await client.VerifyCredentials(Settings.InstanceUrl, Settings.Token);
                var accountId = await client.GetIdForAccountName(Settings.AccountName);
                Settings.Save();

                var followers = await client.GetFollowerIdsForAccountId(accountId);
                foreach (var follower in followers)
                {
                    var statuses = await client.GetStatusesForFollowerId(follower.Id);
                    var totalStatuses = (uint)0;
                    foreach (var status in statuses)
                    {
                        totalStatuses++;
                        var hour = status.CreateAtUtc.ToLocalTime().Hour;
                        var existingHour = StatusesPerHour.FirstOrDefault(x => x.Hour == (byte)hour);
                        if (existingHour == null)
                            StatusesPerHour.Add(new StatusPerHour { Hour = (byte)hour, StatusCount = 1 });
                        else
                            existingHour.StatusCount++;
                    }
                    foreach (var status in StatusesPerHour)
                        status.TotalStatuses = totalStatuses;
                }
            }
            finally
            {
                EnableControls = true;
            }
        }
    }
}
