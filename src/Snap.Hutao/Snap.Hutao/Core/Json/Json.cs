// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Text.Json;

namespace Snap.Hutao.Core.Json;

/// <summary>
/// Json操作
/// </summary>
[Injection(InjectAs.Transient)]
public class Json
{
    private readonly JsonSerializerOptions jsonSerializerOptions;
    private readonly ILogger logger;

    /// <summary>
    /// 初始化一个新的 Json操作 实例
    /// </summary>
    /// <param name="jsonSerializerOptions">配置</param>
    /// <param name="logger">日志器</param>
    public Json(JsonSerializerOptions jsonSerializerOptions, ILogger<Json> logger)
    {
        this.jsonSerializerOptions = jsonSerializerOptions;
        this.logger = logger;
    }

    /// <summary>
    /// 将JSON反序列化为指定的.NET类型
    /// </summary>
    /// <typeparam name="T">要反序列化的对象的类型</typeparam>
    /// <param name="value">要反序列化的JSON</param>
    /// <returns>Json字符串中的反序列化对象, 如果反序列化失败会返回 <see langword="default"/></returns>
    public T? ToObject<T>(string value)
    {
        try
        {
            T? result = JsonSerializer.Deserialize<T>(value);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError("反序列化Json时遇到问题\n{ex}", ex);
        }

        return default(T);
    }

    /// <summary>
    /// 将JSON反序列化为指定的.NET类型
    /// 若为null则返回一个新建的实例
    /// </summary>
    /// <typeparam name="T">指定的类型</typeparam>
    /// <param name="value">字符串</param>
    /// <returns>Json字符串中的反序列化对象, 如果反序列化失败会抛出异常</returns>
    public T ToObjectOrNew<T>(string value)
        where T : new()
    {
        return ToObject<T>(value) ?? new T();
    }

    /// <summary>
    /// 将指定的对象序列化为JSON字符串
    /// </summary>
    /// <param name="value">要序列化的对象</param>
    /// <returns>对象的JSON字符串表示形式</returns>
    public string Stringify(object? value)
    {
        return JsonSerializer.Serialize(value, jsonSerializerOptions);
    }

    /// <summary>
    /// 使用 <see cref="FileMode.Open"/>, <see cref="FileAccess.Read"/> 和 <see cref="FileShare.Read"/> 从文件中读取后转化为实体类
    /// </summary>
    /// <typeparam name="T">要反序列化的对象的类型</typeparam>
    /// <param name="fileName">存放JSON数据的文件路径</param>
    /// <returns>JSON字符串中的反序列化对象, 如果反序列化失败则抛出异常，若文件不存在则返回 <see langword="null"/></returns>
    public T? FromFile<T>(string fileName)
    {
        if (File.Exists(fileName))
        {
            // FileShare.Read is important to read some file
            using (StreamReader sr = new(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                return ToObject<T>(sr.ReadToEnd());
            }
        }
        else
        {
            return default;
        }
    }

    /// <summary>
    /// 使用 <see cref="FileMode.Open"/>, <see cref="FileAccess.Read"/> 和 <see cref="FileShare.Read"/> 从文件中读取后转化为实体类
    /// 若为null则返回一个新建的实例
    /// </summary>
    /// <typeparam name="T">要反序列化的对象的类型</typeparam>
    /// <param name="fileName">存放JSON数据的文件路径</param>
    /// <returns>JSON字符串中的反序列化对象</returns>
    public T FromFileOrNew<T>(string fileName)
        where T : new()
    {
        return FromFile<T>(fileName) ?? new T();
    }

    /// <summary>
    /// 从文件中读取后转化为实体类
    /// </summary>
    /// <typeparam name="T">要反序列化的对象的类型</typeparam>
    /// <param name="file">存放JSON数据的文件</param>
    /// <returns>JSON字符串中的反序列化对象</returns>
    public T? FromFile<T>(FileInfo file)
    {
        using (StreamReader sr = file.OpenText())
        {
            return ToObject<T>(sr.ReadToEnd());
        }
    }

    /// <summary>
    /// 将对象保存到文件
    /// </summary>
    /// <param name="fileName">文件名称</param>
    /// <param name="value">对象</param>
    public void ToFile(string fileName, object? value)
    {
        File.WriteAllText(fileName, Stringify(value));
    }
}
