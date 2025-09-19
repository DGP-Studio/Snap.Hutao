// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.PathAbstraction;

internal sealed class GamePathEntry
{
    [JsonPropertyName("Path")]
    public string Path { get; init; } = default!;

    public static GamePathEntry Create(string path)
    {
        return new()
        {
            Path = path,
        };
    }

    public override string ToString()
    {
        return $"{{ Path = {Path} }}";
    }
}