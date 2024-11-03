// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.Database.Abstraction;
using Snap.Hutao.Model.Entity.Abstraction;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.UI.Xaml.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

[Table("game_accounts")]
internal sealed partial class GameAccount : ObservableObject,
    IAppDbEntity,
    IReorderable,
    IAdvancedCollectionViewItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    public SchemeType Type { get; set; }

    public string Name { get; set; } = default!;

    /// <summary>
    /// [MIHOYOSDK_ADL_PROD_CN_h3123967166]
    /// [MIHOYOSDK_ADL_PROD_OVERSEA_h1158948810]
    /// </summary>
    public string MihoyoSDK { get; set; } = default!;

    public int Index { get; set; }

    public static GameAccount From(string name, string sdk, SchemeType type)
    {
        return new()
        {
            Name = name,
            MihoyoSDK = sdk,
            Type = type,
        };
    }

    public void UpdateName(string name)
    {
        Name = name;
        OnPropertyChanged($"{nameof(Name)}");
    }
}