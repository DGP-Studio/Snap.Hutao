// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppNotifications.Builder;
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
    private readonly IServiceScope scope;

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
    }

    public FrameworkElement TitleBarCaptionAccess { get => TitleView.DragArea; }

    public IEnumerable<FrameworkElement> TitleBarPassthrough { get => TitleView.Passthrough; }

    public SizeInt32 InitSize { get => ScaledSizeInt32.CreateForWindow(1200, 741, this); }

    public void OnWindowClosing(out bool cancel)
    {
        cancel = false;
    }

    public void OnWindowClosed()
    {
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
}