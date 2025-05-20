// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppNotifications.Builder;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service;
using Snap.Hutao.UI.Shell;
using Snap.Hutao.UI.Windowing.Abstraction;
using Snap.Hutao.ViewModel;
using Windows.Graphics;

namespace Snap.Hutao.UI.Xaml.View.Window;

[Injection(InjectAs.Transient)]
internal sealed partial class MainWindow : Microsoft.UI.Xaml.Window,
    IXamlWindowClosedHandler,
    IXamlWindowExtendContentIntoTitleBar,
    IXamlWindowHasInitSize
{
    private readonly LastWindowCloseBehaviorTraits closeBehaviorTraits;
    private readonly App app;

    public MainWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            SizeInt32 minSize = ScaledSizeInt32.CreateForWindow(1000, 600, this);
            presenter.PreferredMinimumWidth = minSize.Width;
            presenter.PreferredMinimumHeight = minSize.Height;
        }

        IServiceScope scope = serviceProvider.CreateScope();
        this.InitializeController(scope.ServiceProvider);
        TitleView.InitializeDataContext<TitleViewModel>(scope.ServiceProvider);
        MainView.InitializeDataContext<MainViewModel>(scope.ServiceProvider);
        closeBehaviorTraits = scope.ServiceProvider.GetRequiredService<LastWindowCloseBehaviorTraits>();
        app = scope.ServiceProvider.GetRequiredService<App>();
    }

    public FrameworkElement TitleBarCaptionAccess { get => TitleView.DragArea; }

    public IEnumerable<FrameworkElement> TitleBarPassthrough { get => TitleView.Passthrough; }

    public SizeInt32 InitSize { get => ScaledSizeInt32.CreateForWindow(1200, 741, this); }

    public void OnWindowClosing(out bool cancel)
    {
        if (!XamlApplicationLifetime.Exiting && XamlApplicationLifetime.NotifyIconCreated && !LocalSetting.Get(SettingKeys.IsLastWindowCloseBehaviorSet, false))
        {
            closeBehaviorTraits.SetAsync(this).SafeForget();
            cancel = true;
            return;
        }

        cancel = false;
    }

    public void OnWindowClosed()
    {
        if (XamlApplicationLifetime.Exiting)
        {
            return;
        }

        if (!XamlApplicationLifetime.NotifyIconCreated || app.Options.LastWindowCloseBehavior is LastWindowCloseBehavior.ExitApplication)
        {
            app.Exit();
            return;
        }

        if (this.TryGetAssociatedServiceProvider(out IServiceProvider serviceProvider) && !serviceProvider.GetRequiredService<NotifyIconController>().IsPromoted())
        {
            try
            {
                new AppNotificationBuilder()
                    .AddText(SH.CoreWindowingNotifyIconPromotedHint)
                    .Show();
            }
            catch
            {
                // Ignore
            }
        }
    }
}