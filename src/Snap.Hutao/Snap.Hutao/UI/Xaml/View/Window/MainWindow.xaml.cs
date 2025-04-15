// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppNotifications.Builder;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service;
using Snap.Hutao.UI.Shell;
using Snap.Hutao.UI.Windowing.Abstraction;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.ViewModel;
using Windows.Graphics;

namespace Snap.Hutao.UI.Xaml.View.Window;

[Injection(InjectAs.Transient)]
internal sealed partial class MainWindow : Microsoft.UI.Xaml.Window,
    IXamlWindowClosedHandler,
    IXamlWindowExtendContentIntoTitleBar,
    IXamlWindowHasInitSize
{
    private readonly IServiceScope scope;

    private readonly IContentDialogFactory contentDialogFactory;
    private readonly ITaskContext taskContext;
    private readonly AppOptions appOptions;
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

        scope = serviceProvider.CreateScope();
        this.InitializeController(scope.ServiceProvider);
        TitleView.InitializeDataContext<TitleViewModel>(scope.ServiceProvider);
        MainView.InitializeDataContext<MainViewModel>(scope.ServiceProvider);
        contentDialogFactory = scope.ServiceProvider.GetRequiredService<IContentDialogFactory>();
        taskContext = scope.ServiceProvider.GetRequiredService<ITaskContext>();
        appOptions = scope.ServiceProvider.GetRequiredService<AppOptions>();
        app = scope.ServiceProvider.GetRequiredService<App>();
    }

    public FrameworkElement TitleBarCaptionAccess { get => TitleView.DragArea; }

    public IEnumerable<FrameworkElement> TitleBarPassthrough { get => TitleView.Passthrough; }

    public SizeInt32 InitSize { get => ScaledSizeInt32.CreateForWindow(1200, 741, this); }

    public void OnWindowClosing(out bool cancel)
    {
        if (!XamlApplicationLifetime.Exiting && XamlApplicationLifetime.NotifyIconCreated && !LocalSetting.Get(SettingKeys.IsCloseButtonBehaviorSet, false))
        {
            SetCloseButtonBehaviorAsync().SafeForget();
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

        if (!XamlApplicationLifetime.NotifyIconCreated || appOptions.CloseButtonBehavior is CloseButtonBehavior.Exit)
        {
            app.Exit();
            return;
        }

        if (!NotifyIcon.IsPromoted(scope.ServiceProvider))
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

    private async ValueTask SetCloseButtonBehaviorAsync()
    {
        CloseButtonBehaviorSetDialog dialog = await contentDialogFactory
            .CreateInstanceAsync<CloseButtonBehaviorSetDialog>(scope.ServiceProvider)
            .ConfigureAwait(false);

        (bool isOk, CloseButtonBehavior behavior) = await dialog.GetCloseButtonBehaviorAsync().ConfigureAwait(false);
        if (!isOk)
        {
            return;
        }

        await taskContext.SwitchToMainThreadAsync();
        appOptions.CloseButtonBehavior = behavior;
        LocalSetting.Set(SettingKeys.IsCloseButtonBehaviorSet, true);
        Close();
    }
}