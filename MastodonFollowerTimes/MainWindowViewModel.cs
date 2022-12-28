using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
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
                var totalStatuses = (uint)0;
                var list = new List<StatusPerHour>();
                foreach (var follower in followers)
                {
                    var statuses = await client.GetStatusesForFollowerId(follower.Id);
                    foreach (var status in statuses)
                    {
                        totalStatuses++;
                        var hour = status.CreateAtUtc.ToLocalTime().Hour;
                        var existingHour = list.FirstOrDefault(x => x.Hour == (byte)hour);
                        if (existingHour == null)
                            list.Add(new StatusPerHour { Hour = (byte)hour, StatusCount = 1 });
                        else
                            existingHour.StatusCount++;
                    }
                }
                foreach (var status in list.OrderBy(x => x.Hour))
                {
                    status.TotalStatuses = totalStatuses;
                    StatusesPerHour.Add(status);
                }
            }
            finally
            {
                EnableControls = true;
            }
        }
    }
}
