// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Miyolive;

internal sealed class CodeListWrapper
{
    [JsonPropertyName("code_list")]
    public List<CodeWrapper> CodeList { get; set; } = default!;
}