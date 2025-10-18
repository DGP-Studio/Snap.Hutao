// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

namespace Snap.Hutao.Service.Game.Account;

internal static class RegistryAccountDataListDecoder
{
    private static ReadOnlySpan<byte> InitializationVector { get => [0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF]; }

    public static string GetMacAddress()
    {
        string address = string.Empty;
        foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            address = networkInterface.GetPhysicalAddress().ToString();

            if (networkInterface.Description is "en0" || !string.IsNullOrEmpty(address))
            {
                break;
            }
        }

        return address;
    }

    public static string Decode(string data, out string address)
    {
        address = GetMacAddress();

        using (DES des = DES.Create())
        {
            string key = string.IsNullOrEmpty(address) || address.Length < 8 ? "FFFFFFFFFFFF" : address[..8];
            des.Key = Encoding.UTF8.GetBytes(key);
            return Encoding.UTF8.GetString(des.DecryptCbc(Convert.FromBase64String(data), InitializationVector));
        }
    }
}