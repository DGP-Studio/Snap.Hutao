// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.ViewModel.GachaLog;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 胡桃云服务
/// </summary>
internal interface IHutaoCloudService
{
    /// <summary>
    /// 异步删除服务器上的祈愿记录
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="token">取消令牌</param>
    /// <returns>是否删除成功</returns>
    Task<ValueResult<bool, string>> DeleteGachaItemsAsync(string uid, CancellationToken token = default);

    /// <summary>
    /// 异步获取祈愿统计信息
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>祈愿统计信息</returns>
    Task<ValueResult<bool, HutaoStatistics>> GetCurrentEventStatisticsAsync(CancellationToken token = default(CancellationToken));

    /// <summary>
    /// 异步获取服务器上的 Uid 列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>服务器上的 Uid 列表</returns>
    Task<Response<List<string>>> GetUidsAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取祈愿记录
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="token">取消令牌</param>
    /// <returns>是否获取成功</returns>
    Task<ValueResult<bool, GachaArchive?>> RetrieveGachaItemsAsync(string uid, CancellationToken token = default);

    /// <summary>
    /// 异步上传祈愿记录
    /// </summary>
    /// <param name="gachaArchive">祈愿档案</param>
    /// <param name="token">取消令牌</param>
    /// <returns>是否上传成功</returns>
    Task<ValueResult<bool, string>> UploadGachaItemsAsync(GachaArchive gachaArchive, CancellationToken token = default);
}