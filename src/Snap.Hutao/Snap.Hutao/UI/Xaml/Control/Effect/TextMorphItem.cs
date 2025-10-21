// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.UI.Xaml.Control.Effect;

internal sealed class TextMorphItem
{
    public required string Text { get; init; }

    public required DoubleTimeline Timeline { get; init; }
}