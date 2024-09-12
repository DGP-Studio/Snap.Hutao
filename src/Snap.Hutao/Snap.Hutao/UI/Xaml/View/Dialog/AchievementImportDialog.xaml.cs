// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Model.InterChange.Achievement;
using Snap.Hutao.Service.Achievement;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty("UIAF", typeof(UIAF))]
internal sealed partial class AchievementImportDialog : ContentDialog
{
    private readonly ITaskContext taskContext;

    public AchievementImportDialog(IServiceProvider serviceProvider, UIAF uiaf)
    {
        InitializeComponent();

        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        UIAF = uiaf;
    }

    public async ValueTask<ValueResult<bool, ImportStrategyKind>> GetImportStrategyAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        ContentDialogResult result = await ShowAsync();
        ImportStrategyKind strategy = (ImportStrategyKind)ImportModeSelector.SelectedIndex;

        return new(result == ContentDialogResult.Primary, strategy);
    }
}
