// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hutao.GachaLog;

namespace Snap.Hutao.ViewModel.GachaLog;

/// <summary>
/// 胡桃云记录操作视图模型
/// </summary>
internal sealed class HutaoCloudEntryOperationViewModel
{
    /// <summary>
    /// 构造一个新的胡桃云记录操作视图模型
    /// </summary>
    /// <param name="entry">记录</param>
    /// <param name="retrieve">获取记录</param>
    /// <param name="delete">删除记录</param>
    public HutaoCloudEntryOperationViewModel(GachaEntry entry, ICommand? retrieve, ICommand delete)
    {
        Uid = entry.Uid;
        ItemCount = entry.ItemCount;
        RetrieveCommand = retrieve;
        DeleteCommand = delete;
    }

    public string Uid { get; }

    public int ItemCount { get; }

    /// <summary>
    /// 获取云端数据
    /// </summary>
    public ICommand? RetrieveCommand { get; }

    /// <summary>
    /// 删除云端数据
    /// </summary>
    public ICommand DeleteCommand { get; }
}