// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Model;
using Snap.Hutao.Service;
using Snap.Hutao.Service.BackgroundImage;
using Snap.Hutao.UI.Xaml.Media.Backdrop;
using System.Globalization;

namespace Snap.Hutao.ViewModel.Setting;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class SettingAppearanceViewModel : Abstraction.ViewModel
{
    public partial CultureOptions CultureOptions { get; }

    public partial AppOptions AppOptions { get; }

    public partial BackgroundImageOptions BackgroundImageOptions { get; }

    public NameValue<CultureInfo>? SelectedCulture
    {
        get => field ??= CultureOptions.GetCurrentCultureForSelectionOrDefault();
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                CultureOptions.CurrentCulture = value.Value;
                AppInstance.Restart(string.Empty);
            }
        }
    }

    public NameValue<DayOfWeek>? SelectedFirstDayOfWeek
    {
        get => field ??= CultureOptions.DayOfWeeks.FirstOrDefault(d => d.Value == CultureOptions.FirstDayOfWeek);
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                CultureOptions.FirstDayOfWeek = value.Value;
            }
        }
    }

    public NameValue<BackdropType>? SelectedBackdropType
    {
        get => field ??= AppOptions.BackdropTypes.Single(t => t.Value == AppOptions.BackdropType);
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                AppOptions.BackdropType = value.Value;
            }
        }
    }

    public NameValue<ElementTheme>? SelectedElementTheme
    {
        get => field ??= AppOptions.LazyElementThemes.Value.Single(t => t.Value == AppOptions.ElementTheme);
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                AppOptions.ElementTheme = value.Value;
            }
        }
    }

    public NameValue<BackgroundImageType>? SelectedBackgroundImageType
    {
        get => field ??= AppOptions.BackgroundImageTypes.Single(t => t.Value == AppOptions.BackgroundImageType);
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                AppOptions.BackgroundImageType = value.Value;
            }
        }
    }
}