﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Navigation;

internal interface INavigationRecipient
{
    ValueTask<bool> ReceiveAsync(INavigationData data);
}