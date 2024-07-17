// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Service.User;
using Snap.Hutao.UI.Xaml.Data;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Binding;

internal sealed partial class UserGameRole : ObservableObject, IAdvancedCollectionViewItem
{
    private string? profilePictureIcon;

    [JsonPropertyName("game_biz")]
    public string GameBiz { get; set; } = default!;

    [JsonPropertyName("region")]
    public Region Region { get; set; } = default!;

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
    public bool IsOfficial { get; set; } = default!;

    [JsonIgnore]
    public string Description
    {
        get => $"{RegionName} | Lv.{Level}";
    }

    [JsonIgnore]
    public string? ProfilePictureIcon
    {
        get => profilePictureIcon;
        set => SetProperty(ref profilePictureIcon, value);
    }

    public static implicit operator PlayerUid(UserGameRole userGameRole)
    {
        return new PlayerUid(userGameRole.GameUid, userGameRole.Region);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Nickname} | {RegionName} | Lv.{Level}";
    }

    [Command("RefreshProfilePictureCommand")]
    private async Task RefreshProfilePictureAsync()
    {
        await Ioc.Default.GetRequiredService<IUserService>().RefreshProfilePictureAsync(this).ConfigureAwait(false);
    }
}