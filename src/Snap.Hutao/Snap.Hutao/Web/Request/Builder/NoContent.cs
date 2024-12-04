// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Request.Builder;

[Serializable]
internal readonly struct NoContent : IEquatable<NoContent>
{
    public static bool operator ==(NoContent a, NoContent b)
    {
        return Equals(a, b);
    }

    public static bool operator !=(NoContent a, NoContent b)
    {
        return !(a == b);
    }

    public override bool Equals(object? obj)
    {
        return obj is NoContent;
    }

    public bool Equals(NoContent other)
    {
        return true;
    }

    public override int GetHashCode()
    {
        return 1;
    }
}