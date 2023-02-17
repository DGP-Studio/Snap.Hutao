// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Model.Entity;

namespace Snap.Hutao.Core.Database;

/// <summary>
/// 设置帮助类
/// </summary>
[HighQuality]
internal static class SettingEntryExtension
{
    /// <summary>
    /// 获取或添加一个对应的设置
    /// </summary>
    /// <param name="dbSet">设置集</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>设置</returns>
    public static SettingEntry SingleOrAdd(this DbSet<SettingEntry> dbSet, string key, string value)
    {
        SettingEntry? entry = dbSet.SingleOrDefault(entry => key == entry.Key);

        if (entry == null)
        {
            entry = new(key, value);
            dbSet.AddAndSave(entry);
        }

        return entry;
    }

    /// <summary>
    /// 获取或添加一个对应的设置
    /// </summary>
    /// <param name="dbSet">设置集</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>设置</returns>
    public static async Task<SettingEntry> SingleOrAddAsync(this DbSet<SettingEntry> dbSet, string key, string value)
    {
        SettingEntry? entry = await dbSet.SingleOrDefaultAsync(entry => key == entry.Key).ConfigureAwait(false);

        if (entry == null)
        {
            entry = new(key, value);
            await dbSet.AddAndSaveAsync(entry).ConfigureAwait(false);
        }

        return entry;
    }

    /// <summary>
    /// 获取 Boolean 值
    /// </summary>
    /// <param name="entry">设置</param>
    /// <returns>值</returns>
    public static bool GetBoolean(this SettingEntry entry)
    {
        return bool.Parse(entry.Value!);
    }

    /// <summary>
    /// 设置 Boolean 值
    /// </summary>
    /// <param name="entry">设置</param>
    /// <param name="value">值</param>
    public static void SetBoolean(this SettingEntry entry, bool value)
    {
        entry.Value = value.ToString();
    }

    /// <summary>
    /// 获取 Int32 值
    /// </summary>
    /// <param name="entry">设置</param>
    /// <returns>值</returns>
    public static int GetInt32(this SettingEntry entry)
    {
        return int.Parse(entry.Value!);
    }

    /// <summary>
    /// 设置 Int32 值
    /// </summary>
    /// <param name="entry">设置</param>
    /// <param name="value">值</param>
    public static void SetInt32(this SettingEntry entry, int value)
    {
        entry.Value = value.ToString();
    }
}