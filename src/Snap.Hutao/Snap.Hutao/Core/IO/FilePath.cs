// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Core.IO;

/// <summary>
/// 文件路径
/// </summary>
internal readonly struct FilePath : IEquatable<FilePath>
{
    /// <summary>
    /// 值
    /// </summary>
    public readonly string Value;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilePath"/> struct.
    /// </summary>
    /// <param name="value">value</param>
    public FilePath(string value)
    {
        Value = value;
    }

    public static implicit operator string(FilePath value)
    {
        return value.Value;
    }

    public static implicit operator FilePath(string value)
    {
        return new(value);
    }

    public static bool operator ==(FilePath left, FilePath right)
    {
        return left.Value == right.Value;
    }

    public static bool operator !=(FilePath left, FilePath right)
    {
        return !(left == right);
    }

    /// <summary>
    /// 异步反序列化文件中的内容
    /// </summary>
    /// <typeparam name="T">内容的类型</typeparam>
    /// <param name="options">序列化选项</param>
    /// <returns>操作是否成功，反序列化后的内容</returns>
    public async Task<ValueResult<bool, T?>> DeserializeFromJsonAsync<T>(JsonSerializerOptions options)
        where T : class
    {
        try
        {
            using (FileStream stream = File.OpenRead(Value))
            {
                T? t = await JsonSerializer.DeserializeAsync<T>(stream, options).ConfigureAwait(false);
                return new(true, t);
            }
        }
        catch (Exception ex)
        {
            _ = ex;
            return new(false, null);
        }
    }

    /// <summary>
    /// 将对象异步序列化入文件
    /// </summary>
    /// <typeparam name="T">对象的类型</typeparam>
    /// <param name="obj">对象</param>
    /// <param name="options">序列化选项</param>
    /// <returns>操作是否成功</returns>
    public async Task<bool> SerializeToJsonAsync<T>(T obj, JsonSerializerOptions options)
    {
        try
        {
            using (FileStream stream = File.Create(Value))
            {
                await JsonSerializer.SerializeAsync(stream, obj, options).ConfigureAwait(false);
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public bool Equals(FilePath other)
    {
        return Value == other.Value;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is FilePath other && Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}