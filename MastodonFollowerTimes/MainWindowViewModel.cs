using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MastodonFollowerTimes
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string WindowTitle => "Mastodon Follower Times: Fine the best time to post";
        public WpfSettings Settings { get; }
        public ObservableCollection<StatusPerHour> StatusesPerHour { get; set; }

        private bool _enableControls;
        public bool EnableControls
        {
            get => _enableControls;
            set
            {
                _enableControls = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EnableControls)));
            }
        }

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

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
