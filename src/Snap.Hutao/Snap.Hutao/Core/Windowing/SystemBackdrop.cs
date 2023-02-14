// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Snap.Hutao.Control.Theme;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using System.Runtime.InteropServices;
using Windows.System;
using WinRT;

namespace Snap.Hutao.Core.Windowing;

/// <summary>
/// 系统背景帮助类
/// </summary>
internal sealed class SystemBackdrop
{
    private readonly Window window;

    private DispatcherQueueHelper? dispatcherQueueHelper;
    private ISystemBackdropControllerWithTargets? backdropController;
    private SystemBackdropConfiguration? configuration;

    /// <summary>
    /// 构造一个新的系统背景帮助类
    /// </summary>
    /// <param name="window">窗体</param>
    public SystemBackdrop(Window window)
    {
        this.window = window;
        using (IServiceScope scope = Ioc.Default.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            SettingEntry entry = appDbContext.Settings.SingleOrAdd(SettingEntry.SystemBackdropType, BackdropType.Mica.ToString());
            BackdropType = Enum.Parse<BackdropType>(entry.Value!);
        }
    }

    /// <summary>
    /// 背景类型
    /// </summary>
    public BackdropType BackdropType { get; set; }

    /// <summary>
    /// 尝试设置背景
    /// </summary>
    /// <returns>是否设置成功</returns>
    public bool TryApply()
    {
        bool isSupport = BackdropType switch
        {
            BackdropType.Acrylic => DesktopAcrylicController.IsSupported(),
            BackdropType.Mica or BackdropType.MicaAlt => MicaController.IsSupported(),
            _ => false,
        };

        if (!isSupport)
        {
            return false;
        }
        else
        {
            // Previous one
            if (backdropController != null)
            {
                backdropController.RemoveAllSystemBackdropTargets();
            }
            else
            {
                dispatcherQueueHelper = new();
                dispatcherQueueHelper.Ensure();
            }

            // Hooking up the policy object
            configuration = new()
            {
                IsInputActive = true, // Initial configuration state.
            };
            SetConfigurationSourceTheme(configuration);

            window.Activated += OnWindowActivated;
            window.Closed += OnWindowClosed;
            ((FrameworkElement)window.Content).ActualThemeChanged += OnWindowThemeChanged;

            backdropController = BackdropType switch
            {
                BackdropType.Acrylic => new DesktopAcrylicController(),
                BackdropType.Mica => new MicaController() { Kind = MicaKind.Base },
                BackdropType.MicaAlt => new MicaController() { Kind = MicaKind.BaseAlt },
                _ => throw Must.NeverHappen(),
            };

            backdropController.AddSystemBackdropTarget(window.As<ICompositionSupportsSystemBackdrop>());
            backdropController.SetSystemBackdropConfiguration(configuration);

            return true;
        }
    }

    private void OnWindowActivated(object sender, WindowActivatedEventArgs args)
    {
        configuration!.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
    }

    private void OnWindowClosed(object sender, WindowEventArgs args)
    {
        // Make sure any Mica/Acrylic controller is disposed so it doesn't try to
        // use this closed window.
        if (backdropController != null)
        {
            backdropController.Dispose();
            backdropController = null;
        }

        window.Activated -= OnWindowActivated;
        configuration = null;
    }

    private void OnWindowThemeChanged(FrameworkElement sender, object args)
    {
        if (configuration != null)
        {
            SetConfigurationSourceTheme(configuration);
        }
    }

    private void SetConfigurationSourceTheme(SystemBackdropConfiguration configuration)
    {
        configuration.Theme = ThemeHelper.ElementToSystemBackdrop(((FrameworkElement)window.Content).ActualTheme);
    }

    private class DispatcherQueueHelper
    {
        private object? dispatcherQueueController;

        /// <summary>
        /// 确保系统调度队列控制器存在
        /// </summary>
        public unsafe void Ensure()
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
                    DwSize = sizeof(DispatcherQueueOptions),
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

        private struct DispatcherQueueOptions
        {
            internal int DwSize;
            internal int ThreadType;
            internal int ApartmentType;
        }
    }
}