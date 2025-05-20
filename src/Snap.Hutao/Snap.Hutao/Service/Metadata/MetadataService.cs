// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.Hashing;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.IO;
using System.Net;
using System.Net.Http;

namespace Snap.Hutao.Service.Metadata;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IMetadataService))]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class MetadataService : IMetadataService
{
    private const string MetaFileName = "Meta.json";

    private readonly TaskCompletionSource initializeCompletionSource = new();

    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly ILogger<MetadataService> logger;
    private readonly MetadataOptions metadataOptions;
    private readonly IInfoBarService infoBarService;
    private readonly JsonSerializerOptions options;

    private FrozenSet<string>? fileNames;
    private bool isInitialized;

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

    private static async ValueTask DownloadMetadataSourceFilesAsync(MetadataDownloadContext context, string fileFullName, CancellationToken token)
    {
        using (IServiceScope scope = context.ServiceScopeFactory.CreateScope())
        {
            IHttpClientFactory httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
            using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(MetadataService)))
            {
                IHttpRequestMessageBuilderFactory requestBuilderFactory = scope.ServiceProvider.GetRequiredService<IHttpRequestMessageBuilderFactory>();
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
        if (LocalSetting.Get(SettingKeys.SuppressMetadataInitialization, false))
        {
            return true;
        }

        MetadataTemplate? template = await GetMetadataTemplateAsync().ConfigureAwait(false);

        if (await DownloadMetadataDescriptionFileAsync(template, token).ConfigureAwait(false) is not { } metadataFileHashes)
        {
            return false;
        }

        if (await CheckMetadataSourceFilesAsync(template, metadataFileHashes, token).ConfigureAwait(false) is not { NoError: true } checkResult)
        {
            return false;
        }

        // Save metadataFile
        FileStream metaFileStream;
        try
        {
            metaFileStream = File.Create(metadataOptions.GetLocalizedLocalPath(MetaFileName));
        }
        catch (IOException ex)
        {
            // The process cannot access the file '?' because it is being used by another process
            infoBarService.Error(ex);
            return false;
        }
        catch (UnauthorizedAccessException ex)
        {
            // Access to the path '?' is denied.
            infoBarService.Error(ex);
            return false;
        }

        using (metaFileStream)
        {
            try
            {
                await JsonSerializer
                    .SerializeAsync(metaFileStream, metadataFileHashes, options, token)
                    .ConfigureAwait(false);
            }
            catch (UnauthorizedAccessException)
            {
                // Access to the path '?' is denied.
                return false;
            }
        }

        fileNames = checkResult.SucceedFiles.ToFrozenSet();
        return true;
    }

    private async ValueTask<MetadataTemplate?> GetMetadataTemplateAsync()
    {
        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            IHttpClientFactory httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
            using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(MetadataService)))
            {
                IHttpRequestMessageBuilderFactory requestBuilderFactory = scope.ServiceProvider.GetRequiredService<IHttpRequestMessageBuilderFactory>();
                HttpRequestMessageBuilder builder = requestBuilderFactory.Create(metadataOptions.GetTemplateEndpoint()).Get();

                Response<MetadataTemplate>? resp = await builder.SendAsync<Response<MetadataTemplate>>(httpClient, CancellationToken.None).ConfigureAwait(false);

                if (!ResponseValidator.TryValidate(Response.DefaultIfNull(resp), infoBarService, out MetadataTemplate? metadataTemplate))
                {
                    return default;
                }

                return metadataTemplate;
            }
        }
    }

    private async ValueTask<ImmutableDictionary<string, string>?> DownloadMetadataDescriptionFileAsync(MetadataTemplate? template, CancellationToken token)
    {
        try
        {
            ImmutableDictionary<string, string>? metadataFileHashes;
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                IHttpClientFactory httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
                using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(MetadataService)))
                {
                    IHttpRequestMessageBuilderFactory requestBuilderFactory = scope.ServiceProvider.GetRequiredService<IHttpRequestMessageBuilderFactory>();
                    HttpRequestMessageBuilder builder = requestBuilderFactory.Create(metadataOptions.GetLocalizedRemoteFile(template, MetaFileName)).Get();

                    // Download meta check file
                    metadataFileHashes = await builder.SendAsync<ImmutableDictionary<string, string>>(httpClient, token).ConfigureAwait(false);
                }
            }

            if (metadataFileHashes is null)
            {
                infoBarService.Error(SH.ServiceMetadataParseFailed);
                return default;
            }

            return metadataFileHashes;
        }
        catch (JsonException ex)
        {
            infoBarService.Error(ex, SH.ServiceMetadataRequestFailed);
            return default;
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode is (HttpStatusCode)418)
            {
                infoBarService.Error(SH.ServiceMetadataVersionNotSupported);
            }
            else
            {
                infoBarService.Error(ex, SH.FormatServiceMetadataHttpRequestFailed(ex.StatusCode, ex.HttpRequestError));
            }

            return default;
        }
    }

    [SuppressMessage("", "SH003")]
    private async ValueTask<MetadataCheckResult> CheckMetadataSourceFilesAsync(MetadataTemplate? template, ImmutableDictionary<string, string> metaHashMap, CancellationToken token)
    {
        MetadataDownloadContext context = new(serviceScopeFactory, metadataOptions, template);

        await Parallel.ForEachAsync(metaHashMap, token, async (pair, token) =>
        {
            (string fileName, string metaHash) = pair;
            string fileFullName = $"{fileName}.json";
            string fileFullPath = context.Options.GetLocalizedLocalPath(fileFullName);
            if (Path.GetDirectoryName(fileFullPath) is { } directory && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (File.Exists(fileFullPath))
            {
                string fileHash = await XxHash64.HashFileAsync(fileFullPath, token).ConfigureAwait(true);
                if (string.Equals(metaHash, fileHash, StringComparison.OrdinalIgnoreCase))
                {
                    context.SetResult(fileName, true);
                    return;
                }
            }

            try
            {
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
        private readonly Dictionary<string, bool> results = [];

        public MetadataDownloadContext(IServiceScopeFactory serviceScopeFactory, MetadataOptions options, MetadataTemplate? template)
        {
            ServiceScopeFactory = serviceScopeFactory;
            Options = options;
            Template = template;
        }

        public IServiceScopeFactory ServiceScopeFactory { get; }

        public MetadataOptions Options { get; }

        public MetadataTemplate? Template { get; }

        public void SetResult(string fileName, bool result)
        {
            lock (syncRoot)
            {
                results.Add(fileName, result);
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

        public MetadataCheckResult(Dictionary<string, bool> results)
        {
            this.results = results.ToImmutableDictionary();
        }

        public bool NoError
        {
            get => results.All(r => r.Value);
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
    }
}