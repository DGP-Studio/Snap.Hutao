// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control.Media;
using Snap.Hutao.Core;
using System.IO;
using Windows.Graphics.Imaging;

namespace Snap.Hutao.Service.BackgroundImage;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IBackgroundImageService))]
internal sealed partial class BackgroundImageService : IBackgroundImageService
{
    private static readonly HashSet<string> AllowedFormats = [".bmp", ".gif", ".ico", ".jpg", ".jpeg", ".png", ".tiff", ".webp"];

    private readonly ITaskContext taskContext;
    private readonly RuntimeOptions runtimeOptions;

    private HashSet<string> backgroundPathMap;

    public async ValueTask<ValueResult<bool, BackgroundImage>> GetNextBackgroundImageAsync()
    {
        HashSet<string> backgroundSet = SkipOrInitBackground();

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

    private HashSet<string> SkipOrInitBackground()
    {
        if (backgroundPathMap is null || backgroundPathMap.Count <= 0)
        {
            string backgroundFolder = runtimeOptions.GetDataFolderBackgroundFolder();
            Directory.CreateDirectory(backgroundFolder);
            backgroundPathMap = Directory
                .GetFiles(backgroundFolder, "*.*", SearchOption.AllDirectories)
                .Where(path => AllowedFormats.Contains(Path.GetExtension(path)))
                .ToHashSet();
        }

        return backgroundPathMap;
    }
}