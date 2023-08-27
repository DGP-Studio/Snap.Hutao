// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata;

/// <summary>
/// 角色ID
/// </summary>
[HighQuality]
internal static class AvatarIds
{
    // 此处的变量名称以 UI_AvatarIcon 为准
    public static readonly AvatarId Ayaka = 10000002;
    public static readonly AvatarId Qin = 10000003;

    public static readonly AvatarId PlayerBoy = 10000005;
    public static readonly AvatarId Lisa = 10000006;
    public static readonly AvatarId PlayerGirl = 10000007;

    public static readonly AvatarId Barbara = 10000014;
    public static readonly AvatarId Kaeya = 10000015;
    public static readonly AvatarId Diluc = 10000016;

    public static readonly AvatarId Razor = 10000020;
    public static readonly AvatarId Ambor = 10000021;
    public static readonly AvatarId Venti = 10000022;
    public static readonly AvatarId Xiangling = 10000023;
    public static readonly AvatarId Beidou = 10000024;
    public static readonly AvatarId Xingqiu = 10000025;
    public static readonly AvatarId Xiao = 10000026;
    public static readonly AvatarId Ningguang = 10000027;

    public static readonly AvatarId Klee = 10000029;
    public static readonly AvatarId Zhongli = 10000030;
    public static readonly AvatarId Fischl = 10000031;
    public static readonly AvatarId Bennett = 10000032;
    public static readonly AvatarId Tartaglia = 10000033;
    public static readonly AvatarId Noel = 10000034;
    public static readonly AvatarId Qiqi = 10000035;
    public static readonly AvatarId Chongyun = 10000036;
    public static readonly AvatarId Ganyu = 10000037;
    public static readonly AvatarId Albedo = 10000038;
    public static readonly AvatarId Diona = 10000039;

    public static readonly AvatarId Mona = 10000041;
    public static readonly AvatarId Keqing = 10000042;
    public static readonly AvatarId Sucrose = 10000043;
    public static readonly AvatarId Xinyan = 10000044;
    public static readonly AvatarId Rosaria = 10000045;
    public static readonly AvatarId Hutao = 10000046;
    public static readonly AvatarId Kazuha = 10000047;
    public static readonly AvatarId Feiyan = 10000048;
    public static readonly AvatarId Yoimiya = 10000049;
    public static readonly AvatarId Tohma = 10000050;
    public static readonly AvatarId Eula = 10000051;
    public static readonly AvatarId Shougun = 10000052;
    public static readonly AvatarId Sayu = 10000053;
    public static readonly AvatarId Kokomi = 10000054;
    public static readonly AvatarId Gorou = 10000055;
    public static readonly AvatarId Sara = 10000056;
    public static readonly AvatarId Itto = 10000057;
    public static readonly AvatarId Yae = 10000058;
    public static readonly AvatarId Heizo = 10000059;
    public static readonly AvatarId Yelan = 10000060;
    public static readonly AvatarId Kirara = 10000061;
    public static readonly AvatarId Aloy = 10000062;
    public static readonly AvatarId Shenhe = 10000063;
    public static readonly AvatarId Yunjin = 10000064;
    public static readonly AvatarId Shinobu = 10000065;
    public static readonly AvatarId Ayato = 10000066;
    public static readonly AvatarId Collei = 10000067;
    public static readonly AvatarId Dori = 10000068;
    public static readonly AvatarId Tighnari = 10000069;
    public static readonly AvatarId Nilou = 10000070;
    public static readonly AvatarId Cyno = 10000071;
    public static readonly AvatarId Candace = 10000072;
    public static readonly AvatarId Nahida = 10000073;
    public static readonly AvatarId Layla = 10000074;
    public static readonly AvatarId Wanderer = 10000075;
    public static readonly AvatarId Faruzan = 10000076;
    public static readonly AvatarId Yaoyao = 10000077;
    public static readonly AvatarId Alhaitham = 10000078;
    public static readonly AvatarId Dehya = 10000079;
    public static readonly AvatarId Mika = 10000080;
    public static readonly AvatarId Kaveh = 10000081;
    public static readonly AvatarId Baizhuer = 10000082;
    public static readonly AvatarId Lynette = 10000083;
    public static readonly AvatarId Lyney = 10000084;
    public static readonly AvatarId Freminet = 10000085;
    public static readonly AvatarId Wriothesley = 10000086;
    public static readonly AvatarId Neuvillette = 10000087;

    /// <summary>
    /// 检查该角色是否为主角
    /// </summary>
    /// <param name="avatarId">角色Id</param>
    /// <returns>角色是否为主角</returns>
    public static bool IsPlayer(in AvatarId avatarId)
    {
        return avatarId == PlayerBoy || avatarId == PlayerGirl;
    }

    /// <summary>
    /// 复制一个映射并加入旅行者的基本信息
    /// </summary>
    /// <param name="idAvatarMap">映射</param>
    /// <returns>加入旅行者基本信息的映射</returns>
    public static Dictionary<AvatarId, Avatar.Avatar> WithPlayers(Dictionary<AvatarId, Avatar.Avatar> idAvatarMap)
    {
        return new(idAvatarMap)
        {
            [PlayerBoy] = new()
            {
                Name = SH.ModelMetadataAvatarPlayerName,
                Icon = "UI_AvatarIcon_PlayerBoy",
                SideIcon = "UI_AvatarIcon_Side_PlayerBoy",
                Quality = Intrinsic.QualityType.QUALITY_ORANGE,
            },

            [PlayerGirl] = new()
            {
                Name = SH.ModelMetadataAvatarPlayerName,
                Icon = "UI_AvatarIcon_PlayerGirl",
                SideIcon = "UI_AvatarIcon_Side_PlayerGirl",
                Quality = Intrinsic.QualityType.QUALITY_ORANGE,
            },
        };
    }
}