// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.Generic;
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

    private readonly List<string> extendedPropertyNames = new() { DateAccessedProperty };

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageCache"/> class.
    /// </summary>
    /// <param name="logger">日志器</param>
    /// <param name="httpClient">http客户端</param>
    public ImageCache(ILogger<ImageCache> logger, HttpClient httpClient)
        : base(logger, httpClient)
    {
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