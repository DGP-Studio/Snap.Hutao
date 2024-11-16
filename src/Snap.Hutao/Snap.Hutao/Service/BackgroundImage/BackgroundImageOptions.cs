// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Web.Hutao.Wallpaper;

namespace Snap.Hutao.Service.BackgroundImage;

[Injection(InjectAs.Singleton)]
internal sealed partial class BackgroundImageOptions : ObservableObject
{
    public Wallpaper? Wallpaper { get; set => SetProperty(ref field, value); }
}