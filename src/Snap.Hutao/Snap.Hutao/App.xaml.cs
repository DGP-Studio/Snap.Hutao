// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Core.LifeCycle.InterProcess;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Core.Windowing.HotKey;
using Snap.Hutao.Core.Windowing.NotifyIcon;
using System.Diagnostics;

namespace Snap.Hutao;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// This class must be public
/// </summary>
[HighQuality]
[Injection(InjectAs.Singleton)]
[SuppressMessage("", "SH001")]
public sealed partial class App : Application
{
    private const string ConsoleBanner = $"""
        ----------------------------------------------------------------
          _____                         _    _         _ 
         / ____|                       | |  | |       | |
        | (___   _ __    __ _  _ __    | |__| | _   _ | |_  __ _   ___
         \___ \ | '_ \  / _` || '_ \   |  __  || | | || __|/ _` | / _ \
         ____) || | | || (_| || |_) |_ | |  | || |_| || |_| (_| || (_) |
        |_____/ |_| |_| \__,_|| .__/(_)|_|  |_| \__,_| \__|\__,_| \___/
                              | |
                              |_|
        
        Snap.Hutao is a open source software developed by DGP Studio.
        Copyright (C) 2022 - 2024 DGP Studio, All Rights Reserved.
        ----------------------------------------------------------------
        """;

    private readonly IServiceProvider serviceProvider;
    private readonly IActivation activation;
    private readonly ILogger<App> logger;

    /// <summary>
    /// Initializes the singleton application object.
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public App(IServiceProvider serviceProvider)
    {
        // Load app resource
        InitializeComponent();
        activation = serviceProvider.GetRequiredService<IActivation>();
        logger = serviceProvider.GetRequiredService<ILogger<App>>();
        serviceProvider.GetRequiredService<ExceptionRecorder>().Record(this);

        this.serviceProvider = serviceProvider;
    }

    public bool IsExiting { get; private set; }

    public new void Exit()
    {
        IsExiting = true;
        base.Exit();
    }

    /// <inheritdoc/>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            AppActivationArguments activatedEventArgs = AppInstance.GetCurrent().GetActivatedEventArgs();

            if (serviceProvider.GetRequiredService<PrivateNamedPipeClient>().TryRedirectActivationTo(activatedEventArgs))
            {
                Exit();
                return;
            }

            logger.LogColorizedInformation((ConsoleBanner, ConsoleColor.DarkYellow));
            LogDiagnosticInformation();

            // Manually invoke
            activation.Activate(HutaoActivationArguments.FromAppActivationArguments(activatedEventArgs));
            activation.PostInitialization();
        }
        catch
        {
            // AppInstance.GetCurrent() calls failed
            Process.GetCurrentProcess().Kill();
        }
    }

    private void LogDiagnosticInformation()
    {
        RuntimeOptions runtimeOptions = serviceProvider.GetRequiredService<RuntimeOptions>();

        logger.LogColorizedInformation(("FamilyName: {Name}", ConsoleColor.Blue), (runtimeOptions.FamilyName, ConsoleColor.Cyan));
        logger.LogColorizedInformation(("Version: {Version}", ConsoleColor.Blue), (runtimeOptions.Version, ConsoleColor.Cyan));
        logger.LogColorizedInformation(("LocalCache: {Path}", ConsoleColor.Blue), (runtimeOptions.LocalCache, ConsoleColor.Cyan));
    }
}