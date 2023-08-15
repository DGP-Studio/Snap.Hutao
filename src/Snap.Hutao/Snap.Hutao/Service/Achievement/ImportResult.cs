// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Achievement;

/// <summary>
/// 导入结果
/// </summary>
[HighQuality]
internal readonly struct ImportResult
{
    /// <summary>
    /// 新增数
    /// </summary>
    public readonly int Add;

    /// <summary>
    /// 更新数
    /// </summary>
    public readonly int Update;

    /// <summary>
    /// 移除数
    /// </summary>
    public readonly int Remove;

    /// <summary>
    /// 构造一个新的导入结果
    /// </summary>
    /// <param name="add">添加数</param>
    /// <param name="update">更新数</param>
    /// <param name="remove">移除数</param>
    public ImportResult(int add, int update, int remove)
    {
        Add = add;
        Update = update;
        Remove = remove;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return SH.ServiceAchievementImportResultFormat.Format(Add, Update, Remove);
    }
}