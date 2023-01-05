using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace MastodonFollowerTimes
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private enum BackgroundWorkerProgressStates
        {
            SetMaximum,
            ReportProgress,
            Done
        }

        private readonly BackgroundWorker _backgroundWorker;
        private readonly MainWindow _view;

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

        public MainWindowViewModel(MainWindow view)
        {
            Settings = WpfSettings.Load();
            StatusesPerHour = new ObservableCollection<StatusPerTimeBlock>();
            _view = view;
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.WorkerReportsProgress = true;
            _backgroundWorker.WorkerSupportsCancellation = false;
            _backgroundWorker.ProgressChanged += BackgroundWorkerOnProgressChanged;
            _backgroundWorker.DoWork += BackgroundWorkerOnDoWork;
            EnableControls = true;
        }

        public void LoadData()
        {
            StatusesPerHour.Clear();
            InProgressValue = 0;
            InProgressMaximum = 0;
            EnableControls = false;
            _backgroundWorker.RunWorkerAsync();
        }

        public async Task SetUpdateButtonVisibility(string productVersion)
        {
            var client = new GitHubApiClient();
            UpdateButtonVisibility = await client.IsNewVersionAvailable(productVersion) ? "Visible" : "Collapsed";
        }

        private void BackgroundWorkerOnDoWork(object? sender, DoWorkEventArgs e)
        {
            try
            {
                var client = new MastodonApiClient();
                client.VerifyCredentials(Settings.InstanceUrl, Settings.Token);
                var accountId = client.GetIdForAccountName(Settings.AccountName);
                Settings.Save();

                var followers = client.GetFollowerIdsForAccountId(accountId);
                _backgroundWorker.ReportProgress(followers.Count, BackgroundWorkerProgressStates.SetMaximum);
                var totalStatuses = (uint)0;
                var list = new List<StatusPerTimeBlock>();
                foreach (var follower in followers)
                {
                    var statuses = client.GetStatusesForFollowerId(follower.Id);
                    foreach (var status in statuses)
                    {
                        totalStatuses++;
                        var localTime = status.CreateAtUtc.ToLocalTime();
                        var hour = localTime.Hour;
                        var minute = localTime.Minute;
                        var existingHour = list.FirstOrDefault(x => x.TimeBlock == (byte)hour);
                        if (existingHour == null)
                        {
                            var statusPerHour = new StatusPerTimeBlock { TimeBlock = (byte)hour, StatusCount = 1 };
                            statusPerHour.StatusesPerMinute.Add(new StatusPerTimeBlock { TimeBlock = (byte)minute, StatusCount = 1 });
                            list.Add(statusPerHour);
                        }
                        else
                        {
                            existingHour.StatusCount++;
                            var existingMinute =
                                existingHour.StatusesPerMinute.FirstOrDefault(x => x.TimeBlock == (byte)minute);
                            if (existingMinute == null)
                                existingHour.StatusesPerMinute.Add(new StatusPerTimeBlock
                                { TimeBlock = (byte)minute, StatusCount = 1 });
                            else
                                existingMinute.StatusCount++;
                        }
                    }
                    _backgroundWorker.ReportProgress(-1, BackgroundWorkerProgressStates.ReportProgress);
                }

                var hourProgressBarMax = list.Max(x => x.StatusCount);
                foreach (var statusPerHour in list)
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
                }
                _backgroundWorker.ReportProgress(-1, list.OrderBy(x => x.TimeBlock).ToList());
            }
            catch (Exception ex)
            {
                _backgroundWorker.ReportProgress(-1, ex);
            }
            finally
            {
                _backgroundWorker.ReportProgress(-1, BackgroundWorkerProgressStates.Done);
            }
        }
        private void BackgroundWorkerOnProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is List<StatusPerTimeBlock> list)
            {
                list.ForEach(StatusesPerHour.Add);
                return;
            }

            if (e.UserState is Exception ex)
            {
                ShowExceptionOnUi(ex);
                return;
            }

            var progressType =
                (BackgroundWorkerProgressStates)(e.UserState ??
                                                 throw new InvalidOperationException("Invalid Progress State"));
            switch (progressType)
            {
                case BackgroundWorkerProgressStates.SetMaximum:
                    InProgressMaximum = (uint)e.ProgressPercentage;
                    break;
                case BackgroundWorkerProgressStates.ReportProgress:
                    InProgressValue++;
                    break;
                case BackgroundWorkerProgressStates.Done:
                    EnableControls = true;
                    break;
            }
        }

        private void ShowExceptionOnUi(Exception exception)
        {
            if (exception is ApplicationException ex)
                MessageBox.Show(_view, ex.Message, WindowTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            else
                MessageBox.Show(_view, exception.ToString(), WindowTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
