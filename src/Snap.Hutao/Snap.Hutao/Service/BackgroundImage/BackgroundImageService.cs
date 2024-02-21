// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control.Media;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Caching;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Web.Hutao.Wallpaper;
using Snap.Hutao.Web.Response;
using Snap.Hutao.Win32.Foundation;
using System.IO;
using System.Runtime.InteropServices;
using Windows.Graphics.Imaging;

namespace Snap.Hutao.Service.BackgroundImage;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IBackgroundImageService))]
internal sealed partial class BackgroundImageService : IBackgroundImageService
{
    private static readonly HashSet<string> AllowedFormats = [".bmp", ".gif", ".ico", ".jpg", ".jpeg", ".png", ".tiff", ".webp"];

    private readonly IServiceProvider serviceProvider;
    private readonly RuntimeOptions runtimeOptions;
    private readonly ITaskContext taskContext;
    private readonly AppOptions appOptions;

    private HashSet<string> currentBackgroundPathSet;

    public async ValueTask<ValueResult<bool, BackgroundImage>> GetNextBackgroundImageAsync(BackgroundImage? previous)
    {
        HashSet<string> backgroundSet = await SkipOrInitBackgroundAsync().ConfigureAwait(false);

        if (backgroundSet.Count <= 0)
        {
            return new(false, default!);
        }

        string path = System.Random.Shared.GetItems([..backgroundSet], 1)[0];
        backgroundSet.Remove(path);

        if (string.Equals(path, previous?.Path, StringComparison.OrdinalIgnoreCase))
        {
            return new(false, default!);
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
                    throw;
                }

                return new(false, default!);
            }

            SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight);
            Bgra32 accentColor = softwareBitmap.GetAccentColor();

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

    private async ValueTask<HashSet<string>> SkipOrInitBackgroundAsync()
    {
        switch (appOptions.BackgroundImageType)
        {
            case BackgroundImageType.LocalFolder:
                {
                    if (currentBackgroundPathSet is not { Count: > 0 })
                    {
                        string backgroundFolder = runtimeOptions.GetDataFolderBackgroundFolder();
                        Directory.CreateDirectory(backgroundFolder);

                        currentBackgroundPathSet = Directory
                            .GetFiles(backgroundFolder, "*.*", SearchOption.AllDirectories)
                            .Where(path => AllowedFormats.Contains(Path.GetExtension(path)))
                            .ToHashSet();
                    }

                    break;
                }

            case BackgroundImageType.HutaoBing:
                await SetCurrentBackgroundPathSetAsync(client => client.GetBingWallpaperAsync()).ConfigureAwait(false);
                break;
            case BackgroundImageType.HutaoDaily:
                await SetCurrentBackgroundPathSetAsync(client => client.GetTodayWallpaperAsync()).ConfigureAwait(false);
                break;
            case BackgroundImageType.HutaoOfficialLauncher:
                await SetCurrentBackgroundPathSetAsync(client => client.GetLauncherWallpaperAsync()).ConfigureAwait(false);
                break;
        }

        currentBackgroundPathSet ??= [];
        return currentBackgroundPathSet;

        async Task SetCurrentBackgroundPathSetAsync(Func<HutaoWallpaperClient, ValueTask<Response<Wallpaper>>> responseFactory)
        {
            HutaoWallpaperClient wallpaperClient = serviceProvider.GetRequiredService<HutaoWallpaperClient>();
            Response<Wallpaper> response = await responseFactory(wallpaperClient).ConfigureAwait(false);
            if (response is { Data.Url: Uri url })
            {
                ValueFile file = await serviceProvider.GetRequiredService<IImageCache>().GetFileFromCacheAsync(url).ConfigureAwait(false);
                currentBackgroundPathSet = [file];
            }
        }
    }
}