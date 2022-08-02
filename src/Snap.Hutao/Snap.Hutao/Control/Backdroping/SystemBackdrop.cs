// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using System.Runtime.InteropServices;
using Windows.System;
using WinRT;

namespace Snap.Hutao.Control.HostBackdrop;

/// <summary>
/// 系统背景帮助类
/// </summary>
public class SystemBackdrop
{
    private readonly Window window;

    private DispatcherQueueHelper? dispatcherQueueHelper;
    private MicaController? backdropController;
    private SystemBackdropConfiguration? configurationSource;

    /// <summary>
    /// 构造一个新的系统背景帮助类
    /// </summary>
    /// <param name="window">窗体</param>
    public SystemBackdrop(Window window)
    {
        this.window = window;
    }

    /// <summary>
    /// 尝试设置背景
    /// </summary>
    /// <returns>是否设置成功</returns>
    public bool TrySetBackdrop()
    {
        if (!MicaController.IsSupported())
        {
            return false;
        }
        else
        {
            dispatcherQueueHelper = new DispatcherQueueHelper();
            dispatcherQueueHelper.EnsureWindowsSystemDispatcherQueueController();

            // Hooking up the policy object
            configurationSource = new SystemBackdropConfiguration();
            window.Activated += WindowActivated;
            window.Closed += WindowClosed;
            ((FrameworkElement)window.Content).ActualThemeChanged += WindowThemeChanged;

            // Initial configuration state.
            configurationSource.IsInputActive = true;
            SetConfigurationSourceTheme();

            backdropController = new MicaController();

            backdropController.AddSystemBackdropTarget(window.As<ICompositionSupportsSystemBackdrop>());
            backdropController.SetSystemBackdropConfiguration(configurationSource);

            return true;
        }
    }

    private void WindowActivated(object sender, WindowActivatedEventArgs args)
    {
        Must.NotNull(configurationSource!);
        configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
    }

    private void WindowClosed(object sender, WindowEventArgs args)
    {
        // Make sure any Mica/Acrylic controller is disposed so it doesn't try to
        // use this closed window.
        if (backdropController != null)
        {
            backdropController.Dispose();
            backdropController = null;
        }

        window.Activated -= WindowActivated;
        configurationSource = null;
    }

    private void WindowThemeChanged(FrameworkElement sender, object args)
    {
        if (configurationSource != null)
        {
            SetConfigurationSourceTheme();
        }
    }

    private void SetConfigurationSourceTheme()
    {
        Must.NotNull(configurationSource!).Theme = ((FrameworkElement)window.Content).ActualTheme switch
        {
            ElementTheme.Default => SystemBackdropTheme.Default,
            ElementTheme.Light => SystemBackdropTheme.Light,
            ElementTheme.Dark => SystemBackdropTheme.Dark,
            _ => throw Must.NeverHappen(),
        };
    }

    private class DispatcherQueueHelper
    {
        private object? dispatcherQueueController = null;

        /// <summary>
        /// 确保系统调度队列控制器存在
        /// </summary>
        public void EnsureWindowsSystemDispatcherQueueController()
        {
            if (DispatcherQueue.GetForCurrentThread() != null)
            {
                // one already exists, so we'll just use it.
                return;
            }

            if (dispatcherQueueController == null)
            {
                DispatcherQueueOptions options = new()
                {
                    DwSize = Marshal.SizeOf(typeof(DispatcherQueueOptions)),
                    ThreadType = 2,    // DQTYPE_THREAD_CURRENT
                    ApartmentType = 2, // DQTAT_COM_STA
                };

                _ = CreateDispatcherQueueController(options, ref dispatcherQueueController);
            }
        }

        [DllImport("CoreMessaging.dll")]
        private static extern int CreateDispatcherQueueController(
            [In] DispatcherQueueOptions options,
            [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object? dispatcherQueueController);

        [StructLayout(LayoutKind.Sequential)]
        private struct DispatcherQueueOptions
        {
            internal int DwSize;
            internal int ThreadType;
            internal int ApartmentType;
        }
    }
}