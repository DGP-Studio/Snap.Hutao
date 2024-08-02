// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.DataSigning;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace Snap.Hutao.Web.Hoyolab.Passport;

internal sealed class MobileCaptcha
{
    [JsonPropertyName("sent_new")]
    public bool SentNew { get; set; }

    [JsonPropertyName("countdown")]
    public int Countdown { get; set; }

    [JsonPropertyName("action_type")]
    public string ActionType { get; set; } = default!;
}