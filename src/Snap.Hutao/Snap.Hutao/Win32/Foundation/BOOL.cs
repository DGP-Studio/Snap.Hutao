﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Foundation;

internal readonly struct BOOL
{
    public static readonly BOOL TRUE = 1;

    public readonly int Value;

    public BOOL(bool value) => Value = value ? 1 : 0;

    public static unsafe implicit operator int(BOOL value) => *(int*)&value;

    public static unsafe implicit operator BOOL(int value) => *(BOOL*)&value;

    public static implicit operator BOOL(bool value) => new(value);

    public static implicit operator bool(BOOL value) => value != 0;
}