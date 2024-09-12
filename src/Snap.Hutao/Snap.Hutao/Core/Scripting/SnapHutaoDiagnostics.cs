// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Storage;

namespace Snap.Hutao.Core.Scripting;

[Injection(InjectAs.Singleton, typeof(ISnapHutaoDiagnostics))]
internal sealed class SnapHutaoDiagnostics : ISnapHutaoDiagnostics
{
    public async ValueTask<string> GetPathFromApplicationUrlAsync(string url)
    {
        StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(url.ToUri());
        return file.Path;
    }
}