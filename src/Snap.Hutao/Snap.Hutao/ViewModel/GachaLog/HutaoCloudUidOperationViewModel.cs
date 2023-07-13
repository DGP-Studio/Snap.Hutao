// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.ViewModel.GachaLog;

/// <summary>
/// 胡桃云Uid操作视图模型
/// </summary>
internal sealed class HutaoCloudUidOperationViewModel
{
    /// <summary>
    /// 构造一个新的 胡桃云Uid操作视图模型
    /// </summary>
    /// <param name="uid">Uid</param>
    /// <param name="retrieve">获取记录</param>
    /// <param name="delete">删除记录</param>
    public HutaoCloudUidOperationViewModel(string uid, ICommand retrieve, ICommand delete)
    {
        Uid = uid;
        RetrieveCommand = retrieve;
        DeleteCommand = delete;
    }

    public string Uid { get; }

    /// <summary>
    /// 获取云端数据
    /// </summary>
    public ICommand RetrieveCommand { get; }

    /// <summary>
    /// 删除云端数据
    /// </summary>
    public ICommand DeleteCommand { get; }
}