// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Core.Database.Abstraction;
using Snap.Hutao.UI.Xaml.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

/// <summary>
/// 成就存档
/// </summary>
[Table("achievement_archives")]
internal sealed partial class AchievementArchive : ISelectable,
    IAdvancedCollectionViewItem,
    IMappingFrom<AchievementArchive, string>
{
    /// <summary>
    /// 内部Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// 是否选中
    /// </summary>
    public bool IsSelected { get; set; }

    /// <summary>
    /// 创建一个新的存档
    /// </summary>
    /// <param name="name">名称</param>
    /// <returns>新存档</returns>
    public static AchievementArchive From(string name)
    {
        return new() { Name = name };
    }
}