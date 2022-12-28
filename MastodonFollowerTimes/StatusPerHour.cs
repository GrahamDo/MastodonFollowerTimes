namespace MastodonFollowerTimes;

internal class StatusPerHour
{
    public byte Hour { get; set; }
    public uint StatusCount { get; set; }
    public uint TotalStatuses { get; set; }
    public string PercentString {
        get
        {
            var percent = (float)StatusCount / (float)TotalStatuses;
            return percent.ToString("0.00%");
        }
    }
}