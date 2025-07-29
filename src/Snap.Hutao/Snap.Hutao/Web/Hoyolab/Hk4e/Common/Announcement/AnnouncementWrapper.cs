// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;

internal sealed class AnnouncementWrapper : ListWrapper<AnnouncementListWrapper>, IJsonOnDeserialized
{
    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("type_list")]
    public List<AnnouncementType> TypeList { get; set; } = default!;

    [JsonPropertyName("alert")]
    public bool Alert { get; set; }

    [JsonPropertyName("alert_id")]
    public int AlertId { get; set; }

    [JsonPropertyName("timezone")]
    public int TimeZone { get; set; }

    [JsonPropertyName("t")]
    public string TimeStamp { get; set; } = default!;

    public void OnDeserialized()
    {
        TimeSpan offset = TimeSpan.FromHours(TimeZone);

        foreach (AnnouncementListWrapper wrapper in List)
        {
            foreach (Announcement item in wrapper.List)
            {
                item.StartTime = UnsafeDateTimeOffset.AdjustOffsetOnly(item.StartTime, offset);
                item.EndTime = UnsafeDateTimeOffset.AdjustOffsetOnly(item.EndTime, offset);
            }
        }
    }
}