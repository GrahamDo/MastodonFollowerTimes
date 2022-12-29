using System.Collections.Generic;

namespace MastodonFollowerTimes;

internal class StatusPerTimeBlock
{
    public byte TimeBlock { get; set; }
    public uint StatusCount { get; set; }
    public uint TotalStatuses { get; set; }
    public uint ProgressBarMaximum { get; set; }
    public string PercentString {
        get
        {
            var percent = (float)StatusCount / (float)TotalStatuses;
            return percent.ToString("0.00%");
        }
    }
    public List<StatusPerTimeBlock> StatusesPerMinute { get; set; }

    public StatusPerTimeBlock()
    {
        StatusesPerMinute = new List<StatusPerTimeBlock>();
    }
}