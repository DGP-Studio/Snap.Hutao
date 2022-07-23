// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Logging;

/// <summary>
/// 事件Id定义
/// </summary>
internal static class EventIds
{
    // 异常

    /// <summary>
    /// 未经处理的异常
    /// </summary>
    public static readonly EventId UnhandledException = 100000;

    /// <summary>
    /// Forget任务执行异常
    /// </summary>
    public static readonly EventId TaskException = 100001;

    /// <summary>
    /// 异步命令执行异常
    /// </summary>
    public static readonly EventId AsyncCommandException = 100002;

    /// <summary>
    /// WebView2环境异常
    /// </summary>
    public static readonly EventId WebView2EnvironmentException = 100003;

    // 服务

    /// <summary>
    /// 导航历史
    /// </summary>
    public static readonly EventId NavigationHistory = 100100;

    /// <summary>
    /// 导航失败
    /// </summary>
    public static readonly EventId NavigationFailed = 100101;

    /// <summary>
    /// 元数据初始化过程
    /// </summary>
    public static readonly EventId MetadataInitialization = 100110;

    /// <summary>
    /// 元数据文件MD5检查
    /// </summary>
    public static readonly EventId MetadataFileMD5Check = 100111;

    // 杂项

    /// <summary>
    /// 杂项Log
    /// </summary>
    public static readonly EventId CommonLog = 200000;

    /// <summary>
    /// 背景状态
    /// </summary>
    public static readonly EventId BackdropState = 200001;
}