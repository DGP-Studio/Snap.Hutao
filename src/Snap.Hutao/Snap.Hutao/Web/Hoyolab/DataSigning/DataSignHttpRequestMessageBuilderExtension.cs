// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Request.Builder;

namespace Snap.Hutao.Web.Hoyolab.DataSigning;

internal static class DataSignHttpRequestMessageBuilderExtension
{
    public static async ValueTask SignDataAsync(this HttpRequestMessageBuilder builder, DataSignAlgorithmVersion version, SaltType saltType, bool includeChars)
    {
        DataSignOptions options = await DataSignOptions.CreateFromHttpRequestMessageBuilderAsync(builder, saltType, includeChars, version).ConfigureAwait(false);
        builder.SetHeader("DS", DataSignAlgorithm.GetDataSign(options));
    }
}