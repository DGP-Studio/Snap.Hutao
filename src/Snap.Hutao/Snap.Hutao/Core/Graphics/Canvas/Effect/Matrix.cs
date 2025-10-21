// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Graphics.Canvas.Effects;

namespace Snap.Hutao.Core.Graphics.Canvas.Effect;

internal static class Matrix
{
    public static Matrix5x4 Create5x4(float[][] values)
    {
        return new()
        {
            M11 = values[0][0],
            M12 = values[0][1],
            M13 = values[0][2],
            M14 = values[0][3],
            M21 = values[1][0],
            M22 = values[1][1],
            M23 = values[1][2],
            M24 = values[1][3],
            M31 = values[2][0],
            M32 = values[2][1],
            M33 = values[2][2],
            M34 = values[2][3],
            M41 = values[3][0],
            M42 = values[3][1],
            M43 = values[3][2],
            M44 = values[3][3],
            M51 = values[4][0],
            M52 = values[4][1],
            M53 = values[4][2],
            M54 = values[4][3],
        };
    }
}