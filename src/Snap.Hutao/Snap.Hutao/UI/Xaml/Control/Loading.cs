// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media.Animation;
using System.Runtime.CompilerServices;
using WinRT;

namespace Snap.Hutao.UI.Xaml.Control;

[TemplateVisualState(Name = "LoadingIn", GroupName = "CommonStates")]
[TemplateVisualState(Name = "LoadingOut", GroupName = "CommonStates")]
[TemplatePart(Name = "ContentGrid", Type = typeof(FrameworkElement))]
[TemplatePart(Name = "LoadingOutStoryboard", Type = typeof(Storyboard))]
[DependencyProperty<bool>("IsLoading", DefaultValue = false, NotNull = true, PropertyChangedCallbackName = nameof(IsLoadingPropertyChanged))]
internal sealed partial class Loading : Microsoft.UI.Xaml.Controls.ContentControl
{
    private FrameworkElement? presenter;

    public Loading()
    {
        DefaultStyleKey = typeof(Loading);
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
        Loading control = d.As<Loading>();

        if (Unsafe.Unbox<bool>(e.NewValue))
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
            presenter = default;
        }
    }
}