// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Bbs.Home;

internal sealed class GameReceptionDataConfiguration
{
    [JsonPropertyName("id")]
    public required int Id { get; init; }

    [JsonPropertyName("game_id")]
    public required int GameId { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("description")]
    public required string Description { get; init; }

    [JsonPropertyName("icon")]
    public required string Icon { get; init; }

    [JsonPropertyName("status")]
    public required string Status { get; init; }

    [JsonPropertyName("rules")]
    public required JsonElement Rules { get; init; }

    [JsonPropertyName("questionnaire")]
    public required JsonElement Questionnaire { get; init; }

    [JsonPropertyName("pkg")]
    public required JsonElement Package { get; init; }

    [JsonPropertyName("detail_servlet")]
    public required JsonElement DetailServlet { get; init; }

    [JsonPropertyName("pre_register_count")]
    public required JsonElement PreRegisterCount { get; init; }

    [JsonPropertyName("is_top")]
    public required bool IsTop { get; init; }

    [JsonPropertyName("last_update_time")]
    public required string LastUpdateTime { get; init; }

    [JsonPropertyName("app_store_switch")]
    public required int AppStoreSwitch { get; init; }

    [JsonPropertyName("download_switch")]
    public required int DownloadSwitch { get; init; }

    [JsonPropertyName("developer")]
    public required string Developer { get; init; }

    [JsonPropertyName("banner_card")]
    public required JsonElement BannerCard { get; init; }
}