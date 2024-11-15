// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Enka.Model;

internal sealed class FetterInfo
{
    [JsonPropertyName("expLevel")]
    public FetterLevel ExpLevel { get; set; }
}