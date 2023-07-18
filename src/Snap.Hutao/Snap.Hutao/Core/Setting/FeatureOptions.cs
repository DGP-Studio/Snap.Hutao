// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections;

namespace Snap.Hutao.Core.Setting;

/// <summary>
/// 功能选项
/// </summary>
internal sealed class FeatureOptions : IReadOnlyCollection<Feature>
{
    /// <summary>
    /// 启用实时便笺无感验证
    /// </summary>
    public Feature IsDailyNoteSilentVerificationEnabled { get; } = new(
        "IsDailyNoteSilentVerificationEnabled", "启用实时便笺无感验证", "IsDailyNoteSilentVerificationEnabled", true);

    /// <summary>
    /// 元数据检查是否忽略
    /// </summary>
    public Feature IsMetadataUpdateCheckSuppressed { get; } = new(
        "IsMetadataUpdateCheckSuppressed", "禁用元数据更新检查", "IsMetadataUpdateCheckSuppressed", false);

    /// <inheritdoc/>
    public int Count { get => 2; }

    /// <inheritdoc/>
    public IEnumerator<Feature> GetEnumerator()
    {
        // TODO: Use source generator
        yield return IsDailyNoteSilentVerificationEnabled;
        yield return IsMetadataUpdateCheckSuppressed;
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}