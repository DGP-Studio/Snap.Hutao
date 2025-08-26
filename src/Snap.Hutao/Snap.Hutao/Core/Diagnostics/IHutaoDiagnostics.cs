// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Storage;

namespace Snap.Hutao.Core.Diagnostics;

[SuppressMessage("", "SH001", Justification = "IHutaoDiagnostics must be public in order to be exposed to the scripting environment")]
public interface IHutaoDiagnostics
{
    ApplicationDataContainer LocalSettings { get; }

    ValueTask<int> ExecuteSqlAsync(string sql);

    ApplicationDataCompositeValue MakeApplicationDataCompositeValue(params string[] keys);
}