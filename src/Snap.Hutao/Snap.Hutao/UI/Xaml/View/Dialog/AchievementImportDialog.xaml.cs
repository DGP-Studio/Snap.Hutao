// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model.InterChange.Achievement;
using Snap.Hutao.Service.Achievement;
using Snap.Hutao.UI.Xaml.View.Window;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty<UIAF>("UIAF")]
[ConstructorGenerated(InitializeComponent = true)]
internal sealed partial class AchievementImportDialog : ContentDialog
{
    private readonly IContentDialogFactory<MainWindow> contentDialogFactory;

    public AchievementImportDialog(IServiceProvider serviceProvider, UIAF uiaf)
        : this(serviceProvider)
    {
        UIAF = uiaf;
    }

    public async ValueTask<ValueResult<bool, ImportStrategyKind>> GetImportStrategyAsync()
    {
        ContentDialogResult result = await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false);
        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();
        return new(result is ContentDialogResult.Primary, (ImportStrategyKind)ImportModeSelector.SelectedIndex);
    }
}