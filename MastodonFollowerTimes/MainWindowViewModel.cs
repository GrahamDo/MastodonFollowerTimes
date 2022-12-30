using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MastodonFollowerTimes
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string WindowTitle
        {
            get
            {
                var asm = Assembly.GetExecutingAssembly();
                var fvi = FileVersionInfo.GetVersionInfo(asm.Location);

#pragma warning disable CS4014
                SetUpdateButtonVisibility(fvi.ProductVersion ?? "0");
                // Deliberately not awaiting because I don't want to lock up the UI
#pragma warning restore CS4014
                return $"{fvi.ProductName} (Version {fvi.ProductVersion})";
            }
        }

        public WpfSettings Settings { get; }
        public ObservableCollection<StatusPerTimeBlock> StatusesPerHour { get; set; }

        private string _updateButtonVisibility = "Collapsed";
        public string UpdateButtonVisibility
        {
            get => _updateButtonVisibility;
            set
            {
                _updateButtonVisibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UpdateButtonVisibility)));
            }
        }

        private bool _enableControls;
        public bool EnableControls
        {
            get => _enableControls;
            set
            {
                _enableControls = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EnableControls)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InProgressVisibility)));
            }
        }

        public string InProgressVisibility => EnableControls ? "Collapsed" : "Visible";
        public bool InProgressIsIndeterminate => InProgressMaximum == 0;
        private uint _inProgressValue;
        public uint InProgressValue
        {
            get => _inProgressValue;
            set
            {
                _inProgressValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InProgressValue)));
            }
        }
        private uint _inProgressMaximum;
        public uint InProgressMaximum
        {
            get => _inProgressMaximum;
            set
            {
                _inProgressMaximum = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InProgressMaximum)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InProgressIsIndeterminate)));
            }
        }

        public MainWindowViewModel()
        {
            Settings = WpfSettings.Load();
            StatusesPerHour = new ObservableCollection<StatusPerTimeBlock>();
            EnableControls = true;
        }

        public async Task LoadData()
        {
            StatusesPerHour.Clear();
            InProgressValue = 0;
            InProgressMaximum = 0;
            EnableControls = false;
            try
            {
                var client = new MastodonApiClient();
                await client.VerifyCredentials(Settings.InstanceUrl, Settings.Token);
                var accountId = await client.GetIdForAccountName(Settings.AccountName);
                Settings.Save();

                var followers = await client.GetFollowerIdsForAccountId(accountId);
                InProgressMaximum = (uint)followers.Count;
                var totalStatuses = (uint)0;
                var list = new List<StatusPerTimeBlock>();
                foreach (var follower in followers)
                {
                    var statuses = await client.GetStatusesForFollowerId(follower.Id);
                    foreach (var status in statuses)
                    {
                        totalStatuses++;
                        var localTime = status.CreateAtUtc.ToLocalTime();
                        var hour = localTime.Hour;
                        var minute = localTime.Minute;
                        var existingHour = list.FirstOrDefault(x => x.TimeBlock == (byte)hour);
                        if (existingHour == null)
                        {
                            var statusPerHour = new StatusPerTimeBlock(TimeBlockTypes.Hour) { TimeBlock = (byte)hour, StatusCount = 1 };
                            statusPerHour.StatusesPerMinute.Add(new StatusPerTimeBlock(TimeBlockTypes.Minute) { TimeBlock = (byte)minute, StatusCount = 1});
                            list.Add(statusPerHour);
                        }
                        else
                        {
                            existingHour.StatusCount++;
                            var existingMinute =
                                existingHour.StatusesPerMinute.FirstOrDefault(x => x.TimeBlock == (byte)minute);
                            if (existingMinute == null)
                                existingHour.StatusesPerMinute.Add(new StatusPerTimeBlock(TimeBlockTypes.Minute)
                                    { TimeBlock = (byte)minute, StatusCount = 1 });
                            else
                                existingMinute.StatusCount++;
                        }
                    }
                    InProgressValue++;
                }

                var hourProgressBarMax = list.Max(x => x.StatusCount);
                foreach (var statusPerHour in list.OrderBy(x => x.TimeBlock))
                {
                    statusPerHour.TotalStatuses = totalStatuses;
                    statusPerHour.ProgressBarMaximum = hourProgressBarMax;
                    var totalStatusesPerMinute = statusPerHour.StatusesPerMinute.Sum(x => x.StatusCount);
                    var minuteProgressBarMax = statusPerHour.StatusesPerMinute.Max(x => x.StatusCount);
                    foreach (var statusPerMinute in statusPerHour.StatusesPerMinute)
                    {
                        statusPerMinute.TotalStatuses = (uint)totalStatusesPerMinute;
                        statusPerMinute.ProgressBarMaximum = minuteProgressBarMax;
                    }

                    statusPerHour.StatusesPerMinute =
                        statusPerHour.StatusesPerMinute.OrderBy(x => x.TimeBlock).ToList();
                    StatusesPerHour.Add(statusPerHour);
                }
            }
            finally
            {
                EnableControls = true;
            }
        }

        public async Task SetUpdateButtonVisibility(string productVersion)
        {
            var client = new GitHubApiClient();
            UpdateButtonVisibility = await client.IsNewVersionAvailable(productVersion) ? "Visible" : "Collapsed";
        }
    }
}
