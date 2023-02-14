// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Core.IO;

/// <summary>
/// 文件操作
/// </summary>
[HighQuality]
internal static class FileOperation
{
    /// <summary>
    /// 将指定文件移动到新位置，提供指定新文件名和覆盖目标文件（如果它已存在）的选项。
    /// </summary>
    /// <param name="sourceFileName">要移动的文件的名称。 可以包括相对或绝对路径。</param>
    /// <param name="destFileName">文件的新路径和名称。</param>
    /// <param name="overwrite">如果要覆盖目标文件</param>
    /// <returns>是否发生了移动操作</returns>
    public static bool Move(string sourceFileName, string destFileName, bool overwrite)
    {
        if (File.Exists(sourceFileName))
        {
            if (overwrite)
            {
                File.Move(sourceFileName, destFileName, overwrite);
                return true;
            }
            else
            {
                if (!File.Exists(destFileName))
                {
                    File.Move(sourceFileName, destFileName, overwrite);
                    return true;
                }
            }
        }

        return false;
    }
}