// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.Threading;
using Snap.Hutao.Extension;
using Snap.Hutao.Service.Game;
using System.IO;
using System.Text;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 浏览器缓存方法
/// </summary>
[Injection(InjectAs.Transient, typeof(IGachaLogUrlProvider))]
internal class GachaLogUrlWebCacheProvider : IGachaLogUrlProvider
{
    private readonly IGameService gameService;

    /// <summary>
    /// 构造一个新的浏览器缓存方法提供器
    /// </summary>
    /// <param name="gameService">游戏服务</param>
    public GachaLogUrlWebCacheProvider(IGameService gameService)
    {
        this.gameService = gameService;
    }

    /// <inheritdoc/>
    public string Name { get => nameof(GachaLogUrlWebCacheProvider); }

    /// <inheritdoc/>
    public async Task<ValueResult<bool, string>> GetQueryAsync()
    {
        (bool isOk, string path) = await gameService.GetGamePathAsync().ConfigureAwait(false);

        if (isOk)
        {
            string folder = Path.GetDirectoryName(path) ?? string.Empty;
            string cacheFile = Path.Combine(folder, @"YuanShen_Data\webCaches\Cache\Cache_Data\data_2");

            TemporaryFile tempFile;
            try
            {
                tempFile = TemporaryFile.CreateFromFileCopy(cacheFile);
            }
            catch (DirectoryNotFoundException)
            {
                return new(false, $"找不到原神内置浏览器缓存路径：\n{cacheFile}");
            }

            using (tempFile)
            {
                using (FileStream fileStream = new(tempFile.Path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (BinaryReader reader = new(fileStream))
                    {
                        string url = string.Empty;
                        while (!reader.EndOfStream())
                        {
                            uint test = reader.ReadUInt32();

                            if (test == 0x2F302F31)
                            {
                                byte[] chars = ReadBytesUntilZero(reader);
                                string result = Encoding.UTF8.GetString(chars.AsSpan());

                                if (result.Contains("&auth_appid=webview_gacha"))
                                {
                                    url = result;
                                }

                                // align up
                                long offset = reader.BaseStream.Position % 128;
                                reader.BaseStream.Position += 128 - offset;
                            }
                        }

                        return new(!string.IsNullOrEmpty(url), url);
                    }
                }
            }
        }
        else
        {
            return new(false, $"未正确提供原神路径，或当前设置的路径不正确");
        }
    }

    private static byte[] ReadBytesUntilZero(BinaryReader binaryReader)
    {
        return ReadByteEnumerableUntilZero(binaryReader).ToArray();
    }

    private static IEnumerable<byte> ReadByteEnumerableUntilZero(BinaryReader binaryReader)
    {
        while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
        {
            byte b = binaryReader.ReadByte();

            if (b == 0x00)
            {
                yield break;
            }
            else
            {
                yield return b;
            }
        }
    }
}
