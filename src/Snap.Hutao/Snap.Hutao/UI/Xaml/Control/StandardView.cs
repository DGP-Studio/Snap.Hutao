// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.UI.Xaml.Control;

[TemplateVisualState(Name = "Show", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Empty", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Hide", GroupName = "CommonStates")]
[DependencyProperty<UIElement>("EmptyContent")]
[DependencyProperty<bool>("ShowCondition", DefaultValue = false, PropertyChangedCallbackName = nameof(OnShowConditionChanged), NotNull = true)]
[DependencyProperty<bool>("HideCondition", DefaultValue = false, PropertyChangedCallbackName = nameof(OnHideConditionChanged), NotNull = true)]
internal sealed partial class StandardView : ContentControl
{
    public StandardView()
    {
        DefaultStyleKey = typeof(StandardView);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        UpdateState();
    }

    private static void OnShowConditionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is StandardView view)
        {
            view.UpdateState();
        }
    }

    private static void OnHideConditionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is StandardView view)
        {
            view.UpdateState();
        }
    }

    private void UpdateState()
    {
        if (HideCondition)
        {
            VisualStateManager.GoToState(this, "Hide", true);
            return;
        }

        if (ShowCondition)
        {
            VisualStateManager.GoToState(this, "Show", true);
        }
        else
        {
            VisualStateManager.GoToState(this, "Empty", true);
        }
    }
}