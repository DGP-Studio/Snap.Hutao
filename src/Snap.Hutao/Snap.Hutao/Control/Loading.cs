// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;

namespace Snap.Hutao.Control;

[TemplateVisualState(Name = "LoadingIn", GroupName = "CommonStates")]
[TemplateVisualState(Name = "LoadingOut", GroupName = "CommonStates")]
[TemplatePart(Name = "ContentGrid", Type = typeof(FrameworkElement))]
internal class Loading : Microsoft.UI.Xaml.Controls.ContentControl
{
    public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register(nameof(IsLoading), typeof(bool), typeof(Loading), new PropertyMetadata(default(bool), IsLoadingPropertyChanged));

    private FrameworkElement? presenter;

    public Loading()
    {
        DefaultStyleKey = typeof(Loading);
        DefaultStyleResourceUri = "ms-appx:///Control/Loading.xaml".ToUri();
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

        if ((bool)e.NewValue)
        {
            control.presenter ??= control.GetTemplateChild("ContentGrid") as FrameworkElement;
        }
        else if (control.presenter is not null)
        {
            XamlMarkupHelper.UnloadObject(control.presenter);
            control.presenter = null;
        }

        control.Update();
    }

    private void Update()
    {
        VisualStateManager.GoToState(this, IsLoading ? "LoadingIn" : "LoadingOut", true);
    }
}