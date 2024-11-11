// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.UI.Windowing.Abstraction;
using Snap.Hutao.Web.WebView2;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using Windows.Graphics;
using static Snap.Hutao.Win32.Macros;
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.UI.Xaml.View.Window.WebView2;

[SuppressMessage("", "CA1001")]
[INotifyPropertyChanged]
internal sealed partial class CompactWebView2Window : Microsoft.UI.Xaml.Window,
    IXamlWindowExtendContentIntoTitleBar,
    IXamlWindowRectPersisted,
    IXamlWindowClosedHandler
{
    private readonly CancellationTokenSource loadCts = new();

    private readonly IServiceScope windowScope;
    private readonly IWebView2ContentProvider contentProvider;
    private readonly InputPointerSource inputPointerSource;
    private readonly InputNonClientPointerSource inputNonClientPointerSource;

    private bool isPointerInClientArea;
    private bool isPointerInNonClientArea;
    private bool isLocked;

    public CompactWebView2Window(IWebView2ContentProvider contentProvider)
    {
        windowScope = Ioc.Default.CreateScope();

        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.SetBorderAndTitleBar(true, false);
            presenter.IsAlwaysOnTop = true;
        }

        SetTitleBar(CustomTitleBar);

        this.contentProvider = contentProvider;
        contentProvider.CloseWindowAction = Close;

        InitializeComponent();
        RootGrid.DataContext = this;

        inputPointerSource = this.GetInputPointerSource();
        inputPointerSource.PointerEntered += OnWindowPointerEntered;
        inputPointerSource.PointerExited += OnWindowPointerExited;

        inputNonClientPointerSource = this.GetInputNonClientPointerSource();
        inputNonClientPointerSource.PointerEntered += OnWindowNonClientPointerEntered;
        inputNonClientPointerSource.PointerExited += OnWindowNonClientPointerExited;

        WebView.Loaded += OnWebViewLoaded;
        WebView.Unloaded += OnWebViewUnloaded;

        this.InitializeController(windowScope.ServiceProvider);

        this.AddExStyleLayered();
        SetLayeredWindowAttributes(this.GetWindowHandle(), RGB(0, 0, 0), 128, LAYERED_WINDOW_ATTRIBUTES_FLAGS.LWA_COLORKEY | LAYERED_WINDOW_ATTRIBUTES_FLAGS.LWA_ALPHA);
    }

    public FrameworkElement TitleBarCaptionAccess { get => TitleArea; }

    public string Source
    {
        get => WebView.Source?.ToString() ?? string.Empty;
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                string url = value.StartsWith("https://", StringComparison.OrdinalIgnoreCase) || value.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                    ? value
                    : $"https://{value}";

                if (Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
                {
                    SetProperty(WebView.Source, uri, WebView, static (view, v) => view.Source = v);
                }
            }
        }
    }

    public IEnumerable<FrameworkElement> TitleBarPassthrough
    {
        get { yield return SourceTextBox; }
    }

    public string PersistRectKey { get => SettingKeys.CompactWebView2WindowRect; }

    public string PersistScaleKey { get => SettingKeys.CompactWebView2WindowScale; }

    public SizeInt32 InitSize { get => new(800, 600); }

    public SizeInt32 MinSize { get => new(200, 200); }

    public bool IsLocked { get => isLocked; private set => SetProperty(ref isLocked, value, nameof(IsLockedVisibility)); }

    public Visibility IsLockedVisibility { get => IsLocked ? Visibility.Collapsed : Visibility.Visible; }

    public void OnWindowClosed()
    {
        inputPointerSource.PointerEntered -= OnWindowPointerEntered;
        inputPointerSource.PointerExited -= OnWindowPointerExited;
        inputNonClientPointerSource.PointerEntered -= OnWindowNonClientPointerEntered;
        inputNonClientPointerSource.PointerExited -= OnWindowNonClientPointerExited;
        windowScope.Dispose();
    }

    private void OnWindowPointerEntered(InputPointerSource source, PointerEventArgs args)
    {
        isPointerInClientArea = true;
        UpdateLayeredWindow();
    }

    private void OnWindowNonClientPointerEntered(InputNonClientPointerSource source, NonClientPointerEventArgs args)
    {
        isPointerInNonClientArea = true;
        UpdateLayeredWindow();
    }

    private void OnWindowPointerExited(InputPointerSource source, PointerEventArgs args)
    {
        isPointerInClientArea = false;
        UpdateLayeredWindow();
    }

    private void OnWindowNonClientPointerExited(InputNonClientPointerSource source, NonClientPointerEventArgs args)
    {
        isPointerInNonClientArea = false;
        UpdateLayeredWindow();
    }

    private void UpdateLayeredWindow()
    {
        bool isPointerInWindow = isPointerInClientArea || isPointerInNonClientArea;
        if (isPointerInWindow)
        {
            this.RemoveExStyleLayered();
            RootGrid.Height = 48;
        }
        else
        {
            this.AddExStyleLayered();
            SetLayeredWindowAttributes(this.GetWindowHandle(), RGB(0, 0, 0), 128, LAYERED_WINDOW_ATTRIBUTES_FLAGS.LWA_COLORKEY | LAYERED_WINDOW_ATTRIBUTES_FLAGS.LWA_ALPHA);

            if (IsLocked)
            {
                RootGrid.Height = 0;
            }
        }
    }

    [Command("GoBackCommand")]
    private void GoBack()
    {
        if (WebView.CoreWebView2.CanGoBack)
        {
            WebView.CoreWebView2.GoBack();
        }
    }

    [Command("RefreshCommand")]
    private void Refresh()
    {
        WebView.CoreWebView2.Reload();
    }

    [Command("SwitchLockCommand")]
    private void SwitchLock()
    {
        IsLocked = !IsLocked;
    }

    [Command("CloseCommand")]
    private void CloseWindow()
    {
        Close();
    }

    private void OnWebViewLoaded(object sender, RoutedEventArgs e)
    {
        _ = OnWebViewLoadedAsync();

        [SuppressMessage("", "SH003")]
        async Task OnWebViewLoadedAsync()
        {
            await WebView.EnsureCoreWebView2Async();
            WebView.CoreWebView2.DocumentTitleChanged += OnDocumentTitleChanged;
            WebView.CoreWebView2.DownloadStarting += OnDownloadStarting;
            WebView.CoreWebView2.SourceChanged += OnSourceChanged;
            WebView.CoreWebView2.HistoryChanged += OnHistoryChanged;
            WebView.CoreWebView2.NewWindowRequested += OnNewWindowRequested;
            WebView.CoreWebView2.DisableDevToolsForReleaseBuild();
            contentProvider.CoreWebView2 = WebView.CoreWebView2;
            await contentProvider.InitializeAsync(windowScope.ServiceProvider, loadCts.Token).ConfigureAwait(false);
        }
    }

    private void OnWebViewUnloaded(object sender, RoutedEventArgs e)
    {
        loadCts.Cancel();
        loadCts.Dispose();
        contentProvider.Unload();

        if (WebView.CoreWebView2 is not null)
        {
            WebView.CoreWebView2.DocumentTitleChanged -= OnDocumentTitleChanged;
            WebView.CoreWebView2.SourceChanged -= OnSourceChanged;
            WebView.CoreWebView2.DownloadStarting -= OnDownloadStarting;
            WebView.CoreWebView2.HistoryChanged -= OnHistoryChanged;
        }

        WebView.Loaded -= OnWebViewLoaded;
        WebView.Unloaded -= OnWebViewUnloaded;
    }

    private void OnDocumentTitleChanged(CoreWebView2 sender, object args)
    {
        DocumentTitle.Text = sender.DocumentTitle;
    }

    private void OnHistoryChanged(CoreWebView2 sender, object args)
    {
        GoBackButton.IsEnabled = sender.CanGoBack;
    }

    private void OnSourceChanged(CoreWebView2 sender, CoreWebView2SourceChangedEventArgs args)
    {
        OnPropertyChanged(nameof(Source));
    }

    private void OnDownloadStarting(CoreWebView2 sender, CoreWebView2DownloadStartingEventArgs args)
    {
        args.Cancel = true;
    }

    private void OnNewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs args)
    {
        args.Handled = true;
        ((CoreWebView2)sender!).Navigate(args.Uri);
    }

    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        contentProvider.ActualTheme = sender.ActualTheme;
    }
}
