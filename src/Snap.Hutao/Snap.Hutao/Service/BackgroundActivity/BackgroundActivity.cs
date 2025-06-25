// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
using CommunityToolkit.Mvvm.ComponentModel;

namespace Snap.Hutao.Service.BackgroundActivity;

internal sealed partial class BackgroundActivity : ObservableObject
{
    public BackgroundActivity(string id)
    {
        Id = id;
    }

    public string Id { get; }

    [ObservableProperty]
    public partial string? Name { get; set; }

    [ObservableProperty]
    public partial string? Description { get; set; }

    [ObservableProperty]
    public partial bool IsIndeterminate { get; set; }

    [ObservableProperty]
    public partial double ProgressValue { get; set; }
}