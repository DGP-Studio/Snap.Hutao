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
    public Feature IsDailyNoteSlientVerificationEnabled { get; } = new("IsDailyNoteSlientVerificationEnabled", "启用实时便笺无感验证", "IsDailyNoteSlientVerificationEnabled", true);

    /// <inheritdoc/>
    public int Count { get => 1; }

    /// <inheritdoc/>
    public IEnumerator<Feature> GetEnumerator()
    {
        // TODO: Use source generator
        yield return IsDailyNoteSlientVerificationEnabled;
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}