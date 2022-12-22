// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Binding.Cultivation;

/// <summary>
/// 养成物品帮助类
/// </summary>
public static class CultivateItemHelper
{
    /// <summary>
    /// 判断是否为当日物品
    /// </summary>
    /// <param name="itemId">材料Id</param>
    /// <param name="now">时间</param>
    /// <returns>是否为当日物品</returns>
    public static bool IsTodaysMaterial(int itemId, DateTimeOffset now)
    {
        DateTimeOffset utcNow = now.ToUniversalTime();
        utcNow = utcNow.AddHours(4);
        DayOfWeek dayOfWeek = utcNow.DayOfWeek;

        return dayOfWeek switch
        {
            DayOfWeek.Monday or DayOfWeek.Thursday => itemId switch
            {
                104301 or 104302 or 104303 => true, // 「自由」
                104310 or 104311 or 104312 => true, // 「繁荣」
                104320 or 104321 or 104322 => true, // 「浮世」
                104329 or 104330 or 104331 => true, // 「诤言」
                114001 or 114002 or 114003 or 114004 => true, // 高塔孤王
                114013 or 114014 or 114015 or 114016 => true, // 孤云寒林
                114025 or 114026 or 114027 or 114028 => true, // 远海夷地
                114037 or 114038 or 114039 or 114040 => true, // 谧林涓露
                _ => false,
            },
            DayOfWeek.Tuesday or DayOfWeek.Friday => itemId switch
            {
                104304 or 104305 or 104306 => true, // 「抗争」
                104313 or 104314 or 104315 => true, // 「勤劳」
                104323 or 104324 or 104325 => true, // 「风雅」
                104332 or 104333 or 104334 => true, // 「巧思」
                114005 or 114006 or 114007 or 114008 => true, // 凛风奔狼
                114017 or 114018 or 114019 or 114020 => true, // 雾海云间
                114029 or 114030 or 114031 or 114032 => true, // 鸣神御灵
                114041 or 114042 or 114043 or 114044 => true, // 绿洲花园
                _ => false,
            },
            DayOfWeek.Wednesday or DayOfWeek.Saturday => itemId switch
            {
                104307 or 104308 or 104309 => true, // 「诗文」
                104316 or 104317 or 104318 => true, // 「黄金」
                104326 or 104327 or 104328 => true, // 「天光」
                104335 or 104336 or 104337 => true, // 「笃行」
                114009 or 114010 or 114011 or 114012 => true, // 狮牙斗士
                114021 or 114022 or 114023 or 114024 => true, // 漆黑陨铁
                114033 or 114034 or 114035 or 114036 => true, // 今昔剧画
                114045 or 114046 or 114047 or 114048 => true, // 谧林涓露
                _ => false,
            },
            _ => false,
        };
    }
}
