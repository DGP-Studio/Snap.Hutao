// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.ViewModel.Cultivation;

internal enum DaysOfWeek
{
    Any,

    [LocalizationKey(nameof(SH.ModelBindingCultivationDaysOfWeek14))]
    MondayAndThursday,

    [LocalizationKey(nameof(SH.ModelBindingCultivationDaysOfWeek25))]
    TuesdayAndFriday,

    [LocalizationKey(nameof(SH.ModelBindingCultivationDaysOfWeek36))]
    WednesdayAndSaturday,
}