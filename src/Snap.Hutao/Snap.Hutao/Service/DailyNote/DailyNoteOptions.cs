// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Abstraction;

namespace Snap.Hutao.Service.DailyNote;

/// <summary>
/// 实时便笺选项
/// </summary>
[Injection(InjectAs.Singleton)]
internal sealed class DailyNoteOptions : DbStoreOptions
{
    /// <summary>
    /// 构造一个新的实时便笺选项
    /// </summary>
    /// <param name="serviceScopeFactory">服务范围工厂</param>
    public DailyNoteOptions(IServiceScopeFactory serviceScopeFactory)
        : base(serviceScopeFactory)
    {
    }
}