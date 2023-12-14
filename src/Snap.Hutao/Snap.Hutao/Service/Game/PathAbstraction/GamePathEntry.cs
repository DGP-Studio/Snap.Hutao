// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.PathAbstraction;

internal sealed class GamePathEntry
{
    [JsonPropertyName("Path")]
    public string Path { get; set; } = default!;

    [JsonIgnore]
    public GamePathKind Kind { get => GetKind(Path); }

    public static GamePathEntry Create(string path)
    {
        return new()
        {
            Path = path,
        };
    }

    private static GamePathKind GetKind(string path)
    {
        return GamePathKind.None;
    }
}