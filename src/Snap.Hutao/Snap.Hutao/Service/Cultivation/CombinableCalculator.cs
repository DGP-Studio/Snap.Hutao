// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Cultivation;

internal static class CombinableCalculator
{
    /// <summary>
    /// The parameters should be passed after unifying the measurement unit.
    /// </summary>
    /// <param name="orangeItems">Multiply by 27</param>
    /// <param name="purpleItems">Multiply by 9</param>
    /// <param name="blueItems">Multiply by 3</param>
    /// <param name="greenItems">The base unit</param>
    public static double Calculate(double orangeItems, double purpleItems, double blueItems, double greenItems)
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

                    return CalculateDelta(orangeItems + purpleItems + blueItems, -greenItems);
                }

                if (greenItems > 0)
                {
                    return CalculateDelta(orangeItems + purpleItems, -blueItems) + greenItems;
                }

                return CalculateDelta(orangeItems + purpleItems, -(blueItems + greenItems));
            }

            if (blueItems > 0)
            {
                if (greenItems > 0)
                {
                    return CalculateDelta(orangeItems, -purpleItems) + blueItems + greenItems;
                }

                return CalculateDelta(CalculateDelta(orangeItems, -purpleItems) + blueItems, -greenItems);
            }

            if (greenItems > 0)
            {
                return CalculateDelta(orangeItems, -(purpleItems + blueItems)) + greenItems;
            }

            return CalculateDelta(orangeItems, -(purpleItems + blueItems + greenItems));
        }

        if (purpleItems > 0)
        {
            if (blueItems > 0)
            {
                if (greenItems > 0)
                {
                    return purpleItems + blueItems + greenItems;
                }

                return CalculateDelta(purpleItems + blueItems, -greenItems);
            }

            if (greenItems > 0)
            {
                return CalculateDelta(purpleItems, -blueItems) + greenItems;
            }

            return CalculateDelta(purpleItems, -(blueItems + greenItems));
        }

        if (blueItems > 0)
        {
            if (greenItems > 0)
            {
                return blueItems + greenItems;
            }

            return CalculateDelta(blueItems, -greenItems);
        }

        if (greenItems > 0)
        {
            return greenItems;
        }

        return 0D;
    }

    private static double CalculateDelta(double expect, double actual)
    {
        return expect - Math.Min(expect, actual);
    }
}