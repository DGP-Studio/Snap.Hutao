// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Scripting;

[SuppressMessage("", "SH001", Justification = "ScriptContext must be public in order to be exposed to the scripting environment")]
public sealed class ScriptContext
{
    public IServiceProvider ServiceProvider { get; } = Ioc.Default;
}