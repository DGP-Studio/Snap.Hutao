// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32;
using Windows.Foundation.Metadata;

namespace Snap.Hutao.Core;

internal static class UniversalApiContract
{
    public static LazySlim<bool> IsCurrentWindowsVersionSupported { get; } = new(() => new Version("10", "0", "19045", "5371") > WindowsVersion);

    public static Version? WindowsVersion
    {
        get
        {
            if (field is null)
            {
                using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
                {
                    if (key is not null)
                    {
                        object? major = key.GetValue("CurrentMajorVersionNumber");
                        object? minor = key.GetValue("CurrentMinorVersionNumber");
                        object? build = key.GetValue("CurrentBuildNumber");
                        object? revision = key.GetValue("UBR");
                        field = new($"{major}", $"{minor}", $"{build}", $"{revision}");
                    }
                    else
                    {
                        field = new Version("0", "0", "0", "0");
                    }
                }
            }

            return field;
        }
    }

    public static bool IsPresent(WindowsVersion version)
    {
        return ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", (ushort)version);
    }

    internal sealed class Version : IComparable<Version>
    {
        public Version(string? major, string? minor, string? build, string? revision)
        {
            Major = major;
            Minor = minor;
            Build = build;
            Revision = revision;
        }

        public string? Major { get; }

        public string? Minor { get; }

        public string? Build { get; }

        public string? Revision { get; }

        public static bool operator <(Version? left, Version? right)
        {
            if (left is null && right is null)
            {
                return false;
            }

            if (left is null)
            {
                return true;
            }

            return left.CompareTo(right) < 0;
        }

        public static bool operator >(Version? left, Version? right)
        {
            if (left is null && right is null)
            {
                return false;
            }

            if (left is null)
            {
                return false;
            }

            return left.CompareTo(right) > 0;
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Build}.{Revision}";
        }

        public int CompareTo(Version? other)
        {
            if (other is null)
            {
                return 1;
            }

            int result;
            result = ComparePart(Major, other.Major);
            if (result != 0)
            {
                return result;
            }

            result = ComparePart(Minor, other.Minor);
            if (result != 0)
            {
                return result;
            }

            result = ComparePart(Build, other.Build);
            if (result != 0)
            {
                return result;
            }

            return ComparePart(Revision, other.Revision);
        }

        private static int ComparePart(string? a, string? b)
        {
            _ = int.TryParse(a, out int ia);
            _ = int.TryParse(b, out int ib);
            return ia.CompareTo(ib);
        }
    }
}