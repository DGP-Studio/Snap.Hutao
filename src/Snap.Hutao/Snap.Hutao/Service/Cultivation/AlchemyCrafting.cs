// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Service.Cultivation;

internal static class AlchemyCrafting
{
    public static double UnWeighted(double orangeItems, double purpleItems, double blueItems, double greenItems)
    {
        return Weighted(orangeItems * 27, purpleItems * 9, blueItems * 3, greenItems);
    }

    public static double Weighted(double orangeItems, double purpleItems, double blueItems, double greenItems)
    {
        if (orangeItems > 0)
        {
            if (purpleItems > 0)
            {
                if (blueItems > 0)
                {
                    if (greenItems > 0)
                    {
                        return orangeItems + purpleItems + blueItems + greenItems;
                    }

                    return Delta(orangeItems + purpleItems + blueItems, -greenItems);
                }

                if (greenItems > 0)
                {
                    return Delta(orangeItems + purpleItems, -blueItems) + greenItems;
                }

                return Delta(orangeItems + purpleItems, -(blueItems + greenItems));
            }

            if (blueItems > 0)
            {
                if (greenItems > 0)
                {
                    return Delta(orangeItems, -purpleItems) + blueItems + greenItems;
                }

                return Delta(Delta(orangeItems, -purpleItems) + blueItems, -greenItems);
            }

            if (greenItems > 0)
            {
                return Delta(orangeItems, -(purpleItems + blueItems)) + greenItems;
            }

            return Delta(orangeItems, -(purpleItems + blueItems + greenItems));
        }

        if (purpleItems > 0)
        {
            if (blueItems > 0)
            {
                if (greenItems > 0)
                {
                    return purpleItems + blueItems + greenItems;
                }

                return Delta(purpleItems + blueItems, -greenItems);
            }

            if (greenItems > 0)
            {
                return Delta(purpleItems, -blueItems) + greenItems;
            }

            return Delta(purpleItems, -(blueItems + greenItems));
        }

        if (blueItems > 0)
        {
            if (greenItems > 0)
            {
                return blueItems + greenItems;
            }

            return Delta(blueItems, -greenItems);
        }

        if (greenItems > 0)
        {
            return greenItems;
        }

        return 0D;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double Delta(double expect, double actual)
    {
        return expect - Math.Min(expect, actual);
    }
}