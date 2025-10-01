using System;
using System.Net.NetworkInformation;
using System.Text;
using System.Security.Cryptography;

// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Test.IncomingFeature;

[TestClass]
public class GameRegistryAdlTest
{
    private static readonly byte[] EncIv = [0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF];

    // MiHoYo.SDK.DeviceInfoManager$$GetMacAddress
    private static string GetMacAddress()
    {
        string address = "";
        foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            address = networkInterface.GetPhysicalAddress().ToString();
            if (networkInterface.Description is "en0" || address != "")
            {
                return address;
            }
        }

        return address;
    }

    // MiHoYo.SDK.DataStorageManager$$GetEncodeValue
    private static string GetEncodeValue()
    {
        string mac = GetMacAddress();
        return mac == "" || mac.Length < 8 ? "FFFFFFFFFFFF" : mac[..8];
    }

    // MiHoYo.SDK.DataStorageManager$$EncodeString
    public static string EncodeString(string str)
    {
        using DES des = DES.Create();
        des.Key = Encoding.UTF8.GetBytes(GetEncodeValue());
        return Convert.ToBase64String(des.EncryptCbc(Encoding.UTF8.GetBytes(str), EncIv));
    }

    // MiHoYo.SDK.DataStorageManager$$DecodeString
    public static string DecodeString(string str)
    {
        using DES des = DES.Create();
        des.Key = Encoding.UTF8.GetBytes(GetEncodeValue());
        return Encoding.UTF8.GetString(des.DecryptCbc(Convert.FromBase64String(str), EncIv));
    }
}