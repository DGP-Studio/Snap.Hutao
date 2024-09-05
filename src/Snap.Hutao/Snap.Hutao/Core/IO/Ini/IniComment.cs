// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.IO.Ini;

internal sealed class IniComment : IniElement
{
    public IniComment(string comment)
    {
        Comment = comment;
    }

    public string Comment { get; }

    public override string ToString()
    {
        return $";{Comment}";
    }
}