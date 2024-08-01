// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.IO;

internal readonly struct ValueFile
{
    private readonly string value;

    private ValueFile(string value)
    {
        this.value = value;
    }

    public static implicit operator string(ValueFile value)
    {
        return value.value;
    }

    public static implicit operator ValueFile(string value)
    {
        return new(value);
    }

    [SuppressMessage("", "CA1307")]
    public override int GetHashCode()
    {
        return value.GetHashCode();
    }

    public override string ToString()
    {
        return value;
    }
}