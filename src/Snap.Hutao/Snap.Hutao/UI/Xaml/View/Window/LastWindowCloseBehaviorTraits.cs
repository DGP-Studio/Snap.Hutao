// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service;
using Snap.Hutao.UI.Xaml.View.Dialog;

namespace Snap.Hutao.UI.Xaml.View.Window;

[Service(ServiceLifetime.Transient)]
internal sealed partial class LastWindowCloseBehaviorTraits
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;
    private readonly AppOptions appOptions;

    [GeneratedConstructor]
    public partial LastWindowCloseBehaviorTraits(IServiceProvider serviceProvider);

    public async ValueTask SetAsync(Microsoft.UI.Xaml.Window window)
    {
        LastWindowCloseBehaviorConfigurationDialog dialog = await contentDialogFactory
            .CreateInstanceAsync<LastWindowCloseBehaviorConfigurationDialog>(serviceProvider)
            .ConfigureAwait(false);

        if (await dialog.GetLastWindowCloseBehaviorAsync().ConfigureAwait(false) is not (true, var behavior))
        {
            return;
        }

        await taskContext.SwitchToMainThreadAsync();
        appOptions.LastWindowCloseBehavior.Value = behavior;
        LocalSetting.Set(SettingKeys.IsLastWindowCloseBehaviorSet, true);
        window.Close();
    }
}