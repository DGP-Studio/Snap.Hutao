// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.UI.Xaml;

[Flags]
internal enum Theme : byte
{
    BaseMask = 0x03,
    None = 0x00,
    Light = 0x01,
    Dark = 0x02,
    HighContrastMask = 0x1C,
    HighContrastNone = 0x00,
    HighContrast = 0x04,
    HighContrastWhite = 0x08,
    HighContrastBlack = 0x0C,
    HighContrastCustom = 0x10,
}