// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

namespace Snap.Hutao.View.Dialog;

[DependencyProperty("PromotionDelta", typeof(AvatarPromotionDelta))]
internal sealed partial class CultivatePromotionDeltaBatchDialog : ContentDialog
{
    private readonly ITaskContext taskContext;

    public CultivatePromotionDeltaBatchDialog(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        taskContext = serviceProvider.GetRequiredService<ITaskContext>();

        PromotionDelta = AvatarPromotionDelta.CreateForBaseline();
    }

    public async ValueTask<ValueResult<bool, AvatarPromotionDelta>> GetPromotionDeltaBaselineAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        ContentDialogResult result = await ShowAsync();

        return new(result == ContentDialogResult.Primary, PromotionDelta);
    }
}
