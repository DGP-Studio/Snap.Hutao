using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snap.Hutao.Web.Bridge.Model;

/// <summary>
/// JsonElement 拓展
/// </summary>
public static class JsonElementExtension
{
    /// <summary>
    /// 序列化到对应类型
    /// </summary>
    /// <typeparam name="T">对应类型</typeparam>
    /// <param name="jsonElement">元素</param>
    /// <returns>对应类型的实例</returns>
    public static T As<T>(this JsonElement jsonElement)
        where T : notnull
    {
        return jsonElement.Deserialize<T>()!;
    }
}