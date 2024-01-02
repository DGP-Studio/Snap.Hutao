// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Net.Http;

namespace Snap.Hutao.Core.IO;

internal static class ValueFileExtensions
{
    public static async ValueTask<bool> DownloadAsync(this ValueFile valueFile, string url)
    {
        try
        {
            using (HttpClient httpClient = new())
            {
                using (HttpResponseMessage response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
                {
                    long contentLength = response.Content.Headers.ContentLength ?? 0;
                    using (Stream content = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        using (FileStream stream = File.Create((string)valueFile))
                        {
                            Progress<StreamCopyStatus> progress = new();
                            await new StreamCopyWorker(content, stream, contentLength).CopyAsync(progress).ConfigureAwait(false);
                            return true;
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static async ValueTask<bool> WritePngBase64Async(this ValueFile valueFile, string base64)
    {
        try
        {
            using (FileStream stream = File.Create((string)valueFile))
            {
                byte[] bytes = System.Convert.FromBase64String(base64);
                await stream.WriteAsync(bytes).ConfigureAwait(false);
                return true;
            }
        }
        catch (Exception)
        {
            return false;
        }
    }
}
