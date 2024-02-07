// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control.Media;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Caching;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher;
using Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher.Content;
using Snap.Hutao.Web.Response;
using System.IO;
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

    private HashSet<string> backgroundPathSet;

    public async ValueTask<ValueResult<bool, BackgroundImage>> GetNextBackgroundImageAsync()
    {
        HashSet<string> backgroundSet = await SkipOrInitBackgroundAsync().ConfigureAwait(false);

        if (backgroundSet.Count <= 0)
        {
            return new(false, default!);
        }

        string path = System.Random.Shared.GetItems(backgroundSet.ToArray(), 1)[0];
        backgroundSet.Remove(path);
        using (FileStream fileStream = File.OpenRead(path))
        {
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream.AsRandomAccessStream());
            SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight);
            Bgra32 accentColor = softwareBitmap.GetAccentColor();

            await taskContext.SwitchToMainThreadAsync();

            BackgroundImage background = new()
            {
                ImageSource = new(path.ToUri()),
                AccentColor = accentColor,
                Luminance = accentColor.Luminance,
            };

            return new(true, background);
        }
    }

    private async ValueTask<HashSet<string>> SkipOrInitBackgroundAsync()
    {
        if (backgroundPathSet is null || backgroundPathSet.Count <= 0)
        {
            string backgroundFolder = runtimeOptions.GetDataFolderBackgroundFolder();
            Directory.CreateDirectory(backgroundFolder);
            backgroundPathSet = Directory
                .GetFiles(backgroundFolder, "*.*", SearchOption.AllDirectories)
                .Where(path => AllowedFormats.Contains(Path.GetExtension(path)))
                .ToHashSet();

            // No image found
            if (backgroundPathSet.Count <= 0)
            {
                ResourceClient resourceClient = serviceProvider.GetRequiredService<ResourceClient>();
                string launguageCode = serviceProvider.GetRequiredService<CultureOptions>().LanguageCode;
                LaunchScheme scheme = launguageCode is "zh-cn"
                    ? KnownLaunchSchemes.Get().First(scheme => !scheme.IsOversea && scheme.IsNotCompatOnly)
                    : KnownLaunchSchemes.Get().First(scheme => scheme.IsOversea && scheme.IsNotCompatOnly);
                Response<GameContent> response = await resourceClient.GetContentAsync(scheme, launguageCode).ConfigureAwait(false);
                if (response is { Data.Advertisement.Background: string url })
                {
                    ValueFile file = await serviceProvider.GetRequiredService<IImageCache>().GetFileFromCacheAsync(url.ToUri()).ConfigureAwait(false);
                    backgroundPathSet = [file];
                }
            }
        }

        return backgroundPathSet;
    }
}