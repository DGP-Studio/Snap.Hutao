// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.ViewModel.GachaLog;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Hutao.GachaLog;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 胡桃云服务
/// </summary>
internal interface IGachaLogHutaoCloudService
{
    /// <summary>
    /// 异步删除服务器上的祈愿记录
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="token">取消令牌</param>
    /// <returns>是否删除成功</returns>
    ValueTask<ValueResult<bool, string>> DeleteGachaItemsAsync(string uid, CancellationToken token = default);

    /// <summary>
    /// 异步获取祈愿统计信息
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>祈愿统计信息</returns>
    ValueTask<ValueResult<bool, HutaoStatistics>> GetCurrentEventStatisticsAsync(CancellationToken token = default);

    ValueTask<HutaoResponse<List<GachaEntry>>> GetGachaEntriesAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取祈愿记录
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="token">取消令牌</param>
    /// <returns>是否获取成功</returns>
    ValueTask<ValueResult<bool, Guid>> RetrieveGachaItemsAsync(string uid, CancellationToken token = default);

    /// <summary>
    /// 异步上传祈愿记录
    /// </summary>
    /// <param name="gachaArchive">祈愿档案</param>
    /// <param name="token">取消令牌</param>
    /// <returns>是否上传成功</returns>
    ValueTask<ValueResult<bool, string>> UploadGachaItemsAsync(GachaArchive gachaArchive, CancellationToken token = default);
}