// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Scripting;

public sealed class ScriptContext
{
    public IServiceProvider ServiceProvider { get; } = Ioc.Default;
}