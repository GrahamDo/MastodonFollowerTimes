using System.Collections.Generic;

namespace MastodonFollowerTimes;

internal class StatusPerTimeBlock
{
    private readonly TimeBlockTypes _timeBlockType;

    public byte TimeBlock { get; set; }
    public uint StatusCount { get; set; }
    public uint TotalStatuses { get; set; }
    public uint ProgressBarMaximum { get; set; }
    public string ProgressBarTooltip {
        get
        {
            var percent = (float)StatusCount / (float)TotalStatuses;
            return $"{_timeBlockType} {TimeBlock}: {percent:0.00%}";
        }
    }
    public List<StatusPerTimeBlock> StatusesPerMinute { get; set; }

    public StatusPerTimeBlock(TimeBlockTypes type)
    {
        _timeBlockType = type;
        StatusesPerMinute = new List<StatusPerTimeBlock>();
    }
}