// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;

namespace Snap.Hutao.Service.Notification;

internal interface IInfoBarOptionsBuilder : IBuilder
{
    public InfoBarOptions Options { get; }
}