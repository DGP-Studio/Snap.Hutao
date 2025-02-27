// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.UI.Xaml.Control.Panel;

internal static class EqualPanelAlgorithm
{
    public static double GetTotalLength(double itemLength, int itemsCount, double spacing)
    {
        return Math.Max(0, ((itemLength + spacing) * itemsCount) - spacing);
    }

    public static double GetItemLength(double totalLength, int itemsCount, double spacing)
    {
        return Math.Max(0, ((totalLength + spacing) / itemsCount) - spacing);
    }
}