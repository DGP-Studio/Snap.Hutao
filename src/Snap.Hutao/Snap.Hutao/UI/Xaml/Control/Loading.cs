// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media.Animation;

namespace Snap.Hutao.UI.Xaml.Control;

[TemplateVisualState(Name = "LoadingIn", GroupName = "CommonStates")]
[TemplateVisualState(Name = "LoadingOut", GroupName = "CommonStates")]
[TemplatePart(Name = "ContentGrid", Type = typeof(FrameworkElement))]
[TemplatePart(Name = "LoadingOutStoryboard", Type = typeof(Storyboard))]
internal sealed partial class Loading : Microsoft.UI.Xaml.Controls.ContentControl
{
    public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register(nameof(IsLoading), typeof(bool), typeof(Loading), new PropertyMetadata(default(bool), IsLoadingPropertyChanged));

    private FrameworkElement? presenter;

    public Loading()
    {
        DefaultStyleKey = typeof(Loading);
    }

    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        if (GetTemplateChild("LoadingOutStoryboard") is Storyboard storyboard)
        {
            storyboard.Completed -= UnloadPresenter;
            storyboard.Completed += UnloadPresenter;
        }

        Update();
    }

    private static void IsLoadingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Loading control = (Loading)d;

        if ((bool)e.NewValue)
        {
            control.presenter ??= control.GetTemplateChild("ContentGrid") as FrameworkElement;
        }

        control.Update();
    }

    private void Update()
    {
        VisualStateManager.GoToState(this, IsLoading ? "LoadingIn" : "LoadingOut", true);
    }

    private void UnloadPresenter(object? sender, object? args)
    {
        if (presenter is not null)
        {
            XamlMarkupHelper.UnloadObject(presenter);
        }
    }
}