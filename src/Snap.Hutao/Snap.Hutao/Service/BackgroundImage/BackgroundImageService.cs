// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.Caching;
using Snap.Hutao.Core.Graphics.Imaging;
using Snap.Hutao.Core.IO;
using Snap.Hutao.UI;
using Snap.Hutao.Web.Hutao.Wallpaper;
using Snap.Hutao.Web.Response;
using Snap.Hutao.Win32.Foundation;
using System.Collections.Frozen;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Windows.Graphics.Imaging;

namespace Snap.Hutao.Service.BackgroundImage;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IBackgroundImageService))]
internal sealed partial class BackgroundImageService : IBackgroundImageService
{
    private static readonly FrozenSet<string> AllowedFormats = [".bmp", ".gif", ".ico", ".jpg", ".jpeg", ".png", ".tiff", ".webp"];

    private readonly BackgroundImageOptions backgroundImageOptions;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;
    private readonly AppOptions appOptions;

    private HashSet<string>? availableBackgroundPathSet;

    public async ValueTask<ValueResult<bool, BackgroundImage?>> GetNextBackgroundImageAsync(BackgroundImage? previous, CancellationToken token = default)
    {
        // The availableBackgroundSet will be empty if BackgroundImageType is None
        // backgroundImageOptions.Wallpaper will also be set in this method if web wallpaper type is selected
        HashSet<string> availableBackgroundSet = await SkipOrInitAvailableBackgroundAsync(token).ConfigureAwait(false);

        if (availableBackgroundSet.Count <= 0)
        {
            return new(true, default!);
        }

        string path = System.Random.Shared.GetItems([.. availableBackgroundSet], 1)[0];
        availableBackgroundSet.Remove(path);

        if (string.Equals(path, previous?.Path, StringComparison.OrdinalIgnoreCase))
        {
            return new(false, default!);
        }

        if (!File.Exists(path))
        {
            Debugger.Break();
        }

        using (FileStream fileStream = File.OpenRead(path))
        {
            BitmapDecoder decoder;
            try
            {
                decoder = await BitmapDecoder.CreateAsync(fileStream.AsRandomAccessStream());
            }
            catch (COMException comException)
            {
                if (comException.HResult != HRESULT.E_FAIL)
                {
                    comException.Data.Add("BackgroundImageType", appOptions.BackgroundImageType);
                    comException.Data.Add("FilePath", path);
                    throw;
                }

                return new(false, default!);
            }

            SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Straight);
            Rgba32 accentColor = softwareBitmap.GetRgba32AccentColor();

            await taskContext.SwitchToMainThreadAsync();

            BackgroundImage background = new()
            {
                Path = path,
                ImageSource = new(path.ToUri()),
                AccentColor = accentColor,
                Luminance = accentColor.Luminance,
            };

            return new(true, background);
        }
    }

    private async ValueTask<HashSet<string>> SkipOrInitAvailableBackgroundAsync(CancellationToken token = default)
    {
        switch (appOptions.BackgroundImageType)
        {
            case BackgroundImageType.LocalFolder:
                {
                    if (availableBackgroundPathSet is not { Count: > 0 })
                    {
                        string backgroundFolder = HutaoRuntime.GetDataFolderBackgroundFolder();

                        availableBackgroundPathSet = Directory
                            .EnumerateFiles(backgroundFolder, "*", SearchOption.AllDirectories)
                            .Where(path => AllowedFormats.Contains(Path.GetExtension(path)))
                            .ToHashSet();
                    }

                    backgroundImageOptions.Wallpaper = default;
                    break;
                }

            case BackgroundImageType.HutaoBing:
                await SetCurrentBackgroundPathSetAsync(static (client, token) => client.GetBingWallpaperAsync(token), token).ConfigureAwait(false);
                break;
            case BackgroundImageType.HutaoDaily:
                await SetCurrentBackgroundPathSetAsync(static (client, token) => client.GetTodayWallpaperAsync(token), token).ConfigureAwait(false);
                break;
            case BackgroundImageType.HutaoOfficialLauncher:
                await SetCurrentBackgroundPathSetAsync(static (client, token) => client.GetLauncherWallpaperAsync(token), token).ConfigureAwait(false);
                break;
            default:
                availableBackgroundPathSet = [];
                break;
        }

        return availableBackgroundPathSet ??= [];

        async Task SetCurrentBackgroundPathSetAsync(Func<HutaoWallpaperClient, CancellationToken, ValueTask<Response<Wallpaper>>> responseFactory, CancellationToken token = default)
        {
            HutaoWallpaperClient wallpaperClient = serviceProvider.GetRequiredService<HutaoWallpaperClient>();
            Response<Wallpaper> response = await responseFactory(wallpaperClient, token).ConfigureAwait(false);
            if (response is { Data: { } wallpaper })
            {
                if (wallpaper.Url is { } url)
                {
                    ValueFile file = await serviceProvider.GetRequiredService<IImageCache>().GetFileFromCacheAsync(url).ConfigureAwait(false);
                    if (!File.Exists(file))
                    {
                        Debugger.Break();
                    }

                    availableBackgroundPathSet = [file];
                }

                await taskContext.SwitchToMainThreadAsync();
                backgroundImageOptions.Wallpaper = wallpaper;
            }
        }
    }
}