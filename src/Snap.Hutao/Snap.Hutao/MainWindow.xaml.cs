// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Core.Windowing;
using Snap.Hutao.Message;
using Windows.Graphics;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Snap.Hutao;

/// <summary>
/// 主窗体
/// </summary>
[HighQuality]
[Injection(InjectAs.Singleton)]
[SuppressMessage("", "CA1001")]
internal sealed partial class MainWindow : Window, IWindowOptionsSource, IRecipient<WelcomeStateCompleteMessage>
{
    private const int MinWidth = 848;
    private const int MinHeight = 524;

    private readonly WindowOptions windowOptions;

    /// <summary>
    /// 构造一个新的主窗体
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public MainWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        windowOptions = new(this, TitleBarView.DragArea, new(1200, 741), true);
        ExtendedWindow<MainWindow>.Initialize(this, serviceProvider);
        serviceProvider.GetRequiredService<IMessenger>().Register(this);

        // If not complete we should present the welcome view.
        ContentSwitchPresenter.Value = StaticResource.IsAnyUnfulfilledContractPresent();
    }

    /// <inheritdoc/>
    public WindowOptions WindowOptions { get => windowOptions; }

    /// <inheritdoc/>
    public unsafe void ProcessMinMaxInfo(MINMAXINFO* pInfo, double scalingFactor)
    {
        pInfo->ptMinTrackSize.X = (int)Math.Max(MinWidth * scalingFactor, pInfo->ptMinTrackSize.X);
        pInfo->ptMinTrackSize.Y = (int)Math.Max(MinHeight * scalingFactor, pInfo->ptMinTrackSize.Y);
    }

    /// <inheritdoc/>
    public void Receive(WelcomeStateCompleteMessage message)
    {
        ContentSwitchPresenter.Value = false;
    }
}