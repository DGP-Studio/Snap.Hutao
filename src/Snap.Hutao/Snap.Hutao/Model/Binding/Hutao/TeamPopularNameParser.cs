// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Binding.Hutao;

/// <summary>
/// 队伍俗名解析器
/// </summary>
internal static class TeamPopularNameParser
{
    /// <summary>
    /// 已知的队伍名称
    /// </summary>
    private static readonly ImmutableDictionary<string, ImmutableHashSet<int>> KnownTeamNames = new Dictionary<string, ImmutableHashSet<int>>()
    {
        ["雷国|雷行香班"] = new HashSet<int>() { AvatarIds.Shougun, AvatarIds.Xingqiu, AvatarIds.Xiangling, AvatarIds.Bennett, }.ToImmutableHashSet(),
        ["雷国|雷夜香班"] = new HashSet<int>() { AvatarIds.Shougun, AvatarIds.Yelan, AvatarIds.Xiangling, AvatarIds.Bennett, }.ToImmutableHashSet(),
        ["雷国|雷万香班"] = new HashSet<int>() { AvatarIds.Shougun, AvatarIds.Kazuha, AvatarIds.Xiangling, AvatarIds.Bennett, }.ToImmutableHashSet(),
        ["雷九|雷九万班"] = new HashSet<int>() { AvatarIds.Shougun, AvatarIds.Sara, AvatarIds.Kazuha, AvatarIds.Bennett, }.ToImmutableHashSet(),
        ["万达国际"] = new HashSet<int>() { AvatarIds.Kazuha, AvatarIds.Tartaglia, AvatarIds.Xiangling, AvatarIds.Bennett, }.ToImmutableHashSet(),
        ["胡行钟夜"] = new HashSet<int>() { AvatarIds.Hutao, AvatarIds.Xingqiu, AvatarIds.Zhongli, AvatarIds.Yelan, }.ToImmutableHashSet(),
        ["激晴|刻皇万妲"] = new HashSet<int>() { AvatarIds.Keqing, AvatarIds.Fischl, AvatarIds.Kazuha, AvatarIds.Nahida, }.ToImmutableHashSet(),
        ["永冻|神鹤万心"] = new HashSet<int>() { AvatarIds.Ayaka, AvatarIds.Shenhe, AvatarIds.Kazuha, AvatarIds.Kokomi, }.ToImmutableHashSet(),
        ["永冻|神钟万心"] = new HashSet<int>() { AvatarIds.Ayaka, AvatarIds.Zhongli, AvatarIds.Kazuha, AvatarIds.Kokomi, }.ToImmutableHashSet(),
        ["融甘|融化甘雨"] = new HashSet<int>() { AvatarIds.Ganyu, AvatarIds.Zhongli, AvatarIds.Xiangling, AvatarIds.Bennett, }.ToImmutableHashSet(),
        ["妮绽放|女主"] = new HashSet<int>() { AvatarIds.Nilou, AvatarIds.Kokomi, AvatarIds.Nahida, AvatarIds.PlayerGirl, }.ToImmutableHashSet(),
        ["妮绽放|男主"] = new HashSet<int>() { AvatarIds.Nilou, AvatarIds.Kokomi, AvatarIds.Nahida, AvatarIds.PlayerBoy, }.ToImmutableHashSet(),
        ["一斗岩队"] = new HashSet<int>() { AvatarIds.Itto, AvatarIds.Albedo, AvatarIds.Zhongli, AvatarIds.Gorou, }.ToImmutableHashSet(),
    }.ToImmutableDictionary();

    /// <summary>
    /// 获取队伍名称
    /// </summary>
    /// <param name="ids">队伍集</param>
    /// <returns>队伍名称</returns>
    public static string GetName(HashSet<int> ids)
    {
        foreach (KeyValuePair<string, ImmutableHashSet<int>> entry in KnownTeamNames)
        {
            if (entry.Value.SetEquals(ids))
            {
                return entry.Key;
            }
        }

        return "尚未命名";
    }
}