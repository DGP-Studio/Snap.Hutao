using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snap.Hutao.Core.Annotation;

/// <summary>
/// 本地化键
/// </summary>
internal class LocalizationKeyAttribute : Attribute
{
    /// <summary>
    /// 指定本地化键
    /// </summary>
    /// <param name="key">键</param>
    public LocalizationKeyAttribute(string key)
    {
        Key = key;
    }

    /// <summary>
    /// 键
    /// </summary>
    public string Key { get; }
}
