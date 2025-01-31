// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;

namespace Snap.Hutao.Core.LifeCycle.InterProcess.Yae;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal readonly struct YaePropertyTypeValue
{
#pragma warning disable CS0649
    public readonly InterestedPropType Type;
    public readonly double Value;
#pragma warning restore CS0649
}