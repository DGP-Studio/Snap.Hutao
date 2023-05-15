﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Options;
using Microsoft.Web.WebView2.Core;
using Microsoft.Win32;
using Snap.Hutao.Core.Setting;
using System.IO;
using System.Security.Principal;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Snap.Hutao.Core;

/// <summary>
/// 胡桃选项
/// 存储环境相关的选项
/// 运行时运算得到的选项，无数据库交互
/// </summary>
[Injection(InjectAs.Singleton)]
internal sealed class HutaoOptions : IOptions<HutaoOptions>
{
    private readonly ILogger<HutaoOptions> logger;

    private readonly bool isWebView2Supported;
    private readonly string webView2Version = SH.CoreWebView2HelperVersionUndetected;

    private bool? isElevated;

    /// <summary>
    /// 构造一个新的胡桃选项
    /// </summary>
    /// <param name="logger">日志器</param>
    public HutaoOptions(ILogger<HutaoOptions> logger)
    {
        this.logger = logger;

        DataFolder = GetDataFolderPath();
        LocalCache = ApplicationData.Current.LocalCacheFolder.Path;
        InstalledLocation = Package.Current.InstalledLocation.Path;
        FamilyName = Package.Current.Id.FamilyName;

        Version = Package.Current.Id.Version.ToVersion();
        UserAgent = $"Snap Hutao/{Version}";

        DeviceId = GetUniqueUserId();
        DetectWebView2Environment(ref webView2Version, ref isWebView2Supported);
    }

    /// <summary>
    /// 当前版本
    /// </summary>
    public Version Version { get; }

    /// <summary>
    /// 标准UA
    /// </summary>
    public string UserAgent { get; }

    /// <summary>
    /// 安装位置
    /// </summary>
    public string InstalledLocation { get; }

    /// <summary>
    /// 数据文件夹路径
    /// </summary>
    public string DataFolder { get; }

    /// <summary>
    /// 本地缓存
    /// </summary>
    public string LocalCache { get; }

    /// <summary>
    /// 包家族名称
    /// </summary>
    public string FamilyName { get; }

    /// <summary>
    /// 设备Id
    /// </summary>
    public string DeviceId { get; }

    /// <summary>
    /// WebView2 版本
    /// </summary>
    public string WebView2Version { get => webView2Version; }

    /// <summary>
    /// 是否支持 WebView2
    /// </summary>
    public bool IsWebView2Supported { get => isWebView2Supported; }

    /// <summary>
    /// 是否为提升的权限
    /// </summary>
    public bool IsElevated { get => isElevated ??= GetElevated(); }

    /// <inheritdoc/>
    public HutaoOptions Value { get => this; }

    /// <summary>
    /// 以管理员模式重启
    /// </summary>
    /// <returns>任务</returns>
    public async ValueTask RestartAsElevatedAsync()
    {
        if (!IsElevated)
        {
            if (Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.Machine)!
                .Split(';')
                .Any(path => path.EndsWith("System32\\WindowsPowerShell\\v1.0", StringComparison.OrdinalIgnoreCase)))
            {
                // TODO: throw exception
                return;
            }

            string arguments = $"/c start powershell.exe " +
                $"-ExecutionPolicy Bypass -Command \"Start-Process " +
                $"-Verb RunAs " +
                $"-FilePath 'shell:AppsFolder\\{Package.Current.Id.FamilyName}!App' \"";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = true,
                WorkingDirectory = Environment.CurrentDirectory,
                FileName = "cmd.exe",
                Arguments = arguments,
            };

            using (Process process = new Process { StartInfo = startInfo })
            {
                process.Start();
            }

            await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

            Process.GetCurrentProcess().Kill();
        }
    }

    private static string GetDataFolderPath()
    {
        string preferredPath = LocalSetting.Get(SettingKeys.DataFolderPath, string.Empty);

        if (!string.IsNullOrEmpty(preferredPath) && Directory.Exists(preferredPath))
        {
            return preferredPath;
        }

        string myDocument = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
#if RELEASE
        // 将测试版与正式版的文件目录分离
        string folderName = Package.Current.PublisherDisplayName == "DGP Studio CI" ? "HutaoAlpha" : "Hutao";
#else
        // 使得迁移能正常生成
        string folderName = "Hutao";
#endif
        string path = Path.GetFullPath(Path.Combine(myDocument, folderName));
        Directory.CreateDirectory(path);
        return path;
    }

    private static string GetUniqueUserId()
    {
        string userName = Environment.UserName;
        object? machineGuid = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography\", "MachineGuid", userName);
        return Convert.ToMd5HexString($"{userName}{machineGuid}");
    }

    private static bool GetElevated()
    {
#if DEBUG_AS_FAKE_ELEVATED
        return true;
#else
        using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
        {
            WindowsPrincipal principal = new(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
#endif
    }

    private void DetectWebView2Environment(ref string webView2Version, ref bool isWebView2Supported)
    {
        try
        {
            webView2Version = CoreWebView2Environment.GetAvailableBrowserVersionString();
            isWebView2Supported = true;
        }
        catch (FileNotFoundException ex)
        {
            logger.LogError(ex, "WebView2 Runtime not installed.");
        }
    }
}