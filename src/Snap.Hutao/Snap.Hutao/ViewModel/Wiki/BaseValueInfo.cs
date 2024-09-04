// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.ViewModel.Wiki;

internal sealed partial class BaseValueInfo : ObservableObject
{
    private readonly List<PropertyCurveValue> propValues;
    private readonly Dictionary<Level, Dictionary<GrowCurveType, float>> growCurveMap;
    private readonly Dictionary<PromoteLevel, Promote>? promoteMap;

    private uint currentLevel;
    private List<NameValue<string>> values = default!;
    private bool promoted = true;

    public BaseValueInfo(uint maxLevel, List<PropertyCurveValue> propValues, Dictionary<Level, Dictionary<GrowCurveType, float>> growCurveMap, Dictionary<PromoteLevel, Promote>? promoteMap = null)
    {
        this.propValues = propValues;
        this.growCurveMap = growCurveMap;
        this.promoteMap = promoteMap;

        MaxLevel = maxLevel;
        CurrentLevel = maxLevel;
    }

    public uint MaxLevel { get; }

    public List<NameValue<string>> Values { get => values; set => SetProperty(ref values, value); }

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

    public string CurrentLevelFormatted
    {
        get => LevelFormat.Format(CurrentLevel);
    }

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

    private void UpdateValues(Level level, bool promoted)
    {
        Values = propValues.SelectList(propValue => BaseValueInfoFormat.ToNameValue(propValue, level, GetPromoteLevel(level, promoted), growCurveMap, promoteMap));
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