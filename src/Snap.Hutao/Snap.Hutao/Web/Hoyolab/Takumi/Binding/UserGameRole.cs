// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Service.User;
using Snap.Hutao.UI.Xaml.Data;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Binding;

internal sealed partial class UserGameRole : ObservableObject, IPropertyValuesProvider
{
    [JsonPropertyName("game_biz")]
    public string GameBiz { get; set; } = default!;

    [JsonPropertyName("region")]
    public Region Region { get; set; }

    [JsonPropertyName("game_uid")]
    public string GameUid { get; set; } = default!;

    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = default!;

    [JsonPropertyName("level")]
    public int Level { get; set; }

    [JsonPropertyName("is_chosen")]
    public bool IsChosen { get; set; }

    [JsonPropertyName("region_name")]
    public string RegionName { get; set; } = default!;

    [JsonPropertyName("is_official")]
    public bool IsOfficial { get; set; }

    [JsonIgnore]
    public string Description
    {
        get => $"{RegionName} | Lv.{Level}";
    }

    [JsonIgnore]
    [ObservableProperty]
    public partial string? ProfilePictureIcon { get; set; }

    public static implicit operator PlayerUid(UserGameRole userGameRole)
    {
        return new(userGameRole.GameUid, userGameRole.Region);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Nickname} | {RegionName} | Lv.{Level}";
    }

    [Command("RefreshProfilePictureCommand")]
    private async Task RefreshProfilePictureAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Refresh profile picture", "UserGameRole.Command"));
        await Ioc.Default.GetRequiredService<IUserService>().RefreshProfilePictureAsync(this).ConfigureAwait(false);
    }
}