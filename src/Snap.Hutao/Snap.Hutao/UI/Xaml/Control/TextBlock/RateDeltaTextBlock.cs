// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinRT;

namespace Snap.Hutao.UI.Xaml.Control.TextBlock;

[TemplateVisualState(Name = "PositiveValue", GroupName = "CommonStates")]
[TemplateVisualState(Name = "NegativeValue", GroupName = "CommonStates")]
[DependencyProperty<string>("Text", PropertyChangedCallbackName = nameof(OnTextPropertyChanged))]
[DependencyProperty<Style>("TextStyle")]
internal sealed partial class RateDeltaTextBlock : ContentControl
{
    public RateDeltaTextBlock()
    {
        DefaultStyleKey = typeof(RateDeltaTextBlock);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        UpdateState();
    }

    private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        RateDeltaTextBlock control = d.As<RateDeltaTextBlock>();
        control.UpdateState();
    }

    private void UpdateState()
    {
        if (Text is { Length: > 0 } text)
        {
            _ = text.AsSpan()[0] switch
            {
                '+' => VisualStateManager.GoToState(this, "PositiveValue", true),
                '-' => VisualStateManager.GoToState(this, "NegativeValue", true),
                _ => VisualStateManager.GoToState(this, "NoValue", true),
            };
        }
        else
        {
            VisualStateManager.GoToState(this, "NoValue", true);
        }
    }
}