// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.AvatarInfo.Composer;

/// <summary>
/// 角色信息合并器
/// </summary>
/// <typeparam name="TSource">源类型</typeparam>
internal interface IAvatarInfoComposer<TSource>
{
    /// <summary>
    /// 合并到角色信息
    /// </summary>
    /// <param name="avatarInfo">基底，角色Id必定存在</param>
    /// <param name="source">源</param>
    /// <returns>任务</returns>
    ValueTask<Web.Enka.Model.AvatarInfo> ComposeAsync(Web.Enka.Model.AvatarInfo avatarInfo, TSource source);
}