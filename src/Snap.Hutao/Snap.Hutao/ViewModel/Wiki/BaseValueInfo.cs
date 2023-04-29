// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Converter;

namespace Snap.Hutao.ViewModel.Wiki;

/// <summary>
/// 基础数值信息
/// </summary>
internal sealed class BaseValueInfo : ObservableObject
{
    private readonly List<PropertyCurveValue> propValues;
    private readonly Dictionary<int, Dictionary<GrowCurveType, float>> growCurveMap;
    private readonly Dictionary<int, Promote>? promoteMap;

    private int currentLevel;
    private List<NameValue<string>> values = default!;
    private bool promoted = true;

    /// <summary>
    /// 构造一个新的基础数值信息
    /// </summary>
    /// <param name="maxLevel">最大等级</param>
    /// <param name="propValues">属性与初始值</param>
    /// <param name="growCurveMap">生长曲线</param>
    /// <param name="promoteMap">突破加成</param>
    public BaseValueInfo(int maxLevel, List<PropertyCurveValue> propValues, Dictionary<int, Dictionary<GrowCurveType, float>> growCurveMap, Dictionary<int, Promote>? promoteMap = null)
    {
        this.propValues = propValues;
        this.growCurveMap = growCurveMap;
        this.promoteMap = promoteMap;

        MaxLevel = maxLevel;
        CurrentLevel = maxLevel;
    }

    /// <summary>
    /// 最大等级
    /// </summary>
    public int MaxLevel { get; }

    /// <summary>
    /// 对应的基础值
    /// </summary>
    public List<NameValue<string>> Values { get => values; set => SetProperty(ref values, value); }

    /// <summary>
    /// 当前等级
    /// </summary>
    public int CurrentLevel
    {
        get => currentLevel;
        set
        {
            if (SetProperty(ref currentLevel, value))
            {
                OnPropertyChanged(nameof(CurrentLevelFormatted));
                UpdateValues(value, promoted);
            }
        }
    }

    /// <summary>
    /// 格式化当前等级
    /// </summary>
    public string CurrentLevelFormatted
    {
        get => $"Lv.{CurrentLevel}";
    }

    /// <summary>
    /// 是否突破
    /// </summary>
    public bool Promoted
    {
        get => promoted;
        set
        {
            if (SetProperty(ref promoted, value))
            {
                UpdateValues(currentLevel, value);
            }
        }
    }

    private void UpdateValues(int level, bool promoted)
    {
        Values = propValues.SelectList(propValue =>
        {
            float value = propValue.Value * growCurveMap[level].GetValueOrDefault(propValue.Type);
            if (promoteMap != null)
            {
                int promoteLevel = GetPromoteLevel(level, promoted);
                float addValue = promoteMap[promoteLevel].AddProperties.GetValueOrDefault(propValue.Property, 0F);

                value += addValue;
            }

            return FightPropertyFormat.ToNameValue(propValue.Property, value);
        });
    }

    private int GetPromoteLevel(int level, bool promoted)
    {
        if (MaxLevel <= 70)
        {
            if (promoted)
            {
                return level switch
                {
                    >= 60 => 4,
                    >= 50 => 3,
                    >= 40 => 2,
                    >= 20 => 1,
                    _ => 0,
                };
            }
            else
            {
                return level switch
                {
                    > 60 => 4,
                    > 50 => 3,
                    > 40 => 2,
                    > 20 => 1,
                    _ => 0,
                };
            }
        }
        else
        {
            if (promoted)
            {
                return level switch
                {
                    >= 80 => 6,
                    >= 70 => 5,
                    >= 60 => 4,
                    >= 50 => 3,
                    >= 40 => 2,
                    >= 20 => 1,
                    _ => 0,
                };
            }
            else
            {
                return level switch
                {
                    > 80 => 6,
                    > 70 => 5,
                    > 60 => 4,
                    > 50 => 3,
                    > 40 => 2,
                    > 20 => 1,
                    _ => 0,
                };
            }
        }
    }
}