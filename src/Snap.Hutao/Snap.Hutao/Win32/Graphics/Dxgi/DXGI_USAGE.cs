// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Graphics.Dxgi;

internal enum DXGI_USAGE : uint
{
    DXGI_USAGE_SHADER_INPUT = 0x10U,
    DXGI_USAGE_RENDER_TARGET_OUTPUT = 0x20U,
    DXGI_USAGE_BACK_BUFFER = 0x40U,
    DXGI_USAGE_SHARED = 0x80U,
    DXGI_USAGE_READ_ONLY = 0x100U,
    DXGI_USAGE_DISCARD_ON_PRESENT = 0x200U,
    DXGI_USAGE_UNORDERED_ACCESS = 0x400U,
}