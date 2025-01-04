// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Win32;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.NetworkManagement.WindowsFirewall;
using Snap.Hutao.Win32.Security;
using System.Runtime.InteropServices;
using static Snap.Hutao.Win32.AdvApi32;
using static Snap.Hutao.Win32.ConstValues;
using static Snap.Hutao.Win32.FirewallApi;
using static Snap.Hutao.Win32.Kernel32;
using static Snap.Hutao.Win32.Macros;

namespace Snap.Hutao.Core.IO.Http.Loopback;

[Injection(InjectAs.Singleton)]
internal sealed unsafe partial class LoopbackSupport : ObservableObject
{
    private readonly string hutaoContainerStringSid;

    public LoopbackSupport()
    {
        Initialize(out hutaoContainerStringSid);
    }

    [ObservableProperty]
    public partial bool IsLoopbackEnabled { get; private set; }

    public void EnableLoopback()
    {
        NetworkIsolationGetAppContainerConfig(out uint accCount, out SID_AND_ATTRIBUTES* pSids);
        List<SID_AND_ATTRIBUTES> sids = new((int)(accCount + 1));
        for (uint i = 0; i < accCount; i++)
        {
            sids.Add(*(pSids + i));
        }

        ConvertStringSidToSidW(hutaoContainerStringSid, out PSID pSid);
        SID_AND_ATTRIBUTES sidAndAttributes = default;
        sidAndAttributes.Sid = pSid;
        sids.Add(sidAndAttributes);
        IsLoopbackEnabled = NetworkIsolationSetAppContainerConfig(CollectionsMarshal.AsSpan(sids)) is WIN32_ERROR.ERROR_SUCCESS;
    }

    private void Initialize(out string containerStringSid)
    {
        containerStringSid = string.Empty;

        INET_FIREWALL_APP_CONTAINER* pContainers = default;
        try
        {
            WIN32_ERROR error = NetworkIsolationEnumAppContainers(NETISO_FLAG.NETISO_FLAG_MAX, out uint acCount, out pContainers);
            Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(error));
            for (uint i = 0; i < acCount; i++)
            {
                INET_FIREWALL_APP_CONTAINER* pContainer = pContainers + i;
                ReadOnlySpan<char> appContainerName = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(pContainer->appContainerName);
                if (appContainerName.Equals(HutaoRuntime.FamilyName, StringComparison.Ordinal))
                {
                    ConvertSidToStringSidW(pContainer->appContainerSid, out PWSTR stringSid);
                    containerStringSid = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(stringSid).ToString();
                    break;
                }
            }
        }
        catch (COMException exception)
        {
            // 0x800706f4 RPC_X_NULL_REF_POINTER
            // https://github.com/DGP-Studio/Snap.Hutao/issues/2339
            if (exception.HResult == HRESULT_FROM_WIN32((WIN32_ERROR)RPC_X_NULL_REF_POINTER))
            {
                return;
            }

            throw;
        }
        finally
        {
            // This function returns 1 rather than 0 specified in the document.
            _ = NetworkIsolationFreeAppContainers(pContainers);
        }

        SID_AND_ATTRIBUTES* pSids = default;
        uint count = default;
        try
        {
            WIN32_ERROR error = NetworkIsolationGetAppContainerConfig(out count, out pSids);
            Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(error));

            for (uint i = 0; i < count; i++)
            {
                ConvertSidToStringSidW((pSids + i)->Sid, out PWSTR stringSid);
                ReadOnlySpan<char> stringSidSpan = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(stringSid);
                if (stringSidSpan.Equals(containerStringSid, StringComparison.Ordinal))
                {
                    IsLoopbackEnabled = true;
                    break;
                }
            }
        }
        finally
        {
            if (pSids is not null)
            {
                for (uint index = 0; index < count; index++)
                {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    HeapFree(GetProcessHeap(), 0, pSids[index].Sid);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                }
            }

            HeapFree(GetProcessHeap(), 0, pSids);
        }
    }
}