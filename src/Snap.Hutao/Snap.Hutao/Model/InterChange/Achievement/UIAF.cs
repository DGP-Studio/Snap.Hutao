// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.InterChange.Achievement;

/// <summary>
/// 统一可交换成就格式
/// https://www.snapgenshin.com/development/UIAF.html
/// </summary>
public class UIAF
{
    private static readonly List<string> SupportedVersion = new()
    {
        "v1.1",
    };

    /// <summary>
    /// 信息
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public UIAFInfo Info { get; set; } = default!;

    /// <summary>
    /// 列表
    /// </summary>
    public List<UIAFItem> List { get; set; } = default!;

    /// <summary>
    /// 确认当前UIAF对象的版本是否受支持
    /// </summary>
    /// <returns>当前UIAF对象是否受支持</returns>
    public bool IsCurrentVersionSupported()
    {
        return SupportedVersion.Contains(Info.UIAFVersion ?? string.Empty);
    }
}