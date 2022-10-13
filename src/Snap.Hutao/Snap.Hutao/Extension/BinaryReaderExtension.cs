// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Extension;

/// <summary>
/// <see cref="BinaryReader"/> 扩展
/// </summary>
public static class BinaryReaderExtension
{
    /// <summary>
    /// 判断是否处于流的结尾
    /// </summary>
    /// <param name="reader">读取器</param>
    /// <returns>是否处于流的结尾</returns>
    public static bool EndOfStream(this BinaryReader reader)
    {
        return reader.BaseStream.Position >= reader.BaseStream.Length;
    }
}