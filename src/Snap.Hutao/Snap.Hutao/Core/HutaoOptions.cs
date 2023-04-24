// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Options;
using Microsoft.Win32;
using Snap.Hutao.Core.Setting;
using System.IO;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Snap.Hutao.Core;

/// <summary>
/// 胡桃选项
/// 存储环境相关的选项
/// </summary>
[Injection(InjectAs.Singleton)]
internal sealed class HutaoOptions : IOptions<HutaoOptions>
{
    /// <summary>
    /// 构造一个新的胡桃选项
    /// </summary>
    public HutaoOptions()
    {
        DataFolder = GetDataFolderPath();
        LocalCache = ApplicationData.Current.LocalCacheFolder.Path;
        InstalledLocation = Package.Current.InstalledLocation.Path;
        FamilyName = Package.Current.Id.FamilyName;

        Version = Package.Current.Id.Version.ToVersion();
        UserAgent = $"Snap Hutao/{Version}";

        DeviceId = GetUniqueUserId();
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
    /// 数据文件夹路径
    /// </summary>
    public string DataFolder { get; }

    /// <summary>
    /// 安装位置
    /// </summary>
    public string InstalledLocation { get; }

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

    /// <inheritdoc/>
    public HutaoOptions Value { get => this; }

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
}