// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Model;
using Snap.Hutao.Service;
using Snap.Hutao.Service.BackgroundImage;
using Snap.Hutao.UI.Xaml;
using Snap.Hutao.UI.Xaml.Control.Theme;
using Snap.Hutao.UI.Xaml.Media.Backdrop;

namespace Snap.Hutao.ViewModel.Setting;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class SettingAppearanceViewModel : Abstraction.ViewModel
{
    [GeneratedConstructor]
    public partial SettingAppearanceViewModel(IServiceProvider serviceProvider);

    public partial CultureOptions CultureOptions { get; }

    public partial AppOptions AppOptions { get; }

    public partial BackgroundImageOptions BackgroundImageOptions { get; }

    // TODO: Replace with IObservableProperty
    public NameCultureInfoValue? SelectedCulture
    {
        get => field ??= Selection.Initialize(CultureOptions.Cultures, CultureOptions.CurrentCulture.Value);
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                CultureOptions.CurrentCulture.Value = value.Value;
                AppInstance.Restart(string.Empty);
            }
        }
    }

    // TODO: Replace with IObservableProperty
    public NameValue<DayOfWeek>? SelectedFirstDayOfWeek
    {
        get => field ??= CultureOptions.DayOfWeeks.FirstOrDefault(d => d.Value == CultureOptions.FirstDayOfWeek.Value);
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                CultureOptions.FirstDayOfWeek.Value = value.Value;
            }
        }
    }

    // TODO: Replace with IObservableProperty
    public NameValue<BackdropType>? SelectedBackdropType
    {
        get => field ??= AppOptions.BackdropTypes.Single(t => t.Value == AppOptions.BackdropType.Value);
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                AppOptions.BackdropType.Value = value.Value;
            }
        }
    }

    // TODO: Replace with IObservableProperty
    public NameValue<ElementTheme>? SelectedElementTheme
    {
        get => field ??= AppOptions.LazyElementThemes.Value.Single(t => t.Value == AppOptions.ElementTheme.Value);
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                AppOptions.ElementTheme.Value = value.Value;
                FrameworkTheming.SetTheme(ThemeHelper.ElementToFramework(value.Value));
            }
        }
    }

    // TODO: Replace with IObservableProperty
    public NameValue<BackgroundImageType>? SelectedBackgroundImageType
    {
        get => field ??= AppOptions.BackgroundImageTypes.Single(t => t.Value == AppOptions.BackgroundImageType.Value);
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                AppOptions.BackgroundImageType.Value = value.Value;
            }
        }
    }

    // TODO: Replace with IObservableProperty
    public NameValue<LastWindowCloseBehavior>? SelectedLastWindowCloseBehavior
    {
        get => field ??= AppOptions.LastWindowCloseBehaviors.Single(t => t.Value == AppOptions.LastWindowCloseBehavior.Value);
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                AppOptions.LastWindowCloseBehavior.Value = value.Value;
            }
        }
    }
}