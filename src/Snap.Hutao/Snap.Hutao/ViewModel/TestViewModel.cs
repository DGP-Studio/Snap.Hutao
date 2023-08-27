// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Windows.AppLifecycle;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 测试视图模型
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class TestViewModel : Abstraction.ViewModel
{
    /// <inheritdoc/>
    protected override Task OpenUIAsync()
    {
        return Task.CompletedTask;
    }

    [Command("RestartAppCommand")]
    private static void RestartApp(bool elevated)
    {
        AppInstance.Restart(string.Empty);
    }
}