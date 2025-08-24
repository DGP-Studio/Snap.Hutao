// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.Hashing;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using Snap.Hutao.Win32;
using Snap.Hutao.Win32.Foundation;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.IO;
using System.Net.Http;

namespace Snap.Hutao.Service.Metadata;

[ConstructorGenerated]
[Service(ServiceLifetime.Singleton, typeof(IMetadataService))]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class MetadataService : IMetadataService
{
    private const string MetaFileName = "Meta.json";

    private readonly TaskCompletionSource initializeCompletionSource = new();

    private readonly IHttpRequestMessageBuilderFactory requestBuilderFactory;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ILogger<MetadataService> logger;
    private readonly MetadataOptions metadataOptions;
    private readonly IInfoBarService infoBarService;
    private readonly JsonSerializerOptions options;

    private FrozenSet<string>? fileNames;
    private volatile bool isInitialized;

    public partial IMemoryCache MemoryCache { get; }

    public async ValueTask<bool> InitializeAsync()
    {
        await initializeCompletionSource.Task.ConfigureAwait(false);
        return isInitialized;
    }

    public async ValueTask InitializeInternalAsync(CancellationToken token = default)
    {
        if (isInitialized)
        {
            return;
        }

        using (ValueStopwatch.MeasureExecution(logger))
        {
            isInitialized = await DownloadMetadataDescriptionFileAndCheckAsync(token).ConfigureAwait(false);
            initializeCompletionSource.TrySetResult();
        }
    }

    public async ValueTask<ImmutableArray<T>> FromCacheOrFileAsync<T>(MetadataFileStrategy strategy, CancellationToken token)
        where T : class
    {
        Verify.Operation(isInitialized, SH.ServiceMetadataNotInitialized);
        string cacheKey = $"{nameof(MetadataService)}.Cache.{strategy.Name}";

        if (MemoryCache.TryGetValue(cacheKey, out object? value))
        {
            ArgumentNullException.ThrowIfNull(value);
            return (ImmutableArray<T>)value;
        }

        return strategy.IsScattered
            ? await FromCacheOrScatteredFile<T>(strategy, cacheKey, token).ConfigureAwait(false)
            : await FromCacheOrSingleFile<T>(strategy, cacheKey, token).ConfigureAwait(false);
    }

    private async ValueTask DownloadMetadataSourceFilesAsync(MetadataDownloadContext context, string fileFullName, CancellationToken token)
    {
        using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(MetadataService)))
        {
            HttpRequestMessageBuilder builder = requestBuilderFactory.Create(context.Options.GetLocalizedRemoteFile(context.Template, fileFullName)).Get();
            using (HttpRequestMessage message = builder.HttpRequestMessage)
            {
                // We have too much line endings now, should cache the response.
                using (HttpResponseMessage responseMessage = await httpClient.SendAsync(message, HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(false))
                {
                    Stream sourceStream = await responseMessage.Content.ReadAsStreamAsync(token).ConfigureAwait(false);

                    // Write stream while convert LF to CRLF
                    using (StreamReaderWriter readerWriter = new(new(sourceStream), File.CreateText(context.Options.GetLocalizedLocalPath(fileFullName))))
                    {
                        while (await readerWriter.ReadLineAsync(token).ConfigureAwait(false) is { } line)
                        {
                            await readerWriter.WriteAsync(line).ConfigureAwait(false);

                            if (!readerWriter.Reader.EndOfStream)
                            {
                                await readerWriter.WriteAsync("\r\n").ConfigureAwait(false);
                            }
                        }
                    }
                }
            }
        }
    }

    private async ValueTask<ImmutableArray<T>> FromCacheOrSingleFile<T>(MetadataFileStrategy strategy, string cacheKey, CancellationToken token)
        where T : class
    {
        string path = metadataOptions.GetLocalizedLocalPath($"{strategy.Name}.json");
        if (!File.Exists(path))
        {
            FileNotFoundException exception = new(SH.ServiceMetadataFileNotFound, strategy.Name);
            throw HutaoException.Throw(SH.ServiceMetadataFileNotFound, exception);
        }

        using (Stream fileStream = File.OpenRead(path))
        {
            try
            {
                ImmutableArray<T> result = await JsonSerializer.DeserializeAsync<ImmutableArray<T>>(fileStream, options, token).ConfigureAwait(false);
                return MemoryCache.Set(cacheKey, result);
            }
            catch (Exception ex)
            {
                ex.Data.Add("FileName", strategy.Name);
                throw;
            }
        }
    }

    private async ValueTask<ImmutableArray<T>> FromCacheOrScatteredFile<T>(MetadataFileStrategy strategy, string cacheKey, CancellationToken token)
        where T : class
    {
        string path = metadataOptions.GetLocalizedLocalPath(strategy.Name);
        if (!Directory.Exists(path))
        {
            DirectoryNotFoundException exception = new(SH.ServiceMetadataFileNotFound);
            throw HutaoException.Throw(SH.ServiceMetadataFileNotFound, exception);
        }

        ImmutableArray<T>.Builder results = ImmutableArray.CreateBuilder<T>();
        foreach (string file in Directory.GetFiles(path, "*.json"))
        {
            string fileName = $"{strategy.Name}/{Path.GetFileNameWithoutExtension(file)}";
            if (fileNames is not null && !fileNames.Contains(fileName))
            {
                continue;
            }

            using (Stream fileStream = File.OpenRead(file))
            {
                try
                {
                    T? result = await JsonSerializer.DeserializeAsync<T>(fileStream, options, token).ConfigureAwait(false);
                    ArgumentNullException.ThrowIfNull(result);
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    ex.Data.Add("FileName", fileName);
                    throw;
                }
            }
        }

        return MemoryCache.Set(cacheKey, results.ToImmutable());
    }

    private async ValueTask<bool> DownloadMetadataDescriptionFileAndCheckAsync(CancellationToken token)
    {
#if DEBUG
        if (Core.Setting.LocalSetting.Get(Core.Setting.SettingKeys.SuppressMetadataInitialization, false))
        {
            return true;
        }
#endif

        MetadataTemplate? template = await GetMetadataTemplateAsync().ConfigureAwait(false);
        if (await DownloadMetadataDescriptionFileAsync(template, token).ConfigureAwait(false) is not { } metadataFileHashes)
        {
            return false;
        }

        MetadataCheckResult checkResult = await CheckMetadataSourceFilesAsync(template, metadataFileHashes, token).ConfigureAwait(false);
        if (checkResult is not { NoError: true })
        {
            string failedFiles = string.Join(",\r\n", checkResult.FailedFiles);
            infoBarService.Error(SH.FormatServiceMetadataDownloadSourceFilesFailed(failedFiles));
            return false;
        }

        fileNames = checkResult.SucceedFiles.ToFrozenSet();
        return checkResult.NoError;
    }

    private async ValueTask<MetadataTemplate?> GetMetadataTemplateAsync()
    {
        using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(MetadataService)))
        {
            HttpRequestMessageBuilder builder = requestBuilderFactory.Create(metadataOptions.GetTemplateEndpoint()).Get();
            Response<MetadataTemplate>? resp = await builder.SendAsync<Response<MetadataTemplate>>(httpClient, CancellationToken.None).ConfigureAwait(false);

            if (!ResponseValidator.TryValidateWithoutUINotification(Response.DefaultIfNull(resp), out MetadataTemplate? metadataTemplate))
            {
                return default;
            }

            return metadataTemplate;
        }
    }

    private async ValueTask<ImmutableDictionary<string, string>?> DownloadMetadataDescriptionFileAsync(MetadataTemplate? template, CancellationToken token)
    {
        try
        {
            ImmutableDictionary<string, string>? metadataFileHashes;
            using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(MetadataService)))
            {
                HttpRequestMessageBuilder builder = requestBuilderFactory.Create(metadataOptions.GetLocalizedRemoteFile(template, MetaFileName)).Get();
                metadataFileHashes = await builder.SendAsync<ImmutableDictionary<string, string>>(httpClient, token).ConfigureAwait(false);
            }

            ArgumentNullException.ThrowIfNull(metadataFileHashes);
            return metadataFileHashes;
        }
        catch (Exception ex)
        {
            infoBarService.Error(ex, SH.ServiceMetadataParseFailed);
            return default;
        }
    }

    [SuppressMessage("", "SH003")]
    private async ValueTask<MetadataCheckResult> CheckMetadataSourceFilesAsync(MetadataTemplate? template, ImmutableDictionary<string, string> metaHashMap, CancellationToken token)
    {
        MetadataDownloadContext context = new(metadataOptions, template);

        await Parallel.ForEachAsync(metaHashMap, token, async (pair, token) =>
        {
            (string fileName, string metaHash) = pair;

            try
            {
                string fileFullName = $"{fileName}.json";
                string fileFullPath = context.Options.GetLocalizedLocalPath(fileFullName);
                if (Path.GetDirectoryName(fileFullPath) is { } directory && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (File.Exists(fileFullPath))
                {
                    string fileHash;
                    try
                    {
                        fileHash = await XxHash64.HashFileAsync(fileFullPath, token).ConfigureAwait(true);
                    }
                    catch (IOException ex)
                    {
                        if (HutaoNative.IsWin32(ex.HResult, [WIN32_ERROR.ERROR_CLOUD_FILE_UNSUCCESSFUL, WIN32_ERROR.ERROR_SHARING_VIOLATION]))
                        {
                            context.SetResult(fileName, false);
                            return;
                        }

                        throw;
                    }

                    if (string.Equals(metaHash, fileHash, StringComparison.OrdinalIgnoreCase))
                    {
                        context.SetResult(fileName, true);
                        return;
                    }
                }

                await DownloadMetadataSourceFilesAsync(context, fileFullName, token).ConfigureAwait(true);
                context.SetResult(fileName, true);
            }
            catch (Exception)
            {
                // No matter what exception, we set the result to false
                context.SetResult(fileName, false);
            }
        }).ConfigureAwait(false);

        return context.ToResult();
    }

    private sealed class MetadataDownloadContext
    {
        private readonly Lock syncRoot = new();
        private readonly ConcurrentDictionary<string, bool> results = [];

        public MetadataDownloadContext(MetadataOptions options, MetadataTemplate? template)
        {
            Options = options;
            Template = template;
        }

        public MetadataOptions Options { get; }

        public MetadataTemplate? Template { get; }

        public void SetResult(string fileName, bool result)
        {
            lock (syncRoot)
            {
                results.TryAdd(fileName, result);
            }
        }

        public MetadataCheckResult ToResult()
        {
            lock (syncRoot)
            {
                return new(results);
            }
        }
    }

    private sealed class MetadataCheckResult
    {
        private readonly ImmutableDictionary<string, bool> results;

        public MetadataCheckResult(IEnumerable<KeyValuePair<string, bool>> results)
        {
            this.results = results.ToImmutableDictionary();
        }

        public bool NoError
        {
            get => !FailedFiles.Any();
        }

        public IEnumerable<string> SucceedFiles
        {
            get
            {
                foreach ((string fileName, bool result) in results)
                {
                    if (result)
                    {
                        yield return fileName;
                    }
                }
            }
        }

        public IEnumerable<string> FailedFiles
        {
            get
            {
                foreach ((string fileName, bool result) in results)
                {
                    if (!result)
                    {
                        yield return fileName;
                    }
                }
            }
        }
    }
}