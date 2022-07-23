// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace Snap.Hutao.Core.Caching;

/// <summary>
/// Provides methods and tools to cache files in a folder
/// The class's name will become the cache folder's name
/// </summary>
[Injection(InjectAs.Singleton, typeof(IImageCache))]
public class ImageCache : CacheBase<BitmapImage>, IImageCache
{
    private const string DateAccessedProperty = "System.DateAccessed";

    private readonly List<string> extendedPropertyNames = new()
    {
        DateAccessedProperty,
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageCache"/> class.
    /// </summary>
    /// <param name="logger">日志器</param>
    /// <param name="httpClient">http客户端</param>
    public ImageCache(ILogger<ImageCache> logger, HttpClient httpClient)
        : base(logger, httpClient)
    {
        DispatcherQueue = Program.UIDispatcherQueue;
    }

    /// <summary>
    /// Gets or sets which DispatcherQueue is used to dispatch UI updates.
    /// </summary>
    private DispatcherQueue DispatcherQueue { get; }

    /// <summary>
    /// Cache specific hooks to process items from HTTP response
    /// </summary>
    /// <param name="stream">input stream</param>
    /// <returns>awaitable task</returns>
    protected override Task<BitmapImage> InitializeTypeAsync(Stream stream)
    {
        if (stream.Length == 0)
        {
            throw new FileNotFoundException();
        }

        return DispatcherQueue.EnqueueAsync(async () =>
        {
            BitmapImage image = new();

            // This action will run on the UI thread, no need to care which thread to continue with
            await image.SetSourceAsync(stream.AsRandomAccessStream()).AsTask().ConfigureAwait(false);

            return image;
        });
    }

    /// <summary>
    /// Cache specific hooks to process items from HTTP response
    /// </summary>
    /// <param name="baseFile">storage file</param>
    /// <returns>awaitable task</returns>
    protected override async Task<BitmapImage> InitializeTypeAsync(StorageFile baseFile)
    {
        using (Stream stream = await baseFile.OpenStreamForReadAsync().ConfigureAwait(false))
        {
            return await InitializeTypeAsync(stream).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Override-able method that checks whether file is valid or not.
    /// </summary>
    /// <param name="file">storage file</param>
    /// <param name="duration">cache duration</param>
    /// <param name="treatNullFileAsOutOfDate">option to mark uninitialized file as expired</param>
    /// <returns>bool indicate whether file has expired or not</returns>
    protected override async Task<bool> IsFileOutOfDateAsync(StorageFile file, TimeSpan duration, bool treatNullFileAsOutOfDate = true)
    {
        if (file == null)
        {
            return treatNullFileAsOutOfDate;
        }

        // Get extended properties.
        IDictionary<string, object> extraProperties = await file.Properties
            .RetrievePropertiesAsync(extendedPropertyNames)
            .AsTask()
            .ConfigureAwait(false);

        // Get date-accessed property.
        object? propValue = extraProperties[DateAccessedProperty];

        if (propValue != null)
        {
            DateTimeOffset? lastAccess = propValue as DateTimeOffset?;

            if (lastAccess.HasValue)
            {
                return DateTime.Now.Subtract(lastAccess.Value.DateTime) > duration;
            }
        }

        BasicProperties properties = await file
            .GetBasicPropertiesAsync()
            .AsTask()
            .ConfigureAwait(false);

        return properties.Size == 0 || DateTime.Now.Subtract(properties.DateModified.DateTime) > duration;
    }
}