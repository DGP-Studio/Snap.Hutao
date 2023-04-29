// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Metadata.Avatar;

/// <summary>
/// 描述与参数
/// </summary>
[HighQuality]
internal sealed class DescriptionsParameters
{
    /// <summary>
    /// 描述
    /// </summary>
    public List<string> Descriptions { get; set; } = default!;

    /// <summary>
    /// 参数
    /// </summary>
    public List<LevelParameters<int, float>> Parameters { get; set; } = default!;
}
