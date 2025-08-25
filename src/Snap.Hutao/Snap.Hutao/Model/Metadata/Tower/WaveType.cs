// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Metadata.Tower;

[ExtendedEnum]
internal enum WaveType
{
    [LocalizationKey(nameof(SH.ModelMetadataTowerWaveTypeIndependent))]
    Independent = 0,

    [LocalizationKey(nameof(SH.ModelMetadataTowerWaveTypeWave1))]
    Wave1 = 1,

    [LocalizationKey(nameof(SH.ModelMetadataTowerWaveTypeWave2))]
    Wave2 = 2,

    [LocalizationKey(nameof(SH.ModelMetadataTowerWaveTypeWave3))]
    Wave3 = 3,

    [LocalizationKey(nameof(SH.ModelMetadataTowerWaveTypeWave4))]
    Wave4 = 4,

    [LocalizationKey(nameof(SH.ModelMetadataTowerWaveTypeGroupA))]
    GroupA = 10,

    [LocalizationKey(nameof(SH.ModelMetadataTowerWaveTypeGroupAWave1))]
    GroupAWave1 = 11,

    [LocalizationKey(nameof(SH.ModelMetadataTowerWaveTypeGroupAWave2))]
    GroupAWave2 = 12,

    [LocalizationKey(nameof(SH.ModelMetadataTowerWaveTypeGroupAWave3))]
    GroupAWave3 = 13,

    [LocalizationKey(nameof(SH.ModelMetadataTowerWaveTypeGroupB))]
    GroupB = 20,

    [LocalizationKey(nameof(SH.ModelMetadataTowerWaveTypeGroupBWave1))]
    GroupBWave1 = 21,

    [LocalizationKey(nameof(SH.ModelMetadataTowerWaveTypeGroupBWave2))]
    GroupBWave2 = 22,

    [LocalizationKey(nameof(SH.ModelMetadataTowerWaveTypeGroupBWave3))]
    GroupBWave3 = 23,

    [LocalizationKey(nameof(SH.ModelMetadataTowerWaveTypeGroupC))]
    GroupC = 30,

    [LocalizationKey(nameof(SH.ModelMetadataTowerWaveTypeGroupCWave1))]
    GroupCWave1 = 31,

    [LocalizationKey(nameof(SH.ModelMetadataTowerWaveTypeGroupCWave2))]
    GroupCWave2 = 32,

    [LocalizationKey(nameof(SH.ModelMetadataTowerWaveTypeGroupCWave3))]
    GroupCWave3 = 33,

    [LocalizationKey(nameof(SH.ModelMetadataTowerWaveTypeGroupD))]
    GroupD = 40,

    [LocalizationKey(nameof(SH.ModelMetadataTowerWaveTypeGroupDWave1))]
    GroupDWave1 = 41,

    [LocalizationKey(nameof(SH.ModelMetadataTowerWaveTypeGroupDWave2))]
    GroupDWave2 = 42,

    [LocalizationKey(nameof(SH.ModelMetadataTowerWaveTypeGroupDWave3))]
    GroupDWave3 = 43,

    [LocalizationKey(nameof(SH.ModelMetadataTowerWaveTypeSuppressed))]
    Suppressed = 99,

    [LocalizationKey(nameof(SH.ModelMetadataTowerWaveTypeAdditional))]
    Additional = 999,

    [LocalizationKey(nameof(SH.ModelMetadataTowerWaveTypeWave1Additional))]
    Wave1Additional = 1999,

    [LocalizationKey(nameof(SH.ModelMetadataTowerWaveTypeWave99999))]
    Wave99999 = 99999,

    [LocalizationKey(nameof(SH.ModelMetadataTowerWaveTypeIndependent))]
    Wave114514 = 114514,
}