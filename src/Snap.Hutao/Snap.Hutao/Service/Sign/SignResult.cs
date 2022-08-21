// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Sign;

/// <summary>
/// 签到操作结果
/// </summary>
public struct SignResult
{
    /// <summary>
    /// 构造一个新的签到操作结果
    /// </summary>
    /// <param name="totalCount">总次数</param>
    /// <param name="retryCount">重试次数</param>
    public SignResult(int totalCount, int retryCount, TimeSpan time)
    {
        TotalCount = totalCount;
        RetryCount = retryCount;
        Time = time;
    }

    /// <summary>
    /// 总次数
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 重试次数
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// 用时
    /// </summary>
    public TimeSpan Time { get; set; }
}