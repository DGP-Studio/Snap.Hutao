// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.Control;

[TemplateVisualState(Name = "LoadingIn", GroupName = "CommonStates")]
[TemplateVisualState(Name = "LoadingOut", GroupName = "CommonStates")]
internal class Loading : Microsoft.UI.Xaml.Controls.ContentControl
{
    public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register(nameof(IsLoading), typeof(bool), typeof(Loading), new PropertyMetadata(default(bool), IsLoadingPropertyChanged));

    [SuppressMessage("", "IDE0052")]
    private FrameworkElement? presenter;

    public Loading()
    {
        DefaultStyleKey = typeof(Loading);
        DefaultStyleResourceUri = new("ms-appx:///Control/Loading.xaml");
    }

    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        Update();
    }

    private static void IsLoadingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Loading control = (Loading)d;
        control.presenter ??= control.GetTemplateChild("ContentGrid") as FrameworkElement;

        control?.Update();
    }

    private void Update()
    {
        VisualStateManager.GoToState(this, IsLoading ? "LoadingIn" : "LoadingOut", true);
    }
}