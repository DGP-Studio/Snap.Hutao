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
    private readonly BackgroundImageOptions backgroundImageOptions;
    private readonly CultureOptions cultureOptions;
    private readonly AppOptions appOptions;

    private NameValue<CultureInfo>? selectedCulture;
    private NameValue<BackdropType>? selectedBackdropType;
    private NameValue<ElementTheme>? selectedElementTheme;
    private NameValue<BackgroundImageType>? selectedBackgroundImageType;

    public CultureOptions CultureOptions { get => cultureOptions; }

    public AppOptions AppOptions { get => appOptions; }

    public BackgroundImageOptions BackgroundImageOptions { get => backgroundImageOptions; }

    public NameValue<CultureInfo>? SelectedCulture
    {
        get => selectedCulture ??= CultureOptions.GetCurrentCultureForSelectionOrDefault();
        set
        {
            if (SetProperty(ref selectedCulture, value) && value is not null)
            {
                CultureOptions.CurrentCulture = value.Value;
                AppInstance.Restart(string.Empty);
            }
        }
    }

    public NameValue<BackdropType>? SelectedBackdropType
    {
        get => selectedBackdropType ??= AppOptions.BackdropTypes.Single(t => t.Value == AppOptions.BackdropType);
        set
        {
            if (SetProperty(ref selectedBackdropType, value) && value is not null)
            {
                AppOptions.BackdropType = value.Value;
            }
        }
    }

    public NameValue<ElementTheme>? SelectedElementTheme
    {
        get => selectedElementTheme ??= AppOptions.LazyElementThemes.Value.Single(t => t.Value == AppOptions.ElementTheme);
        set
        {
            if (SetProperty(ref selectedElementTheme, value) && value is not null)
            {
                AppOptions.ElementTheme = value.Value;
            }
        }
    }

    public NameValue<BackgroundImageType>? SelectedBackgroundImageType
    {
        get => selectedBackgroundImageType ??= AppOptions.BackgroundImageTypes.Single(t => t.Value == AppOptions.BackgroundImageType);
        set
        {
            if (SetProperty(ref selectedBackgroundImageType, value) && value is not null)
            {
                AppOptions.BackgroundImageType = value.Value;
            }
        }
    }
}