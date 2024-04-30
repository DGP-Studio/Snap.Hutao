// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.IO.Hashing;
using Snap.Hutao.Core.IO.Http.Sharding;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Hutao.Response;
using Snap.Hutao.Web.Response;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using Windows.Storage;

namespace Snap.Hutao.Service.Update;

internal sealed class CheckUpdateResult
{
    public CheckUpdateResultKind Kind { get; set; }

    public HutaoVersionInformation? HutaoVersionInformation { get; set; }
}