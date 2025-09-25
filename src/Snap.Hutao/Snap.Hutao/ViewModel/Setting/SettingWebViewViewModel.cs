// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Service;
using Snap.Hutao.Web.Bridge;

namespace Snap.Hutao.ViewModel.Setting;

[ConstructorGenerated]
[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class SettingWebViewViewModel : Abstraction.ViewModel
{
    public partial AppOptions AppOptions { get; }

    public NameValue<BridgeShareSaveType>? SelectedShareSaveType
    {
        get => field ??= AppOptions.BridgeShareSaveTypes.Single(t => t.Value == AppOptions.BridgeShareSaveType.Value);
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                AppOptions.BridgeShareSaveType.Value = value.Value;
            }
        }
    }
}