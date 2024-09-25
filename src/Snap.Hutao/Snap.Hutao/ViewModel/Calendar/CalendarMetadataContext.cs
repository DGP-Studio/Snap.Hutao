// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Service;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.UI.Xaml.Data;
using System.Collections.Immutable;
using System.Globalization;

namespace Snap.Hutao.ViewModel.Calendar;

internal sealed class CalendarMetadataContext : IMetadataContext, IMetadataArrayAvatarSource
{
    public ImmutableArray<Avatar> Avatars { get; set; }
}