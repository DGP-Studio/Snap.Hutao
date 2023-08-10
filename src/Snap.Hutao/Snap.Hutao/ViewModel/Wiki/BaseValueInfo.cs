// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.ViewModel.Wiki;

/// <summary>
/// 基础数值信息
/// </summary>
internal sealed class BaseValueInfo : ObservableObject
{
    private readonly List<PropertyCurveValue> propValues;
    private readonly Dictionary<Level, Dictionary<GrowCurveType, float>> growCurveMap;
    private readonly Dictionary<PromoteLevel, Promote>? promoteMap;

    private uint currentLevel;
    private List<NameValue<string>> values = default!;
    private bool promoted = true;

    /// <summary>
    /// 构造一个新的基础数值信息
    /// </summary>
    /// <param name="maxLevel">最大等级</param>
    /// <param name="propValues">属性与初始值</param>
    /// <param name="growCurveMap">生长曲线</param>
    /// <param name="promoteMap">突破加成</param>
    public BaseValueInfo(uint maxLevel, List<PropertyCurveValue> propValues, Dictionary<Level, Dictionary<GrowCurveType, float>> growCurveMap, Dictionary<PromoteLevel, Promote>? promoteMap = null)
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
    public uint MaxLevel { get; }

    /// <summary>
    /// 对应的基础值
    /// </summary>
    public List<NameValue<string>> Values { get => values; set => SetProperty(ref values, value); }

    /// <summary>
    /// 当前等级
    /// </summary>
    public uint CurrentLevel
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
        get => LevelFormat.Format(CurrentLevel);
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

    [SuppressMessage("", "SH002")]
    private void UpdateValues(Level level, bool promoted)
    {
        Values = propValues.SelectList(propValue =>
        {
            float value = propValue.Value * growCurveMap[level].GetValueOrDefault(propValue.Type);
            if (promoteMap is not null)
            {
                PromoteLevel promoteLevel = GetPromoteLevel(level, promoted);
                float addValue = promoteMap[promoteLevel].GetValue(propValue.Property);

                value += addValue;
            }

            return FightPropertyFormat.ToNameValue(propValue.Property, value);
        });
    }

    private PromoteLevel GetPromoteLevel(in Level level, bool promoted)
    {
        if (MaxLevel <= 70 && level.Value == 70U)
        {
            return 4;
        }

        if (promoted)
        {
            return level.Value switch
            {
                >= 80U => 6,
                >= 70U => 5,
                >= 60U => 4,
                >= 50U => 3,
                >= 40U => 2,
                >= 20U => 1,
                _ => 0,
            };
        }
        else
        {
            return level.Value switch
            {
                > 80U => 6,
                > 70U => 5,
                > 60U => 4,
                > 50U => 3,
                > 40U => 2,
                > 20U => 1,
                _ => 0,
            };
        }
    }
}