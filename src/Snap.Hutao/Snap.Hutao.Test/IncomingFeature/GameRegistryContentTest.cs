using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json;

namespace Snap.Hutao.Test.IncomingFeature;

[TestClass]
public class GameRegistryContentTest
{
    [TestMethod]
    [SupportedOSPlatform("windows")]
    public void GetRegistryContent()
    {
        GetRegistryContentCore(@"Software\miHoYo\原神");
        GetRegistryContentCore(@"Software\miHoYo\Genshin Impact");
    }

    [SupportedOSPlatform("windows")]
    private static void GetRegistryContentCore(string subkey)
    {
        using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
        {
            RegistryKey? gameKey = key.OpenSubKey(subkey);
            Assert.IsNotNull(gameKey);

            Dictionary<string, object> data = [];
            foreach (string valueName in gameKey.GetValueNames())
            {
                data[valueName] = gameKey.GetValueKind(valueName) switch
                {
                    RegistryValueKind.DWord => (int)gameKey.GetValue(valueName)!,
                    RegistryValueKind.Binary => GetString((byte[])gameKey.GetValue(valueName)!),
                    _ => throw new NotImplementedException()
                };
            }

            JsonSerializerOptions options = new()
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };

            Console.WriteLine($"Subkey: {subkey}");
            Console.WriteLine(JsonSerializer.Serialize(data, options));
        }
    }

    private static unsafe string GetString(byte[] bytes)
    {
        fixed (byte* pByte = bytes)
        {
            ReadOnlySpan<byte> span = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(pByte);
            return Encoding.UTF8.GetString(span);
        }
    }
}