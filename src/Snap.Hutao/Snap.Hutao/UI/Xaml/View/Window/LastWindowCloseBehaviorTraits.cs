﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service;
using Snap.Hutao.UI.Xaml.View.Dialog;

namespace Snap.Hutao.UI.Xaml.View.Window;

[Service(ServiceLifetime.Transient)]
[ConstructorGenerated]
internal sealed partial class LastWindowCloseBehaviorTraits
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;
    private readonly AppOptions appOptions;

    public async ValueTask SetAsync(Microsoft.UI.Xaml.Window window)
    {
        LastWindowCloseBehaviorConfigurationDialog dialog = await contentDialogFactory
            .CreateInstanceAsync<LastWindowCloseBehaviorConfigurationDialog>(serviceProvider)
            .ConfigureAwait(false);

        if (await dialog.GetLastWindowCloseBehaviorAsync().ConfigureAwait(false) is not (true, { } behavior))
        {
            return;
        }

        await taskContext.SwitchToMainThreadAsync();
        appOptions.LastWindowCloseBehavior = behavior;
        LocalSetting.Set(SettingKeys.IsLastWindowCloseBehaviorSet, true);
        window.Close();
    }
}