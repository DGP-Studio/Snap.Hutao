// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.IO.Ini;

internal sealed class IniParameter : IniElement
{
    public IniParameter(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public string Key { get; }

    public string Value { get; private set; }

    public bool Set(string value)
    {
        if (Value == value)
        {
            return false;
        }

        Value = value;
        return true;
    }

    public override string ToString()
    {
        return $"{Key}={Value}";
    }
}
