// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.Wiki;

internal sealed partial class BaseValueInfo : ObservableObject
{
    private readonly ImmutableArray<PropertyCurveValue> propValues;
    private readonly ImmutableDictionary<Level, TypeValueCollection<GrowCurveType, float>> growCurveMap;
    private readonly ImmutableDictionary<PromoteLevel, Promote>? promoteMap;

    private uint currentLevel;
    private bool promoted = true;

    public BaseValueInfo(uint maxLevel, ImmutableArray<PropertyCurveValue> propValues, ImmutableDictionary<Level, TypeValueCollection<GrowCurveType, float>> growCurveMap, ImmutableDictionary<PromoteLevel, Promote>? promoteMap = null)
    {
        this.propValues = propValues;
        this.growCurveMap = growCurveMap;
        this.promoteMap = promoteMap;

        MaxLevel = maxLevel;
        CurrentLevel = maxLevel;
    }

    public uint MaxLevel { get; }

    public List<NameValue<string>> Values { get; set => SetProperty(ref field, value); } = default!;

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
        Values = propValues.Select(propValue => BaseValueInfoFormat.ToNameValue(propValue, level, GetPromoteLevel(level, promoted), growCurveMap, promoteMap)).ToList();
    }

    private PromoteLevel GetPromoteLevel(in Level level, bool promoted)
    {
        if (MaxLevel <= 70 && level.Value == 70U)
        {
            return 4U;
        }

        if (promoted)
        {
            return level.Value switch
            {
                >= 80U => 6U,
                >= 70U => 5U,
                >= 60U => 4U,
                >= 50U => 3U,
                >= 40U => 2U,
                >= 20U => 1U,
                _ => 0U,
            };
        }

        return level.Value switch
        {
            > 80U => 6U,
            > 70U => 5U,
            > 60U => 4U,
            > 50U => 3U,
            > 40U => 2U,
            > 20U => 1U,
            _ => 0U,
        };
    }
}