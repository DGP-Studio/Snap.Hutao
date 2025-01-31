// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
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

    public LoopbackSupport(IServiceProvider serviceProvider)
    {
        Initialize(out hutaoContainerStringSid);
        serviceProvider.GetRequiredService<ILogger<LoopbackSupport>>().LogInformation("Container SID: {SID}", hutaoContainerStringSid);
    }

    [ObservableProperty]
    public partial bool IsLoopbackEnabled { get; private set; }

    public void EnableLoopback()
    {
        ConvertStringSidToSidW(hutaoContainerStringSid, out PSID hutaoSid);
        SID_AND_ATTRIBUTES hutaoSidAttribute = default;
        hutaoSidAttribute.Sid = hutaoSid;

        ReadOnlySpan<SID_AND_ATTRIBUTES> sidAttributes = default;
        try
        {
            NetworkIsolationGetAppContainerConfig(out sidAttributes);
            IsLoopbackEnabled = NetworkIsolationSetAppContainerConfig([.. sidAttributes, hutaoSidAttribute]) is WIN32_ERROR.ERROR_SUCCESS;
        }
        finally
        {
            if (!sidAttributes.IsEmpty)
            {
                foreach (ref readonly SID_AND_ATTRIBUTES sid in sidAttributes)
                {
                    HeapFree(GetProcessHeap(), 0, sid.Sid);
                }
            }

            HeapFree(GetProcessHeap(), 0, ref MemoryMarshal.GetReference(sidAttributes));
        }
    }

    private void Initialize(out string containerStringSid)
    {
        containerStringSid = string.Empty;

        ReadOnlySpan<INET_FIREWALL_APP_CONTAINER> containers = default;
        try
        {
            WIN32_ERROR error = NetworkIsolationEnumAppContainers(NETISO_FLAG.NETISO_FLAG_MAX, out containers);
            Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(error));
            foreach (ref readonly INET_FIREWALL_APP_CONTAINER container in containers)
            {
                ReadOnlySpan<char> appContainerName = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(container.appContainerName);
                if (appContainerName.Equals(HutaoRuntime.FamilyName, StringComparison.Ordinal))
                {
                    ConvertSidToStringSidW(container.appContainerSid, out PWSTR stringSid);
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
            _ = NetworkIsolationFreeAppContainers(ref MemoryMarshal.GetReference(containers));
        }

        ReadOnlySpan<SID_AND_ATTRIBUTES> sidAttributes = default;
        try
        {
            WIN32_ERROR error = NetworkIsolationGetAppContainerConfig(out sidAttributes);
            Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(error));

            foreach (ref readonly SID_AND_ATTRIBUTES sidAttribute in sidAttributes)
            {
                ConvertSidToStringSidW(sidAttribute.Sid, out PWSTR stringSid);
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
            if (!sidAttributes.IsEmpty)
            {
                foreach (ref readonly SID_AND_ATTRIBUTES sid in sidAttributes)
                {
                    HeapFree(GetProcessHeap(), 0, sid.Sid);
                }
            }

            HeapFree(GetProcessHeap(), 0, ref MemoryMarshal.GetReference(sidAttributes));
        }
    }
}