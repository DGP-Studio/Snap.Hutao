// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.IO.Ini;

internal sealed class IniComment : IniElement, IEquatable<IniComment>
{
    public IniComment(string comment)
    {
        Comment = comment;
    }

    public string Comment { get; }

    public bool Equals(IniComment? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Comment == other.Comment;
    }

    public override string ToString()
    {
        return $";{Comment}";
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as IniComment);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Comment);
    }
}