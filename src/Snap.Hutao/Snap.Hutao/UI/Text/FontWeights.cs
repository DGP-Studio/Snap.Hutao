// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.UI.Text;

namespace Snap.Hutao.UI.Text;

internal sealed class FontWeights
{
    public static FontWeight ExtraBlack { get; } = new(950);

    public static FontWeight Black { get; } = new(900);

    public static FontWeight ExtraBold { get; } = new(800);

    public static FontWeight Bold { get; } = new(700);

    public static FontWeight SemiBold { get; } = new(600);

    public static FontWeight Medium { get; } = new(500);

    public static FontWeight Normal { get; } = new(400);

    public static FontWeight SemiLight { get; } = new(350);

    public static FontWeight Light { get; } = new(300);

    public static FontWeight ExtraLight { get; } = new(200);

    public static FontWeight Thin { get; } = new(100);
}