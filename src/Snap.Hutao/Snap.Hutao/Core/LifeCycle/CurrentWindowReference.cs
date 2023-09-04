// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Notifications;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service.DailyNote;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.ViewModel.Guide;
using System.Diagnostics;

namespace Snap.Hutao.Core.LifeCycle;

[Injection(InjectAs.Singleton, typeof(ICurrentWindowReference))]
internal sealed class CurrentWindowReference : ICurrentWindowReference
{
    public Window Window { get; set; } = default!;
}