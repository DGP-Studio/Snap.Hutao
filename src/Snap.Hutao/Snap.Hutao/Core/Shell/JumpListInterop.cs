// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.UI.StartScreen;

namespace Snap.Hutao.Core.Shell;

[Injection(InjectAs.Transient, typeof(IJumpListInterop))]
internal sealed class JumpListInterop : IJumpListInterop
{
    public async ValueTask ClearAsync()
    {
        if (JumpList.IsSupported())
        {
            JumpList list = await JumpList.LoadCurrentAsync();

            list.Items.Clear();

            await list.SaveAsync();
        }
    }
}