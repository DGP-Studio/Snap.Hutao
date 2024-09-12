// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Core.Database.Abstraction;
using Snap.Hutao.UI.Xaml.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

[Table("cultivate_projects")]
internal sealed partial class CultivateProject : ISelectable,
    IAdvancedCollectionViewItem,
    IMappingFrom<CultivateProject, string>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    public bool IsSelected { get; set; }

    public string Name { get; set; } = default!;

    public static CultivateProject From(string name)
    {
        return new() { Name = name };
    }
}