// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Metadata;

/// <summary>
/// 元数据上下文
/// </summary>
internal interface IMetadataContext
{
    /// <summary>
    /// 获取对应元数据类型的列表
    /// </summary>
    /// <typeparam name="TMetadata">元数据类型</typeparam>
    /// <param name="name">名称，留空则使用类型名称</param>
    /// <returns>对应元数据类型的列表</returns>
    List<TMetadata> List<TMetadata>(string name = default!);

    /// <summary>
    /// 获取对应元数据类型的映射
    /// </summary>
    /// <typeparam name="Tkey">键类型</typeparam>
    /// <typeparam name="TMetadata">元数据类型</typeparam>
    /// <param name="keySelector">键选择器</param>
    /// <param name="name">名称，留空则使用类型名称</param>
    /// <returns>对应元数据类型的映射</returns>
    Dictionary<Tkey, TMetadata> Map<Tkey, TMetadata>(Func<TMetadata, Tkey> keySelector, string name = default!)
        where Tkey : notnull;

    /// <summary>
    /// 获取对应元数据类型的映射
    /// </summary>
    /// <typeparam name="Tkey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <typeparam name="TMetadata">元数据类型</typeparam>
    /// <param name="keySelector">键选择器</param>
    /// <param name="dataSelector">值选择器</param>
    /// <param name="name">名称，留空则使用类型名称</param>
    /// <returns>对应元数据类型的映射</returns>
    Dictionary<Tkey, TValue> Map<Tkey, TValue, TMetadata>(Func<TMetadata, Tkey> keySelector, Func<TMetadata, TValue> dataSelector, string name = default!)
        where Tkey : notnull;
}