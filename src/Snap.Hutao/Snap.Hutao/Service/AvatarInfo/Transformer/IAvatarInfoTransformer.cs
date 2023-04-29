// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.AvatarInfo.Transformer;

/// <summary>
/// 角色信息转换器
/// </summary>
/// <typeparam name="TSource">源类型</typeparam>
[HighQuality]
internal interface IAvatarInfoTransformer<TSource>
{
    /// <summary>
    /// 合并到角色信息
    /// </summary>
    /// <param name="avatarInfo">基底，角色Id必定存在</param>
    /// <param name="source">源</param>
    void Transform(ref Web.Enka.Model.AvatarInfo avatarInfo, TSource source);
}