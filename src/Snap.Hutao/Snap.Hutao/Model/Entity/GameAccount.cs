// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Model.Entity.Primitive;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

/// <summary>
/// 游戏内账号
/// </summary>
[HighQuality]
[Table("game_accounts")]
internal sealed class GameAccount : ObservableObject
{
    /// <summary>
    /// 内部Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    /// <summary>
    /// 对应的Uid
    /// </summary>
    public string? AttachUid { get; set; }

    /// <summary>
    /// 服务器类型
    /// </summary>
    public SchemeType Type { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// [MIHOYOSDK_ADL_PROD_CN_h3123967166]
    /// </summary>
    public string MihoyoSDK { get; set; } = default!;

    /// <summary>
    /// 构造一个新的游戏内账号
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="sdk">sdk</param>
    /// <returns>游戏内账号</returns>
    public static GameAccount Create(string name, string sdk)
    {
        return new()
        {
            Name = name,
            MihoyoSDK = sdk,
        };
    }

    /// <summary>
    /// 更新绑定的Uid
    /// </summary>
    /// <param name="uid">uid</param>
    public void UpdateAttachUid(string? uid)
    {
        AttachUid = uid;
        OnPropertyChanged(nameof(AttachUid));
    }

    /// <summary>
    /// 更新名称
    /// </summary>
    /// <param name="name">新名称</param>
    public void UpdateName(string name)
    {
        Name = name;
        OnPropertyChanged($"{nameof(Name)}");
    }
}